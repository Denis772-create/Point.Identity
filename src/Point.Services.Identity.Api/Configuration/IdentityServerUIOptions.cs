using Point.Services.Identity.Infrastructure.Configuration;

namespace Point.Services.Identity.Web.Configuration;

public class IdentityServerUIOptions
{

    /// <summary>
    /// The settings for the admin services.
    /// </summary>
    public AdminConfiguration Admin { get; set; } = new();

    /// <summary>
    /// The settings for database migrations.
    /// </summary>
    public DatabaseMigrationsConfiguration DatabaseMigrations { get; set; } = new();

    /// <summary>
    /// The settings for globalization.
    /// </summary>
    public CultureConfiguration Culture { get; set; } = new();

    /// <summary>
    /// An action to configure ASP.NET Core Identity.
    /// </summary>
    public Action<IdentityOptions> IdentityConfigureAction { get; set; } = options => { };

    /// <summary>
    /// The settings for data protection.
    /// </summary>
    public DataProtectionConfiguration DataProtection { get; set; } = new();

    /// <summary>
    /// The settings for Azure key vault.
    /// </summary>
    public AzureKeyVaultConfiguration AzureKeyVault { get; set; } = new();

    /// <summary>
    /// Identity data to seed the databases.
    /// </summary>
    public IdentityData IdentityData { get; set; } = new();

    /// <summary>
    /// Identity server data to seed the databases.
    /// </summary>
    public IdentityServerData IdentityServerData { get; set; } = new();

    /// <summary>
    /// Customizes the health checks builder used to add health checks.
    /// </summary>
    public Func<IServiceCollection, IHealthChecksBuilder> HealthChecksBuilderFactory { get; set; }

    /// <summary>
    /// Applies configuration parsed from an appsettings file into these options.
    /// </summary>
    /// <param name="configuration">The configuration to bind into this instance.</param>
    public void BindConfiguration(IConfiguration configuration)
    {
        configuration.GetSection(nameof(AdminConfiguration)).Bind(Admin);
        configuration.GetSection(nameof(DatabaseMigrationsConfiguration)).Bind(DatabaseMigrations);
        configuration.GetSection(nameof(CultureConfiguration)).Bind(Culture);
        configuration.GetSection(nameof(DataProtectionConfiguration)).Bind(DataProtection);
        configuration.GetSection(nameof(AzureKeyVaultConfiguration)).Bind(AzureKeyVault);
        IdentityConfigureAction = options => configuration.GetSection(nameof(IdentityOptions)).Bind(options);
        configuration.GetSection(nameof(IdentityServerData)).Bind(IdentityServerData);
        configuration.GetSection(nameof(IdentityData)).Bind(IdentityData);
    }
}