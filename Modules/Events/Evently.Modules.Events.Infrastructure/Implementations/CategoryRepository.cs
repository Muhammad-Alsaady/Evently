using Evently.Modules.Events.Domain.Categories;

namespace Evently.Modules.Events.Infrastructure.Implementations;

internal class CategoryRepository : ICategoryRepository
{
	public Task<Category?> GetAsync(Guid id, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void Insert(Category category)
	{
		throw new NotImplementedException();
	}
}
