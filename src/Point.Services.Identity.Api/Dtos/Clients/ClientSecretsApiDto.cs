namespace Point.Services.Identity.Web.DTOs.Clients;

public class ClientSecretsApiDto
{
    public ClientSecretsApiDto()
    {
        ClientSecrets = new List<ClientSecretApiDto>();
    }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public List<ClientSecretApiDto> ClientSecrets { get; set; }
}