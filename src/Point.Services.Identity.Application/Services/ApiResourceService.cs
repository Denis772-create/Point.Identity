namespace Point.Services.Identity.Application.Services;

public class ApiResourceService : IApiResourceService
{
    protected readonly IApiResourceRepository _apiResourceRepository;
    protected readonly IApiResourceServiceResources ApiResourceServiceResources;
    private const string SharedSecret = "SharedSecret";


    public ApiResourceService(IApiResourceRepository apiResourceRepository,
        IApiResourceServiceResources apiResourceServiceResources)
    {
        this._apiResourceRepository = apiResourceRepository;
        ApiResourceServiceResources = apiResourceServiceResources;
    }

    public async Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = await _apiResourceRepository.GetApiResources(search, page, pageSize);
        var apiResourcesDto = pagedList.ToModel();

        //TODO: to create audit log

        return apiResourcesDto;
    }

    public virtual async Task<ApiResourceDto> GetApiResourceAsync(int id)
    {
        var apiResource = await _apiResourceRepository.GetApiResource(id);
        if (apiResource == null) 
            throw new UserFriendlyErrorPageException(ApiResourceServiceResources.ApiResourceDoesNotExist().Description,
                ApiResourceServiceResources.ApiResourceDoesNotExist().Description);
        
        var apiResourceDto = apiResource.ToModel();

        //TODO: to create audit log

        return apiResourceDto;
    }

    public virtual async Task<int> AddApiResourceAsync(ApiResourceDto apiResource)
    {
        var canInsert = await CanInsertApiResourceAsync(apiResource);
        if (!canInsert)
        {
            throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name),
                ApiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
        }

        var resource = apiResource.ToEntity();

        var added = await _apiResourceRepository.AddApiResource(resource);

        //TODO: to create audit log

        return added;
    }

    public async Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource)
    {
        var canInsert = await CanInsertApiResourceAsync(apiResource);
        if (!canInsert)
        {
            throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name), ApiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
        }

        var resource = apiResource.ToEntity();

        var originalApiResource = await GetApiResourceAsync(apiResource.Id);

        var updated = await _apiResourceRepository.UpdateApiResourceAsync(resource);

        //TODO: to create audit log

        return updated;
    }

    public async Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource)
    {
        var resource = apiResource.ToEntity();

        var deleted = await _apiResourceRepository.DeleteApiResourceAsync(resource);

        //TODO: to create audit log

        return deleted;
    }

    public virtual async Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
    {
        var apiResource = await _apiResourceRepository.GetApiResource(apiResourceId);
        if (apiResource == null) 
            throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId),
            ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

        var pagedList = await _apiResourceRepository.GetApiSecrets(apiResourceId, page, pageSize);
        var apiSecretsDto = pagedList.ToModel();
        apiSecretsDto.ApiResourceId = apiResourceId;
        apiSecretsDto.ApiResourceName = await _apiResourceRepository.GetApiResourceName(apiResourceId);

        // remove secret values for dto
        apiSecretsDto.ApiSecrets.ForEach(x => x.Value = null);

        //TODO: to create audit log

        return apiSecretsDto;
    }

    public virtual async Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId)
    {
        var apiSecret = await _apiResourceRepository.GetApiSecret(apiSecretId);
        if (apiSecret == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiSecretDoesNotExist().Description, apiSecretId), ApiResourceServiceResources.ApiSecretDoesNotExist().Description);
        var apiSecretsDto = apiSecret.ToModel();

        // remove secret value for dto
        apiSecretsDto.Value = null;

        //TODO: to create audit log

        return apiSecretsDto;
    }

    public virtual async Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret)
    {
        HashApiSharedSecret(apiSecret);

        var secret = apiSecret.ToEntity();

        var added = await _apiResourceRepository.AddApiSecret(apiSecret.ApiResourceId, secret);

        //TODO: to create audit log

        return added;
    }

    public async Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret)
    {
        var secret = apiSecret.ToEntity();

        var deleted = await _apiResourceRepository.DeleteApiSecretAsync(secret);

        //TODO: to create audit log

        return deleted;
    }

    public virtual async Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10)
    {
        var apiResource = await _apiResourceRepository.GetApiResource(apiResourceId);
        if (apiResource == null) 
            throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

        var pagedList = await _apiResourceRepository.GetApiResourcePropertiesAsync(apiResourceId, page, pageSize);
        var apiResourcePropertiesDto = pagedList.ToModel();
        apiResourcePropertiesDto.ApiResourceId = apiResourceId;
        apiResourcePropertiesDto.ApiResourceName = await _apiResourceRepository.GetApiResourceName(apiResourceId);

        //TODO: to create audit log

        return apiResourcePropertiesDto;
    }

    public virtual async Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId)
    {
        var apiResourceProperty = await _apiResourceRepository.GetApiResourceProperty(apiResourcePropertyId);
        if (apiResourceProperty == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourcePropertyDoesNotExist().Description, apiResourcePropertyId));

        var apiResourcePropertiesDto = apiResourceProperty.ToModel();
        apiResourcePropertiesDto.ApiResourceId = apiResourceProperty.ApiResourceId;
        apiResourcePropertiesDto.ApiResourceName = await _apiResourceRepository.GetApiResourceName(apiResourceProperty.ApiResourceId);

        //TODO: to create audit log

        return apiResourcePropertiesDto;
    }

    public virtual async Task<int> AddApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperties)
    {
        var canInsert = await CanInsertApiResourcePropertyAsync(apiResourceProperties);
        if (!canInsert)
        {
            await BuildApiResourcePropertiesViewModelAsync(apiResourceProperties);
            throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourcePropertyExistsValue().Description, apiResourceProperties.Key),
                ApiResourceServiceResources.ApiResourcePropertyExistsKey().Description, apiResourceProperties);
        }

        var apiResourceProperty = apiResourceProperties.ToEntity();

        var saved = await _apiResourceRepository.AddApiResourcePropertyAsync(apiResourceProperties.ApiResourceId, apiResourceProperty);

        //TODO: to create audit log

        return saved;
    }

    public async Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
    {
        var propertyEntity = apiResourceProperty.ToEntity();

        var deleted = await _apiResourceRepository.DeleteApiResourcePropertyAsync(propertyEntity);

        //TODO: to create audit log

        return deleted;
    }

    private async Task BuildApiResourcePropertiesViewModelAsync(ApiResourcePropertiesDto apiResourceProperties)
    {
        var apiResourcePropertiesDto = await GetApiResourcePropertiesAsync(apiResourceProperties.ApiResourceId);
        apiResourceProperties.ApiResourceProperties.AddRange(apiResourcePropertiesDto.ApiResourceProperties);
        apiResourceProperties.TotalCount = apiResourcePropertiesDto.TotalCount;
    }

    public virtual async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
    {
        var resource = apiResourceProperty.ToEntity();

        return await _apiResourceRepository.CanInsertApiResourcePropertyAsync(resource);
    }

    private void HashApiSharedSecret(ApiSecretsDto apiSecret)
    {
        if (apiSecret.Type != SharedSecret) return;

        apiSecret.Value = apiSecret.HashTypeEnum switch
        {
            HashType.Sha256 => apiSecret.Value.Sha256(),
            HashType.Sha512 => apiSecret.Value.Sha512(),
            _ => apiSecret.Value
        };
    }

    public virtual async Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource)
    {
        var resource = apiResource.ToEntity();

        return await _apiResourceRepository.CanInsertApiResource(resource);
    }

}