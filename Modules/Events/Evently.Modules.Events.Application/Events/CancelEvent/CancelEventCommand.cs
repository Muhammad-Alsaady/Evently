using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.Events.CancelEvent;

[InvalidateCache("events:event:{EventId}")]
public sealed record CancelEventCommand(Guid EventId) : ICommand;
