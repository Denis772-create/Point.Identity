namespace Point.Services.Identity.Application.Configuration.Authentication;

public class AuthenticationHelpers
{
    public static void CheckSameSite(HttpContext httpContext, CookieOptions options)
    {
        if (options.SameSite != SameSiteMode.None) return;

        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        if (!httpContext.Request.IsHttps || DisallowsSameSiteNone(userAgent))
        {
            options.SameSite = SameSiteMode.Unspecified;
        }
    }

    public static bool DisallowsSameSiteNone(string userAgent)
    {
        // All of which are broken by SameSite=None, because they use the iOS networking stack
        if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
        {
            return true;
        }

        // Because they do not use the Mac OS networking stack.
        if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
            userAgent.Contains("Version/") && userAgent.Contains("Safari"))
        {
            return true;
        }

        return userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6");
    }

}