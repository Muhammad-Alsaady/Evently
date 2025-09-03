using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Application.Abstractions.Messaging;

namespace Evently.Modules.Events.Application.Events.CancelEvent;
internal sealed record CancelEventCommand(Guid EventId) : ICommand<Result>
{
}
