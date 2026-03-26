namespace Evently.Modules.Attendance.Domain.Attendees;

public interface IAttendeeRepository
{
    Task<Attendee?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Attendee>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    void Insert(Attendee attendee);
}
