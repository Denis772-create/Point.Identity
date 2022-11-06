namespace Point.Services.Identity.Application.Helpers;

public class ViewHelpers
{
    public static string GetClientName(string clientId, string clientName) => $"{clientId} ({clientName})";
}