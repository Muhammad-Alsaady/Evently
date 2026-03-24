using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.Events.RescheduleEvent;

[InvalidateCache("events:event:{EventId}")]
public sealed record RescheduleEventCommand(
    Guid EventId,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc) : ICommand;
