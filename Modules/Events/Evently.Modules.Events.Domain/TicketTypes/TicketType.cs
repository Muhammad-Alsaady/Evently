using Evently.Common.Domain;

namespace Evently.Modules.Events.Domain.TicketTypes;

public sealed class TicketType : Entity
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    private TicketType() { }

    public static TicketType Create(Guid id, Guid eventId, string name, decimal price, int quantity)
    {
        var ticketType = new TicketType
        {
            Id = id,
            EventId = eventId,
            Name = name,
            Price = price,
            Quantity = quantity
        };

        ticketType.Raise(new TicketTypeCreatedDomainEvent(id));

        return ticketType;
    }

    public void UpdatePrice(decimal price)
    {
        Price = price;
        Raise(new TicketTypePriceUpdatedDomainEvent(Id, price));
    }
}
