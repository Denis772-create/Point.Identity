namespace Point.Services.Identity.Application.Services;

public class ApiScopeService : IApiScopeService
{
    protected readonly IApiScopeRepository _apiScopeRepository;
    protected readonly IApiScopeServiceResources ApiScopeServiceResources;

    public ApiScopeService(IApiScopeServiceResources apiScopeServiceResources, IApiScopeRepository apiScopeRepository)
    {
        _apiScopeRepository = apiScopeRepository;
        ApiScopeServiceResources = apiScopeServiceResources;
    }

    public virtual async Task<ApiScopesDto> GetApiScopesAsync(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = await _apiScopeRepository.GetApiScopes(search, page, pageSize);

        var apiScopesDto = pagedList.ToModel();

        //TODO: to create audit log

        return apiScopesDto;
    }

    public virtual async Task<ICollection<string>> GetApiScopesNameAsync(string scope, int limit = 0)
    {
        return await _apiScopeRepository.GetApiScopesName(scope, limit);
    }

    public virtual async Task<ApiScopeDto> GetApiScopeAsync(int id)
    {
        var apiScope = await _apiScopeRepository.GetApiScope(id);
        if (apiScope == null)
            throw new UserFriendlyErrorPageException(string.Format(ApiScopeServiceResources.ApiScopeDoesNotExist().Description, id),
                ApiScopeServiceResources.ApiScopeDoesNotExist().Description);

        var apiScopeDto = apiScope.ToModel();

        //TODO: to create audit log

        return apiScopeDto;
    }

    public virtual async Task<int> AddApiScopeAsync(ApiScopeDto scope)
    {
        var canInsert = await CanInsertApiScopeAsync(scope);
        if (!canInsert)
            throw new UserFriendlyViewException(string.Format(ApiScopeServiceResources.ApiScopeExistsValue().Description, scope.Name),
                ApiScopeServiceResources.ApiScopeExistsKey().Description, scope);

        var entityScope = scope.ToEntity();

        var added = await _apiScopeRepository.AddApiScope(entityScope);

        //TODO: to create audit log

        return added;
    }

    public virtual async Task<bool> CanInsertApiScopeAsync(ApiScopeDto apiScopeDto)
    {
        var apiScope = apiScopeDto.ToEntity();

        return await _apiScopeRepository.CanInsertApiScope(apiScope);
    }

    public async Task<int> UpdateApiScopeAsync(ApiScopeDto apiScope)
    {
        var canInsert = await CanInsertApiScopeAsync(apiScope);
        if (!canInsert)
            throw new UserFriendlyViewException(string.Format(ApiScopeServiceResources.ApiScopeExistsValue().Description, apiScope.Name), ApiScopeServiceResources.ApiScopeExistsKey().Description, apiScope);

        var scope = apiScope.ToEntity();

        var originalApiScope = await GetApiScopeAsync(apiScope.Id);

        var updated = await _apiScopeRepository.UpdateApiScopeAsync(scope);

        //TODO: to create audit log

        return updated;
    }

    public async Task<int> DeleteApiScopeAsync(ApiScopeDto apiScope)
    {
        var scope = apiScope.ToEntity();

        var deleted = await _apiScopeRepository.DeleteApiScopeAsync(scope);

        //TODO: to create audit log

        return deleted;
    }

    public virtual async Task<ApiScopePropertiesDto> GetApiScopePropertiesAsync(int apiScopeId, int page = 1, int pageSize = 10)
    {
        var apiScope = await _apiScopeRepository.GetApiScope(apiScopeId);
        if (apiScope == null)
            throw new UserFriendlyErrorPageException(string.Format(ApiScopeServiceResources.ApiScopeDoesNotExist().Description, apiScopeId),
                ApiScopeServiceResources.ApiScopeDoesNotExist().Description);

        PagedList<ApiScopeProperty> pagedList = await _apiScopeRepository.GetApiScopeProperties(apiScopeId, page, pageSize);
        var apiScopePropertiesDto = pagedList.ToModel();
        apiScopePropertiesDto.ApiScopeId = apiScopeId;
        apiScopePropertiesDto.ApiScopeName = await _apiScopeRepository.GetApiScopeName(apiScopeId);

        //TODO: to create audit log

        return apiScopePropertiesDto;
    }

    public virtual async Task<int> AddApiScopePropertyAsync(ApiScopePropertiesDto apiScopeProperties)
    {
        var canInsert = await CanInsertApiScopePropertyAsync(apiScopeProperties);
        if (!canInsert)
        {
            await BuildApiScopePropertiesViewModelAsync(apiScopeProperties);
            throw new UserFriendlyViewException(string.Format(ApiScopeServiceResources.ApiScopePropertyExistsValue().Description, apiScopeProperties.Key), ApiScopeServiceResources.ApiScopePropertyExistsKey().Description, apiScopeProperties);
        }

        var apiScopeProperty = apiScopeProperties.ToEntity();

        var saved = await _apiScopeRepository.AddApiScopeProperty(apiScopeProperties.ApiScopeId, apiScopeProperty);

        //TODO: to create audit log

        return saved;
    }

    private async Task BuildApiScopePropertiesViewModelAsync(ApiScopePropertiesDto apiScopeProperties)
    {
        var apiResourcePropertiesDto = await GetApiScopePropertiesAsync(apiScopeProperties.ApiScopeId);
        apiScopeProperties.ApiScopeProperties.AddRange(apiResourcePropertiesDto.ApiScopeProperties);
        apiScopeProperties.TotalCount = apiResourcePropertiesDto.TotalCount;
    }

    public virtual async Task<ApiScopePropertiesDto> GetApiScopePropertyAsync(int apiScopePropertyId)
    {
        var apiScopeProperty = await _apiScopeRepository.GetApiScopeProperty(apiScopePropertyId);
        if (apiScopeProperty == null) throw new UserFriendlyErrorPageException(string.Format(ApiScopeServiceResources.ApiScopePropertyDoesNotExist().Description, apiScopePropertyId));

        var apiScopePropertiesDto = apiScopeProperty.ToModel();
        apiScopePropertiesDto.ApiScopeId = apiScopeProperty.ScopeId;
        apiScopePropertiesDto.ApiScopeName = await _apiScopeRepository.GetApiScopeName(apiScopeProperty.ScopeId);

        //TODO: to create audit log

        return apiScopePropertiesDto;
    }

    public virtual async Task<bool> CanInsertApiScopePropertyAsync(ApiScopePropertiesDto apiResourceProperty)
    {
        var resource = apiResourceProperty.ToEntity();

        return await _apiScopeRepository.CanInsertApiScopeProperty(resource);
    }

    public async Task<int> DeleteApiScopePropertyAsync(ApiScopePropertiesDto apiScopeProperty)
    {
        var propertyEntity = apiScopeProperty.ToEntity();

        var deleted = await _apiScopeRepository.DeleteApiScopePropertyAsync(propertyEntity);

        //TODO: to create audit log

        return deleted;
    }
}