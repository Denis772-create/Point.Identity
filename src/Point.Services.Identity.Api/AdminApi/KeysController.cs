namespace Point.Services.Identity.Web.AdminApi;

[Route("api/[controller]")]
[ApiController]
[TypeFilter(typeof(ControllerExceptionFilterAttribute))]
[Produces("application/json")]
[Authorize(Policy = ConfigurationConsts.AdministrationPolicy)]
public class KeysController : ControllerBase
{
    private readonly IKeyService _keyService;

    public KeysController(IKeyService keyService)
    {
        _keyService = keyService;
    }

    [HttpGet]
    public async Task<ActionResult<KeysApiDto>> Get(int page = 1, int pageSize = 10)
    {
        var keys = await _keyService.GetKeysAsync(page, pageSize);
        var keysApi = keys.ToKeyApiModel<KeysApiDto>();

        return Ok(keysApi);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<KeyApiDto>> Get(string id)
    {
        var key = await _keyService.GetKeyAsync(id);

        var keyApi = key.ToKeyApiModel<KeyApiDto>();

        return Ok(keyApi);
    }
}