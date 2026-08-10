using Recipe.Domain.Models.Inventory;

namespace Recipe.Application.Interfaces;

public interface IIngredientInstanceRepository
{
    Task<IngredientInstance?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<IngredientInstance>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<IngredientInstance> CreateAsync(IngredientInstance instance, CancellationToken ct = default);
    Task<IngredientInstance?> UpdateAsync(IngredientInstance instance, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
