namespace Point.Services.Identity.Web.DTOs.PersistedGrants;

public class PersistedGrantsApiDto
{
    public PersistedGrantsApiDto()
    {
        PersistedGrants = new List<PersistedGrantApiDto>();
    }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public List<PersistedGrantApiDto> PersistedGrants { get; set; }
}