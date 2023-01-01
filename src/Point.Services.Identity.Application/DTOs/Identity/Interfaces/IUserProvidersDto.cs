 
 

namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IUserProvidersDto : IUserProviderDto
{
    List<IUserProviderDto> Providers { get; }
}