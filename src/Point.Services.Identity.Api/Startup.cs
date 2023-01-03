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
        services.RegisterDbContexts<AspIdentityDbContext, ServerConfigurationDbContext,
            ServerPersistedGrantDbContext, ProtectionDbContext>(Configuration);

        services.AddDataProtection<ProtectionDbContext>(Configuration);

        // Add services for authentication, including Identity model and external providers
        services.AddAuthenticationServices<AspIdentityDbContext, UserIdentity,
            UserIdentityRole, UserIdentity, UserIdentityRole>(Configuration);

        services.AddIdentityServer<ServerConfigurationDbContext,
            ServerPersistedGrantDbContext, UserIdentity>(Configuration);

        services.AddIdentityServerAdminConfiguration(Configuration);

        services.AddEmailSenders(Configuration);

        services.AddCustomHealthCheck(Configuration);

        var adminApiConfiguration = Configuration.GetSection(nameof(AdminApiConfiguration))
            .Get<AdminApiConfiguration>();

        services.AddSingleton(adminApiConfiguration);

        var profileTypes = new HashSet<Type>
        {
            typeof(IdentityMapperProfile<RoleDto, UserRolesDto, Guid, UserClaimsDto, UserClaimDto, UserProviderDto, UserProvidersDto, UserChangePasswordDto, RoleClaimDto, RoleClaimsDto>)
        };

        services.AddAdminAspNetIdentityServices<AspIdentityDbContext, ServerPersistedGrantDbContext,
            UserDto, RoleDto, UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole,
            UserIdentityLogin, UserIdentityRoleClaim, UserIdentityToken,
            UsersDto, RolesDto, UserRolesDto,
            UserClaimsDto, UserProviderDto, UserProvidersDto, UserChangePasswordDto,
            RoleClaimsDto, UserClaimDto, RoleClaimDto>(profileTypes);

        services.AddAdminServices<ServerConfigurationDbContext, ServerPersistedGrantDbContext>();

        services.AddSingleton(services.BuildServiceProvider());

        services.AddCors(adminApiConfiguration);

        services.AddControllersAndMvcServices<UserDto, RoleDto,
            UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole,
            UserIdentityLogin, UserIdentityRoleClaim, UserIdentityToken,
            UsersDto, RolesDto, UserRolesDto,
            UserClaimsDto, UserProviderDto, UserProvidersDto, UserChangePasswordDto,
            RoleClaimsDto, UserClaimDto, RoleClaimDto>(Configuration);

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