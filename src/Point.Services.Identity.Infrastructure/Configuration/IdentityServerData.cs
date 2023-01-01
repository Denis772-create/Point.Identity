using IS4 = IdentityServer4.Models;

namespace Point.Services.Identity.Infrastructure.Configuration;

public class IdentityServerData
{
    public List<Client> Clients { get; set; } = new();
    public List<IS4.IdentityResource> IdentityResources { get; set; } = new();
    public List<IS4.ApiResource> ApiResources { get; set; } = new();
    public List<IS4.ApiScope> ApiScopes { get; set; } = new();
}