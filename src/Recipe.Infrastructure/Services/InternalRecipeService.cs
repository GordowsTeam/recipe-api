using MongoDB.Driver;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Application.Validators;

namespace Recipe.Infrastructure.Services
{
    public class InternalRecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;

        public InternalRecipeService(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<RecipeDetailResponse?> GetRecipeByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var recipeId))
                return null;

            var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
                return null;

            RecipeDetailResponse response = ToRecipeDetailResponse(recipe);

            return response;
        }

        public async Task<IEnumerable<RecipeListResponse>?> GetRecipesAsync(RecipeRequest request)
        {
            if(!request.IsValid(out var errorMessage))
                throw new ArgumentException(errorMessage);

            var recipes = await _recipeRepository.GetRecipesAsync(request);
            
            return recipes?.Select(r => new RecipeListResponse
            {
                Id = r.Id.ToString(),
                Name = r.Name,
                Images = r.Images?.Select(i => new Image { Url = i.Url, Main = i.Main }) ?? [],
                RecipeSourceType = r.RecipeSourceType
            });
        }


        private static RecipeDetailResponse ToRecipeDetailResponse(Domain.Models.Recipe recipe)
        {
            return new RecipeDetailResponse
            {
                Id = recipe.Id.ToString(),
                Name = recipe.Name,
                Images = recipe.Images?.Select(i => new Image { Url = i.Url, Main = i.Main }) ?? [],
                Ingredients = recipe.Ingredients?.Select(i => new Ingredient
                {
                    Text = i.Name,
                    Quantity = i.Quantity,
                    Measure = i.Measure,
                    Weight = i.Weight,
                    FoodCategory = i.FoodCategory,
                    Image = i.Image
                }).ToList() ?? [],
                Calories = recipe.Calories,
                TotalTime = recipe.TotalTime,
                CuisinTypes = recipe.CuisinTypes ?? [],
                MealTypes = recipe.MealTypes ?? [],
                Directions = recipe.Directions?.Select(d => new Application.Dtos.Direction
                {
                    Step = d.Step,
                    Image = d.Image,
                    InstructionText = d.InstructionText
                }).ToList() ?? [],
                RecipeSourceType = recipe.RecipeSourceType
            };
        }

    }
}
