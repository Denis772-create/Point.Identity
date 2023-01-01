namespace Point.Services.Identity.Web.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, AdminApiConfiguration adminApiConfiguration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(adminApiConfiguration.ApiVersion, new OpenApiInfo
            {
                Title = adminApiConfiguration.ApiName,
                Version = adminApiConfiguration.ApiVersion
            });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{adminApiConfiguration.IdentityServerBaseUrl}/connect/authorize"),
                        TokenUrl = new Uri($"{adminApiConfiguration.IdentityServerBaseUrl}/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { adminApiConfiguration.OidcApiName, adminApiConfiguration.ApiName }
                        }
                    }
                }
            });
            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
        return services;
    }
}