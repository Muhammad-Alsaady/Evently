using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Users.Domain.Policies;
using Evently.Modules.Users.Features.Users.Shared.Routes;

namespace Evently.Modules.Users.Features.Users.UpdateUserRole;

public sealed record UpdateUserRoleRequest(string NewRole);

public class UpdateUserRoleEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost(RouteConsts.UpdateUserRole, Handle)
            .RequireAuthorization(UserPolicyConsts.UpdatePolicy);
    }

    private static async Task<IResult> Handle(
        string userId,
        [FromBody] UpdateUserRoleRequest request,
        IValidator<UpdateUserRoleRequest> validator,
        IUpdateUserRoleHandler handler,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await handler.HandleAsync(userId, request, cancellationToken);
        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return Results.NoContent();
    }
}
