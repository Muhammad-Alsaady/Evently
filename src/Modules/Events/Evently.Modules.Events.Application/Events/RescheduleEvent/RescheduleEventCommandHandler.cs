using Evently.Common.Application.Clock;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Application.Events.PublishEvent;
using Evently.Modules.Events.Domain.Events.Models;
using Evently.Modules.Events.Domain.Events.Repository;

namespace Evently.Modules.Events.Application.Events.RescheduleEvent;
internal sealed class RescheduleEventCommandHandler(IDateTimeProvider dateTimeProvider, IEventRepository eventRepository, IUnitOfWork unitOfWork) : ICommandHandler<PublishEventCommand>
{
    public async Task<Result> Handle(PublishEventCommand request, CancellationToken cancellationToken)
    {
        Event @event = await eventRepository.GetAsync(request.EventId, cancellationToken);
        if (@event is null)
        {
            return Result.Failure<Result>(EventErrors.NotFound(request.EventId));
        }
        if (request.StartDate < dateTimeProvider.UtcNow)
        {
            return Result.Failure(EventErrors.Started);
        }
        Result result = @event.Reschedule(request.StartDate, request.EndDate);
        if (result.IsFailure)
        {
            return Result.Failure<Result>(result.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
