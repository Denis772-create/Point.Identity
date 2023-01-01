namespace Point.Services.Identity.Application.Interfaces;

public interface IApiResourceService
{
    // Resources
    Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10);
    Task<ApiResourceDto> GetApiResourceAsync(int id);
    Task<int> AddApiResourceAsync(ApiResourceDto apiResource);
    Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource);
    Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource);
    // Secrets
    Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10);
    Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId);
    Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret);
    Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret);
    // ResourceProperties
    Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10);
    Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId);
    Task<int> AddApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperties);
    Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty);
}