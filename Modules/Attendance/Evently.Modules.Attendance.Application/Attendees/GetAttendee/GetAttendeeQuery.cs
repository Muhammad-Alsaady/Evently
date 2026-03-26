using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Attendance.Application.Attendees.GetAttendee;

[Cache(Key = "attendance:attendee:{AttendeeId}", Duration = 300)]
public sealed record GetAttendeeQuery(Guid AttendeeId) : IQuery<AttendeeResponse>;
