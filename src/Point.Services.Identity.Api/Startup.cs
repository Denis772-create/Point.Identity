using Point.Services.Identity.Application.Configuration;

namespace Point.Services.Identity.Api;

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
        app.UseStaticFiles();

        app.UseIdentityServer();

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
}

