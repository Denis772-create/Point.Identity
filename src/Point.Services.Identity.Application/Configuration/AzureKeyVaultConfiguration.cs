namespace Point.Services.Identity.Application.Configuration;

public class AzureKeyVaultConfiguration
{
    public string AzureKeyVaultEndpoint { get; set; } = string.Empty;

    public string TenantId { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public bool UseClientCredentials { get; set; }

    public string IdentityServerCertificateName { get; set; } = string.Empty;

    public string DataProtectionKeyIdentifier { get; set; } = string.Empty;

    public bool ReadConfigurationFromKeyVault { get; set; }
}