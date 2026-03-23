using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Ticketing.Application.Abstractions.Data;
using Evently.Modules.Ticketing.Domain.Events;

namespace Evently.Modules.Ticketing.Application.Events.CancelEvent;

internal class CancelEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork) : ICommandHandler<CancelEventCommand>
{
	public async Task<Result<Success>> Handle(CancelEventCommand request, CancellationToken cancellationToken = default)
	{
		Event? @event = await eventRepository.GetAsync(request.EventId);
		if (@event is null)
		{
			return EventErrors.NotFound(request.EventId);
		}
		var result = @event.Cancel();
		if(result.IsError)
		{
			return result;
		}
		await unitOfWork.SaveChangesAsync(cancellationToken);
		return Result.Success;
	}

}
