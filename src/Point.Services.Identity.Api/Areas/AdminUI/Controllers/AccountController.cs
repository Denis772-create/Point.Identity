namespace Point.Services.Identity.Web.Areas.AdminUI.Controllers;

[Authorize]
[Area(CommonConsts.AdminUIArea)]
public class AccountController : BaseController
{
    public AccountController(ILogger<ConfigurationController> logger) : base(logger)
    {
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}