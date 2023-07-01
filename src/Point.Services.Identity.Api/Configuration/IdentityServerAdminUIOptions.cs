namespace Point.Services.Identity.Web.Configuration;

public class IdentityServerAdminUIOptions
{
    /// <summary>
    /// The settings for the admin services.
    /// </summary>
    public AdminConfiguration Admin { get; set; } = new();

    /// <summary>
    /// The settings for globalization.
    /// </summary>
    public CultureConfiguration Culture { get; set; } = new();

    /// <summary>
    /// An action to configure ASP.NET Core Identity.
    /// </summary>
    public Action<IdentityOptions> IdentityConfigureAction { get; set; } = options => { };


    /// <summary>
    /// The settings for security features.
    /// </summary>
    public SecurityConfiguration Security { get; set; } = new();

    /// <summary>
    /// The settings for the HTTP hosting environment.
    /// </summary>
    public HttpConfiguration Http { get; set; } = new();

    /// <summary>
    /// Applies configuration parsed from an appsettings file into these options.
    /// </summary>
    /// <param name="configuration">The configuration to bind into this instance.</param>
    public void BindConfiguration(IConfiguration configuration)
    {
        configuration.GetSection(nameof(AdminConfiguration)).Bind(Admin);
        configuration.GetSection(nameof(CultureConfiguration)).Bind(Culture);
        IdentityConfigureAction = options => configuration.GetSection(nameof(IdentityOptions)).Bind(options);
        configuration.GetSection(nameof(SecurityConfiguration)).Bind(Security);
        configuration.GetSection(nameof(HttpConfiguration)).Bind(Http);
    }
}
