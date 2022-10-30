namespace Point.Services.Identity.Application.STS.Helpers;

public static class IdentityServerBuilderExtensions
{
    public static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder builder,
        IConfiguration configuration)
    {
        // TODO: implement 
        return builder;
    }

    public static IIdentityServerBuilder AddCustomValidationKey(this IIdentityServerBuilder builder,
        IConfiguration configuration)
    {
        // TODO: implement 
        return builder;
    }
}