using Recipe.Domain.Models.Inventory;

namespace Recipe.Application.Interfaces;

public interface IIngredientRepository
{
    Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Ingredient>> GetAllAsync(CancellationToken ct = default);
    Task<Ingredient> CreateAsync(Ingredient ingredient, CancellationToken ct = default);
    Task<Ingredient?> UpdateAsync(Ingredient ingredient, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
