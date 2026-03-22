using Evently.Common.Domain.Errors;

namespace Evently.Modules.Events.Domain.Events;

public static class EventErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Events.NotFound", $"Event with ID '{id}' was not found.");

    public static Error NotDraft(Guid id) =>
        Error.Conflict("Events.NotDraft", $"Event with ID '{id}' is not in Draft status.");

    public static Error NotPublished(Guid id) =>
        Error.Conflict("Events.NotPublished", $"Event with ID '{id}' is not in Published status.");

    public static Error AlreadyCancelled(Guid id) =>
        Error.Conflict("Events.AlreadyCancelled", $"Event with ID '{id}' is already cancelled.");
}
