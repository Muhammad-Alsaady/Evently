namespace Evently.Modules.Events.Domain.Category.DomainEvents;
public class CategoryArchivedDomainEvent : Common.Domain.DomainEvents.DomainEvents
{
    public required Guid CategoryId { get; init; }
}
