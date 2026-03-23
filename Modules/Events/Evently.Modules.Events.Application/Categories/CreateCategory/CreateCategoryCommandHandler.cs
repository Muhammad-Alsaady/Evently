using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Categories;

namespace Evently.Modules.Events.Application.Categories.CreateCategory;

internal sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = Category.Create(Guid.NewGuid(), command.Name);

        categoryRepository.Insert(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
