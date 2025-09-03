namespace Evently.Common.Domain.Events;
public interface IDomainEvents
{
    public Guid Id { get; init; }
    public DateTime OccurredAtUtc { get; init; }
}
