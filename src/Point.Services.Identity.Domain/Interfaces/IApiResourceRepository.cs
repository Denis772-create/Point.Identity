namespace Point.Services.Identity.Domain.Interfaces;

public interface IApiResourceRepository
{
    Task<PagedList<ApiResource>> GetApiResources(string search, int page = 1, int pageSize = 10);
    Task<ApiResource?> GetApiResource(int apiResourceId);
    Task<bool> CanInsertApiResource(ApiResource apiResource);
    Task<int> AddApiResource(ApiResource apiResource);
    Task<string?> GetApiResourceName(int apiResourceId);


    Task<PagedList<ApiResourceSecret>> GetApiSecrets(int apiResourceId, int page = 1, int pageSize = 10);
    Task<ApiResourceSecret?> GetApiSecret(int apiSecretId);
    Task<int> AddApiSecret(int apiResourceId, ApiResourceSecret apiSecret);


    Task<PagedList<ApiResourceProperty>> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10);
    Task<ApiResourceProperty?> GetApiResourceProperty(int apiResourcePropertyId);
    Task<bool> CanInsertApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty);
    Task<int> AddApiResourcePropertyAsync(int apiResourceId, ApiResourceProperty apiResourceProperty);
}