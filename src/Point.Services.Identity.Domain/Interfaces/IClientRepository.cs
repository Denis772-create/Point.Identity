namespace Point.Services.Identity.Domain.Interfaces;

public interface IClientRepository
{
    // Clients
    Task<int> AddClient(Client client);
    Task<bool> CanInsertClient(Client client, bool isCloned = false);
    Task<Client?> GetClient(int clientId);
    Task<PagedList<Client>> GetClients(string search = "", int page = 1, int pageSize = 10);
    Task<(string? ClientId, string? ClientName)> GetClientId(int clientId);
    Task<int> CloneClientAsync(Client client,
        bool cloneClientCorsOrigins = true,
        bool cloneClientGrantTypes = true,
        bool cloneClientIdPRestrictions = true,
        bool cloneClientPostLogoutRedirectUris = true,
        bool cloneClientScopes = true,
        bool cloneClientRedirectUris = true,
        bool cloneClientClaims = true,
        bool cloneClientProperties = true);
    Task<int> UpdateClientAsync(Client client, bool updateClientClaims = false, bool updateClientProperties = false);
    Task<int> RemoveClientAsync(Client client);

    // Secrets
    Task<int> AddClientSecret(int clientId, ClientSecret clientSecret);
    Task<PagedList<ClientSecret>> GetClientSecrets(int clientId, int page = 1, int pageSize = 10);
    Task<ClientSecret?> GetClientSecret(int clientSecretId);
    Task<int> DeleteClientSecretAsync(ClientSecret clientSecret);

    // Properties
    Task<PagedList<ClientProperty>> GetClientProperties(int clientId, int page = 1, int pageSize = 10);
    Task<ClientProperty?> GetClientProperty(int clientPropertyId);
    Task<int> AddClientProperty(int clientId, ClientProperty clientProperty);
    Task<int> DeleteClientPropertyAsync(ClientProperty clientProperty);

    // Claims
    Task<PagedList<ClientClaim>> GetClientClaims(int clientId, int page = 1, int pageSize = 10);
    Task<ClientClaim?> GetClientClaim(int clientClaimId);
    Task<int> AddClientClaim(int clientId, ClientClaim clientClaim);
    Task<int> DeleteClientClaimAsync(ClientClaim clientClaim);
}