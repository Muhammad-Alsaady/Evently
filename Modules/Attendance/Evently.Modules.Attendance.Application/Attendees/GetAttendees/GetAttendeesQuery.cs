using Evently.Common.Application.Messaging;
using Evently.Modules.Attendance.Application.Attendees.GetAttendee;

namespace Evently.Modules.Attendance.Application.Attendees.GetAttendees;

public sealed record GetAttendeesQuery(Guid EventId) : IQuery<IReadOnlyList<AttendeeResponse>>;
