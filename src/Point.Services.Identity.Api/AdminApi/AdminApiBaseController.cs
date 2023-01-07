namespace Point.Services.Identity.Web.AdminApi;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json", "application/problem+json")]
public class AdminApiBaseController : ControllerBase
{
    public AdminApiBaseController(IApiErrorResources errorResources)
    {
        ErrorResources = errorResources ?? throw new ArgumentNullException(nameof(errorResources));
    }

    public AdminApiBaseController()
    {
    }

    public IApiErrorResources ErrorResources { get; } = null!;
}