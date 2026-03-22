using Evently.Common.Domain.Errors;

namespace Evently.Modules.Events.Domain.Categories;

public static class CategoryErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Categories.NotFound", $"Category with ID '{id}' was not found.");

    public static Error AlreadyArchived(Guid id) =>
        Error.Conflict("Categories.AlreadyArchived", $"Category with ID '{id}' is already archived.");
}
