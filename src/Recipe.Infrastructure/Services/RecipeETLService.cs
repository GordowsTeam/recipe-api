using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Domain.Enums;

namespace Recipe.Infrastructure.Services
{
    public class RecipeETLService : IRecipeETLService
    {
        private readonly IRecipeSearchUseCase _recipeSearchUseCase;
        private readonly IAIEnricher _aIEnricher;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IGetRecipeUseCase _getRecipeUseCase;
        private readonly IDuplicateFinder _duplicateFinder;

        public RecipeETLService(IRecipeSearchUseCase recipeSearchUseCase, 
            IGetRecipeUseCase getRecipeUseCase, 
            IAIEnricher aIEnricher, 
            IRecipeRepository recipeRepository,
            IDuplicateFinder duplicateFinder)
        {
            _recipeSearchUseCase = recipeSearchUseCase;
            this._aIEnricher = aIEnricher;
            this._recipeRepository = recipeRepository;
            _getRecipeUseCase = getRecipeUseCase;
            _duplicateFinder = duplicateFinder;
        }

        public async Task<bool> ProcessRecipesAsync(IEnumerable<string> ingredients, string language = "en")
        {
            foreach(var ingredient in ingredients)
            {
                if (string.IsNullOrWhiteSpace(ingredient))
                {
                    throw new ArgumentException("Ingredient cannot be null or empty");
                }
                var recipeRequest = new RecipeRequest()
                {
                    Ingredients = new List<string>() { ingredient }
                };
                var recipeDetailResponses = await GetRecipes(recipeRequest);

                // TODO: Filter out recipes that already exist in the database
                // Then enrich and store the new recipes
                foreach (var recipeDetailResponse in recipeDetailResponses)
                {
                    var recipe = recipeDetailResponse.ToRecipe();
                    if (recipe == null || await _duplicateFinder.IsDuplicateAsync(recipe!))
                    {
                        continue;
                    }

                    var enriched = await _aIEnricher.EnrichRecipeAsync(recipe);
                    await _recipeRepository.InsertRecipeAsync(enriched);
                }
            }

            return true;
        }

        private async Task<List<RecipeDetailResponse>> GetRecipes(RecipeRequest recipeRequest)
        {
            var recipeResponse = new List<RecipeDetailResponse>();
            foreach (RecipeSourceType recipeSourceType in Enum.GetValues(typeof(RecipeSourceType)))
            {
                try
                {
                    //if (Environment.GetEnvironmentVariable($"{recipeSourceType.ToString().ToLower()}_active") != "true")
                    //{
                    //    continue;
                    //}
                    if(recipeSourceType != RecipeSourceType.Spoonacular)
                    {
                        continue;
                    }

                    var response = await _recipeSearchUseCase.ExecuteAsync(recipeRequest, recipeSourceType);
                    if (response != null && response.Any())
                    {
                        foreach(var r in response)
                        {
                            var responseDetails = await _getRecipeUseCase.ExecuteAsync(r.Id.ToString(), recipeSourceType);
                            if (responseDetails != null)
                            {
                                recipeResponse.Add(responseDetails);
                            }
                        }   
                    }
                }
                catch (ArgumentException ex)
                {
                }
                catch (Exception ex)
                {
                }
            }
            return recipeResponse;
        }
    }
}
