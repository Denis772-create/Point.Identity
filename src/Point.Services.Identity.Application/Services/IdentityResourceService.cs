namespace Point.Services.Identity.Application.Services;

public class IdentityResourceService : IIdentityResourceService
{
    protected readonly IIdentityResourceRepository IdentityResourceRepository;
    protected readonly IIdentityResourceServiceResources IdentityResourceServiceResources;

    public IdentityResourceService(IIdentityResourceRepository identityResourceRepository,
        IIdentityResourceServiceResources identityResourceServiceResources)
    {
        IdentityResourceRepository = identityResourceRepository;
        IdentityResourceServiceResources = identityResourceServiceResources;
    }

    public virtual async Task<IdentityResourcesDto> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = await IdentityResourceRepository.GetIdentityResources(search, page, pageSize);
        var identityResourcesDto = pagedList.ToModel();

        // TODO: create audit log

        return identityResourcesDto;
    }

    public virtual async Task<IdentityResourceDto> GetIdentityResourceAsync(int identityResourceId)
    {
        var identityResource = await IdentityResourceRepository.GetIdentityResource(identityResourceId);
        if (identityResource == null)
            throw new UserFriendlyErrorPageException(string.Format(IdentityResourceServiceResources.IdentityResourceDoesNotExist().Description, identityResourceId));

        var identityResourceDto = identityResource.ToModel();

        // TODO: create audit log

        return identityResourceDto;
    }

    public virtual async Task<bool> CanInsertIdentityResourceAsync(IdentityResourceDto identityResource)
    {
        var resource = identityResource.ToEntity();

        return await IdentityResourceRepository.CanInsertIdentityResource(resource);
    }

    public virtual async Task<int> AddIdentityResourceAsync(IdentityResourceDto identityResource)
    {
        var canInsert = await CanInsertIdentityResourceAsync(identityResource);
        if (!canInsert)
            throw new UserFriendlyViewException(string.Format(IdentityResourceServiceResources.IdentityResourceExistsValue().Description, identityResource.Name),
                IdentityResourceServiceResources.IdentityResourceExistsKey().Description, identityResource);

        var resource = identityResource.ToEntity();

        var saved = await IdentityResourceRepository.AddIdentityResource(resource);

        // TODO: create audit log

        return saved;
    }

    public virtual async Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertiesAsync(int identityResourceId, int page = 1, int pageSize = 10)
    {
        var identityResource = await IdentityResourceRepository.GetIdentityResource(identityResourceId);
        if (identityResource == null)
            throw new UserFriendlyErrorPageException(string.Format(IdentityResourceServiceResources.IdentityResourceDoesNotExist().Description, identityResourceId), IdentityResourceServiceResources.IdentityResourceDoesNotExist().Description);

        var pagedList = await IdentityResourceRepository.GetIdentityResourceProperties(identityResourceId, page, pageSize);
        var identityResourcePropertiesAsync = pagedList.ToModel();
        identityResourcePropertiesAsync.IdentityResourceId = identityResourceId;
        identityResourcePropertiesAsync.IdentityResourceName = identityResource.Name;

        // TODO: create audit log

        return identityResourcePropertiesAsync;
    }

    public virtual async Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertyAsync(int identityResourcePropertyId)
    {
        var identityResourceProperty = await IdentityResourceRepository.GetIdentityResourceProperty(identityResourcePropertyId);
        if (identityResourceProperty == null) throw new UserFriendlyErrorPageException(string.Format(IdentityResourceServiceResources.IdentityResourcePropertyDoesNotExist().Description, identityResourcePropertyId));

        var identityResource = await IdentityResourceRepository.GetIdentityResource(identityResourceProperty.IdentityResourceId);

        var identityResourcePropertiesDto = identityResourceProperty.ToModel();
        identityResourcePropertiesDto.IdentityResourceId = identityResourceProperty.IdentityResourceId;
        identityResourcePropertiesDto.IdentityResourceName = identityResource.Name;

        // TODO: create audit log

        return identityResourcePropertiesDto;
    }

    public virtual async Task<int> AddIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperties)
    {
        var canInsert = await CanInsertIdentityResourcePropertyAsync(identityResourceProperties);
        if (!canInsert)
        {
            await BuildIdentityResourcePropertiesViewModelAsync(identityResourceProperties);
            throw new UserFriendlyViewException(string.Format(IdentityResourceServiceResources.IdentityResourcePropertyExistsValue().Description, identityResourceProperties.Key), IdentityResourceServiceResources.IdentityResourcePropertyExistsKey().Description, identityResourceProperties);
        }

        var identityResourceProperty = identityResourceProperties.ToEntity();

        var added = await IdentityResourceRepository.AddIdentityResourceProperty(identityResourceProperties.IdentityResourceId, identityResourceProperty);

        // TODO: create audit log

        return added;
    }

    private async Task BuildIdentityResourcePropertiesViewModelAsync(IdentityResourcePropertiesDto identityResourceProperties)
    {
        var propertiesDto = await GetIdentityResourcePropertiesAsync(identityResourceProperties.IdentityResourceId);
        identityResourceProperties.IdentityResourceProperties.AddRange(propertiesDto.IdentityResourceProperties);
        identityResourceProperties.TotalCount = propertiesDto.TotalCount;
    }


    public virtual async Task<bool> CanInsertIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourcePropertiesDto)
    {
        var resource = identityResourcePropertiesDto.ToEntity();

        return await IdentityResourceRepository.CanInsertIdentityResourceProperty(resource);
    }
}