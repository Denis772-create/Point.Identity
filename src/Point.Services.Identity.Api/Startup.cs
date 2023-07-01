namespace Point.Services.Identity.Web;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add essential configurations
        var rootConfiguration = CreateRootConfiguration();
        services.AddSingleton(rootConfiguration);

        var adminApiConfiguration = Configuration.GetSection(nameof(AdminApiConfiguration)).Get<AdminApiConfiguration>();
        services.AddSingleton(adminApiConfiguration);

        // Register DbContexts for IdentityServer and ASP Identity
        services.RegisterDbContexts<AspIdentityDbContext, ServerConfigurationDbContext,
            ServerPersistedGrantDbContext, ProtectionDbContext>(Configuration);

        // Add services for authentication, including ASP Identity model and external providers
        services.AddAuthenticationServices<AspIdentityDbContext, UserIdentity,
            UserIdentityRole, UserIdentity, UserIdentityRole>(Configuration);

        services.AddIdentityServer<ServerConfigurationDbContext,
            ServerPersistedGrantDbContext, UserIdentity>(Configuration);

        services.AddDataProtection<ProtectionDbContext>(Configuration);

        services.AddAdminAspNetIdentityServices<AspIdentityDbContext, ServerPersistedGrantDbContext,
            UserDto, RoleDto, UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole,
            UserIdentityLogin, UserIdentityRoleClaim, UserIdentityToken,
            UsersDto, RolesDto, UserRolesDto,
            UserClaimsDto, UserProviderDto, UserProvidersDto, UserChangePasswordDto,
            RoleClaimsDto, UserClaimDto, RoleClaimDto>();

        services.AddAdminServices<ServerConfigurationDbContext, ServerPersistedGrantDbContext>();

        services.AddControllersAndMvcServices<UserDto, RoleDto,
            UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole,
            UserIdentityLogin, UserIdentityRoleClaim, UserIdentityToken,
            UsersDto, RolesDto, UserRolesDto,
            UserClaimsDto, UserProviderDto, UserProvidersDto, UserChangePasswordDto,
            RoleClaimsDto, UserClaimDto, RoleClaimDto>(Configuration);

        // Adds the IdentityServer Admin UI with custom options.
        services.AddIdentityServerAdminUI(Configuration);

        services
            .AddAuthorizationPolicies(CreateRootConfiguration())
            .AddHstsOptions()
            .AddEventBus(Configuration)
            .AddSwagger(adminApiConfiguration)
            .AddCors(adminApiConfiguration)
            .AddIdentityServerAdminConfiguration(Configuration)
            .AddCustomHealthCheck(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
        AdminApiConfiguration adminApiConfiguration, SecurityConfiguration securityConfiguration)
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

        app.UseSecurityHeaders(securityConfiguration.CspTrustedDomains);

        app.UseRouting();

        app.UseMvcLocalizationServices();

        app.UseCors("AdminCors");

        app.UseAuthentication()
           .UseAuthorization()
           .UseIdentityServer();

        app.UseSerilogRequestLogging();

        app.UseSwagger()
           .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", adminApiConfiguration.ApiName);

                c.OAuthClientId(adminApiConfiguration.OidcSwaggerUIClientId);
                c.OAuthAppName(adminApiConfiguration.ApiName);
                c.OAuthUsePkce();
            });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapIdentityServerAdminUi();
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
}