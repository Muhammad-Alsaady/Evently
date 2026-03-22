using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Evently.Common.API.Abstractions;
using Evently.Common.API.Extensions;
using Evently.Modules.Users.Features.Users.Shared.Routes;

namespace Evently.Modules.Users.Features.Users.RefreshToken;

public sealed record RefreshTokenRequest(string Token, string RefreshToken);

public class RefreshTokenEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost(RouteConsts.RefreshToken, Handle);
    }

    private static async Task<IResult> Handle(
        [FromBody] RefreshTokenRequest request,
        IValidator<RefreshTokenRequest> validator,
        IRefreshTokenHandler handler,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await handler.HandleAsync(request, cancellationToken);
        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return Results.Ok(response.Value);
    }
}
