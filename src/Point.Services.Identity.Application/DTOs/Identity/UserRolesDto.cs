namespace Point.Services.Identity.Application.DTOs.Identity;

public class UserRolesDto<TRoleDto, TKey> : BaseUserRolesDto<TKey>, IUserRolesDto
    where TRoleDto : RoleDto<TKey>
{
    public UserRolesDto()
    {
        Roles = new List<TRoleDto>(); 
    }

    public string UserName { get; set; }

    public List<SelectItemDto> RolesList { get; set; }

    public List<TRoleDto> Roles { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    List<IRoleDto> IUserRolesDto.Roles => Roles.Cast<IRoleDto>().ToList();
}