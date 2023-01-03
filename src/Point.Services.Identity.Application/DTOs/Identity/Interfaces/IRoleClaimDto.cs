namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IRoleClaimDto : IBaseRoleClaimDto
{
    string ClaimType { get; set; }
    string ClaimValue { get; set; }
}