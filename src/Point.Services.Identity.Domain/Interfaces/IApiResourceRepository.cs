namespace Point.Services.Identity.Domain.Interfaces;

public interface IApiResourceRepository
{
    // Resources
    Task<PagedList<ApiResource>> GetApiResources(string search, int page = 1, int pageSize = 10);
    Task<ApiResource?> GetApiResource(int apiResourceId);
    Task<bool> CanInsertApiResource(ApiResource apiResource);
    Task<int> AddApiResource(ApiResource apiResource);
    Task<string?> GetApiResourceName(int apiResourceId);
    Task<int> UpdateApiResourceAsync(ApiResource apiResource);
    Task<int> DeleteApiResourceAsync(ApiResource apiResource);
    // Secrets
    Task<PagedList<ApiResourceSecret>> GetApiSecrets(int apiResourceId, int page = 1, int pageSize = 10);
    Task<ApiResourceSecret?> GetApiSecret(int apiSecretId);
    Task<int> AddApiSecret(int apiResourceId, ApiResourceSecret apiSecret);
    Task<int> DeleteApiSecretAsync(ApiResourceSecret apiSecret);
    // ResourceProperties
    Task<PagedList<ApiResourceProperty>> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10);
    Task<ApiResourceProperty?> GetApiResourceProperty(int apiResourcePropertyId);
    Task<bool> CanInsertApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty);
    Task<int> AddApiResourcePropertyAsync(int apiResourceId, ApiResourceProperty apiResourceProperty);
    Task<int> DeleteApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty);
}