using Evently.Modules.Attendance.Domain.Attendees;
using Evently.Modules.Attendance.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Evently.Modules.Attendance.Infrastructure.Implementations;

internal sealed class AttendeeRepository(AttendanceDbContext context) : IAttendeeRepository
{
    public async Task<Attendee?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Attendees.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<Attendee>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        => await context.Attendees
            .Where(a => a.EventId == eventId)
            .ToListAsync(cancellationToken);

    public void Insert(Attendee attendee) => context.Attendees.Add(attendee);
}
