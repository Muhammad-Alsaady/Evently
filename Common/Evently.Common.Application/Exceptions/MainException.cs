using Evently.Common.Domain.Errors;

namespace Evently.Common.Application.Exceptions;
public sealed class MainException : Exception
{
	public MainException(string requestName, Error? error = default, Exception? innerException = default)
		: base("Application exception", innerException)
	{
		RequestName = requestName;
		Error = error;
	}

	public string RequestName { get; }

	public Error? Error { get; }
}
