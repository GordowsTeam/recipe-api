using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Ingredient = Recipe.Domain.Models.Inventory.Ingredient;

namespace Recipe.Application.Services;

public class IngredientCatalogService(IIngredientRepository ingredientRepository) : IIngredientCatalogService
{
    private readonly IIngredientRepository _ingredientRepository = ingredientRepository;

    public async Task<IngredientResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(id, ct);
        return ingredient == null ? null : ToResponse(ingredient);
    }

    public async Task<IReadOnlyList<IngredientResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var ingredients = await _ingredientRepository.GetAllAsync(ct);
        return ingredients.Select(ToResponse).ToList();
    }

    public async Task<IngredientResponse> CreateAsync(IngredientRequest request, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var ingredient = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            FoodCategory = request.FoodCategory,
            Image = request.Image,
            CreatedDateTime = now,
            UpdatedDateTime = now
        };
        var created = await _ingredientRepository.CreateAsync(ingredient, ct);
        return ToResponse(created);
    }

    public async Task<IngredientResponse?> UpdateAsync(Guid id, IngredientRequest request, CancellationToken ct = default)
    {
        var existing = await _ingredientRepository.GetByIdAsync(id, ct);
        if (existing == null)
            return null;

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.FoodCategory = request.FoodCategory;
        existing.Image = request.Image;
        existing.UpdatedDateTime = DateTime.UtcNow;

        var updated = await _ingredientRepository.UpdateAsync(existing, ct);
        return updated == null ? null : ToResponse(updated);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) => _ingredientRepository.DeleteAsync(id, ct);

    private static IngredientResponse ToResponse(Ingredient ingredient)
    {
        return new IngredientResponse
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            FoodCategory = ingredient.FoodCategory,
            Image = ingredient.Image,
            CreatedDateTime = ingredient.CreatedDateTime,
            UpdatedDateTime = ingredient.UpdatedDateTime
        };
    }
}
