 
 

namespace Point.Services.Identity.Application.DTOs.Identity.Interfaces;

public interface IUserChangePasswordDto : IBaseUserChangePasswordDto
{
    string UserName { get; set; }
    string Password { get; set; }
    string ConfirmPassword { get; set; }
}