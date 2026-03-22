using Evently.Common.Domain.Events;

namespace Evently.Modules.Events.Domain.Categories;

public sealed record CategoryCreatedDomainEvent(Guid CategoryId, string Name) : DomainEvent;
