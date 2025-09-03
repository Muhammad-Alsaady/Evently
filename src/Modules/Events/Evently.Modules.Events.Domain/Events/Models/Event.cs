using Evently.Common.Domain.BaseEntity;
using Evently.Common.Domain.Errors;
using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Domain.Events.DomainEvents;

namespace Evently.Modules.Events.Domain.Events.Models;

public sealed class Event : Entity
{
    public Guid Id { get; private init; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string Location { get; private set; }
    public EventStatus Status { get; private set; }

    public Event()
    {

    }
    public static Result<Event> Create(string title, string description, DateTime startDate, DateTime? endDate, string location)
    {
        if (endDate.HasValue && endDate < startDate)
        {
            return EventErrors.EndDatePrecedesStartDate;
        }

        var @event = new Event
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            Location = location,
            Status = EventStatus.Draft
        };

        @event.AddDomainEvent(new EventCreatedDomainEvent() { EventId = @event.Id });
        return @event;
    }

    public Result Publish()
    {
        if (Status != EventStatus.Draft)
        {
            return EventErrors.NotDrafted;
        }
        Status = EventStatus.Published;
        AddDomainEvent(new EventPublishedDomainEvent() { EventId = Id });
        return Result.Success();
    }

    public Result Cancel(DateTime eventStartDate)
    {
        if (StartDate < eventStartDate)
        {
            return EventErrors.Started;
        }
        if (Status == EventStatus.Cancelled)
        {
            return EventErrors.Cancelled;
        }

        Status = EventStatus.Cancelled;
        AddDomainEvent(new EventCancelledDomainEvent() { EventId = Id });
        return Result.Success();
    }

    public Result Reschedule(DateTime newStartDate, DateTime? newEndDate)
    {
        if (newEndDate.HasValue && newEndDate < newStartDate)
        {
            return EventErrors.EndDatePrecedesStartDate;
        }
        StartDate = newStartDate;
        EndDate = newEndDate;
        AddDomainEvent(new EventRescheduledDomainEvent() { EventId = Id, EndsAtUtc = newEndDate, StartsAtUtc = newStartDate });
        return Result.Success();
    }
}

public static class EventErrors
{
    public static Error NotFound(Guid eventId) => Error.NotFound("Events.NotFound", $"The event with the identifier {eventId} was not found");

    public static readonly Error EndDatePrecedesStartDate = Error.Problem("Events.EndDatePrecedesStartDate", "The event end date precedes the start date");
    public static readonly Error NotDrafted = Error.Problem("Events.NotDrafted", "The event is not drafted");
    public static readonly Error Started = Error.Problem("Events.Started", "The event is already started");
    public static readonly Error Cancelled = Error.Problem("Events.Cancelled", "The event is already cancelled");
}
