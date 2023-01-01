using Point.Services.Identity.Domain;

namespace Point.Services.Identity.Application.Mappers;

public static class KeyMappers
{
    internal static IMapper Mapper { get; }

    static KeyMappers()
    {
        Mapper = new MapperConfiguration(cfg => cfg.AddProfile<KeyMapperProfile>())
            .CreateMapper();
    }

    public static KeyDto ToModel(this Key key)
    {
        return key == null ? null : Mapper.Map<KeyDto>(key);
    }

    public static KeysDto ToModel(this PagedList<Key> grant)
    {
        return grant == null ? null : Mapper.Map<KeysDto>(grant);
    }
}
