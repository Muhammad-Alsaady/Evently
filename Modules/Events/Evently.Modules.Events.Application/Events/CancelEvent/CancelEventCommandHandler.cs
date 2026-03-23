using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Events;

namespace Evently.Modules.Events.Application.Events.CancelEvent;

internal sealed class CancelEventCommandHandler(
    IEventRepository eventRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CancelEventCommand>
{
    public async Task<Result<Success>> Handle(CancelEventCommand command, CancellationToken cancellationToken)
    {
        Event? @event = await eventRepository.GetAsync(command.EventId, cancellationToken);
        if (@event is null)
		{
			return EventErrors.NotFound(command.EventId);
		}

		var result = @event.Cancel();
        if (result.IsError)
		{
			return result;
		}

		await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
