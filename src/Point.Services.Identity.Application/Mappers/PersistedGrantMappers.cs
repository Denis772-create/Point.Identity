using Point.Services.Identity.Domain;
using PersistedGrant = IdentityServer4.EntityFramework.Entities.PersistedGrant;

namespace Point.Services.Identity.Application.Mappers;

public static class PersistedGrantMappers
{
    static PersistedGrantMappers()
    {
        Mapper = new MapperConfiguration(cfg => cfg.AddProfile<PersistedGrantMapperProfile>())
            .CreateMapper();
    }

    internal static IMapper Mapper { get; }

    public static PersistedGrantsDto ToModel(this PagedList<PersistedGrantDataView> grant)
    {
        return grant == null ? null : Mapper.Map<PersistedGrantsDto>(grant);
    }

    public static PersistedGrantsDto ToModel(this PagedList<PersistedGrant> grant)
    {
        return grant == null ? null : Mapper.Map<PersistedGrantsDto>(grant);
    }

    public static PersistedGrantDto ToModel(this PersistedGrant grant)
    {
        return grant == null ? null : Mapper.Map<PersistedGrantDto>(grant);
    }
}