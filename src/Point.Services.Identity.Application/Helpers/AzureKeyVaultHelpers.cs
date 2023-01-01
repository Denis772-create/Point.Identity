using System.Security.Cryptography.X509Certificates;
using Point.Services.Identity.Application.Configuration;
using Point.Services.Identity.Application.Services;

namespace Point.Services.Identity.Application.Helpers;

public class AzureKeyVaultHelpers
{
    public static async Task<(X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate)> GetCertificates(AzureKeyVaultConfiguration certificateConfiguration)
    {
        (X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate) certs = (null, null);

        if (!string.IsNullOrEmpty(certificateConfiguration.AzureKeyVaultEndpoint))
        {
            var keyVaultCertificateService = new AzureKeyVaultService(certificateConfiguration);

            certs = await keyVaultCertificateService.GetCertificatesFromKeyVault().ConfigureAwait(false);
        }

        return certs;
    }
}