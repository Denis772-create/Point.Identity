namespace Point.Services.Identity.Infrastructure.Configuration;

public class IdentityData
{
    public List<Role> Roles { get; set; } = new();
    public List<User> Users { get; set; } = new();

}