using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Attendance.Domain.Attendees;

namespace Evently.Modules.Attendance.Application.Attendees.GetAttendee;

internal sealed class GetAttendeeQueryHandler(IAttendeeRepository attendeeRepository)
    : IQueryHandler<GetAttendeeQuery, AttendeeResponse>
{
    public async Task<Result<AttendeeResponse>> Handle(GetAttendeeQuery request, CancellationToken cancellationToken = default)
    {
        var attendee = await attendeeRepository.GetAsync(request.AttendeeId, cancellationToken);

        if (attendee is null)
        {
            return AttendeeErrors.NotFound(request.AttendeeId);
        }

        return new AttendeeResponse(attendee.Id, attendee.OrderId, attendee.CustomerId, attendee.EventId, attendee.CreatedAtUtc);
    }
}
