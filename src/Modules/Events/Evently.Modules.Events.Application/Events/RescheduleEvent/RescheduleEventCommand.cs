using Evently.Common.Application.Messaging;
using Evently.Common.Domain.ResultPattern;

namespace Evently.Modules.Events.Application.Events.RescheduleEvent;
internal sealed record RescheduleEventCommand(Guid EventId) : ICommand<Result>
{
}
