namespace Point.Services.Identity.Application.Configuration;

public class AdminConfiguration
{
    public string IdentityAdminRedirectUri { get; set; } = string.Empty;
    public string PageTitle { get; set; } = string.Empty;
    public string HomePageLogoUri { get; set; } = string.Empty;
    public string FaviconUri { get; set; } = string.Empty;
    public string AdminBaseUrl { get; set; } = string.Empty;
    public string AdminApiBaseUrl { get; set; } = string.Empty;
    public string AdministrationRole { get; set; } = string.Empty;
    public string IdentityAdminCookieName { get; set; } = string.Empty;
    public string IdentityServerBaseUrl { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; }
    public double IdentityAdminCookieExpiresUtcHours { get; set; }
    public string OidcResponseType { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CustomThemeCss { get; set; } = string.Empty;
    public string AdminApiScope { get; set; } = string.Empty;
    public bool HideUIForMSSqlErrorLogging { get; set; } = false;
    public string[] Scopes { get; set; } = Array.Empty<string>();
}