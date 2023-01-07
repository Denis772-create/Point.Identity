namespace Point.Services.Identity.Web.AdminApi;

public class KeysController : AdminApiBaseController
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _keyService.DeleteKeyAsync(id);

        return Ok();
    }
}