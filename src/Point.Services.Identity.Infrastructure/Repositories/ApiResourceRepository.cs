﻿namespace Point.Services.Identity.Infrastructure.Repositories;

public class ApiResourceRepository<TDbContext> : IApiResourceRepository
    where TDbContext : DbContext, IAdminConfigurationDbContext
{
    protected readonly TDbContext DbContext;

    public bool AutoSaveChanges { get; set; } = true;

    public ApiResourceRepository(TDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(typeof(TDbContext).ToString());
    }

    public async Task<PagedList<ApiResource>> GetApiResources(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<ApiResource>();
        Expression<Func<ApiResource, bool>> searchCondition = x => x.Name.Contains(search);

        var apiResources = await DbContext.ApiResources.WhereIf(!string.IsNullOrEmpty(search), searchCondition)
            .PageBy(x => x.Name, page, pageSize).ToListAsync();

        pagedList.Data.AddRange(apiResources);
        pagedList.TotalCount = await DbContext.ApiResources.WhereIf(!string.IsNullOrEmpty(search), searchCondition)
            .CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public async Task<ApiResource?> GetApiResource(int id)
    {
        return await DbContext.ApiResources
            .Include(x => x.UserClaims)
            .Include(x => x.Scopes)
            .Where(x => x.Id == id)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<bool> CanInsertApiResource(ApiResource apiResource)
    {
        if (apiResource.Id == 0)
        {
            var existsWithSameName = await DbContext.ApiResources.Where(x => x.Name == apiResource.Name).SingleOrDefaultAsync();
            return existsWithSameName == null;
        }
        else
        {
            var existsWithSameName = await DbContext.ApiResources.Where(x => x.Name == apiResource.Name && x.Id != apiResource.Id).SingleOrDefaultAsync();
            return existsWithSameName == null;
        }
    }

    public virtual async Task<int> AddApiResource(ApiResource apiResource)
    {
        DbContext.ApiResources.Add(apiResource);

        await AutoSaveChangesAsync();

        return apiResource.Id;
    }

    public async Task<int> DeleteApiResourceAsync(ApiResource apiResource)
    {
        var resource = await DbContext.ApiResources
            .Where(x => x.Id == apiResource.Id)
            .SingleOrDefaultAsync();

        if (resource is null) return -1;

        DbContext.Remove(resource);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<PagedList<ApiResourceSecret>> GetApiSecrets(int apiResourceId, int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<ApiResourceSecret>();
        var apiSecrets = await DbContext.ApiSecrets
            .Where(x => x.ApiResource.Id == apiResourceId)
            .PageBy(x => x.Id, page, pageSize)
            .ToListAsync();

        pagedList.Data.AddRange(apiSecrets);
        pagedList.TotalCount = await DbContext.ApiSecrets
            .Where(x => x.ApiResource.Id == apiResourceId)
            .CountAsync();

        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public virtual Task<ApiResourceSecret?> GetApiSecret(int apiSecretId)
    {
        return DbContext.ApiSecrets
            .Include(x => x.ApiResource)
            .Where(x => x.Id == apiSecretId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<int> AddApiSecret(int apiResourceId, ApiResourceSecret apiSecret)
    {
        apiSecret.ApiResource = await DbContext.ApiResources.Where(x => x.Id == apiResourceId).SingleOrDefaultAsync();
        await DbContext.ApiSecrets.AddAsync(apiSecret);

        return await AutoSaveChangesAsync();
    }

    public async Task<int> DeleteApiSecretAsync(ApiResourceSecret apiSecret)
    {
        var apiSecretToDelete = await DbContext.ApiSecrets.Where(x => x.Id == apiSecret.Id).SingleOrDefaultAsync();

        if (apiSecretToDelete is null) return -1;

        DbContext.ApiSecrets.Remove(apiSecretToDelete);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<PagedList<ApiResourceProperty>> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<ApiResourceProperty>();

        var properties = await DbContext.ApiResourceProperties
            .Where(x => x.ApiResource.Id == apiResourceId)
            .PageBy(x => x.Id, page, pageSize)
            .ToListAsync();

        pagedList.Data.AddRange(properties);
        pagedList.TotalCount = await DbContext.ApiResourceProperties
            .Where(x => x.ApiResource.Id == apiResourceId)
            .CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public virtual Task<ApiResourceProperty?> GetApiResourceProperty(int apiResourcePropertyId)
    {
        return DbContext.ApiResourceProperties
            .Include(x => x.ApiResource)
            .Where(x => x.Id == apiResourcePropertyId)
            .SingleOrDefaultAsync();
    }

    public virtual async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty)
    {
        var existsWithSameName = await DbContext.ApiResourceProperties
            .Where(x => x.Key == apiResourceProperty.Key
            && x.ApiResource.Id == apiResourceProperty.ApiResourceId)
            .SingleOrDefaultAsync();
        return existsWithSameName == null;
    }

    protected virtual async Task<int> AutoSaveChangesAsync()
    {
        return AutoSaveChanges
            ? await DbContext.SaveChangesAsync()
            : (int)SavedStatus.WillBeSavedExplicitly;
    }

    public virtual async Task<int> AddApiResourcePropertyAsync(int apiResourceId, ApiResourceProperty apiResourceProperty)
    {
        var apiResource = await DbContext.ApiResources
            .Where(x => x.Id == apiResourceId)
            .SingleOrDefaultAsync();

        apiResourceProperty.ApiResource = apiResource;
        await DbContext.ApiResourceProperties.AddAsync(apiResourceProperty);

        return await AutoSaveChangesAsync();
    }

    public async Task<int> DeleteApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty)
    {
        var propertyToDelete = await DbContext.ApiResourceProperties.Where(x => x.Id == apiResourceProperty.Id).SingleOrDefaultAsync();

        if (propertyToDelete is null) return -1;

        DbContext.ApiResourceProperties.Remove(propertyToDelete);
        return await AutoSaveChangesAsync();
    }

    public virtual async Task<int> SaveAllChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }

    public virtual async Task<string?> GetApiResourceName(int apiResourceId)
    {
        var apiResourceName = await DbContext.ApiResources.Where(x => x.Id == apiResourceId)
            .Select(x => x.Name)
            .SingleOrDefaultAsync();

        return apiResourceName;
    }

    public async Task<int> UpdateApiResourceAsync(ApiResource apiResource)
    {
        //Remove old relations
        await RemoveApiResourceClaimsAsync(apiResource);
        await RemoveApiResourceScopesAsync(apiResource);

        //Update with new data
        DbContext.ApiResources.Update(apiResource);

        return await AutoSaveChangesAsync();
    }

    private async Task RemoveApiResourceClaimsAsync(ApiResource apiResource)
    {
        //Remove old api resource claims
        var apiResourceClaims = await DbContext.ApiResourceClaims
            .Where(x => x.ApiResource.Id == apiResource.Id)
            .ToListAsync();

        DbContext.ApiResourceClaims.RemoveRange(apiResourceClaims);
    }

    private async Task RemoveApiResourceScopesAsync(ApiResource apiResource)
    {
        //Remove old api resource scopes
        var apiResourceScopes = await DbContext.ApiResourceScopes
            .Where(x => x.ApiResource.Id == apiResource.Id)
            .ToListAsync();

        DbContext.ApiResourceScopes.RemoveRange(apiResourceScopes);
    }

}