using Evently.Common.Domain.Events;

namespace Evently.Common.Domain.DomainEvents;
public class DomainEvents : IDomainEvents
{
    public Guid Id { get; init; }
    public DateTime OccurredAtUtc { get; init; }
}
