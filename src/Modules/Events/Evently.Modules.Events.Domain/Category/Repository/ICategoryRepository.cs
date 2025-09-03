namespace Evently.Modules.Events.Domain.Category.Repository;

public interface ICategoryRepository
{
    Task<IEnumerable<Category.Models.Category>> GetAllAsync();
    Task<Category.Models.Category> GetByIdAsync(Guid id);
    Task AddAsync(Category.Models.Category category);
}
