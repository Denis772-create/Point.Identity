namespace Point.Services.Identity.Application.DTOs.Configuration;

public class ClientClaimDto
{
    public int Id { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required] 
    public string Value { get; set; } = string.Empty;
}