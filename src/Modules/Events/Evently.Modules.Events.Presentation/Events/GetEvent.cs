using Evently.Modules.Events.Application.Events.GetEvent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Evently.Modules.Events.Presentation.Events;
internal static class GetEvent
{
    public static void MapEndPoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/events/{id}", async (Guid id, ISender sender) =>
            {
                EventResponse? @event = await sender.Send(new GetEventQuery(id));

                return @event is not null
                    ? Results.Ok(@event)
                    : Results.NotFound();
            }).WithTags(Tag.Event);
    }
}
