namespace Point.Services.Identity.Infrastructure.Configuration;

public class Client : IdentityServer4.Models.Client
{
    public List<Claim> ClientClaims { get; set; } = new();
}