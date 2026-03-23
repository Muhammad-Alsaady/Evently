using FluentValidation;

namespace Evently.Modules.Events.Application.Events.CreateEvent;

internal sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(c => c.CategoryId).NotEmpty();
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Description).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.StartsAtUtc).NotEmpty();
        RuleFor(c => c.EndsAtUtc)
            .GreaterThan(c => c.StartsAtUtc)
            .When(c => c.EndsAtUtc.HasValue);
    }
}
