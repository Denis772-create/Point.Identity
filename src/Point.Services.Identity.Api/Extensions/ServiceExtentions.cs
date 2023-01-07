namespace Point.Services.Identity.Web.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddHstsOptions(this IServiceCollection services)
    {
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });
        return services;
    }
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEventBusSubscriptionsManager, EventBusSubscriptionsManager>();

        services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            new DefaultServiceBusPersistentConnection(configuration["EventBusConnection"]));

        services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
        {
            var serviceBusPersistentConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
            var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
            var subscriptionName = configuration["SubscriptionClientName"];

            return new EventBusServiceBus(serviceBusPersistentConnection, logger,
                eventBusSubscriptionsManager, iLifetimeScope, subscriptionName);
        });

        return services;
    }

    public static void UseMvcLocalizationServices(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(options!.Value);
    }

    public static IServiceCollection ConfigureVersions(this IServiceCollection services)
        => services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
        });

    public static void AddDataProtection<TDbContext>(this IServiceCollection services, IConfiguration configuration)
        where TDbContext : DbContext, IDataProtectionKeyContext
    {
        AddDataProtection<TDbContext>(
            services,
            configuration.GetSection(nameof(DataProtectionConfiguration))
                .Get<DataProtectionConfiguration>(),
            configuration.GetSection(nameof(AzureKeyVaultConfiguration))
                .Get<AzureKeyVaultConfiguration>());
    }

    public static void AddDataProtection<TDbContext>(this IServiceCollection services, DataProtectionConfiguration dataProtectionConfiguration, AzureKeyVaultConfiguration azureKeyVaultConfiguration)
        where TDbContext : DbContext, IDataProtectionKeyContext
    {
        var dataProtectionBuilder = services.AddDataProtection()
            .SetApplicationName("Point.IdentityServer")
            .PersistKeysToDbContext<TDbContext>();

        if (!dataProtectionConfiguration.ProtectKeysWithAzureKeyVault) return;

        if (azureKeyVaultConfiguration.UseClientCredentials)
        {
            dataProtectionBuilder.ProtectKeysWithAzureKeyVault(
                new Uri(azureKeyVaultConfiguration.DataProtectionKeyIdentifier),
                new ClientSecretCredential(azureKeyVaultConfiguration.TenantId,
                    azureKeyVaultConfiguration.ClientId, azureKeyVaultConfiguration.ClientSecret));
        }
        else
        {
            dataProtectionBuilder.ProtectKeysWithAzureKeyVault(new Uri(azureKeyVaultConfiguration.DataProtectionKeyIdentifier), new DefaultAzureCredential());
        }
    }


    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services,
        IRootConfiguration rootConfiguration)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(ConfigurationConsts.AdministrationPolicy,
                policy => policy.RequireRole(rootConfiguration.AdminConfiguration.AdministrationRole));
        });
        return services;
    }

    public static IServiceCollection AddCors(this IServiceCollection services, AdminApiConfiguration adminApiConfiguration)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    if (adminApiConfiguration.CorsAllowAnyOrigin)
                    {
                        builder.AllowAnyOrigin();
                    }
                    else
                    {
                        builder.WithOrigins(adminApiConfiguration.CorsAllowOrigins);
                    }

                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });

            options.DefaultPolicyName = "AdminCors";
        });

        return services;
    }

    public static void AddControllersAndMvcServices<TUserDto, TRoleDto,
        TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(
        this IServiceCollection services, IConfiguration configuration)
        where TUserDto : UserDto<TKey>, new()
        where TRoleDto : RoleDto<TKey>, new()
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TUsersDto : UsersDto<TUserDto, TKey>
        where TRolesDto : RolesDto<TRoleDto, TKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
        where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TUserProviderDto, TKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TKey>
        where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, TKey>
        where TUserClaimDto : UserClaimDto<TKey>
        where TRoleClaimDto : RoleClaimDto<TKey>
    {
        services.AddLocalization(opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; });

        services.TryAddTransient(typeof(IGenericControllerLocalizer<>), typeof(GenericControllerLocalizer<>));

        services.AddTransient<IViewLocalizer, ResourceViewLocalizer>();

        services.ConfigureVersions();

        services
            .AddOptions()
            .AddControllersWithViews(opt =>
            {
                opt.Conventions.Add(new GenericControllerRouteConvention());
                opt.Filters.Add(typeof(ControllerExceptionFilterAttribute));
            })
            .AddControllersAsServices()
            .AddNewtonsoftJson()
            .AddViewLocalization(
                LanguageViewLocationExpanderFormat.Suffix,
                opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; })
            .AddDataAnnotationsLocalization()
            .ConfigureApplicationPartManager(m =>
            {
                m.FeatureProviders.Add(
                    new GenericTypeApiControllerFeatureProvider<TUserDto, TRoleDto,
                        TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>());
            });

        services.AddScoped<IApiErrorResources, ApiErrorResources>();
        services.AddScoped<ControllerExceptionFilterAttribute>();

        // add generic controllers
        services.AddScoped<AccountController<UserIdentity, Guid>>();
        services.AddScoped<ManageController<UserIdentity, Guid>>();
        services.AddScoped<UsersController<UserDto, RoleDto, UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim,
            UserIdentityUserRole, UserIdentityLogin, UserIdentityRoleClaim, UserIdentityToken, UsersDto, RolesDto,
            UserRolesDto, UserClaimsDto, UserProviderDto, UserProvidersDto, UserChangePasswordDto, RoleClaimsDto, UserClaimDto, RoleClaimDto>>();
        services.AddScoped<RolesController<UserDto, RoleDto, UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim,
            UserIdentityUserRole, UserIdentityLogin, UserIdentityRoleClaim, UserIdentityToken, UsersDto, RolesDto,
            UserRolesDto, UserClaimsDto, UserProviderDto, UserProvidersDto, UserChangePasswordDto, RoleClaimsDto, UserClaimDto, RoleClaimDto>>();

        // configure Localization
        var cultureConfiguration = configuration.GetSection(nameof(CultureConfiguration))
            .Get<CultureConfiguration>();

        services.Configure<RequestLocalizationOptions>(opts =>
        {
            var supportedCultureCodes = (cultureConfiguration?.Cultures?.Count > 0
                ? cultureConfiguration.Cultures.Intersect(CultureConfiguration.AvailableCultures)
                : CultureConfiguration.AvailableCultures).ToArray();

            if (!supportedCultureCodes.Any())
                supportedCultureCodes = CultureConfiguration.AvailableCultures;

            var supportedCultures = supportedCultureCodes
                .Select(c => new CultureInfo(c)).ToList();

            var defaultCultureCode = string.IsNullOrEmpty(cultureConfiguration?.DefaultCulture)
                ? CultureConfiguration.DefaultRequestCulture
                : cultureConfiguration.DefaultCulture;

            if (!supportedCultureCodes.Contains(defaultCultureCode))
                defaultCultureCode = supportedCultureCodes.FirstOrDefault();

            opts.DefaultRequestCulture = new RequestCulture(defaultCultureCode);
            opts.SupportedCultures = supportedCultures;
            opts.SupportedUICultures = supportedCultures;
        });
    }

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        => services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddCheck("IdentityDb-check",
                new SqlConnectionHealthCheck(configuration["ConnectionStrings:IdentityDbConnection"]),
                HealthStatus.Unhealthy,
                new[] { "identity-db" })
            .Services;

    public static void RegisterDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext,
        TDataProtectionDbContext>(this IServiceCollection services, IConfiguration configuration)
        where TIdentityDbContext : DbContext
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
    {
        var identityConnectionString = configuration.GetConnectionString(ConfigurationConsts.IdentityDbConnectionStringKey);
        var configurationConnectionString = configuration.GetConnectionString(ConfigurationConsts.ConfigurationDbConnectionStringKey);
        var persistedGrantsConnectionString = configuration.GetConnectionString(ConfigurationConsts.PersistedGrantDbConnectionStringKey);
        var dataProtectionConnectionString = configuration.GetConnectionString(ConfigurationConsts.DataProtectionDbConnectionStringKey);

        services.RegisterSqlServerDbContexts<TIdentityDbContext, TConfigurationDbContext,
            TPersistedGrantDbContext, TDataProtectionDbContext>(identityConnectionString, configurationConnectionString, persistedGrantsConnectionString, dataProtectionConnectionString);
    }

    public static void AddAuthenticationServices<TIdentityDbContext, TUserIdentity, TUserIdentityRole, TUser, TRole>(this IServiceCollection services,
        IConfiguration configuration)
        where TIdentityDbContext : DbContext
        where TUserIdentity : class
        where TUserIdentityRole : class
        where TRole : class
        where TUser : class

    {
        var loginConfiguration = GetLoginConfiguration(configuration);
        var registrationConfiguration = GetRegistrationConfiguration(configuration);
        var identityOptions = configuration.GetSection(nameof(IdentityOptions))
            .Get<IdentityOptions>() ?? new IdentityOptions();

        services
            .AddSingleton(registrationConfiguration)
            .AddSingleton(loginConfiguration)
            .AddSingleton(identityOptions)
            .AddScoped<ApplicationSignInManager<TUserIdentity>>()
            .AddScoped<UserResolver<TUserIdentity>>()
            .AddIdentity<TUserIdentity, TUserIdentityRole>(options => configuration.GetSection(nameof(IdentityOptions)).Bind(options))
            .AddEntityFrameworkStores<TIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityCore<TUser>(options =>
                configuration.GetSection(nameof(IdentityOptions)).Bind(options))
            .AddRoles<TRole>();

        services.ConfigureApplicationCookie(options =>
            options.Cookie.Name = "IdentityServer.Cookies");

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            options.Secure = CookieSecurePolicy.SameAsRequest;
            options.OnAppendCookie = cookieContext =>
                AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            options.OnDeleteCookie = cookieContext =>
                AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });

        AddApiAuthentication(services, configuration);

        // TODO: add External Providers
    }

    public static AuthenticationBuilder AddApiAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        var adminApiConfiguration = configuration.GetSection(nameof(AdminApiConfiguration))
            .Get<AdminApiConfiguration>();

        return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
             {
                 options.Authority = adminApiConfiguration.IdentityServerBaseUrl;
                 options.RequireHttpsMetadata = adminApiConfiguration.RequireHttpsMetadata;
                 options.Audience = adminApiConfiguration.OidcApiName;
             });
    }

    private static LoginConfiguration GetLoginConfiguration(IConfiguration configuration)
    {
        var loginConfiguration = configuration.GetSection(nameof(LoginConfiguration))
            .Get<LoginConfiguration>();

        return loginConfiguration ?? new LoginConfiguration();
    }

    private static RegisterConfiguration GetRegistrationConfiguration(IConfiguration configuration)
    {
        var registerConfiguration = configuration.GetSection(nameof(RegisterConfiguration))
            .Get<RegisterConfiguration>();

        return registerConfiguration ?? new RegisterConfiguration();
    }

    public static IIdentityServerBuilder AddIdentityServer<TConfigurationDbContext, TPersistedGrantDbContext, TUserIdentity>(this IServiceCollection services,
        IConfiguration configuration)
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        where TUserIdentity : class
    {
        var configurationSection = configuration.GetSection(nameof(IdentityServerOptions));

        var builder = services
            .AddIdentityServer(opt => configurationSection.Bind(opt))
            .AddConfigurationStore<TConfigurationDbContext>()
            .AddOperationalStore<TPersistedGrantDbContext>()
            .AddAspNetIdentity<TUserIdentity>();

        var identityServerOptions = configurationSection.Get<IdentityServerExtraOptions>();

        if (identityServerOptions.KeyManagement.Enabled)
        {
            builder.AddCustomSigningCredential(configuration);
            builder.AddCustomValidationKey(configuration);
        }

        builder.AddExtensionGrantValidator<DelegationGrantValidator>();

        return builder;
    }

    public static IServiceCollection AddEmailSenders(this IServiceCollection services, IConfiguration configuration)
    {
        var smtpConfiguration = configuration.GetSection(nameof(SmtpConfiguration)).Get<SmtpConfiguration>();

        services.AddSingleton(smtpConfiguration);
        services.AddTransient<IEmailSender, SmtpEmailSender>();
        return services;
    }


    public static IServiceCollection AddIdentityServerAdminConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = new IdentityServerUIOptions();
        options.BindConfiguration(configuration);

        services.AddSingleton(options.Admin);
        services.AddSingleton(options.IdentityServerData);
        services.AddSingleton(options.IdentityData);

        return services;
    }
}