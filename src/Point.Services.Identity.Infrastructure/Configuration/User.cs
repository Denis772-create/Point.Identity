namespace Point.Services.Identity.Infrastructure.Configuration;

public class User
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<Claim> Claims { get; set; } = new();
    public List<string> Roles { get; set; } = new();
}
