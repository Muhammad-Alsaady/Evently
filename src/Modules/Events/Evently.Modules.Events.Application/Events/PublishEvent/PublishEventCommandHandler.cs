using Evently.Common.Application.Messaging;
using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Domain.Events.Models;
using Evently.Modules.Events.Domain.Events.Repository;

namespace Evently.Modules.Events.Application.Events.PublishEvent;
internal sealed class PublishEventCommandHandler(IEventRepository eventRepository) : ICommandHandler<PublishEventCommand>
{
    public async Task<Result> Handle(PublishEventCommand request, CancellationToken cancellationToken)
    {
        Event @event = await eventRepository.GetAsync(request.EventId, cancellationToken);
        if (@event is null)
        {
            return Result.Failure<Result>(EventErrors.NotFound(request.EventId));
        }
        Result result = @event.Publish();
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }
        return Result.Success();
    }
}
