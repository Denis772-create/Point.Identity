namespace Point.Services.Identity.Infrastructure.Repositories;

public class IdentityResourceRepository<TDbContext> : IIdentityResourceRepository
    where TDbContext : DbContext, IAdminConfigurationDbContext
{
    protected readonly TDbContext DbContext;

    public bool AutoSaveChanges { get; set; } = true;

    public IdentityResourceRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<PagedList<IdentityResource>> GetIdentityResources(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<IdentityResource>();

        Expression<Func<IdentityResource, bool>> searchCondition = x => x.Name.Contains(search);

        var identityResources = await DbContext.IdentityResources
            .WhereIf(!string.IsNullOrEmpty(search), searchCondition)
            .PageBy(x => x.Name, page, pageSize)
            .ToListAsync();

        pagedList.Data.AddRange(identityResources);
        pagedList.TotalCount = await DbContext.IdentityResources
            .WhereIf(!string.IsNullOrEmpty(search), searchCondition)
            .CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public virtual async Task<IdentityResource?> GetIdentityResource(int identityResourceId)
    {
        return await DbContext.IdentityResources
            .Include(x => x.UserClaims)
            .Where(x => x.Id == identityResourceId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<bool> CanInsertIdentityResource(IdentityResource identityResource)
    {
        if (identityResource.Id == 0)
        {
            var existsWithSameName = await DbContext.IdentityResources
                .Where(x => x.Name == identityResource.Name)
                .SingleOrDefaultAsync();
            return existsWithSameName == null;
        }
        else
        {
            var existsWithSameName = await DbContext.IdentityResources
                .Where(x => x.Name == identityResource.Name && x.Id != identityResource.Id)
                .SingleOrDefaultAsync();
            return existsWithSameName == null;
        }
    }

    public virtual async Task<int> AddIdentityResource(IdentityResource identityResource)
    {
        DbContext.IdentityResources.Add(identityResource);

        await AutoSaveChangesAsync();

        return identityResource.Id;
    }

    public virtual async Task<bool> CanInsertIdentityResourceProperty(IdentityResourceProperty identityResourceProperty)
    {
        var existsWithSameName = await DbContext.IdentityResourceProperties
            .Where(x => x.Key == identityResourceProperty.Key && x.IdentityResource.Id == identityResourceProperty.IdentityResourceId)
            .SingleOrDefaultAsync();
        return existsWithSameName == null;
    }

    public virtual async Task<PagedList<IdentityResourceProperty>> GetIdentityResourceProperties(int identityResourceId, int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<IdentityResourceProperty>();

        var properties = await DbContext.IdentityResourceProperties
            .Where(x => x.IdentityResource.Id == identityResourceId)
            .PageBy(x => x.Id, page, pageSize)
            .ToListAsync();

        pagedList.Data.AddRange(properties);
        pagedList.TotalCount = await DbContext.IdentityResourceProperties
            .Where(x => x.IdentityResource.Id == identityResourceId)
            .CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public virtual async Task<IdentityResourceProperty?> GetIdentityResourceProperty(int identityResourcePropertyId)
    {
        return await DbContext.IdentityResourceProperties
            .Include(x => x.IdentityResource)
            .Where(x => x.Id == identityResourcePropertyId)
            .SingleOrDefaultAsync();
    }

    public virtual async Task<int> AddIdentityResourceProperty(int identityResourceId, IdentityResourceProperty identityResourceProperty)
    {
        var identityResource = await DbContext.IdentityResources
            .Where(x => x.Id == identityResourceId)
            .SingleOrDefaultAsync();

        identityResourceProperty.IdentityResource = identityResource;
        await DbContext.IdentityResourceProperties.AddAsync(identityResourceProperty);

        return await AutoSaveChangesAsync();
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