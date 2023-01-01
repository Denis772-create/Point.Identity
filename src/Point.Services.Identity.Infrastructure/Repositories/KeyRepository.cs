using Point.Services.Identity.Domain;

namespace Point.Services.Identity.Infrastructure.Repositories;

public class KeyRepository<TDbContext> : IKeyRepository
    where TDbContext : DbContext, IAdminPersistedGrantDbContext
{
    protected readonly TDbContext DbContext;
    public bool AutoSaveChanges { get; set; } = true;

    public KeyRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<PagedList<Key>> GetKeys(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var pagedList = new PagedList<Key>();

        var clients = await DbContext.Keys.PageBy(x => x.Id, page, pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);

        pagedList.Data.AddRange(clients);

        pagedList.TotalCount = await DbContext.Keys.CountAsync(cancellationToken: cancellationToken);
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public virtual async Task<Key?> GetKey(string id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Keys.Where(x => x.Id == id)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> ExistsKey(string id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Keys
            .Where(x => x.Id == id)
            .AnyAsync(cancellationToken: cancellationToken);
    }

    public async Task DeleteKeyAsync(string id, CancellationToken cancellationToken = default)
    {
        var keyToDelete = await DbContext.Keys.Where(x => x.Id == id).SingleOrDefaultAsync(cancellationToken: cancellationToken);

        if (keyToDelete is null) return;

        DbContext.Keys.Remove(keyToDelete);
        await AutoSaveChangesAsync(cancellationToken);
    }

    public virtual async Task<int> SaveAllChanges(CancellationToken cancellationToken = default)
    {
        return await DbContext.SaveChangesAsync(cancellationToken);
    }

    protected virtual async Task<int> AutoSaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return AutoSaveChanges
            ? await DbContext.SaveChangesAsync(cancellationToken)
            : (int)SavedStatus.WillBeSavedExplicitly;
    }
}