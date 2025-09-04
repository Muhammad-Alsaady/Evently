using Evently.Common.Application.Messaging;
using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.TicketTypes;

namespace Evently.Modules.Events.Application.TicketTypes.UpdateTicketTypePrice;
internal sealed class UpdateTicketTypePriceCommandHandler(ITicketTypeRepository ticketTypeRepository, IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateTicketTypePriceCommand>
{
    public async Task<Result> Handle(UpdateTicketTypePriceCommand request, CancellationToken cancellationToken)
    {
        TicketType ticketType = await ticketTypeRepository.GetAsync(request.TicketTypeId, cancellationToken);
        if (ticketType == null)
        {
            return Result.Failure(TicketTypeError.NotFound(request.TicketTypeId));
        }

        ticketType.UpdatePrice(request.NewPrice);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
