namespace Evently.Modules.Events.Application.Events.GetEvents;

public sealed record EventResponse(
    Guid Id,
    Guid CategoryId,
    string Title,
    string Status,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc);
