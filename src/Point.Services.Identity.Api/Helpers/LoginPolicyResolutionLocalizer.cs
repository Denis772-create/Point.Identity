namespace Point.Services.Identity.Web.Helpers;

public static class LoginPolicyResolutionLocalizer
{
    public static string GetUserNameLocalizationKey(LoginResolutionPolicy policy)
    {
        return policy switch
        {
            LoginResolutionPolicy.Username => "Username",
            LoginResolutionPolicy.Email => "Email",
            _ => "Username"
        };
    }
}