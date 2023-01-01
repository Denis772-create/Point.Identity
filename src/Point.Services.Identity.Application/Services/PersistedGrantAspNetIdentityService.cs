namespace Point.Services.Identity.Application.Services;

public class PersistedGrantAspNetIdentityService : IPersistedGrantAspNetIdentityService
{
    protected readonly IPersistedGrantAspNetIdentityRepository _persistedGrantAspNetIdentityRepository;
    protected readonly IPersistedGrantAspNetIdentityServiceResources PersistedGrantAspNetIdentityServiceResources;

    public PersistedGrantAspNetIdentityService(IPersistedGrantAspNetIdentityRepository persistedGrantAspNetIdentityRepository,
        IPersistedGrantAspNetIdentityServiceResources persistedGrantAspNetIdentityServiceResources)
    {
        _persistedGrantAspNetIdentityRepository = persistedGrantAspNetIdentityRepository;
        PersistedGrantAspNetIdentityServiceResources = persistedGrantAspNetIdentityServiceResources;
    }

    public virtual async Task<PersistedGrantsDto> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = await _persistedGrantAspNetIdentityRepository.GetPersistedGrantsByUsersAsync(search, page, pageSize);
        var persistedGrantsDto = pagedList.ToModel();

        // TODO: create audit log

        return persistedGrantsDto;
    }

    public virtual async Task<PersistedGrantsDto> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10)
    {
        var exists = await _persistedGrantAspNetIdentityRepository.ExistsPersistedGrantsAsync(subjectId);
        if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description, subjectId), PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description);

        var pagedList = await _persistedGrantAspNetIdentityRepository.GetPersistedGrantsByUserAsync(subjectId, page, pageSize);
        var persistedGrantsDto = pagedList.ToModel();

        // TODO: create audit log

        return persistedGrantsDto;
    }

    public virtual async Task<PersistedGrantDto> GetPersistedGrantAsync(string key)
    {
        var persistedGrant = await _persistedGrantAspNetIdentityRepository.GetPersistedGrantAsync(key);
        if (persistedGrant == null) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description, key), PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description);
        var persistedGrantDto = persistedGrant.ToModel();

        // TODO: create audit log

        return persistedGrantDto;
    }

    public async Task<int> DeletePersistedGrantAsync(string key)
    {
        var exists = await _persistedGrantAspNetIdentityRepository.ExistsPersistedGrantAsync(key);
        if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description, key), PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description);

        var deleted = await _persistedGrantAspNetIdentityRepository.DeletePersistedGrantAsync(key);

        // TODO: create audit log

        return deleted;
    }

    public async Task<int> DeletePersistedGrantsAsync(string userId)
    {
        var exists = await _persistedGrantAspNetIdentityRepository.ExistsPersistedGrantsAsync(userId);
        if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description, userId), PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description);

        var deleted = await _persistedGrantAspNetIdentityRepository.DeletePersistedGrantsAsync(userId);

        // TODO: create audit log

        return deleted;
    }
}