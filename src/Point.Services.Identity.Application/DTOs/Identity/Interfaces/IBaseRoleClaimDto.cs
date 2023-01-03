namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IBaseRoleClaimDto
{
    int ClaimId { get; set; }
    object RoleId { get; }
}