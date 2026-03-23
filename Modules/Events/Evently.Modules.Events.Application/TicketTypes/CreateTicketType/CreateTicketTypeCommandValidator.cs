using FluentValidation;

namespace Evently.Modules.Events.Application.TicketTypes.CreateTicketType;

internal sealed class CreateTicketTypeCommandValidator : AbstractValidator<CreateTicketTypeCommand>
{
    public CreateTicketTypeCommandValidator()
    {
        RuleFor(c => c.EventId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Price).GreaterThanOrEqualTo(0);
        RuleFor(c => c.Quantity).GreaterThan(0);
    }
}
