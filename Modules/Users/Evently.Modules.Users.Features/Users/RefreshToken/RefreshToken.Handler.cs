using Evently.Common.Domain.Handlers;
using Evently.Common.Domain.Results;
using Evently.Modules.Users.Domain.Authentication;

namespace Evently.Modules.Users.Features.Users.RefreshToken;

internal interface IRefreshTokenHandler : IHandler
{
    Task<Result<RefreshTokenResponse>> HandleAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
}

internal sealed class RefreshTokenHandler(IClientAuthorizationService authorizationService)
    : IRefreshTokenHandler
{
    public async Task<Result<RefreshTokenResponse>> HandleAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authorizationService.RefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result;
    }
}
