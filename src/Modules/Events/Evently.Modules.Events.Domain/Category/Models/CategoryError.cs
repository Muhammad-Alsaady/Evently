using Evently.Common.Domain.Errors;

namespace Evently.Modules.Events.Domain.Category.Models;
public static class CategoryError
{
    public static Error NotFound(Guid categoryId) =>
        Error.NotFound(
            "Categories.NotFound",
            $"The category with the identifier {categoryId} was not found");

    public static readonly Error AlreadyArchived = Error.Problem(
        "Categories.AlreadyArchived",
        "The category was already archived");
}
