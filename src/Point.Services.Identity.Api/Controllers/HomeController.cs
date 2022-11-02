namespace Point.Services.Identity.Api.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}