using Point.Services.Identity.Api.Helpers;

namespace Point.Services.Identity.Api.Extentions;

public static class ServiceExtentions
{
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
}

