namespace Point.Services.Identity.Web.AdminApi;

[Route("api/[controller]")]
[ApiController]
[TypeFilter(typeof(ControllerExceptionFilterAttribute))]
[Produces("application/json", "application/problem+json")]
[Authorize(Policy = ConfigurationConsts.AdministrationPolicy)]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly IApiErrorResources _errorResources;

    public ClientsController(IClientService clientService, IApiErrorResources errorResources)
    {
        _clientService = clientService;
        _errorResources = errorResources;
    }

    [HttpGet]
    public async Task<ActionResult<ClientsApiDto>> Get(string searchText, int page = 1, int pageSize = 10)
    {
        var clientsDto = await _clientService.GetClientsAsync(searchText, page, pageSize);
        var clientsApiDto = clientsDto.ToClientApiModel<ClientsApiDto>();

        return Ok(clientsApiDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientApiDto>> Get(int id)
    {
        var clientDto = await _clientService.GetClientAsync(id);
        var clientApiDto = clientDto.ToClientApiModel<ClientApiDto>();

        return Ok(clientApiDto);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Post([FromBody] ClientApiDto client)
    {
        var clientDto = client.ToClientApiModel<ClientDto>();

        if (!clientDto.Id.Equals(default))
        {
            return BadRequest(_errorResources.CannotSetId());
        }

        var id = await _clientService.AddClientAsync(clientDto);
        client.Id = id;

        return CreatedAtAction(nameof(Get), new { id }, client);
    }

    [HttpGet("{id}/Secrets")]
    public async Task<ActionResult<ClientSecretsApiDto>> GetSecrets(int id, int page = 1, int pageSize = 10)
    {
        var clientSecretsDto = await _clientService.GetClientSecretsAsync(id, page, pageSize);
        var clientSecretsApiDto = clientSecretsDto.ToClientApiModel<ClientSecretsApiDto>();

        return Ok(clientSecretsApiDto);
    }

    [HttpGet("Secrets/{secretId}")]
    public async Task<ActionResult<ClientSecretApiDto>> GetSecret(int secretId)
    {
        var clientSecretsDto = await _clientService.GetClientSecretAsync(secretId);
        var clientSecretDto = clientSecretsDto.ToClientApiModel<ClientSecretApiDto>();

        return Ok(clientSecretDto);
    }

    [HttpPost("{id}/Secrets")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostSecret(int id, [FromBody] ClientSecretApiDto clientSecretApi)
    {
        var secretsDto = clientSecretApi.ToClientApiModel<ClientSecretsDto>();
        secretsDto.ClientId = id;

        if (!secretsDto.ClientSecretId.Equals(default))
        {
            return BadRequest(_errorResources.CannotSetId());
        }

        var secretId = await _clientService.AddClientSecretAsync(secretsDto);
        clientSecretApi.Id = secretId;

        return CreatedAtAction(nameof(GetSecret), new { secretId }, clientSecretApi);
    }

    [HttpGet("{id}/Properties")]
    public async Task<ActionResult<ClientPropertiesApiDto>> GetProperties(int id, int page = 1, int pageSize = 10)
    {
        var clientPropertiesDto = await _clientService.GetClientPropertiesAsync(id, page, pageSize);
        var clientPropertiesApiDto = clientPropertiesDto.ToClientApiModel<ClientPropertiesApiDto>();

        return Ok(clientPropertiesApiDto);
    }

    [HttpGet("Properties/{propertyId}")]
    public async Task<ActionResult<ClientPropertyApiDto>> GetProperty(int propertyId)
    {
        var clientPropertiesDto = await _clientService.GetClientPropertyAsync(propertyId);
        var clientPropertyApiDto = clientPropertiesDto.ToClientApiModel<ClientPropertyApiDto>();

        return Ok(clientPropertyApiDto);
    }

    [HttpPost("{id}/Properties")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostProperty(int id, [FromBody] ClientPropertyApiDto clientPropertyApi)
    {
        var clientPropertiesDto = clientPropertyApi.ToClientApiModel<ClientPropertiesDto>();
        clientPropertiesDto.ClientId = id;

        if (!clientPropertiesDto.ClientPropertyId.Equals(default))
        {
            return BadRequest(_errorResources.CannotSetId());
        }

        var propertyId = await _clientService.AddClientPropertyAsync(clientPropertiesDto);
        clientPropertyApi.Id = propertyId;

        return CreatedAtAction(nameof(GetProperty), new { propertyId }, clientPropertyApi);
    }

    [HttpGet("{id}/Claims")]
    public async Task<ActionResult<ClientClaimsApiDto>> GetClaims(int id, int page = 1, int pageSize = 10)
    {
        var clientClaimsDto = await _clientService.GetClientClaimsAsync(id, page, pageSize);
        var clientClaimsApiDto = clientClaimsDto.ToClientApiModel<ClientClaimsApiDto>();

        return Ok(clientClaimsApiDto);
    }

    [HttpGet("Claims/{claimId}")]
    public async Task<ActionResult<ClientClaimApiDto>> GetClaim(int claimId)
    {
        var clientClaimsDto = await _clientService.GetClientClaimAsync(claimId);
        var clientClaimApiDto = clientClaimsDto.ToClientApiModel<ClientClaimApiDto>();

        return Ok(clientClaimApiDto);
    }

    [HttpPost("{id}/Claims")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostClaim(int id, [FromBody] ClientClaimApiDto clientClaimApiDto)
    {
        var clientClaimsDto = clientClaimApiDto.ToClientApiModel<ClientClaimsDto>();
        clientClaimsDto.ClientId = id;

        if (!clientClaimsDto.ClientClaimId.Equals(default))
        {
            return BadRequest(_errorResources.CannotSetId());
        }

        var claimId = await _clientService.AddClientClaimAsync(clientClaimsDto);
        clientClaimApiDto.Id = claimId;

        return CreatedAtAction(nameof(GetClaim), new { claimId }, clientClaimApiDto);
    }

}