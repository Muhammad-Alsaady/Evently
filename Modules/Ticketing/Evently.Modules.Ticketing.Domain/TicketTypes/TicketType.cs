using Evently.Common.Domain;

namespace Evently.Modules.Ticketing.Domain.TicketTypes;

// Read model — mirrors a ticket type published by the Events module.
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
        return new TicketType
        {
            Id = id,
            EventId = eventId,
            Name = name,
            Price = price,
            Quantity = quantity
        };
    }
}
