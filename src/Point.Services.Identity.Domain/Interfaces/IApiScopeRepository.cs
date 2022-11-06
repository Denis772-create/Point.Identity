namespace Point.Services.Identity.Domain.Interfaces;

public interface IApiScopeRepository
{
    // Scopes
    Task<PagedList<ApiScope>> GetApiScopes(string search, int page = 1, int pageSize = 10);
    Task<ApiScope?> GetApiScope(int apiScopeId);
    Task<int> AddApiScope(ApiScope apiScope);
    Task<bool> CanInsertApiScope(ApiScope apiScope);
    Task<ICollection<string>> GetApiScopesName(string scope, int limit = 0);
    Task<string?> GetApiScopeName(int apiScopeId);

    // Scope Properties
    Task<PagedList<ApiScopeProperty>> GetApiScopeProperties(int apiScopeId, int page = 1, int pageSize = 10);
    Task<ApiScopeProperty?> GetApiScopeProperty(int apiScopePropertyId);
    Task<int> AddApiScopeProperty(int apiScopeId, ApiScopeProperty apiScopeProperty);
    Task<bool> CanInsertApiScopeProperty(ApiScopeProperty apiScopeProperty);

    // Save
    Task<int> SaveAllChanges();
    bool AutoSaveChanges { get; set; }
}