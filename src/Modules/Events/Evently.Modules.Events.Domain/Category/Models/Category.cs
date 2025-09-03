using Evently.Common.Domain.BaseEntity;
using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Domain.Category.DomainEvents;

namespace Evently.Modules.Events.Domain.Category.Models;

public sealed class Category : Entity
{
    public Guid Id { get; private init; }
    public string Name { get; private set; }
    public bool IsArchived { get; private set; }

    public Category()
    {
    }
    public static Result<Category> Create(string name)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsArchived = false,
        };

        category.AddDomainEvent(new CategoryCreatedDomainEvent() { CategoryId = category.Id });
        return category;
    }

    public void Update(string name)
    {
        if (Name == name)
        { return; }
        Name = name;
        AddDomainEvent(new CategoryUpdatedDomainEvent() { CategoryId = Id, Name = name });

    }

    public Result Archive()
    {
        if (IsArchived)
        { return CategoryError.AlreadyArchived; }
        IsArchived = !IsArchived;
        AddDomainEvent(new CategoryArchivedDomainEvent() { CategoryId = Id });
        return Result.Success();
    }
}


