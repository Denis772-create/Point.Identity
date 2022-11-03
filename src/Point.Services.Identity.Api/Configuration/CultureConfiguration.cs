namespace Point.Services.Identity.Api.Configuration;

public class CultureConfiguration
{
    public static readonly string[] AvailableCultures = { "en", "ru" };
    public static readonly string DefaultRequestCulture = "en";

    public List<string> Cultures { get; set; }
    public string DefaultCulture { get; set; } = DefaultRequestCulture;
}