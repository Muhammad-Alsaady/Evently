using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Attendance.Application.Abstractions.Data;
using Evently.Modules.Attendance.Domain.Attendees;

namespace Evently.Modules.Attendance.Application.Attendees.CreateAttendee;

internal sealed class CreateAttendeeCommandHandler(
    IAttendeeRepository attendeeRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateAttendeeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAttendeeCommand request, CancellationToken cancellationToken = default)
    {
        var attendee = Attendee.Create(Guid.NewGuid(), request.OrderId, request.CustomerId, request.EventId);

        attendeeRepository.Insert(attendee);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return attendee.Id;
    }
}
