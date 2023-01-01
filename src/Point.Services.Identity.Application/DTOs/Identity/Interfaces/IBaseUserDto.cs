 
 

namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IBaseUserDto
{
    object Id { get; }
    bool IsDefaultId();
}