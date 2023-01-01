namespace Point.Services.Identity.Application.Services;

public class ClientService : IClientService
{
    protected readonly IClientRepository _clientRepository;
    protected readonly IClientServiceResources ClientServiceResources;
    private const string SharedSecret = "SharedSecret";

    public ClientService(IClientRepository clientRepository,
        IClientServiceResources clientServiceResources)
    {
        _clientRepository = clientRepository;
        ClientServiceResources = clientServiceResources;
    }

    public virtual async Task<ClientDto> GetClientAsync(int clientId)
    {
        var client = await _clientRepository.GetClient(clientId);

        if (client == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

        var clientDto = client.ToModel();

        // TODO: to create audit log

        return clientDto;
    }

    public virtual async Task<ClientsDto> GetClientsAsync(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = await _clientRepository.GetClients(search, page, pageSize);
        var clientsDto = pagedList.ToModel();

        // TODO: to create audit log

        return clientsDto;
    }

    public virtual async Task<bool> CanInsertClientAsync(ClientDto client, bool isCloned = false)
    {
        var clientEntity = client.ToEntity();

        return await _clientRepository.CanInsertClient(clientEntity, isCloned);
    }

    public virtual async Task<int> AddClientAsync(ClientDto client)
    {
        var canInsert = await CanInsertClientAsync(client);
        if (!canInsert)
        {
            throw new UserFriendlyViewException(string.Format(ClientServiceResources.ClientExistsValue().Description, client.ClientId), ClientServiceResources.ClientExistsKey().Description, client);
        }

        PrepareClientTypeForNewClient(client);
        var clientEntity = client.ToEntity();

        var added = await _clientRepository.AddClient(clientEntity);

        // TODO: to create audit log

        return added;
    }

    public async Task<int> UpdateClientAsync(ClientDto client, bool updateClientClaims = false, bool updateClientProperties = false)
    {
        var canInsert = await CanInsertClientAsync(client);
        if (!canInsert)
        {
            throw new UserFriendlyViewException(string.Format(ClientServiceResources.ClientExistsValue().Description, client.ClientId), ClientServiceResources.ClientExistsKey().Description, client);
        }

        var clientEntity = client.ToEntity();

        var originalClient = await GetClientAsync(client.Id);

        var updated = await _clientRepository.UpdateClientAsync(clientEntity);

        // TODO: to create audit log

        return updated;
    }

    public async Task<int> RemoveClientAsync(ClientDto client)
    {
        var clientEntity = client.ToEntity();

        var deleted = await _clientRepository.RemoveClientAsync(clientEntity);

        // TODO: to create audit log

        return deleted;
    }

    public async Task<int> CloneClientAsync(ClientCloneDto client)
    {
        var canInsert = await CanInsertClientAsync(client);
        if (!canInsert)
        {
            throw new UserFriendlyViewException(string.Format(ClientServiceResources.ClientExistsValue().Description, client.ClientId), ClientServiceResources.ClientExistsKey().Description, client);
        }

        var clientEntity = client.ToEntity();

        var originalClient = await GetClientAsync(client.Id);

        var updated = await _clientRepository.UpdateClientAsync(clientEntity);

        // TODO: to create audit log

        return updated;
    }

    public virtual async Task<int> AddClientSecretAsync(ClientSecretsDto clientSecret)
    {
        HashClientSharedSecret(clientSecret);

        var clientSecretEntity = clientSecret.ToEntity();
        var added = await _clientRepository.AddClientSecret(clientSecret.ClientId, clientSecretEntity);

        // TODO: to create audit log

        return added;
    }

    private static void HashClientSharedSecret(ClientSecretsDto clientSecret)
    {
        if (clientSecret.Type != SharedSecret) return;

        clientSecret.Value = clientSecret.HashTypeEnum switch
        {
            HashType.Sha256 => clientSecret.Value.Sha256(),
            HashType.Sha512 => clientSecret.Value.Sha512(),
            _ => clientSecret.Value
        };
    }


    public virtual async Task<ClientSecretsDto> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10)
    {
        var clientInfo = await _clientRepository.GetClientId(clientId);
        if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

        var pagedList = await _clientRepository.GetClientSecrets(clientId, page, pageSize);
        var clientSecretsDto = pagedList.ToModel();
        clientSecretsDto.ClientId = clientId;
        clientSecretsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

        // remove secret value from dto
        clientSecretsDto.ClientSecrets.ForEach(x => x.Value = null);

        // TODO: to create audit log

        return clientSecretsDto;
    }

    public virtual async Task<ClientSecretsDto> GetClientSecretAsync(int clientSecretId)
    {
        var clientSecret = await _clientRepository.GetClientSecret(clientSecretId);
        if (clientSecret == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientSecretDoesNotExist().Description, clientSecretId));

        var clientInfo = await _clientRepository.GetClientId(clientSecret.Client.Id);
        if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientSecret.Client.Id));

        var clientSecretsDto = clientSecret.ToModel();
        clientSecretsDto.ClientId = clientSecret.Client.Id;
        clientSecretsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

        // remove secret value for dto
        clientSecretsDto.Value = null;

        // TODO: to create audit log

        return clientSecretsDto;
    }

    public Task<int> DeleteClientSecretAsync(ClientSecretsDto clientSecret)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<ClientPropertiesDto> GetClientPropertyAsync(int clientPropertyId)
    {
        var clientProperty = await _clientRepository.GetClientProperty(clientPropertyId);
        if (clientProperty == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientPropertyDoesNotExist().Description, clientPropertyId));

        var clientInfo = await _clientRepository.GetClientId(clientProperty.Client.Id);
        if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientProperty.Client.Id));

        var clientPropertiesDto = clientProperty.ToModel();
        clientPropertiesDto.ClientId = clientProperty.Client.Id;
        clientPropertiesDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

        // TODO: to create audit log

        return clientPropertiesDto;
    }

    public virtual async Task<int> AddClientPropertyAsync(ClientPropertiesDto clientProperties)
    {
        var clientProperty = clientProperties.ToEntity();

        var saved = await _clientRepository.AddClientProperty(clientProperties.ClientId, clientProperty);

        // TODO: to create audit log

        return saved;
    }

    public virtual async Task<ClientPropertiesDto> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10)
    {
        var clientInfo = await _clientRepository.GetClientId(clientId);
        if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

        var pagedList = await _clientRepository.GetClientProperties(clientId, page, pageSize);
        var clientPropertiesDto = pagedList.ToModel();
        clientPropertiesDto.ClientId = clientId;
        clientPropertiesDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

        // TODO: to create audit log

        return clientPropertiesDto;
    }

    public async Task<int> DeleteClientPropertyAsync(ClientPropertiesDto clientProperty)
    {
        var clientPropertyEntity = clientProperty.ToEntity();

        var deleted = await _clientRepository.DeleteClientPropertyAsync(clientPropertyEntity);

        // TODO: to create audit log

        return deleted;
    }

    public virtual async Task<ClientClaimsDto> GetClientClaimAsync(int clientClaimId)
    {
        var clientClaim = await _clientRepository.GetClientClaim(clientClaimId);
        if (clientClaim == null)
            throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientClaimDoesNotExist().Description, clientClaimId));

        var clientInfo = await _clientRepository.GetClientId(clientClaim.Client.Id);
        if (clientInfo.ClientId == null) 
            throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientClaim.Client.Id));

        var clientClaimsDto = clientClaim.ToModel();
        clientClaimsDto.ClientId = clientClaim.Client.Id;
        clientClaimsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

        // TODO: to create audit log

        return clientClaimsDto;
    }

    public virtual async Task<ClientClaimsDto> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10)
    {
        var clientInfo = await _clientRepository.GetClientId(clientId);
        if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

        var pagedList = await _clientRepository.GetClientClaims(clientId, page, pageSize);
        var clientClaimsDto = pagedList.ToModel();
        clientClaimsDto.ClientId = clientId;
        clientClaimsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

        // TODO: to create audit log

        return clientClaimsDto;
    }

    public virtual async Task<int> AddClientClaimAsync(ClientClaimsDto clientClaim)
    {
        var clientClaimEntity = clientClaim.ToEntity();

        var saved = await _clientRepository.AddClientClaim(clientClaim.ClientId, clientClaimEntity);

        // TODO: to create audit log

        return saved;
    }

    public async Task<int> DeleteClientClaimAsync(ClientClaimsDto clientClaim)
    {
        var clientClaimEntity = clientClaim.ToEntity();

        var deleted = await _clientRepository.DeleteClientClaimAsync(clientClaimEntity);

        // TODO: to create audit log

        return deleted;
    }

    private static void PrepareClientTypeForNewClient(ClientDto client)
    {
        switch (client.ClientType)
        {
            case ClientType.Empty:
                break;
            case ClientType.Web:
                client.AllowedGrantTypes.AddRange(GrantTypes.Code);
                client.RequirePkce = true;
                client.RequireClientSecret = true;
                break;
            case ClientType.Spa:
                client.AllowedGrantTypes.AddRange(GrantTypes.Code);
                client.RequirePkce = true;
                client.RequireClientSecret = false;
                break;
            case ClientType.Native:
                client.AllowedGrantTypes.AddRange(GrantTypes.Code);
                client.RequirePkce = true;
                client.RequireClientSecret = false;
                break;
            case ClientType.Machine:
                client.AllowedGrantTypes.AddRange(GrantTypes.ClientCredentials);
                break;
            case ClientType.Device:
                client.AllowedGrantTypes.AddRange(GrantTypes.DeviceFlow);
                client.RequireClientSecret = false;
                client.AllowOfflineAccess = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}