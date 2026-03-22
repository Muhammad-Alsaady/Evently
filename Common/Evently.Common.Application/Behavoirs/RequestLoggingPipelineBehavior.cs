using Microsoft.Extensions.Logging;
using Evently.Common.Domain.Results;
using Waseet.CQRS;

namespace Evently.Common.Application.Behavoirs;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger) :
	IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var requestName = typeof(TRequest).Name;
		logger.LogInformation("Processing request {RequestName}", requestName);
		TResponse result = await next();

		if (result is IResult { IsError: true } failedResult)
		{
			logger.LogWarning("Request {RequestName} failed with errors: {@Errors}",
				requestName, failedResult.Errors);
		}

		logger.LogInformation("Completed request {RequestName}", requestName);

		return result;
	}
}
