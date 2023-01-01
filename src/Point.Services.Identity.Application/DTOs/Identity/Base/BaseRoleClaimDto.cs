 
using Point.Services.Identity.Application.DTOs.Identity.Interfaces;

namespace Point.Services.Identity.Application.DTOs.Identity.Base;

public class BaseRoleClaimDto<TRoleId> : IBaseRoleClaimDto
{
    public int ClaimId { get; set; }

    public TRoleId RoleId { get; set; }

    object IBaseRoleClaimDto.RoleId => RoleId;
}