﻿using Point.Services.Identity.Web.Helpers;

namespace Point.Services.Identity.Web.AdminApi;

[Route("api/[controller]")]
[ApiController]
[TypeFilter(typeof(ControllerExceptionFilterAttribute))]
[Produces("application/json")]
[Authorize(Policy = ConfigurationConsts.AdministrationPolicy)]
public class PersistedGrantsController : ControllerBase
{
    private readonly IPersistedGrantAspNetIdentityService _persistedGrantsService;

    public PersistedGrantsController(IPersistedGrantAspNetIdentityService persistedGrantsService)
    {
        _persistedGrantsService = persistedGrantsService;
    }

    [HttpGet("Subjects")]
    public async Task<ActionResult<PersistedGrantSubjectsApiDto>> Get(string searchText, int page = 1, int pageSize = 10)
    {
        var persistedGrantsDto = await _persistedGrantsService.GetPersistedGrantsByUsersAsync(searchText, page, pageSize);
        var persistedGrantSubjectsApiDto = persistedGrantsDto.ToPersistedGrantApiModel<PersistedGrantSubjectsApiDto>();

        return Ok(persistedGrantSubjectsApiDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PersistedGrantApiDto>> Get(string id)
    {
        var persistedGrantDto = await _persistedGrantsService.GetPersistedGrantAsync(UrlHelpers.QueryStringUnSafeHash(id));
        var persistedGrantApiDto = persistedGrantDto.ToPersistedGrantApiModel<PersistedGrantApiDto>();

        ParsePersistedGrantKey(persistedGrantApiDto);

        return Ok(persistedGrantApiDto);
    }

    [HttpGet("Subjects/{subjectId}")]
    public async Task<ActionResult<PersistedGrantsApiDto>> GetBySubject(string subjectId, int page = 1, int pageSize = 10)
    {
        var persistedGrantDto = await _persistedGrantsService.GetPersistedGrantsByUserAsync(subjectId, page, pageSize);
        var persistedGrantApiDto = persistedGrantDto.ToPersistedGrantApiModel<PersistedGrantsApiDto>();

        ParsePersistedGrantKeys(persistedGrantApiDto);

        return Ok(persistedGrantApiDto);
    }

    private static void ParsePersistedGrantKey(PersistedGrantApiDto persistedGrantApiDto)
    {
        if (!string.IsNullOrEmpty(persistedGrantApiDto.Key))
        {
            persistedGrantApiDto.Key = UrlHelpers.QueryStringSafeHash(persistedGrantApiDto.Key);
        }
    }

    private static void ParsePersistedGrantKeys(PersistedGrantsApiDto persistedGrantApiDto)
    {
        persistedGrantApiDto.PersistedGrants.ForEach(ParsePersistedGrantKey);
    }
}