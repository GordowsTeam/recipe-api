using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;

public interface IIngredientInstanceService
{
    Task<IngredientInstanceResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Lists inventory instances belonging to the current user.</summary>
    Task<IReadOnlyList<IngredientInstanceResponse>> GetMineAsync(CancellationToken ct = default);
    Task<IngredientInstanceResponse> CreateAsync(IngredientInstanceRequest request, CancellationToken ct = default);
    Task<IngredientInstanceResponse?> UpdateAsync(Guid id, IngredientInstanceRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
