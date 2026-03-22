using Evently.Common.Domain;
using Evently.Common.Domain.Results;

namespace Evently.Modules.Events.Domain.Events;

public sealed class Event : Entity
{
    public Guid Id { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string? Location { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime? EndsAtUtc { get; private set; }
    public EventStatus Status { get; private set; }

    private Event() { }

    public static Event Create(
        Guid id,
        Guid categoryId,
        string title,
        string description,
        string? location,
        DateTime startsAtUtc,
        DateTime? endsAtUtc)
    {
        var @event = new Event
        {
            Id = id,
            CategoryId = categoryId,
            Title = title,
            Description = description,
            Location = location,
            StartsAtUtc = startsAtUtc,
            EndsAtUtc = endsAtUtc,
            Status = EventStatus.Draft
        };

        @event.Raise(new EventCreatedDomainEvent(id));

        return @event;
    }

    public Result<Success> Publish()
    {
        if (Status != EventStatus.Draft)
            return EventErrors.NotDraft(Id);

        Status = EventStatus.Published;
        Raise(new EventPublishedDomainEvent(Id));

        return Result.Success;
    }

    public Result<Success> Cancel()
    {
        if (Status == EventStatus.Cancelled)
            return EventErrors.AlreadyCancelled(Id);

        Status = EventStatus.Cancelled;
        Raise(new EventCancelledDomainEvent(Id));

        return Result.Success;
    }

    public Result<Success> Reschedule(DateTime startsAtUtc, DateTime? endsAtUtc)
    {
        if (Status != EventStatus.Published)
            return EventErrors.NotPublished(Id);

        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Raise(new EventRescheduledDomainEvent(Id, startsAtUtc, endsAtUtc));

        return Result.Success;
    }
}
