namespace Point.Services.Identity.Web.DTOs.Clients;

public class ClientClaimApiDto
{
    public int Id { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string Value { get; set; }
}