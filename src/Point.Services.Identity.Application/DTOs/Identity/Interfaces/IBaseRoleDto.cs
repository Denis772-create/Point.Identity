 
 

namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IBaseRoleDto
{
    object Id { get; }
    bool IsDefaultId();
}