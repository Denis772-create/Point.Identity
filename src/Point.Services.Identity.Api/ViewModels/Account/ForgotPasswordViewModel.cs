namespace Point.Services.Identity.Web.ViewModels.Account;

public class ForgotPasswordViewModel
{
    [Required]
    public LoginResolutionPolicy? Policy { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public string Username { get; set; }
}