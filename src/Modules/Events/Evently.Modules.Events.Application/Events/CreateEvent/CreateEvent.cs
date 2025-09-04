using Evently.Common.Application.Messaging;
using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Events.Models;
using Evently.Modules.Events.Domain.Events.Repository;

namespace Evently.Modules.Events.Application.Events.CreateEvent;

public record CreateEventCommand(string Title, string Description, DateTime StartDate, DateTime? EndDate, string Location) : ICommand<Guid>;

internal sealed class CreateEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork) :
    ICommandHandler<CreateEventCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {

        Result<Event> result = Event.Create(request.Title, request.Description, request.StartDate, request.EndDate,
            request.Location);
        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }
        await eventRepository.Insert(result);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return result.Value.Id;
    }
}
