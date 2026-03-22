namespace Evently.Common.Domain.Events;

public interface IDomainEvent : IEvent
{
	Guid Id { get; }
	DateTime OccurredOnUtc { get; }
}
