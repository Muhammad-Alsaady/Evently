using Evently.Modules.Events.Application.Abstractions.Messaging;

namespace Evently.Modules.Events.Application.Events.PublishEvent;
internal sealed record PublishEventCommand(Guid EventId, DateTime StartDate, DateTime EndDate) : ICommand
{
}
