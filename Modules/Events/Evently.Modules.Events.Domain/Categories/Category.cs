using Evently.Common.Domain;
using Evently.Common.Domain.Results;

namespace Evently.Modules.Events.Domain.Categories;

public sealed class Category : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public bool IsArchived { get; private set; }

    private Category() { }

    public static Category Create(Guid id, string name)
    {
        var category = new Category
        {
            Id = id,
            Name = name,
            IsArchived = false
        };

        category.Raise(new CategoryCreatedDomainEvent(id, name));

        return category;
    }

    public void ChangeName(string name)
    {
        Name = name;
        Raise(new CategoryNameChangedDomainEvent(Id, name));
    }

    public Result<Success> Archive()
    {
        if (IsArchived)
		{
			return CategoryErrors.AlreadyArchived(Id);
		}

		IsArchived = true;
        Raise(new CategoryArchivedDomainEvent(Id));

        return Result.Success;
    }
}
