using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;

public interface IIngredientCatalogService
{
    Task<IngredientResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<IngredientResponse>> GetAllAsync(CancellationToken ct = default);
    Task<IngredientResponse> CreateAsync(IngredientRequest request, CancellationToken ct = default);
    Task<IngredientResponse?> UpdateAsync(Guid id, IngredientRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
