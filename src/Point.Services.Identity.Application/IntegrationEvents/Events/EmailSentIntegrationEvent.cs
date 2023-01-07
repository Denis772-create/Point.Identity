namespace Point.Services.Identity.Application.IntegrationEvents.Events;

public record EmailSentIntegrationEvent(
    string To,
    string MessageSubject,
    string? HtmlBody,
    string? PlainBody = null) : IntegrationEvent("EmailToSend");