using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Application.Abstractions.Messaging;
using Evently.Modules.Events.Domain.TicketTypes;

namespace Evently.Modules.Events.Application.TicketTypes.CreateTicketType;
internal class CreateTicketTypeCommandHandler(ITicketTypeRepository repository, IUnitOfWork unitOfWork) : ICommandHandler<CreateTicketTypeCommand, Result>
{
    public async Task<Result<Result>> Handle(CreateTicketTypeCommand request, CancellationToken cancellationToken)
    {
        Result<Domain.TicketTypes.TicketType> ticketType = Domain.TicketTypes.TicketType.Create(
            request.EventId,
            request.Name,
            request.Currency,
            request.Price,
            request.Quantity);

        repository.Insert(ticketType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(ticketType);
    }
}
