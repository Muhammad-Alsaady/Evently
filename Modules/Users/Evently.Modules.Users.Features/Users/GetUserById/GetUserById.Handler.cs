using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Evently.Common.Domain.Handlers;
using Evently.Common.Domain.Results;
using Evently.Modules.Users.Domain.Errors;
using Evently.Modules.Users.Features.Users.Shared;
using Evently.Modules.Users.Infrastructure.Database;

namespace Evently.Modules.Users.Features.Users.GetUserById;

internal interface IGetUserByIdHandler : IHandler
{
    Task<Result<UserResponse>> HandleAsync(string userId, CancellationToken cancellationToken);
}

internal sealed class GetUserByIdHandler(
    UsersDbContext context,
    ILogger<GetUserByIdHandler> logger) 
    : IGetUserByIdHandler
{
    public async Task<Result<UserResponse>> HandleAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            logger.LogInformation("User with ID {UserId} not found", userId);
            return UserErrors.NotFound(userId);
        }

        logger.LogInformation("Retrieved user with ID: {UserId}", userId);
        return new UserResponse(user.Id, user.Email!);
    }
}
