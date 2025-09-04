using Evently.Common.Application.Messaging;
using Evently.Common.Domain.ResultPattern;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Category.Models;
using Evently.Modules.Events.Domain.Category.Repository;


namespace Evently.Modules.Events.Application.Categories.CreateCategory;

internal sealed class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        Category category = Category.Create(request.Name);

        await categoryRepository.Insert(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
