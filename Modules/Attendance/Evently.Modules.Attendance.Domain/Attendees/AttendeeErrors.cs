using Evently.Common.Domain.Errors;

namespace Evently.Modules.Attendance.Domain.Attendees;

public sealed class AttendeeErrors
{
    public static Error NotFound(Guid attendeeId) =>
        Error.NotFound("Attendance.Attendee.NotFound", $"Attendee with ID '{attendeeId}' was not found.");
}
