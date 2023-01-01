namespace Point.Services.Identity.Domain.Interfaces
{
    public interface IKeyRepository
    {
        Task<PagedList<Key>> GetKeys(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<Key?> GetKey(string id, CancellationToken cancellationToken = default);
        Task<bool> ExistsKey(string id, CancellationToken cancellationToken = default);


        Task<int> SaveAllChanges(CancellationToken cancellationToken = default);
        bool AutoSaveChanges { get; set; }
    }
}
