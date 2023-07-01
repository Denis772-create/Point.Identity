namespace Point.Services.Identity.Application.Services;

public class ClientService : IClientService
{
    protected readonly IClientRepository ClientRepository;
    protected readonly IClientServiceResources ClientServiceResources;
    private const string SharedSecret = "SharedSecret";

    public ClientService(IClientRepository clientRepository,
        IClientServiceResources clientServiceResources)
    {
        ClientRepository = clientRepository;
        ClientServiceResources = clientServiceResources;
    }


    public virtual List<string> GetStandardClaims(string claim, int limit = 0)
    {
        var standardClaims = ClientRepository.GetStandardClaims(claim, limit);

        return standardClaims;
    }

    public virtual List<string> GetSigningAlgorithms(string algorithm, int limit = 0)
    {
        var signingAlgorithms = ClientConsts.SigningAlgorithms()
            .WhereIf(!string.IsNullOrWhiteSpace(algorithm), x => x.Contains(algorithm))
            .TakeIf(x => x, limit > 0, limit)
            .OrderBy(x => x)
            .ToList();

        return signingAlgorithms;
    }


    public virtual ClientCloneDto BuildClientCloneViewModel(int id, ClientDto clientDto)
    {
        var client = new ClientCloneDto
        {
            CloneClientCorsOrigins = true,
            CloneClientGrantTypes = true,
            CloneClientIdPRestrictions = true,
            CloneClientPostLogoutRedirectUris = true,
            CloneClientRedirectUris = true,
            CloneClientScopes = true,
            CloneClientClaims = true,
            CloneClientProperties = true,
            ClientIdOriginal = clientDto.ClientId,
            ClientNameOriginal = clientDto.ClientName,
            Id = id
        };

        return client;
    }

    public virtual ClientSecretsDto BuildClientSecretsViewModel(ClientSecretsDto clientSecrets)
    {
        clientSecrets.HashTypes = GetHashTypes();
        clientSecrets.TypeList = GetSecretTypes();

        return clientSecrets;
    }

    public virtual ClientDto BuildClientViewModel(ClientDto client = null)
    {
        if (client == null)
        {
            var clientDto = new ClientDto
            {
                AccessTokenTypes = GetAccessTokenTypes(),
                RefreshTokenExpirations = GetTokenExpirations(),
                RefreshTokenUsages = GetTokenUsage(),
                ProtocolTypes = GetProtocolTypes(),
                Id = 0
            };

            return clientDto;
        }

        client.AccessTokenTypes = GetAccessTokenTypes();
        client.RefreshTokenExpirations = GetTokenExpirations();
        client.RefreshTokenUsages = GetTokenUsage();
        client.ProtocolTypes = GetProtocolTypes();

        PopulateClientRelations(client);

        return client;
    }


    public virtual async Task<ClientDto> GetClientAsync(int clientId)
    {
        var client = await ClientRepository.GetClient(clientId);

        if (client == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

        var clientDto = client.ToModel();

        // TODO: to create audit log

        return clientDto;
    }

    public virtual async Task<ClientsDto> GetClientsAsync(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = await ClientRepository.GetClients(search, page, pageSize);
        var clientsDto = pagedList.ToModel();

        // TODO: to create audit log

        return clientsDto;
    }

    public virtual async Task<bool> CanInsertClientAsync(ClientDto client, bool isCloned = false)
    {
        var clientEntity = client.ToEntity();

        return await ClientRepository.CanInsertClient(clientEntity, isCloned);
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

        var added = await ClientRepository.AddClient(clientEntity);

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

        var updated = await ClientRepository.UpdateClientAsync(clientEntity);

        // TODO: to create audit log

        return updated;
    }

    public async Task<int> RemoveClientAsync(ClientDto client)
    {
        var clientEntity = client.ToEntity();

        var deleted = await ClientRepository.RemoveClientAsync(clientEntity);

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

        var updated = await ClientRepository.UpdateClientAsync(clientEntity);

        // TODO: to create audit log

        return updated;
    }

    public virtual async Task<int> AddClientSecretAsync(ClientSecretsDto clientSecret)
    {
        HashClientSharedSecret(clientSecret);

        var clientSecretEntity = clientSecret.ToEntity();
        var added = await ClientRepository.AddClientSecret(clientSecret.ClientId, clientSecretEntity);

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
        var clientInfo = await ClientRepository.GetClientId(clientId);
        if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

        var pagedList = await ClientRepository.GetClientSecrets(clientId, page, pageSize);
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
        var clientSecret = await ClientRepository.GetClientSecret(clientSecretId);
        if (clientSecret == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientSecretDoesNotExist().Description, clientSecretId));

        var clientInfo = await ClientRepository.GetClientId(clientSecret.Client.Id);
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
        var clientProperty = await ClientRepository.GetClientProperty(clientPropertyId);
        if (clientProperty == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientPropertyDoesNotExist().Description, clientPropertyId));

        var clientInfo = await ClientRepository.GetClientId(clientProperty.Client.Id);
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

        var saved = await ClientRepository.AddClientProperty(clientProperties.ClientId, clientProperty);

        // TODO: to create audit log

        return saved;
    }

    public virtual async Task<ClientPropertiesDto> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10)
    {
        var clientInfo = await ClientRepository.GetClientId(clientId);
        if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

        var pagedList = await ClientRepository.GetClientProperties(clientId, page, pageSize);
        var clientPropertiesDto = pagedList.ToModel();
        clientPropertiesDto.ClientId = clientId;
        clientPropertiesDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

        // TODO: to create audit log

        return clientPropertiesDto;
    }

    public async Task<int> DeleteClientPropertyAsync(ClientPropertiesDto clientProperty)
    {
        var clientPropertyEntity = clientProperty.ToEntity();

        var deleted = await ClientRepository.DeleteClientPropertyAsync(clientPropertyEntity);

        // TODO: to create audit log

        return deleted;
    }

    public virtual async Task<ClientClaimsDto> GetClientClaimAsync(int clientClaimId)
    {
        var clientClaim = await ClientRepository.GetClientClaim(clientClaimId);
        if (clientClaim == null)
            throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientClaimDoesNotExist().Description, clientClaimId));

        var clientInfo = await ClientRepository.GetClientId(clientClaim.Client.Id);
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
        var clientInfo = await ClientRepository.GetClientId(clientId);
        if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

        var pagedList = await ClientRepository.GetClientClaims(clientId, page, pageSize);
        var clientClaimsDto = pagedList.ToModel();
        clientClaimsDto.ClientId = clientId;
        clientClaimsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

        // TODO: to create audit log

        return clientClaimsDto;
    }

    public virtual async Task<int> AddClientClaimAsync(ClientClaimsDto clientClaim)
    {
        var clientClaimEntity = clientClaim.ToEntity();

        var saved = await ClientRepository.AddClientClaim(clientClaim.ClientId, clientClaimEntity);

        // TODO: to create audit log

        return saved;
    }

    public async Task<int> DeleteClientClaimAsync(ClientClaimsDto clientClaim)
    {
        var clientClaimEntity = clientClaim.ToEntity();

        var deleted = await ClientRepository.DeleteClientClaimAsync(clientClaimEntity);

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

    public List<SelectItemDto> GetHashTypes()
    {
        var hashTypes = ClientRepository.GetHashTypes().ToModel();

        return hashTypes;
    }

    public virtual List<SelectItemDto> GetSecretTypes()
    {
        var secretTypes = ClientRepository.GetSecretTypes().ToModel();

        return secretTypes;
    }

    public virtual List<SelectItemDto> GetProtocolTypes()
    {
        var protocolTypes = ClientRepository.GetProtocolTypes().ToModel();

        return protocolTypes;
    }

    public virtual List<SelectItemDto> GetTokenExpirations()
    {
        var tokenExpirations = ClientRepository.GetTokenExpirations().ToModel();

        return tokenExpirations;
    }

    public virtual List<SelectItemDto> GetTokenUsage()
    {
        var tokenUsage = ClientRepository.GetTokenUsage().ToModel();

        return tokenUsage;
    }

    public virtual List<SelectItemDto> GetAccessTokenTypes()
    {
        var accessTokenTypes = ClientRepository.GetAccessTokenTypes().ToModel();

        return accessTokenTypes;
    }


    public virtual List<string> GetGrantTypes(string grant, int limit = 0)
    {
        var grantTypes = ClientRepository.GetGrantTypes(grant, limit);

        return grantTypes;
    }

    private void PopulateClientRelations(ClientDto client)
    {
        ComboBoxHelpers.PopulateValuesToList(client.AllowedScopesItems, client.AllowedScopes);
        ComboBoxHelpers.PopulateValuesToList(client.PostLogoutRedirectUrisItems, client.PostLogoutRedirectUris);
        ComboBoxHelpers.PopulateValuesToList(client.IdentityProviderRestrictionsItems, client.IdentityProviderRestrictions);
        ComboBoxHelpers.PopulateValuesToList(client.RedirectUrisItems, client.RedirectUris);
        ComboBoxHelpers.PopulateValuesToList(client.AllowedCorsOriginsItems, client.AllowedCorsOrigins);
        ComboBoxHelpers.PopulateValuesToList(client.AllowedGrantTypesItems, client.AllowedGrantTypes);
        ComboBoxHelpers.PopulateValuesToList(client.AllowedIdentityTokenSigningAlgorithmsItems, client.AllowedIdentityTokenSigningAlgorithms);
    }

    public virtual async Task<List<string>> GetScopesAsync(string scope, int limit = 0)
    {
        var scopes = await ClientRepository.GetScopesAsync(scope, limit);

        return scopes;
    }
}