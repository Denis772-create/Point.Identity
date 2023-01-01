 
 

using Point.Services.Identity.Application.DTOs.Identity.Interfaces;

namespace Point.Services.Identity.Application.DTOs.Identity.Base;

public class BaseUserDto<TUserId> : IBaseUserDto
{
    public TUserId Id { get; set; }

    public bool IsDefaultId() => EqualityComparer<TUserId>.Default.Equals(Id, default(TUserId));

    object IBaseUserDto.Id => Id;
}