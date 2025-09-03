using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Application.Abstractions.Messaging;

namespace Evently.Modules.Events.Application.Events.RescheduleEvent;
internal sealed record RescheduleEventCommand(Guid EventId) : ICommand<Result>
{
}
