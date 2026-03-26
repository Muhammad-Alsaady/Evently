using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Attendance.Application.Attendees.GetAttendee;
using Evently.Modules.Attendance.Domain.Attendees;

namespace Evently.Modules.Attendance.Application.Attendees.GetAttendees;

internal sealed class GetAttendeesQueryHandler(IAttendeeRepository attendeeRepository)
    : IQueryHandler<GetAttendeesQuery, IReadOnlyList<AttendeeResponse>>
{
    public async Task<Result<IReadOnlyList<AttendeeResponse>>> Handle(GetAttendeesQuery request, CancellationToken cancellationToken = default)
    {
        var attendees = await attendeeRepository.GetByEventIdAsync(request.EventId, cancellationToken);

        return attendees
            .Select(a => new AttendeeResponse(a.Id, a.OrderId, a.CustomerId, a.EventId, a.CreatedAtUtc))
            .ToList();
    }
}
