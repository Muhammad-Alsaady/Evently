using Evently.Common.Domain.Errors;

namespace Evently.Common.Domain.Errors;

/// <summary>
/// Represents a validation error that aggregates multiple validation failures.
/// </summary>
public sealed record ValidationError
{
	public ValidationError(Error[] errors)
	{
		ArgumentNullException.ThrowIfNull(errors);
		
		if (errors.Length == 0)
		{
			throw new ArgumentException("Validation error must contain at least one error.", nameof(errors));
		}

		Code = "General.Validation";
		Description = "One or more validation errors occurred";
		Type = ErrorType.Validation;
		Errors = errors;
	}

	/// <summary>
	/// Gets the unique error code.
	/// </summary>
	public string Code { get; }

	/// <summary>
	/// Gets the error description.
	/// </summary>
	public string Description { get; }

	/// <summary>
	/// Gets the error type.
	/// </summary>
	public ErrorType Type { get; }

	/// <summary>
	/// Gets the collection of validation errors.
	/// </summary>
	public Error[] Errors { get; }

	/// <summary>
	/// Creates a ValidationError from multiple results, extracting errors from failed results.
	/// </summary>
	public static ValidationError FromResults(IEnumerable<Results.Result<object>> results) =>
		new(results.Where(r => r.IsError).SelectMany(r => r.Errors).ToArray());
}
