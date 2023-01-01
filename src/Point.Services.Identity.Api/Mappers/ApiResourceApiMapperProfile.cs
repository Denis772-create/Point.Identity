using Point.Services.Identity.Web.DTOs.ApiResources;
using ApiResourcePropertiesDto = Point.Services.Identity.Web.DTOs.ApiResources.ApiResourcePropertiesDto;
using ApiResourcePropertyDto = Point.Services.Identity.Web.DTOs.ApiResources.ApiResourcePropertyDto;

namespace Point.Services.Identity.Web.Mappers;

public class ApiResourceApiMapperProfile : Profile
{
    public ApiResourceApiMapperProfile()
    {
        // Api Resources
        CreateMap<ApiResourcesDto, ApiResourcesApiDto>(MemberList.Destination)
            .ReverseMap();

        CreateMap<ApiResourceDto, ApiResourceApiDto>(MemberList.Destination)
            .ReverseMap();

        // Api Secrets
        CreateMap<ApiSecretsDto, ApiSecretApiDto>(MemberList.Destination)
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApiSecretId))
            .ReverseMap();

        CreateMap<ApiSecretDto, ApiSecretApiDto>(MemberList.Destination);
        CreateMap<ApiSecretsDto, ApiSecretsApiDto>(MemberList.Destination);

        // Api Properties
        CreateMap<ApiResourcePropertiesDto, ApiResourcePropertyApiDto>(MemberList.Destination)
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApiResourcePropertyId))
            .ReverseMap();

        CreateMap<ApiResourcePropertyDto, ApiResourcePropertyApiDto>(MemberList.Destination);
        CreateMap<ApiResourcePropertiesDto, ApiResourcePropertiesApiDto>(MemberList.Destination);

    }
}