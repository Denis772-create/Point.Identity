namespace Point.Services.Identity.Web.AdminApi;

[Route("api/[controller]")]
[Produces("application/json", "application/problem+json")]
[Authorize(Policy = ConfigurationConsts.AdministrationPolicy)]
[ApiController]
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