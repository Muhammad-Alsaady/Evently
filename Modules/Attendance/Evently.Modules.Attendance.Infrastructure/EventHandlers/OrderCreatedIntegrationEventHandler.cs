using Evently.Common.Domain.Events;
using Evently.Modules.Attendance.Application.Attendees.CreateAttendee;
using Evently.Modules.Ticketing.PublicApi;
using Waseet.CQRS;

namespace Evently.Modules.Attendance.Infrastructure.EventHandlers;

internal sealed class OrderCreatedIntegrationEventHandler(IMediator mediator)
    : IEventHandler<OrderCreatedIntegrationEvent>
{
    public async Task HandleAsync(OrderCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new CreateAttendeeCommand(
                integrationEvent.OrderId,
                integrationEvent.CustomerId,
                integrationEvent.EventId),
            cancellationToken);
    }
}
