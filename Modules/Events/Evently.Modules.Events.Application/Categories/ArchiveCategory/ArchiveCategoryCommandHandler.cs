using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Results;
using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Categories;

namespace Evently.Modules.Events.Application.Categories.ArchiveCategory;

internal sealed class ArchiveCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) : ICommandHandler<ArchiveCategoryCommand>
{
	public async Task<Result<Success>> Handle(ArchiveCategoryCommand request, CancellationToken cancellationToken = default)
	{
		var category = await categoryRepository.GetAsync(request.Id, cancellationToken);
		if(category == null)
		{
			return CategoryErrors.NotFound(request.Id);
		}
		var result = category.Archive();
		if (result.IsError)
		{
			return result;
		}

		await unitOfWork.SaveChangesAsync(cancellationToken);
		return Result.Success;
	}
}
