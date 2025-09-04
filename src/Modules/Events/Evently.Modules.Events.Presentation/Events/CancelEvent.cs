using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Application.Events.CancelEvent;
using Evently.Modules.Events.Presentation.ApiResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Evently.Modules.Events.Presentation.Events;
internal static class CancelEvent
{
    public static void MapEndPoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/events/{id}/cancel", async (Guid id, ISender sender) =>
        {
            var command = new CancelEventCommand(id);
            Result result = await sender.Send(command);
            return result.Match(Results.NoContent, ApiResults.ApiResults.Problem);
        });
    }
}
