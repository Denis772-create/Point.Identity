namespace Point.Services.Identity.Web.Mappers;

public class ApiScopeApiMapperProfile : Profile
{
    public ApiScopeApiMapperProfile()
    {
        // Api Scopes
        CreateMap<ApiScopesDto, ApiScopesApiDto>(MemberList.Destination)
            .ReverseMap();

        CreateMap<ApiScopeDto, ApiScopeApiDto>(MemberList.Destination)
            .ReverseMap();

        // Api Scope Properties
        CreateMap<ApiScopePropertiesDto, ApiScopePropertiesApiDto>(MemberList.Destination)
            .ReverseMap();

        CreateMap<ApiScopePropertyDto, ApiScopePropertyApiDto>(MemberList.Destination)
            .ReverseMap();

        CreateMap<ApiScopePropertiesDto, ApiScopePropertyApiDto>(MemberList.Destination)
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApiScopePropertyId))
            .ReverseMap();
    }
}