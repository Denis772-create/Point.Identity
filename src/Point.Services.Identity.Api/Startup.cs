using Point.Services.Identity.Web.Configuration.Constants;
using Point.Services.Identity.Web.Extentions;

namespace Point.Services.Identity.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
        => Configuration = configuration;

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var rootConfiguration = CreateRootConfiguration();
        services.AddSingleton(rootConfiguration);

        // Register DbContexts for IdentityServer and Identity
        services.RegisterDbContexts<AspIdentityDbContext, IdentityServerConfigurationDbContext,
            IdentityServerPersistedGrantDbContext, ProtectionDbContext>(Configuration);

        // Add services for authentication, including Identity model and external providers
        services.AddAuthenticationServices<AspIdentityDbContext, UserIdentity, UserIdentityRole>(Configuration);
        services.AddIdentityServer<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, UserIdentity>(Configuration);

        RegisterHstsOptions(services);

        services.AddMvcWithLocalization<UserIdentity, Guid>(Configuration);

        services.AddApiConfiguration()
            .AddCustomHealthCheck(Configuration)
            .ConfigureVersions()
            .ConfigureSwagger()
            .AddCORS("CORS-Policy");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

        app.UsePathBase(Configuration.GetValue<string>("BasePath"));

        app.UseStaticFiles();

        app.UseIdentityServer();

        app.UseMvcLocalizationServices();

        app.UseRouting();
        app.UseAuthorization();

        app.UseSerilogRequestLogging();
        app.UseSwagger();
        app.UseSwaggerUI(s =>
        {
            s.SwaggerEndpoint("/swagger/v1/swagger.json", "POINT.Identity API v1");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute();
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = healthCheckRegistration => healthCheckRegistration.Name.Contains("self")
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
}