namespace Point.Services.Identity.Application.DTOs.Configuration;

public class IdentityResourcesDto
{
    public IdentityResourcesDto()
    {
        IdentityResources = new List<IdentityResourceDto>();
    }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public List<IdentityResourceDto> IdentityResources { get; set; }
}