namespace Point.Services.Identity.Application.STS.Configuration;

public class ExternalProvidersConfiguration
{
    public bool UseGitHubProvider { get; set; }
    public string GitHubClientId { get; set; }
    public string GitHubClientSecret { get; set; }
    public string GitHubCallbackPath { get; set; }
}