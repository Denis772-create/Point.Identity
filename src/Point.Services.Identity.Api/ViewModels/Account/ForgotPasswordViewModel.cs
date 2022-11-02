using System.ComponentModel.DataAnnotations;

namespace Point.Services.Identity.Api.ViewModels.Account;

public class ForgotPasswordViewModel
{
    [Required]
    public LoginResolutionPolicy? Policy { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public string Username { get; set; }
}