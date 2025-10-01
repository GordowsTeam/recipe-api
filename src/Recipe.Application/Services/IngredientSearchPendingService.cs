using Recipe.Application.Interfaces;
using Recipe.Core.Models;

namespace Recipe.Application.Services
{
    public interface IIngredientSearchPendingService
    {
        Task AddSearchRequestAsync(List<string> ingredients);
        Task<List<IngredientSearchPending>> GetPendingAsync(int limit = 50);
        Task MarkProcessingAsync(Guid id);
        Task MarkCompletedAsync(Guid id);
        Task MarkFailedAsync(Guid id, string error);
    }

    public class IngredientSearchPendingService : IIngredientSearchPendingService
    {
        private readonly IIngredientSearchPendingRepository _repository;

        public IngredientSearchPendingService(IIngredientSearchPendingRepository ingredientSearchPendingRepository)
        {
            _repository = ingredientSearchPendingRepository;
        }

        public async Task AddSearchRequestAsync(List<string> ingredients)
        {
            var pending = new IngredientSearchPending
            {
                Ingredients = ingredients
            };
            await _repository.AddAsync(pending);
        }

        public async Task<List<IngredientSearchPending>> GetPendingAsync(int limit = 50)
        {
            return await _repository.GetUnprocessedAsync(limit);
        }

        public async Task MarkProcessingAsync(Guid id)
        {
            await _repository.MarkProcessingAsync(id);
        }

        public async Task MarkCompletedAsync(Guid id)
        {
            await _repository.MarkCompletedAsync(id);
        }

        public async Task MarkFailedAsync(Guid id, string error)
        {
            await _repository.MarkFailedAsync(id, error);
        }
    }
}
