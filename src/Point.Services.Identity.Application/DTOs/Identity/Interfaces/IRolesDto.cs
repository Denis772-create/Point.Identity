 
 

namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IRolesDto
{
    int PageSize { get; set; }
    int TotalCount { get; set; }
    List<IRoleDto> Roles { get; }
}