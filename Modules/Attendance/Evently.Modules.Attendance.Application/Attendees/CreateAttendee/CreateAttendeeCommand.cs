using Evently.Common.Application.Messaging;

namespace Evently.Modules.Attendance.Application.Attendees.CreateAttendee;

public sealed record CreateAttendeeCommand(Guid OrderId, Guid CustomerId, Guid EventId) : ICommand<Guid>;
