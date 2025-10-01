using Recipe.Core.Models;

namespace Recipe.Application.Interfaces
{
    public interface IIngredientSearchPendingRepository
    {
        Task AddAsync(IngredientSearchPending ingredientSearchPending);
        Task<List<IngredientSearchPending>> GetUnprocessedAsync(int limit = 50);
        Task MarkProcessingAsync(Guid id);
        Task MarkCompletedAsync(Guid id);
        Task MarkFailedAsync(Guid id, string error);
    }
}
