using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Categories;
using Evently.Modules.Events.Domain.Events;

namespace Evently.Modules.Events.Application.Events.CreateEvent;

internal sealed class CreateEventCommandHandler(
    ICategoryRepository categoryRepository,
    IEventRepository eventRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateEventCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        // cross-aggregate validation: verify category exists
        Category? category = await categoryRepository.GetAsync(command.CategoryId, cancellationToken);
        if (category is null)
		{
			return CategoryErrors.NotFound(command.CategoryId);
		}

		var @event = Event.Create(
            Guid.NewGuid(),
            command.CategoryId,
            command.Title,
            command.Description,
            command.Location,
            command.StartsAtUtc,
            command.EndsAtUtc);

        eventRepository.Insert(@event);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return @event.Id;
    }
}
