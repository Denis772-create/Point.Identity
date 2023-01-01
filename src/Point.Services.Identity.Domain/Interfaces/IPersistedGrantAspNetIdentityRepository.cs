namespace Point.Services.Identity.Domain.Interfaces;

public interface IPersistedGrantAspNetIdentityRepository
{
    Task<PagedList<PersistedGrantDataView>> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10);
    Task<PagedList<PersistedGrant>> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10);
    Task<PersistedGrant?> GetPersistedGrantAsync(string key);
    Task<bool> ExistsPersistedGrantsAsync(string subjectId);
    Task<bool> ExistsPersistedGrantAsync(string key);
    Task<int> SaveAllChangesAsync();
    bool AutoSaveChanges { get; set; }

}