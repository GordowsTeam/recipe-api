using MongoDB.Driver;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Application.Validators;
using Recipe.Core.Enums;
using Recipe.Core.Models;

namespace Recipe.Infrastructure.Services
{
    public class InternalRecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;

        public InternalRecipeService(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<RecipeDetailResponse?> GetRecipeByIdAsync(string id, Language language = Language.Spanish)
        {
            if (!Guid.TryParse(id, out var recipeId))
                return null;

            var recipe = await _recipeRepository.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
                return null;

            RecipeDetailResponse response = ToRecipeDetailResponse(recipe, language);

            return response;
        }

        public async Task<IEnumerable<RecipeListResponse>?> GetRecipesAsync(RecipeRequest request)
        {
            if(!request.IsValid(out var errorMessage))
                throw new ArgumentException(errorMessage);

            var recipes = await _recipeRepository.GetRecipesAsync(request);

            var recipeResponseList = recipes?.Select(r => new RecipeListResponse
            {
                Id = r.Id.ToString(),
                Name = GetName(r, request.Language) ?? string.Empty,
                Images = r.Images?.Select(i => new Image { Url = i.Url, Main = i.Main }) ?? [],
                RecipeSourceType = r.RecipeSourceType
            });
            
            return recipeResponseList?.Where(r => !string.IsNullOrEmpty(r.Name));
        }

        private string? GetName(Domain.Models.Recipe recipe, Language language) 
        {
            if (recipe == null || recipe.Translations == null)
                return null;
            
            recipe.Translations.TryGetValue(language, out var translatedRecipe);
            
            return translatedRecipe?.Name ?? null;
        }

        private static RecipeTranslation? GetRecipeTranslation(Dictionary<Language, RecipeTranslation>? recipeTranslations, Language language) 
        {
            if (recipeTranslations == null)
                return null;

            recipeTranslations.TryGetValue(language, out var recipeTranslation);
            return recipeTranslation;
        }

        private static RecipeDetailResponse ToRecipeDetailResponse(Domain.Models.Recipe recipe, Language language)
        {
            var recipeTranslation = GetRecipeTranslation(recipe.Translations, language);
            if (recipeTranslation == null)
                throw new Exception("Recipe translation not found");

            return new RecipeDetailResponse
            {
                Id = recipe.Id.ToString(),
                Images = recipe.Images?.Select(i => new Image { Url = i.Url, Main = i.Main }) ?? [],
                Name = recipeTranslation.Name,
                Ingredients = recipeTranslation.Ingredients?.Select(i => new Ingredient
                {
                    Text = i.Name,
                    Quantity = i.Quantity,
                    Measure = i.Measure,
                    Weight = i.Weight,
                    FoodCategory = i.FoodCategory,
                    Image = i.Image
                }).ToList() ?? [],
                CuisinTypes = recipeTranslation.CuisinTypes ?? [],
                MealTypes = recipeTranslation.MealTypes ?? [],
                Directions = recipeTranslation.Directions?.Select(d => new Application.Dtos.Direction
                {
                    Step = d.Step,
                    Image = d.Image,
                    InstructionText = d.InstructionText
                }).ToList() ?? [],
                Calories = recipe.Calories,
                TotalTime = recipe.TotalTime,
                
                RecipeSourceType = recipe.RecipeSourceType
            };
        }

    }
}
