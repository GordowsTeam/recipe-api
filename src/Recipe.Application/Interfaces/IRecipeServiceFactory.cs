using Recipe.Domain.Enums;

namespace Recipe.Application.Interfaces;
public interface IRecipeServiceFactory
{
    public IRecipeService CreateRecipeService(RecipeSourceType recipeSourceType);
}