using Point.Services.Identity.Application.DTOs.Identity.Base;
using Point.Services.Identity.Application.DTOs.Identity.Interfaces;

namespace Point.Services.Identity.Application.DTOs.Identity;

public class UserChangePasswordDto<TKey> : BaseUserChangePasswordDto<TKey>, IUserChangePasswordDto
{
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}