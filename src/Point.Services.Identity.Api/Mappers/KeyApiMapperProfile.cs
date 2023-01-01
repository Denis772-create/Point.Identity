namespace Point.Services.Identity.Web.Mappers;

public class KeyApiMapperProfile : Profile
{
    public KeyApiMapperProfile()
    {
        CreateMap<KeyDto, KeyApiDto>(MemberList.Destination)
            .ReverseMap();

        CreateMap<KeysDto, KeysApiDto>(MemberList.Destination)
            .ReverseMap();
    }
}