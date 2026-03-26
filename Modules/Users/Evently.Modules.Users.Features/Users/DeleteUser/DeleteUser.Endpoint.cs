using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Users.Domain.Policies;
using Evently.Modules.Users.Features.Users.Shared.Routes;

namespace Evently.Modules.Users.Features.Users.DeleteUser;

public class DeleteUserEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete(RouteConsts.DeleteUser, Handle)
            .WithTags("Users")
            .RequireAuthorization(UserPolicyConsts.DeletePolicy);
    }

    private static async Task<IResult> Handle(
        string userId,
        IDeleteUserHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(userId, cancellationToken);
        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return Results.NoContent();
    }
}
