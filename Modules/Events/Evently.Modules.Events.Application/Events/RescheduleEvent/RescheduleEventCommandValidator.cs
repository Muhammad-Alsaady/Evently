using FluentValidation;

namespace Evently.Modules.Events.Application.Events.RescheduleEvent;

internal sealed class RescheduleEventCommandValidator : AbstractValidator<RescheduleEventCommand>
{
    public RescheduleEventCommandValidator()
    {
        RuleFor(c => c.EventId).NotEmpty();
        RuleFor(c => c.StartsAtUtc).NotEmpty();
        RuleFor(c => c.EndsAtUtc)
            .GreaterThan(c => c.StartsAtUtc)
            .When(c => c.EndsAtUtc.HasValue);
    }
}
