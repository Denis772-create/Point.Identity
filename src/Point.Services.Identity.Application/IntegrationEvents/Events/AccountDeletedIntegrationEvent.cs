namespace Point.Services.Identity.Application.IntegrationEvents.Events;

public record AccountDeletedIntegrationEvent(string Email) : IntegrationEvent;