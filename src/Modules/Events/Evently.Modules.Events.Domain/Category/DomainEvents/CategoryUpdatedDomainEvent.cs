namespace Evently.Modules.Events.Domain.Category.DomainEvents;
public sealed class CategoryUpdatedDomainEvent : Common.Domain.DomainEvents.DomainEvents
{
    public Guid CategoryId { get; init; }
    public required string Name { get; init; }

}
