namespace Point.Services.Identity.Application.Configuration;

internal class CertificateConfiguration
{
    public bool UseTemporarySigningKeyForDevelopment { get; set; }

    public string CertificateStoreLocation { get; set; } = string.Empty;
    public bool CertificateValidOnly { get; set; }

    public bool UseSigningCertificateThumbprint { get; set; }

    public string SigningCertificateThumbprint { get; set; } = string.Empty;

    public bool UseSigningCertificatePfxFile { get; set; }

    public string SigningCertificatePfxFilePath { get; set; } = string.Empty;

    public string SigningCertificatePfxFilePassword { get; set; } = string.Empty;

    public bool UseValidationCertificateThumbprint { get; set; }    

    public string ValidationCertificateThumbprint { get; set; } = string.Empty;

    public bool UseValidationCertificatePfxFile { get; set; }

    public string ValidationCertificatePfxFilePath { get; set; } = string.Empty;

    public string ValidationCertificatePfxFilePassword { get; set; } = string.Empty;

    public bool UseSigningCertificateForAzureKeyVault { get; set; }

    public bool UseValidationCertificateForAzureKeyVault { get; set; }
}