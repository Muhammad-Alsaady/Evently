using Evently.Modules.Events.Domain.Category.Models;
using Evently.Modules.Events.Domain.Category.Repository;
using Evently.Modules.Events.Infrastructure.Database;

namespace Evently.Modules.Events.Infrastructure.Categories;
internal sealed class CategoryRepository(EventsDbContext context) : ICategoryRepository
{
    public async Task<Category?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Categories.FindAsync([id], cancellationToken);
    }

    public async Task Insert(Category category)
    {
        await context.Categories.AddAsync(category);
    }
}
