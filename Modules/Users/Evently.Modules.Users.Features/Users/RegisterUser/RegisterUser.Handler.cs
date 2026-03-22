using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Evently.Common.Domain.Handlers;
using Evently.Common.Domain.Results;
using Evently.Modules.Users.Domain.Errors;
using Evently.Modules.Users.Domain.Users;
using Evently.Modules.Users.Features.Users.Shared;

namespace Evently.Modules.Users.Features.Users.RegisterUser;

internal interface IRegisterUserHandler : IHandler
{
    Task<Result<UserResponse>> HandleAsync(RegisterUserRequest request, CancellationToken cancellationToken);
}

internal sealed class RegisterUserHandler(
    UserManager<User> userManager,
    ILogger<RegisterUserHandler> logger) 
    : IRegisterUserHandler
{
    public async Task<Result<UserResponse>> HandleAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            UserName = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            logger.LogInformation("Failed to register user: {@Errors}", result.Errors);
            return UserErrors.RegistrationFailed(result.Errors);
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            await userManager.AddToRoleAsync(user, request.Role);
        }

        logger.LogInformation("Created user with ID: {UserId}", user.Id);

        return new UserResponse(user.Id, user.Email);
    }
}
