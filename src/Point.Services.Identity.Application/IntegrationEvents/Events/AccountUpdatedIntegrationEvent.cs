namespace Point.Services.Identity.Application.IntegrationEvents.Events;

public record AccountUpdatedIntegrationEvent(
    string Email,
    string? PhoneNumber,
    string? Name,
    string? Website,
    string? City,
    string? Country) : IntegrationEvent;