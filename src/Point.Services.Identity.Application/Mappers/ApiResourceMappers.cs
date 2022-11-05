using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace Point.Services.Identity.Application.Mappers;


public static class ApiResourceMappers
{
    internal static IMapper Mapper { get; }

    static ApiResourceMappers()
    {
        Mapper = new MapperConfiguration(cfg =>
            cfg.AddProfile<ApiResourceMapperProfile>()).CreateMapper();
    }

    public static ApiResourceDto ToModel(this ApiResource resource)
    {
        return resource == null ? null : Mapper.Map<ApiResourceDto>(resource);
    }

    public static ApiResourcesDto ToModel(this PagedList<ApiResource> resources)
    {
        return resources == null ? null : Mapper.Map<ApiResourcesDto>(resources);
    }

    public static ApiResource ToEntity(this ApiResourceDto resource)
    {
        return resource == null ? null : Mapper.Map<ApiResource>(resource);
    }

    public static ApiSecretsDto ToModel(this PagedList<ApiResourceSecret> secrets)
    {
        return secrets == null ? null : Mapper.Map<ApiSecretsDto>(secrets);
    }

    public static ApiSecretsDto ToModel(this ApiResourceSecret resource)
    {
        return resource == null ? null : Mapper.Map<ApiSecretsDto>(resource);
    }

    public static ApiResourceSecret ToEntity(this ApiSecretsDto resource)
    {
        return resource == null ? null : Mapper.Map<ApiResourceSecret>(resource);
    }

    public static ApiResourcePropertiesDto ToModel(this ApiResourceProperty apiResourceProperty)
    {
        return Mapper.Map<ApiResourcePropertiesDto>(apiResourceProperty);
    }

    public static ApiResourcePropertiesDto ToModel(this PagedList<ApiResourceProperty> apiResourceProperties)
    {
        return Mapper.Map<ApiResourcePropertiesDto>(apiResourceProperties);
    }

    public static ApiResourceProperty ToEntity(this ApiResourcePropertiesDto apiResourceProperties)
    {
        return Mapper.Map<ApiResourceProperty>(apiResourceProperties);
    }
}