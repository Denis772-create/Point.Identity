using Point.Services.Identity.Infrastructure.Interfaces;
using Point.Services.Identity.Web.Configuration;
using Point.Services.Identity.Web.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Point.Services.Identity.Web.Extentions;

public static class ServiceExtentions
{
    public static void UseMvcLocalizationServices(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(options!.Value);
    }

    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    => services.AddOptions()
               .AddControllers(opt =>
                   opt.Filters.Add(typeof(HttpGlobalExceptionFilter)))
               .AddControllersAsServices()
               .AddNewtonsoftJson()
               .Services;

    public static IServiceCollection ConfigureVersions(this IServiceCollection services)
        => services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
        });

    public static IServiceCollection AddCORS(this IServiceCollection services, string name)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name, builder =>
            {
                builder
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyMethod();
            });
        });
        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        => services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Point Identity API",
                Version = "v1",
                Description = "Service Identity for Point proj",
            });

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                    },
                    new List<string>()
                }
            });
        });

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        => services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddCheck("IdentityDb-check",
                new SqlConnectionHealthCheck(configuration["ConnectionStrings:IdentityDbConnection"]),
                HealthStatus.Unhealthy,
                new string[] { "identity-db" })
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

    public static void AddAuthenticationServices<TIdentityDbContext, TUserIdentity, TUserIdentityRole>(
        this IServiceCollection services, IConfiguration configuration)
        where TIdentityDbContext : DbContext
        where TUserIdentity : class
        where TUserIdentityRole : class
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

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            options.Secure = CookieSecurePolicy.SameAsRequest;
            options.OnAppendCookie = cookieContext =>
                AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            options.OnDeleteCookie = cookieContext =>
                AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });

        var authenticationBuilder = services.AddAuthentication();

        AddExternalProviders(authenticationBuilder, configuration);
    }

    private static void AddExternalProviders(AuthenticationBuilder authenticationBuilder,
        IConfiguration configuration)
    {
        // TODO: delete it for production
        var externalProviderConfiguration = configuration.GetSection(nameof(ExternalProvidersConfiguration))
            .Get<ExternalProvidersConfiguration>();

        if (externalProviderConfiguration.UseGitHubProvider)
        {
            authenticationBuilder.AddGitHub(options =>
            {
                options.ClientId = externalProviderConfiguration.GitHubClientId;
                options.ClientSecret = externalProviderConfiguration.GitHubClientSecret;
                options.CallbackPath = externalProviderConfiguration.GitHubCallbackPath;
                options.Scope.Add("user:email");
            });
        }

        // add here any External provider
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

    public static IIdentityServerBuilder AddIdentityServer<TConfigurationDbContext, TPersistedGrantDbContext,
        TUserIdentity>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        where TUserIdentity : class
    {
        var configurationSection = configuration.GetSection(nameof(IdentityServerOptions));

        var builder = services.AddIdentityServer(opt =>
                configurationSection.Bind(opt))
            .AddConfigurationStore<TConfigurationDbContext>()
            .AddOperationalStore<TPersistedGrantDbContext>()
            .AddAspNetIdentity<TUserIdentity>();

        builder.AddExtensionGrantValidator<DelegationGrantValidator>();

        return builder;
    }

    public static IMvcBuilder AddMvcWithLocalization<TUser, TKey>(this IServiceCollection services,
        IConfiguration configuration)
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        services.AddLocalization(opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; });

        services.TryAddTransient(typeof(IGenericLocalizer<>), typeof(GenericLocalizer<>));

        var mvcBuilder = services.AddControllersWithViews(o =>
        {
            o.Conventions.Add(new GenericControllerRouteConvention());
        })
          .AddViewLocalization(
              LanguageViewLocationExpanderFormat.Suffix,
              opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; })
          .AddDataAnnotationsLocalization()
          .ConfigureApplicationPartManager(m =>
          {
              m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider<TUser, TKey>());
          });


        var cultureConfiguration = configuration.GetSection(nameof(CultureConfiguration))
            .Get<CultureConfiguration>();
        services.Configure<RequestLocalizationOptions>(opts =>
        {
            var supportedCultureCodes = (cultureConfiguration?.Cultures?.Count > 0
                ? cultureConfiguration.Cultures.Intersect(CultureConfiguration.AvailableCultures)
                : CultureConfiguration.AvailableCultures).ToArray();

            if (!supportedCultureCodes.Any())
                supportedCultureCodes = CultureConfiguration.AvailableCultures;

            var supportedCultures = supportedCultureCodes.Select(c => new CultureInfo(c)).ToList();

            var defaultCultureCode = string.IsNullOrEmpty(cultureConfiguration?.DefaultCulture)
                ? CultureConfiguration.DefaultRequestCulture
                : cultureConfiguration.DefaultCulture;

            if (!supportedCultureCodes.Contains(defaultCultureCode))
                defaultCultureCode = supportedCultureCodes.FirstOrDefault();

            opts.DefaultRequestCulture = new RequestCulture(defaultCultureCode);
            opts.SupportedCultures = supportedCultures;
            opts.SupportedUICultures = supportedCultures;
        });

        return mvcBuilder;
    }
}

