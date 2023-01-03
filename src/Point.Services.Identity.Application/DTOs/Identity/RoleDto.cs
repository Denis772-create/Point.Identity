namespace Point.Services.Identity.Application.DTOs.Identity;

public class RoleDto<TKey> : BaseRoleDto<TKey>, IRoleDto
{      
    [Required]
    public string Name { get; set; }
}