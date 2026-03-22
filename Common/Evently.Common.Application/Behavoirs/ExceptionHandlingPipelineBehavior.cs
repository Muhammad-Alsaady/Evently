using Microsoft.Extensions.Logging;
using Waseet.CQRS;
using Evently.Common.Application.Exceptions;

namespace Evently.Common.Application.Behavoirs;

internal sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
	ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		try
		{
			return await next();
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Unhandled exception for {RequestName}", typeof(TRequest).Name);

			throw new MainException(typeof(TRequest).Name, innerException: exception);
		}
	}
}
