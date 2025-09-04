using Evently.Common.Application.Clock;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Events.Models;
using Evently.Modules.Events.Domain.Events.Repository;

namespace Evently.Modules.Events.Application.Events.CancelEvent;
internal sealed class CancelEventCommandHandler(IDateTimeProvider dateTimeProvider, IEventRepository eventRepository, IUnitOfWork unitOfWork) : ICommandHandler<CancelEventCommand>
{
    public async Task<Result> Handle(CancelEventCommand request, CancellationToken cancellationToken)
    {
        Event? @event = await eventRepository.GetAsync(request.EventId, cancellationToken);
        if (@event is null)
        {
            return Result.Failure<Result<Event>>(EventErrors.NotFound(request.EventId));
        }
        Result result = @event.Cancel(dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
