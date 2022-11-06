using IdentityServer4.EntityFramework.Entities;

namespace Point.Services.Identity.Infrastructure.Repositories;

public class ClientRepository<TDbContext> : IClientRepository
    where TDbContext : DbContext, IAdminConfigurationDbContext
{
    protected readonly TDbContext DbContext;
    public bool AutoSaveChanges { get; set; } = true;

    public ClientRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<int> AddClient(Client client)
    {
        DbContext.Clients.Add(client);

        await AutoSaveChangesAsync();

        return client.Id;
    }

    public virtual async Task<bool> CanInsertClient(Client client, bool isCloned = false)
    {
        if (client.Id == 0 || isCloned)
        {
            var existsWithClientName = await DbContext.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();
            return existsWithClientName == null;
        }
        else
        {
            var existsWithClientName = await DbContext.Clients.Where(x => x.ClientId == client.ClientId && x.Id != client.Id).SingleOrDefaultAsync();
            return existsWithClientName == null;
        }
    }

    public virtual async Task<Client?> GetClient(int clientId)
    {
        return await DbContext.Clients
            .Include(x => x.AllowedGrantTypes)
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .Include(x => x.ClientSecrets)
            .Include(x => x.Claims)
            .Include(x => x.IdentityProviderRestrictions)
            .Include(x => x.AllowedCorsOrigins)
            .Include(x => x.Properties)
            .Where(x => x.Id == clientId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<PagedList<Client>> GetClients(string search = "", int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<Client>();

        Expression<Func<Client, bool>> searchCondition = x => x.ClientId.Contains(search) || x.ClientName.Contains(search);
        var clients = await DbContext.Clients
            .WhereIf(!string.IsNullOrEmpty(search), searchCondition)
            .PageBy(x => x.Id, page, pageSize)
            .ToListAsync();

        pagedList.Data.AddRange(clients);
        pagedList.TotalCount = await DbContext.Clients
            .WhereIf(!string.IsNullOrEmpty(search), searchCondition)
            .CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public virtual async Task<int> AddClientSecret(int clientId, ClientSecret clientSecret)
    {
        var client = await DbContext.Clients
            .Where(x => x.Id == clientId)
            .SingleOrDefaultAsync();

        clientSecret.Client = client;

        await DbContext.ClientSecrets.AddAsync(clientSecret);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<PagedList<ClientSecret>> GetClientSecrets(int clientId, int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<ClientSecret>();

        var secrets = await DbContext.ClientSecrets
            .Where(x => x.Client.Id == clientId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        pagedList.Data.AddRange(secrets);
        pagedList.TotalCount = await DbContext.ClientSecrets.Where(x => x.Client.Id == clientId).CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public Task<ClientSecret?> GetClientSecret(int clientSecretId)
    {
        return DbContext.ClientSecrets
            .Include(x => x.Client)
            .Where(x => x.Id == clientSecretId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<PagedList<ClientProperty>> GetClientProperties(int clientId, int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<ClientProperty>();

        var properties = await DbContext.ClientProperties.Where(x => x.Client.Id == clientId).PageBy(x => x.Id, page, pageSize)
            .ToListAsync();

        pagedList.Data.AddRange(properties);
        pagedList.TotalCount = await DbContext.ClientProperties.Where(x => x.Client.Id == clientId).CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public virtual async Task<ClientProperty?> GetClientProperty(int clientPropertyId)
    {
        return await DbContext.ClientProperties
            .Include(x => x.Client)
            .Where(x => x.Id == clientPropertyId)
            .SingleOrDefaultAsync();
    }

    public virtual async Task<int> AddClientProperty(int clientId, ClientProperty clientProperty)
    {
        var client = await DbContext.Clients
            .Where(x => x.Id == clientId)
            .SingleOrDefaultAsync();

        clientProperty.Client = client;
        await DbContext.ClientProperties.AddAsync(clientProperty);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<PagedList<ClientClaim>> GetClientClaims(int clientId, int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<ClientClaim>();

        var claims = await DbContext.ClientClaims
            .Where(x => x.Client.Id == clientId)
            .PageBy(x => x.Id, page, pageSize)
            .ToListAsync();

        pagedList.Data.AddRange(claims);
        pagedList.TotalCount = await DbContext.ClientClaims
            .Where(x => x.Client.Id == clientId)
            .CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public virtual async Task<ClientClaim?> GetClientClaim(int clientClaimId)
    {
        return await DbContext.ClientClaims
            .Include(x => x.Client)
            .Where(x => x.Id == clientClaimId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<int> AddClientClaim(int clientId, ClientClaim clientClaim)
    {
        var client = await DbContext.Clients
            .Where(x => x.Id == clientId)
            .SingleOrDefaultAsync();

        clientClaim.Client = client;
        await DbContext.ClientClaims.AddAsync(clientClaim);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<(string? ClientId, string? ClientName)> GetClientId(int clientId)
    {
        var client = await DbContext.Clients
            .Where(x => x.Id == clientId)
            .Select(x => new { x.ClientId, x.ClientName })
            .SingleOrDefaultAsync();

        return (client?.ClientId, client?.ClientName);
    }

    protected virtual async Task<int> AutoSaveChangesAsync()
    {
        return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
    }

    public virtual async Task<int> SaveAllChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }
}