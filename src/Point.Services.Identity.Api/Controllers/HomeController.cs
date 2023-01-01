using Point.Services.Identity.Web.ViewModels.Home;

namespace Point.Services.Identity.Web.Controllers;

[SecurityHeaders]
public class HomeController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;

    public HomeController(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
        return LocalRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SelectTheme(string theme, string returnUrl)
    {
        Response.Cookies.Append(
            ThemeHelpers.CookieThemeKey,
            theme,
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }

    public async Task<IActionResult> Error(string errorId)
    {
        var vm = new ErrorViewModel();

        // retrieve error details from identityserver
        var message = await _interaction.GetErrorContextAsync(errorId);
        if (message != null)
        {
            vm.Error = message;
        }

        return View("Error", vm);
    }

}