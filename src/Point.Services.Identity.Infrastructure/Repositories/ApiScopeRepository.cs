namespace Point.Services.Identity.Infrastructure.Repositories;

public class ApiScopeRepository<TDbContext> : IApiScopeRepository
    where TDbContext : DbContext, IAdminConfigurationDbContext
{
    protected readonly TDbContext DbContext;

    public bool AutoSaveChanges { get; set; } = true;

    public ApiScopeRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<PagedList<ApiScope>> GetApiScopes(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<ApiScope>();
        Expression<Func<ApiScope, bool>> searchCondition = x => x.Name.Contains(search);

        var filteredApiScopes = DbContext.ApiScopes
            .WhereIf(!string.IsNullOrEmpty(search), searchCondition);

        var apiScopes = await filteredApiScopes
            .PageBy(x => x.Name, page, pageSize).ToListAsync();

        pagedList.Data.AddRange(apiScopes);
        pagedList.TotalCount = await filteredApiScopes.CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public virtual async Task<ApiScope?> GetApiScope(int apiScopeId)
    {
        return await DbContext.ApiScopes
            .Include(x => x.UserClaims)
            .Include(x => x.Properties)
            .Where(x => x.Id == apiScopeId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<int> AddApiScope(ApiScope apiScope)
    {
        await DbContext.ApiScopes.AddAsync(apiScope);

        await AutoSaveChangesAsync();

        return apiScope.Id;
    }

    public virtual async Task<string?> GetApiScopeName(int apiScopeId)
    {
        var apiScopeName = await DbContext.ApiScopes.Where(x => x.Id == apiScopeId).Select(x => x.Name).SingleOrDefaultAsync();

        return apiScopeName;
    }

    public async Task<int> UpdateApiScopeAsync(ApiScope apiScope)
    {
        // Remove old relations
        await RemoveApiScopeClaimsAsync(apiScope);

        // Update with new data
        DbContext.ApiScopes.Update(apiScope);

        return await AutoSaveChangesAsync();
    }

    private async Task RemoveApiScopeClaimsAsync(ApiScope apiScope)
    {
        //Remove old api scope claims
        var apiScopeClaims = await DbContext.ApiScopeClaims
            .Where(x => x.Scope.Id == apiScope.Id)
            .ToListAsync();

        DbContext.ApiScopeClaims.RemoveRange(apiScopeClaims);
    }

    public async Task<int> DeleteApiScopeAsync(ApiScope apiScope)
    {
        var apiScopeToDelete = await DbContext.ApiScopes.Where(x => x.Id == apiScope.Id).SingleOrDefaultAsync();
        if (apiScopeToDelete is null) return -1;

        DbContext.ApiScopes.Remove(apiScopeToDelete);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<PagedList<ApiScopeProperty>> GetApiScopeProperties(int apiScopeId, int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<ApiScopeProperty>();

        var apiScopeProperties = DbContext.ApiScopeProperties.Where(x => x.Scope.Id == apiScopeId);

        var properties = await apiScopeProperties.PageBy(x => x.Id, page, pageSize)
            .ToListAsync();

        pagedList.Data.AddRange(properties);
        pagedList.TotalCount = await apiScopeProperties.CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public Task<ApiScopeProperty?> GetApiScopeProperty(int apiScopePropertyId)
    {
        return DbContext.ApiScopeProperties
            .Include(x => x.Scope)
            .Where(x => x.Id == apiScopePropertyId)
            .SingleOrDefaultAsync();
    }

    public virtual async Task<int> AddApiScopeProperty(int apiScopeId, ApiScopeProperty apiScopeProperty)
    {
        var apiScope = await DbContext.ApiScopes
            .Where(x => x.Id == apiScopeId)
            .SingleOrDefaultAsync();

        apiScopeProperty.Scope = apiScope;
        await DbContext.ApiScopeProperties.AddAsync(apiScopeProperty);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<bool> CanInsertApiScopeProperty(ApiScopeProperty apiScopeProperty)
    {
        var existsWithSameName = await DbContext.ApiScopeProperties
            .Where(x => x.Key == apiScopeProperty.Key && x.Scope.Id == apiScopeProperty.Scope.Id)
            .SingleOrDefaultAsync();
        return existsWithSameName == null;
    }

    public async Task<int> DeleteApiScopePropertyAsync(ApiScopeProperty apiScopeProperty)
    {
        var propertyToDelete = await DbContext.ApiScopeProperties
            .Where(x => x.Id == apiScopeProperty.Id)
            .SingleOrDefaultAsync();

        if (propertyToDelete == null) return -1;

        DbContext.ApiScopeProperties.Remove(propertyToDelete);
        return await AutoSaveChangesAsync();
    }

    public virtual async Task<bool> CanInsertApiScope(ApiScope apiScope)
    {
        if (apiScope.Id == 0)
        {
            var existsWithSameName = await DbContext.ApiScopes
                .Where(x => x.Name == apiScope.Name)
                .SingleOrDefaultAsync();
            return existsWithSameName == null;
        }
        else
        {
            var existsWithSameName = await DbContext.ApiScopes
                .Where(x => x.Name == apiScope.Name && x.Id != apiScope.Id)
                .SingleOrDefaultAsync();
            return existsWithSameName == null;
        }
    }

    public virtual async Task<ICollection<string>> GetApiScopesName(string scope, int limit = 0)
    {
        var apiScopes = await DbContext.ApiScopes
            .WhereIf(!string.IsNullOrEmpty(scope), x => x.Name.Contains(scope))
            .TakeIf(x => x.Id, limit > 0, limit)
            .Select(x => x.Name).ToListAsync();

        return apiScopes;
    }

    protected virtual async Task<int> AutoSaveChangesAsync()
    {
        return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
    }

    public virtual async Task<int> SaveAllChanges()
    {
        return await DbContext.SaveChangesAsync();
    }
}