using Evently.Common.Domain;

namespace Evently.Modules.Attendance.Domain.Attendees;

public sealed class Attendee : Entity
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid EventId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Attendee() { }

    public static Attendee Create(Guid id, Guid orderId, Guid customerId, Guid eventId)
    {
        return new Attendee
        {
            Id = id,
            OrderId = orderId,
            CustomerId = customerId,
            EventId = eventId,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
