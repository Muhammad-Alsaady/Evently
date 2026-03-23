using Waseet.CQRS;
using Evently.Common.Application.Behavoirs;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationBehaviorsExtensions
{
    public static IServiceCollection AddApplicationBehaviors(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }
}
