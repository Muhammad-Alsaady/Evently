using Evently.Modules.Events.Domain.Categories;
using Evently.Modules.Events.Infrastructure.Database;

namespace Evently.Modules.Events.Infrastructure.Implementations;

internal sealed class CategoryRepository(EventsDbContext context) : ICategoryRepository
{
    public async Task<Category?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Categories.FindAsync([id], cancellationToken);
    }

    public void Insert(Category category)
    {
        context.Categories.Add(category);
    }
}
