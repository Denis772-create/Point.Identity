namespace Point.Services.Identity.Domain.Interfaces;

public interface IClientRepository
{
    Task<int> AddClient(Client client);
    Task<bool> CanInsertClient(Client client, bool isCloned = false);
    Task<Client?> GetClient(int clientId);
    Task<PagedList<Client>> GetClients(string search = "", int page = 1, int pageSize = 10);
    Task<(string? ClientId, string? ClientName)> GetClientId(int clientId);


    Task<int> AddClientSecret(int clientId, ClientSecret clientSecret);
    Task<PagedList<ClientSecret>> GetClientSecrets(int clientId, int page = 1, int pageSize = 10);
    Task<ClientSecret?> GetClientSecret(int clientSecretId);


    Task<PagedList<ClientProperty>> GetClientProperties(int clientId, int page = 1, int pageSize = 10);
    Task<ClientProperty?> GetClientProperty(int clientPropertyId);
    Task<int> AddClientProperty(int clientId, ClientProperty clientProperty);


    Task<PagedList<ClientClaim>> GetClientClaims(int clientId, int page = 1, int pageSize = 10);
    Task<ClientClaim?> GetClientClaim(int clientClaimId);
    Task<int> AddClientClaim(int clientId, ClientClaim clientClaim);

}