namespace Point.Services.Identity.Application.Interfaces;

public interface IApiScopeService
{
    Task<ApiScopesDto> GetApiScopesAsync(string search, int page = 1, int pageSize = 10);
    Task<ApiScopeDto> GetApiScopeAsync(int id);
    Task<int> AddApiScopeAsync(ApiScopeDto scope);
    Task<bool> CanInsertApiScopeAsync(ApiScopeDto apiScopes);
    Task<int> UpdateApiScopeAsync(ApiScopeDto apiScope);
    Task<int> DeleteApiScopeAsync(ApiScopeDto apiScope);

    Task<ApiScopePropertiesDto> GetApiScopePropertiesAsync(int apiScopeId, int page = 1, int pageSize = 10);
    Task<int> AddApiScopePropertyAsync(ApiScopePropertiesDto apiScopeProperties);
    Task<ApiScopePropertiesDto> GetApiScopePropertyAsync(int apiScopePropertyId);
    Task<bool> CanInsertApiScopePropertyAsync(ApiScopePropertiesDto apiResourceProperty);
    Task<int> DeleteApiScopePropertyAsync(ApiScopePropertiesDto apiScopeProperty);
}