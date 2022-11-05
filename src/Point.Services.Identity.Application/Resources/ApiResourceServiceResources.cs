namespace Point.Services.Identity.Application.Resources;

public class ApiResourceServiceResources : IApiResourceServiceResources
{
    public ResourceMessage ApiResourceDoesNotExist()
    {
        return new ResourceMessage
        {
            Code = nameof(ApiResourceDoesNotExist),
            Description = ApiResourceServiceResource.ApiResourceDoesNotExist
        };
    }

    public ResourceMessage ApiResourceExistsValue()
    {
        return new ResourceMessage
        {
            Code = nameof(ApiResourceExistsValue),
            Description = ApiResourceServiceResource.ApiResourceExistsValue
        };
    }

    public ResourceMessage ApiResourceExistsKey()
    {
        return new ResourceMessage
        {
            Code = nameof(ApiResourceExistsKey),
            Description = ApiResourceServiceResource.ApiResourceExistsKey
        };
    }

    public ResourceMessage ApiSecretDoesNotExist()
    {
        return new ResourceMessage
        {
            Code = nameof(ApiSecretDoesNotExist),
            Description = ApiResourceServiceResource.ApiSecretDoesNotExist
        };
    }

    public ResourceMessage ApiResourcePropertyDoesNotExist()
    {
        return new ResourceMessage
        {
            Code = nameof(ApiResourcePropertyDoesNotExist),
            Description = ApiResourceServiceResource.ApiResourcePropertyDoesNotExist
        };
    }

    public ResourceMessage ApiResourcePropertyExistsKey()
    {
        return new ResourceMessage
        {
            Code = nameof(ApiResourcePropertyExistsKey),
            Description = ApiResourceServiceResource.ApiResourcePropertyExistsKey
        };
    }

    public ResourceMessage ApiResourcePropertyExistsValue()
    {
        return new ResourceMessage
        {
            Code = nameof(ApiResourcePropertyExistsValue),
            Description = ApiResourceServiceResource.ApiResourcePropertyExistsValue
        };
    }
}