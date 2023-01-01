using Point.Services.Identity.Domain;

namespace Point.Services.Identity.Application.Mappers;

public class KeyMapperProfile : Profile
{
    public KeyMapperProfile()
    {
        CreateMap<PagedList<Key>, KeysDto>(MemberList.Destination)
            .ForMember(x => x.Keys,
                opt => opt.MapFrom(src => src.Data));

        CreateMap<Key, KeyDto>(MemberList.Destination)
            .ReverseMap();
    }
}