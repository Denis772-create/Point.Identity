namespace Point.Services.Identity.Application.DTOs.Key;

public class KeyDto
{
    public string Id { get; set; }
    public int Version { get; set; }
    public DateTime Created { get; set; }
    public string Use { get; set; }
    public string Algorithm { get; set; }
    public bool IsX509Certificate { get; set; }
}