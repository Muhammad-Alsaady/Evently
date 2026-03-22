using Evently.Common.Domain.Events;

namespace Evently.Modules.Events.Domain.Categories;

public sealed record CategoryArchivedDomainEvent(Guid CategoryId) : DomainEvent;
