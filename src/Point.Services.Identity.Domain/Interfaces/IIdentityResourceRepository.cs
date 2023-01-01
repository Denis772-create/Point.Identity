namespace Point.Services.Identity.Domain.Interfaces;

public interface IIdentityResourceRepository
{
    Task<PagedList<IdentityResource>> GetIdentityResources(string search, int page = 1, int pageSize = 10);
    Task<IdentityResource?> GetIdentityResource(int identityResourceId);
    Task<bool> CanInsertIdentityResource(IdentityResource identityResource);
    Task<int> AddIdentityResource(IdentityResource identityResource);


    Task<bool> CanInsertIdentityResourceProperty(IdentityResourceProperty identityResourceProperty);
    Task<PagedList<IdentityResourceProperty>> GetIdentityResourceProperties(int identityResourceId, int page = 1, int pageSize = 10);
    Task<IdentityResourceProperty?> GetIdentityResourceProperty(int identityResourcePropertyId);
    Task<int> AddIdentityResourceProperty(int identityResourceId, IdentityResourceProperty identityResourceProperty);


    Task<int> SaveAllChangesAsync();
    bool AutoSaveChanges { get; set; }
}