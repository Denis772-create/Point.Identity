namespace Point.Services.Identity.Infrastructure.Configuration;

public class Role
{
    public string Name { get; set; } = string.Empty;
    public List<Claim> Claims { get; set; } = new();
}
