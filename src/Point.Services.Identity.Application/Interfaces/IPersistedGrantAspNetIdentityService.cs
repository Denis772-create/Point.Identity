namespace Point.Services.Identity.Application.Interfaces;

public interface IPersistedGrantAspNetIdentityService
{
    Task<PersistedGrantsDto> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10);
    Task<PersistedGrantsDto> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10);
    Task<PersistedGrantDto> GetPersistedGrantAsync(string key);
}