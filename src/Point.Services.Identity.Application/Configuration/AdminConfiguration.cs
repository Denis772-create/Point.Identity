namespace Point.Services.Identity.Application.Configuration;

public class AdminConfiguration
{
    public string PageTitle { get; set; } = string.Empty;
    public string HomePageLogoUri { get; set; } = string.Empty;
    public string FaviconUri { get; set; } = string.Empty;
    public string AdminBaseUrl { get; set; } = string.Empty;
    public string AdministrationRole { get; set; } = string.Empty;

    public string Theme { get; set; } = string.Empty;

    public string CustomThemeCss { get; set; } = string.Empty;
}