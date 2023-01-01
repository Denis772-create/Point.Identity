namespace Point.Services.Identity.Application.Services;

public class PersistedGrantAspNetIdentityService : IPersistedGrantAspNetIdentityService
{
    protected readonly IPersistedGrantAspNetIdentityRepository PersistedGrantAspNetIdentityRepository;
    protected readonly IPersistedGrantAspNetIdentityServiceResources PersistedGrantAspNetIdentityServiceResources;

    public PersistedGrantAspNetIdentityService(IPersistedGrantAspNetIdentityRepository persistedGrantAspNetIdentityRepository,
        IPersistedGrantAspNetIdentityServiceResources persistedGrantAspNetIdentityServiceResources)
    {
        PersistedGrantAspNetIdentityRepository = persistedGrantAspNetIdentityRepository;
        PersistedGrantAspNetIdentityServiceResources = persistedGrantAspNetIdentityServiceResources;
    }

    public virtual async Task<PersistedGrantsDto> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = await PersistedGrantAspNetIdentityRepository.GetPersistedGrantsByUsersAsync(search, page, pageSize);
        var persistedGrantsDto = pagedList.ToModel();

        // TODO: create audit log

        return persistedGrantsDto;
    }

    public virtual async Task<PersistedGrantsDto> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10)
    {
        var exists = await PersistedGrantAspNetIdentityRepository.ExistsPersistedGrantsAsync(subjectId);
        if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description, subjectId), PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description);

        var pagedList = await PersistedGrantAspNetIdentityRepository.GetPersistedGrantsByUserAsync(subjectId, page, pageSize);
        var persistedGrantsDto = pagedList.ToModel();

        // TODO: create audit log

        return persistedGrantsDto;
    }

    public virtual async Task<PersistedGrantDto> GetPersistedGrantAsync(string key)
    {
        var persistedGrant = await PersistedGrantAspNetIdentityRepository.GetPersistedGrantAsync(key);
        if (persistedGrant == null) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description, key), PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description);
        var persistedGrantDto = persistedGrant.ToModel();

        // TODO: create audit log

        return persistedGrantDto;
    }
}