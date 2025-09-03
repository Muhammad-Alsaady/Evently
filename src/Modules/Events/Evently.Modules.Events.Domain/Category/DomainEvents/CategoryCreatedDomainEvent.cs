
namespace Evently.Modules.Events.Domain.Category.DomainEvents;

public sealed class CategoryCreatedDomainEvent : Common.Domain.DomainEvents.DomainEvents
{
    public required Guid CategoryId { get; init; }
}
