using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Core.Models;
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
        private readonly IIngredientSearchPendingService _ingredientSearchPendingService;

        public RecipeETLService(
            IIngredientSearchPendingService ingredientSearchPendingService,
            IRecipeSearchUseCase recipeSearchUseCase, 
            IGetRecipeUseCase getRecipeUseCase, 
            IAIEnricher aIEnricher, 
            IRecipeRepository recipeRepository,
            IDuplicateFinder duplicateFinder)
        {
            _recipeSearchUseCase = recipeSearchUseCase;
            _aIEnricher = aIEnricher;
            _recipeRepository = recipeRepository;
            _getRecipeUseCase = getRecipeUseCase;
            _duplicateFinder = duplicateFinder;
            _ingredientSearchPendingService = ingredientSearchPendingService;
        }

        //TODO: Change the return instead of bool a Result<T> with more detailed information
        public async Task<bool> ProcessRecipesAsync(string language = "en")
        {
            var pendingRequests = await _ingredientSearchPendingService.GetPendingAsync(50);
            foreach (var pendingRequest in pendingRequests)
            {
                if (pendingRequest.Ingredients == null || 
                    pendingRequest.Ingredients.Count == 0 ||
                    pendingRequest.Ingredients.Any(string.IsNullOrEmpty))
                {
                    await _ingredientSearchPendingService.MarkFailedAsync(pendingRequest.Id, "Invalid ingredients");
                }
                await _ingredientSearchPendingService.MarkProcessingAsync(pendingRequest.Id);
                await ProcessRecipe(pendingRequest);
            }

            return true;
        }

        private async Task ProcessRecipe(IngredientSearchPending ingredientSearchPending)
        {
            try
            {
                var recipeRequest = new RecipeRequest()
                {
                    Ingredients = ingredientSearchPending.Ingredients,
                    NumberOfRecipes = 10
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
                    await _ingredientSearchPendingService.MarkCompletedAsync(ingredientSearchPending.Id);
                }
            }
            catch(Exception exception) 
            {
                await _ingredientSearchPendingService.MarkFailedAsync(ingredientSearchPending.Id, exception.Message);
            }
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
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return recipeResponse;
        }
    }
}
