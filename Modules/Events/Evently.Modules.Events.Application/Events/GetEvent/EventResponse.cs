namespace Evently.Modules.Events.Application.Events.GetEvent;

public sealed record EventResponse(
    Guid Id,
    Guid CategoryId,
    string Title,
    string Description,
    string? Location,
    string Status,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    IReadOnlyList<TicketTypeResponse> TicketTypes);
