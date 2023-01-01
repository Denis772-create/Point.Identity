 
 

using Point.Services.Identity.Application.DTOs.Identity.Interfaces;

namespace Point.Services.Identity.Application.DTOs.Identity.Base;

public class BaseRoleDto<TRoleId> : IBaseRoleDto
{
    public TRoleId Id { get; set; }

    public bool IsDefaultId() => EqualityComparer<TRoleId>.Default.Equals(Id, default(TRoleId));

    object IBaseRoleDto.Id => Id;
}