namespace Evently.Modules.Ticketing.Domain.Events;

public interface IEventRepository
{
	Task<Event?> GetAsync(Guid id);
    void Insert(Event @event);
}
