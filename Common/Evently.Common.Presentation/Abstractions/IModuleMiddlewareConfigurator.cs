using Microsoft.AspNetCore.Builder;

namespace Evently.Common.API.Abstractions;

public interface IModuleMiddlewareConfigurator
{
    IApplicationBuilder Configure(IApplicationBuilder app);
}
