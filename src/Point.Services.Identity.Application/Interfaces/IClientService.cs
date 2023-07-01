namespace Point.Services.Identity.Application.Interfaces;

public interface IClientService
{
    ClientDto BuildClientViewModel(ClientDto client = null);

    ClientSecretsDto BuildClientSecretsViewModel(ClientSecretsDto clientSecrets);

    ClientCloneDto BuildClientCloneViewModel(int id, ClientDto clientDto);
    List<string> GetStandardClaims(string claim, int limit = 0);
    List<string> GetGrantTypes(string grant, int limit = 0);
    Task<List<string>> GetScopesAsync(string scope, int limit = 0);
    List<string> GetSigningAlgorithms(string algorithm, int limit = 0);
    List<SelectItemDto> GetHashTypes();
    List<SelectItemDto> GetSecretTypes();


    // Clients
    Task<ClientDto> GetClientAsync(int clientId);
    Task<ClientsDto> GetClientsAsync(string search, int page = 1, int pageSize = 10);
    Task<bool> CanInsertClientAsync(ClientDto client, bool isCloned = false);
    Task<int> AddClientAsync(ClientDto client);
    Task<int> UpdateClientAsync(ClientDto client, bool updateClientClaims = false, bool updateClientProperties = false);
    Task<int> RemoveClientAsync(ClientDto client);
    Task<int> CloneClientAsync(ClientCloneDto client);
    // Client Secrets
    Task<int> AddClientSecretAsync(ClientSecretsDto clientSecret);
    Task<ClientSecretsDto> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10);
    Task<ClientSecretsDto> GetClientSecretAsync(int clientSecretId);
    Task<int> DeleteClientSecretAsync(ClientSecretsDto clientSecret);
    // Client Properties
    Task<ClientPropertiesDto> GetClientPropertyAsync(int clientPropertyId);
    Task<int> AddClientPropertyAsync(ClientPropertiesDto clientProperties);
    Task<ClientPropertiesDto> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10);
    Task<int> DeleteClientPropertyAsync(ClientPropertiesDto clientProperty);
    // Client Claims
    Task<ClientClaimsDto> GetClientClaimAsync(int clientClaimId);
    Task<ClientClaimsDto> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10);
    Task<int> AddClientClaimAsync(ClientClaimsDto clientClaim);
    Task<int> DeleteClientClaimAsync(ClientClaimsDto clientClaim);
}