using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.Events.PublishEvent;

[InvalidateCache("events:event:{EventId}")]
public sealed record PublishEventCommand(Guid EventId) : ICommand;
