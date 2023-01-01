using Point.Services.Identity.Application.DTOs.Identity.Base;
using Point.Services.Identity.Application.DTOs.Identity.Interfaces;

namespace Point.Services.Identity.Application.DTOs.Identity;

public class UserClaimDto<TKey> : BaseUserClaimDto<TKey>, IUserClaimDto
{
    [Required]
    public string ClaimType { get; set; }

    [Required]
    public string ClaimValue { get; set; }
}