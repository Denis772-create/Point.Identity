using Point.Services.Identity.Application.DTOs.Identity.Base;
using Point.Services.Identity.Application.DTOs.Identity.Interfaces;

namespace Point.Services.Identity.Application.DTOs.Identity;

public class RoleDto<TKey> : BaseRoleDto<TKey>, IRoleDto
{      
    [Required]
    public string Name { get; set; }
}