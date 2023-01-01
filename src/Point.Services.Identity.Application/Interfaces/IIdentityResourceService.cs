namespace Point.Services.Identity.Application.Interfaces;

public interface IIdentityResourceService
{
    Task<IdentityResourcesDto> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10);
    Task<IdentityResourceDto> GetIdentityResourceAsync(int identityResourceId);
    Task<bool> CanInsertIdentityResourceAsync(IdentityResourceDto identityResource);
    Task<int> AddIdentityResourceAsync(IdentityResourceDto identityResource);
    Task<int> UpdateIdentityResourceAsync(IdentityResourceDto identityResource);
    Task<int> DeleteIdentityResourceAsync(IdentityResourceDto identityResource);


    Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertiesAsync(int identityResourceId, int page = 1, int pageSize = 10);
    Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertyAsync(int identityResourcePropertyId);
    Task<int> AddIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperties);
    Task<bool> CanInsertIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourcePropertiesDto);
    Task<int> DeleteIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperty);
}