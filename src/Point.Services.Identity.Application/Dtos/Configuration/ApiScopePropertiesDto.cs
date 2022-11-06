namespace Point.Services.Identity.Application.Dtos.Configuration;

public class ApiScopePropertiesDto
{
    public int ApiScopePropertyId { get; set; }

    public int ApiScopeId { get; set; }

    public string ApiScopeName { get; set; }

    [Required]
    public string Key { get; set; }

    [Required]
    public string Value { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public List<ApiScopePropertyDto> ApiScopeProperties { get; set; } = new();
}