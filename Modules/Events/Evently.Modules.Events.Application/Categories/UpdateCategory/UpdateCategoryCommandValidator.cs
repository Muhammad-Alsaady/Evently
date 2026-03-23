using FluentValidation;

namespace Evently.Modules.Events.Application.Categories.UpdateCategory;

internal sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
	public UpdateCategoryCommandValidator()
	{
		RuleFor(c => c.Id).NotEmpty();
		RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
	}
}
