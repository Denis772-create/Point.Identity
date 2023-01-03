namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IUserRolesDto : IBaseUserRolesDto
{
    string UserName { get; set; }
    List<SelectItemDto> RolesList { get; set; }
    List<IRoleDto> Roles { get; }
    int PageSize { get; set; }
    int TotalCount { get; set; }
}