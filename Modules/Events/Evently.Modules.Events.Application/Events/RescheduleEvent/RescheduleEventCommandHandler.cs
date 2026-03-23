using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Events;

namespace Evently.Modules.Events.Application.Events.RescheduleEvent;

internal sealed class RescheduleEventCommandHandler(
    IEventRepository eventRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RescheduleEventCommand>
{
    public async Task<Result<Success>> Handle(RescheduleEventCommand command, CancellationToken cancellationToken)
    {
        Event? @event = await eventRepository.GetAsync(command.EventId, cancellationToken);
        if (@event is null)
		{
			return EventErrors.NotFound(command.EventId);
		}

		var result = @event.Reschedule(command.StartsAtUtc, command.EndsAtUtc);
        if (result.IsError)
		{
			return result;
		}

		await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
