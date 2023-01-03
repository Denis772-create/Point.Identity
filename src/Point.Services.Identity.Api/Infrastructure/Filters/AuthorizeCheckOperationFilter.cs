using Swashbuckle.AspNetCore.SwaggerGen;

namespace Point.Services.Identity.Web.Infrastructure.Filters;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    private readonly AdminApiConfiguration _adminApiConfiguration;

    public AuthorizeCheckOperationFilter(AdminApiConfiguration adminApiConfiguration)
    {
        _adminApiConfiguration = adminApiConfiguration;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.DeclaringType != null &&
                           (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                            || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any());

        if (!hasAuthorize) return;

        operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [
                    new OpenApiSecurityScheme {Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"}
                    }
                ] = new[] { _adminApiConfiguration.OidcApiName }
            }
        };
    }
}