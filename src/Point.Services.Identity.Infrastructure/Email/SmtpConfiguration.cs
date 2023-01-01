namespace Point.Services.Identity.Infrastructure.Email;

public class SmtpConfiguration
{
    public string From { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; } = 587; // default smtp port
    public bool UseSsl { get; set; } = true;
}