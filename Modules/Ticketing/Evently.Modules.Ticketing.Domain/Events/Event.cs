using Evently.Common.Domain;

namespace Evently.Modules.Ticketing.Domain.Events;

// Read model — a local mirror of an event published by the Events module.
// Ticketing never reads from events.* schema; this table is the source of truth within ticketing.
public sealed class Event : Entity
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string? Location { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime? EndsAtUtc { get; private set; }

    private Event() { }

    public static Event Create(
        Guid id,
        string title,
        string description,
        string? location,
        DateTime startsAtUtc,
        DateTime? endsAtUtc)
    {
        return new Event
        {
            Id = id,
            Title = title,
            Description = description,
            Location = location,
            StartsAtUtc = startsAtUtc,
            EndsAtUtc = endsAtUtc
        };
    }
}
