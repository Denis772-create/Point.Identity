using Point.Services.Identity.Application.DTOs.Identity.Interfaces;

namespace Point.Services.Identity.Application.DTOs.Identity.Base;

public class BaseUserRolesDto<TKey> : IBaseUserRolesDto
{
    public TKey UserId { get; set; }

    public TKey RoleId { get; set; }

    object IBaseUserRolesDto.UserId => UserId;

    object IBaseUserRolesDto.RoleId => RoleId;
}