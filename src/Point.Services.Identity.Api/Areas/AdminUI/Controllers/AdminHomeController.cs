﻿using AuthorizationConsts = Point.Services.Identity.Web.Configuration.Constants.AuthorizationConsts;

namespace Point.Services.Identity.Web.Areas.AdminUI.Controllers;

[Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
[TypeFilter(typeof(ControllerExceptionFilterAttribute))]
[Area(CommonConsts.AdminUIArea)]
public class AdminHomeController : BaseController
{
    private readonly ILogger<ConfigurationController> _logger;

    public AdminHomeController(ILogger<ConfigurationController> logger) : base(logger)
    {
        _logger = logger;
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

    public IActionResult Error()
    {
        // Get the details of the exception that occurred
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature == null) return View();

        // Get which route the exception occurred at
        string routeWhereExceptionOccurred = exceptionFeature.Path;

        // Get the exception that occurred
        Exception exceptionThatOccurred = exceptionFeature.Error;

        // Log the exception
        _logger.LogError(exceptionThatOccurred, $"Exception at route {routeWhereExceptionOccurred}");

        return View();
    }
}