using Evently.Common.Domain.Events;

namespace Evently.Common.Domain.BaseEntity;

public class Entity
{
    private readonly List<IDomainEvents> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvents> GetDomainEvents() => _domainEvents.ToList();

    protected void AddDomainEvent(IDomainEvents domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
