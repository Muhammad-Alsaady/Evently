namespace Evently.Modules.Attendance.Application.Attendees.GetAttendee;

public sealed record AttendeeResponse(
    Guid Id,
    Guid OrderId,
    Guid CustomerId,
    Guid EventId,
    DateTime CreatedAtUtc);
