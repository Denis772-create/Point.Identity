namespace Point.Services.Identity.Application.IntegrationEvents;

public record IntegrationEvent
{
    public IntegrationEvent(string? subject = null)
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
        Subject = subject;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationDate = createDate;
    }

    [JsonInclude]
    public Guid Id { get; private init; }

    [JsonInclude]
    public DateTime CreationDate { get; private init; }

    public string? Subject { get; }
}

