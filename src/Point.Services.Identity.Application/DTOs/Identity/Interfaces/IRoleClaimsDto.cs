namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IRoleClaimsDto : IRoleClaimDto
{
    string RoleName { get; set; }
    List<IRoleClaimDto> Claims { get; }
    int TotalCount { get; set; }
    int PageSize { get; set; }
}