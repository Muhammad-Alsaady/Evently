namespace Evently.Modules.Events.Domain.Category.Repository;

public interface ICategoryRepository
{
    Task<Models.Category?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task Insert(Category.Models.Category category);
}
