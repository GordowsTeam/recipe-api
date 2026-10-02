using Recipe.Domain.Models.Inventory;

namespace Recipe.Application.Interfaces;

public interface IIngredientEnricher
{
    /// <summary>Asks the AI model for catalog metadata (description, food category, image) about an ingredient name.</summary>
    Task<Ingredient?> EnrichIngredientAsync(string name);
}
