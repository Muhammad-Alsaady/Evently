using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Categories;

namespace Evently.Modules.Events.Application.Categories.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateCategoryCommand>
{
	public async Task<Result<Success>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken = default)
	{
		var category = await categoryRepository.GetAsync(request.Id, cancellationToken);
		if (category == null)
		{
			return CategoryErrors.NotFound(request.Id);
		}
		category.ChangeName(request.Name);
		await unitOfWork.SaveChangesAsync(cancellationToken);
		return Result.Success;
	}
}
