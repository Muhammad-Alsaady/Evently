using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Waseet.CQRS;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Errors;
using Evently.Common.Domain.Results;

namespace Evently.Common.Application.Behavoirs;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
	IEnumerable<IValidator<TRequest>> validators)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>, IBaseCommand
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var validationError = await ValidateAsync(request);

		if (validationError is null)
		{
			return await next();
		}

		// Case 1: Result<T>
		if (typeof(TResponse).IsGenericType &&
			typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
		{
			// Uses: Result<T>.ValidationFailure(...)
			return (TResponse)CreateGenericValidationFailure(
				typeof(TResponse),
				validationError);
		}

		// Case 2: Result<Success>
		if (typeof(TResponse) == typeof(Result<Success>))
		{
			return (TResponse)(object)Result.Failure(validationError);
		}

		// Case 3: non-Result response → throw
		throw new ValidationException(
			validationError.Errors
				.Select(e => new ValidationFailure(e.Code, e.Description))
				.ToList());
	}

	private async Task<ValidationError?> ValidateAsync(TRequest request)
	{
		if (!validators.Any())
		{
			return null;
		}

		var context = new ValidationContext<TRequest>(request);

		ValidationResult[] results = await Task.WhenAll(
			validators.Select(v => v.ValidateAsync(context)));

		var errors = results
			.Where(r => !r.IsValid)
			.SelectMany(r => r.Errors)
			.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage))
			.ToArray();

		return errors.Length == 0
			? null
			: new ValidationError(errors);
	}

	private static object CreateGenericValidationFailure(
		Type responseType,
		ValidationError validationError)
	{
		Type valueType = responseType.GetGenericArguments()[0];

		MethodInfo? method = typeof(Result<>)
			.MakeGenericType(valueType)
			.GetMethod(
				nameof(Result<object>.ValidationFailure),
				BindingFlags.Public | BindingFlags.Static);

		if (method is null)
		{
			throw new InvalidOperationException(
				$"ValidationFailure method not found on Result<{valueType.Name}>");
		}

		return method.Invoke(null, [validationError])!;
	}
}
