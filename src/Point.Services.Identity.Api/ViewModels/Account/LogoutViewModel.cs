namespace Point.Services.Identity.Api.ViewModels.Account;

public class LogoutViewModel : LogoutInputModel
{
    public bool ShowLogoutPrompt { get; set; } = true;
}