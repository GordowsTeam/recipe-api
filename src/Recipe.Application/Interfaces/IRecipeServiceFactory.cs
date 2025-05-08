using Recipe.Core.Enums;
using Recipe.Application.Interfaces;

namespace Recipe.Application.Interfaces;
public interface IRecipeServiceFactory
{
    public IRecipeService CreateRecipeService(RecipeSourceType recipeSourceType);
}