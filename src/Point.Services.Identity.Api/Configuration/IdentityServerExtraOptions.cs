namespace Point.Services.Identity.Web.Configuration;

public class IdentityServerExtraOptions : IdentityServerOptions
{
    public KeyManagementOptions KeyManagement { get; set; } = new();
}