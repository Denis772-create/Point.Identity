namespace Point.Services.Identity.Application.IntegrationEvents.Events;

public record AccountCreatedIntegrationEvent(string UserName, string Email) : IntegrationEvent;