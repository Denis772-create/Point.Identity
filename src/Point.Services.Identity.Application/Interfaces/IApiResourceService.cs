namespace Point.Services.Identity.Application.Interfaces;

public interface IApiResourceService
{
    Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10);
    Task<ApiResourceDto> GetApiResourceAsync(int id);
    Task<int> AddApiResourceAsync(ApiResourceDto apiResource);


    Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10);
    Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId);
    Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret);


    Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10);
    Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId);
    Task<int> AddApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperties);
}