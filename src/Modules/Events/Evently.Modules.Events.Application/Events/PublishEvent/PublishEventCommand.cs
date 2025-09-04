using Evently.Common.Application.Messaging;

namespace Evently.Modules.Events.Application.Events.PublishEvent;
internal sealed record PublishEventCommand(Guid EventId, DateTime StartDate, DateTime EndDate) : ICommand
{
}
