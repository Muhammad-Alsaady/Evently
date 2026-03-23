namespace Evently.Modules.Ticketing.Domain.Events;

public interface IEventRepository
{
    void Insert(Event @event);
}
