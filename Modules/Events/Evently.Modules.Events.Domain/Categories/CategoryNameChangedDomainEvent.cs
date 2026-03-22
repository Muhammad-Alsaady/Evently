using Evently.Common.Domain.Events;

namespace Evently.Modules.Events.Domain.Categories;

public sealed record CategoryNameChangedDomainEvent(Guid CategoryId, string Name) : DomainEvent;
