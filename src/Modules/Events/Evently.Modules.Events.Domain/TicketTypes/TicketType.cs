using Evently.Common.Domain.BaseEntity;
using Evently.Common.Domain.ResultPattern;

namespace Evently.Modules.Events.Domain.TicketTypes;
public sealed class TicketType : Entity
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Name { get; set; }
    public string Currency { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    private TicketType() { }

    public static Result<TicketType> Create(Guid eventId, string name, string currency, decimal price, int quantity)
    {
        var ticketType = new TicketType
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Name = name,
            Currency = currency,
            Price = price,
            Quantity = quantity
        };

        ticketType.AddDomainEvent(new TicketTypeCreatedDomainEvent() { TicketTypeId = ticketType.Id });
        return ticketType;
    }

    public Result UpdatePrice(decimal newPrice)
    {
        if (Price == newPrice)
        {
            return TicketTypeError.HasSamePrice;
        }
        Price = newPrice;
        AddDomainEvent(new TicketTypePriceChangedDomainEvent() { TicketTypeId = Id, NewPrice = newPrice });
        return Result.Success();
    }
}
