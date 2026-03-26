using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Attendance.Application.Attendees.GetAttendee;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Waseet.CQRS;

namespace Evently.Modules.Attendance.Presentation.Attendees;

internal sealed class GetAttendeeEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("attendance/attendees/{attendeeId:guid}", Handle).WithTags("Attendance");
    }

    private static async Task<IResult> Handle(
        Guid attendeeId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAttendeeQuery(attendeeId), cancellationToken);

        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Results.Ok(result.Value);
    }
}
