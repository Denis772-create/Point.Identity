namespace Point.Services.Identity.Web;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var rootConfiguration = CreateRootConfiguration();
        services.AddSingleton(rootConfiguration);

        // Register DbContexts for IdentityServer and ASP Identity
        services.RegisterDbContexts<AspIdentityDbContext, IdentityServerConfigurationDbContext,
            IdentityServerPersistedGrantDbContext, ProtectionDbContext>(Configuration);

        services.AddDataProtection<ProtectionDbContext>(Configuration);

        // Add services for authentication, including Identity model and external providers
        services.AddAuthenticationServices<AspIdentityDbContext, UserIdentity,
            UserIdentityRole, UserIdentity, UserIdentityRole>(Configuration);

        services.AddIdentityServer<IdentityServerConfigurationDbContext,
            IdentityServerPersistedGrantDbContext, UserIdentity>(Configuration);

        services.AddIdentityServerAdminConfiguration(Configuration);

        services.AddEmailSenders(Configuration);

        services.AddCustomHealthCheck(Configuration);

        var adminApiConfiguration = Configuration.GetSection(nameof(AdminApiConfiguration))
            .Get<AdminApiConfiguration>();

        services.AddSingleton(adminApiConfiguration);

        var profileTypes = new HashSet<Type>
        {
            typeof(IdentityMapperProfile<IdentityRoleDto, IdentityUserRolesDto, Guid, IdentityUserClaimsDto, IdentityUserClaimDto, IdentityUserProviderDto, IdentityUserProvidersDto, UserChangePasswordDto, IdentityRoleClaimDto, IdentityRoleClaimsDto>)
        };

        services.AddAdminAspNetIdentityServices<AspIdentityDbContext, IdentityServerPersistedGrantDbContext,
            IdentityUserDto, IdentityRoleDto, UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole,
            UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
            IdentityUsersDto, IdentityRolesDto, IdentityUserRolesDto,
            IdentityUserClaimsDto, IdentityUserProviderDto, IdentityUserProvidersDto, UserChangePasswordDto,
            IdentityRoleClaimsDto, IdentityUserClaimDto, IdentityRoleClaimDto>(profileTypes);

        services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext>();

        services.AddSingleton(services.BuildServiceProvider());

        services.AddCors(adminApiConfiguration);

        services.AddControllersAndMvcServices<IdentityUserDto, IdentityRoleDto,
            UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole,
            UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
            IdentityUsersDto, IdentityRolesDto, IdentityUserRolesDto,
            IdentityUserClaimsDto, IdentityUserProviderDto, IdentityUserProvidersDto, UserChangePasswordDto,
            IdentityRoleClaimsDto, IdentityUserClaimDto, IdentityRoleClaimDto>(Configuration);

        RegisterAuthorization(services);
        RegisterHstsOptions(services);

        services.AddSwagger(adminApiConfiguration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AdminApiConfiguration adminApiConfiguration)
    {
        app.UseCookiePolicy();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UsePathBase(Configuration.GetValue<string>("BasePath"));

        app.UseStaticFiles();

        app.UseRouting();

        app.UseMvcLocalizationServices();

        app.UseCors("AdminCors");

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseIdentityServer();

        app.UseSerilogRequestLogging();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", adminApiConfiguration.ApiName);

            c.OAuthClientId(adminApiConfiguration.OidcSwaggerUIClientId);
            c.OAuthAppName(adminApiConfiguration.ApiName);
            c.OAuthUsePkce();
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });
    }


    protected IRootConfiguration CreateRootConfiguration()
    {
        var rootConfiguration = new RootConfiguration();
        Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).Bind(rootConfiguration.AdminConfiguration);
        Configuration.GetSection(ConfigurationConsts.RegisterConfigurationKey).Bind(rootConfiguration.RegisterConfiguration);
        return rootConfiguration;
    }

    public virtual void RegisterHstsOptions(IServiceCollection services)
    {
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });
    }

    public virtual void RegisterAuthorization(IServiceCollection services)
    {
        var rootConfiguration = CreateRootConfiguration();
        services.AddAuthorizationPolicies(rootConfiguration);
    }
}