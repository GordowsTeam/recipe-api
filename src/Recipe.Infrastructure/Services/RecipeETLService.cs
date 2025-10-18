using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Core.Enums;
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
        private readonly IRecipeTranslationService _recipeTranslationService;

        public RecipeETLService(
            IIngredientSearchPendingService ingredientSearchPendingService,
            IRecipeSearchUseCase recipeSearchUseCase, 
            IGetRecipeUseCase getRecipeUseCase, 
            IAIEnricher aIEnricher, 
            IRecipeRepository recipeRepository,
            IDuplicateFinder duplicateFinder,
            IRecipeTranslationService recipeTranslationService)
        {
            _recipeSearchUseCase = recipeSearchUseCase;
            _aIEnricher = aIEnricher;
            _recipeRepository = recipeRepository;
            _getRecipeUseCase = getRecipeUseCase;
            _duplicateFinder = duplicateFinder;
            _ingredientSearchPendingService = ingredientSearchPendingService;
            _recipeTranslationService = recipeTranslationService;
        }

        //TODO: Change the return instead of bool a Result<T> with more detailed information
        public async Task<bool> ProcessRecipesAsync(Language language = Language.Spanish)
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
                await ProcessRecipe(pendingRequest, language);
            }

            return true;
        }

        private async Task ProcessRecipe(IngredientSearchPending ingredientSearchPending, Language language)
        {
            try
            {
                var recipeRequest = new RecipeRequest()
                {
                    Ingredients = ingredientSearchPending.Ingredients,
                    NumberOfRecipes = 10
                };
                var recipeDetailResponses = await GetRecipes(recipeRequest, language);

                // Then enrich and store the new recipes
                foreach (var recipeDetailResponse in recipeDetailResponses)
                {
                    var recipe = recipeDetailResponse.ToRecipe();
                    if (recipe == null || await _duplicateFinder.IsDuplicateAsync(recipe!, language))
                    {
                        continue;
                    }

                    var enriched = await _aIEnricher.EnrichRecipeAsync(recipe);
                    var translated = await _recipeTranslationService.TranslateRecipeAsync(enriched, language);
                    AddTranslatedRecipe(language, enriched, translated);

                    await _recipeRepository.InsertRecipeAsync(enriched);
                    await _ingredientSearchPendingService.MarkCompletedAsync(ingredientSearchPending.Id);
                }
            }
            catch(Exception exception) 
            {
                await _ingredientSearchPendingService.MarkFailedAsync(ingredientSearchPending.Id, exception.Message);
            }
        }

        private static void AddTranslatedRecipe(Language language, Domain.Models.Recipe enriched, RecipeTranslation? translated)
        {
            if (translated == null || enriched == null)
                return;

            if (enriched.Translations != null && enriched.Translations.Any())
                enriched.Translations.Add(language, translated);
            else
            {
                enriched.Translations = new Dictionary<Core.Enums.Language, Core.Models.RecipeTranslation>
                {
                    { language, translated }
                };
            }
        }

        private async Task<List<RecipeDetailResponse>> GetRecipes(RecipeRequest recipeRequest, Language language)
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
                            var responseDetails = await _getRecipeUseCase.ExecuteAsync(r.Id.ToString(), recipeSourceType, language);
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
