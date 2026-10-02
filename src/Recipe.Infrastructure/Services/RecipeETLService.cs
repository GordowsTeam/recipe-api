using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Core.Enums;
using Recipe.Core.Models;
using Recipe.Domain.Enums;
using System.Diagnostics;

namespace Recipe.Infrastructure.Services
{
    public class RecipeETLService : IRecipeETLService
    {
        [Conditional("DEBUG")]
        private static void LogStep(string message) => Console.WriteLine($"[RecipeETLService] {message}");

        private readonly IRecipeSearchUseCase _recipeSearchUseCase;
        private readonly IAIEnricher _aIEnricher;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IGetRecipeUseCase _getRecipeUseCase;
        private readonly IDuplicateFinder _duplicateFinder;
        private readonly IIngredientSearchPendingService _ingredientSearchPendingService;
        private readonly IRecipeTranslationService _recipeTranslationService;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IIngredientEnricher _ingredientEnricher;

        public RecipeETLService(
            IIngredientSearchPendingService ingredientSearchPendingService,
            IRecipeSearchUseCase recipeSearchUseCase,
            IGetRecipeUseCase getRecipeUseCase,
            IAIEnricher aIEnricher,
            IRecipeRepository recipeRepository,
            IDuplicateFinder duplicateFinder,
            IRecipeTranslationService recipeTranslationService,
            IIngredientRepository ingredientRepository,
            IIngredientEnricher ingredientEnricher)
        {
            _recipeSearchUseCase = recipeSearchUseCase;
            _aIEnricher = aIEnricher;
            _recipeRepository = recipeRepository;
            _getRecipeUseCase = getRecipeUseCase;
            _duplicateFinder = duplicateFinder;
            _ingredientSearchPendingService = ingredientSearchPendingService;
            _recipeTranslationService = recipeTranslationService;
            _ingredientRepository = ingredientRepository;
            _ingredientEnricher = ingredientEnricher;
        }

        //TODO: Change the return instead of bool a Result<T> with more detailed information
        public async Task<bool> ProcessRecipesAsync(Language language = Language.Spanish)
        {
            LogStep($"Starting ETL run for language '{language}'.");
            var pendingRequests = await _ingredientSearchPendingService.GetPendingAsync(50);
            LogStep($"Fetched {pendingRequests.Count()} pending request(s).");
            foreach (var pendingRequest in pendingRequests)
            {
                LogStep($"Processing pending request '{pendingRequest.Id}' with ingredients: {string.Join(", ", pendingRequest.Ingredients ?? new List<string>())}.");
                if (pendingRequest.Ingredients == null ||
                    pendingRequest.Ingredients.Count == 0 ||
                    pendingRequest.Ingredients.Any(string.IsNullOrEmpty))
                {
                    LogStep($"Pending request '{pendingRequest.Id}' has invalid ingredients. Marking as failed.");
                    await _ingredientSearchPendingService.MarkFailedAsync(pendingRequest.Id, "Invalid ingredients");
                }
                LogStep($"Marking pending request '{pendingRequest.Id}' as processing.");
                await _ingredientSearchPendingService.MarkProcessingAsync(pendingRequest.Id);
                await ProcessRecipe(pendingRequest, language);
            }

            LogStep("ETL run finished.");
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
                LogStep($"Searching recipes for pending request '{ingredientSearchPending.Id}'.");
                var recipeDetailResponses = await GetRecipes(recipeRequest, language);
                LogStep($"Found {recipeDetailResponses.Count} recipe detail response(s) for pending request '{ingredientSearchPending.Id}'.");

                // Then enrich and store the new recipes
                foreach (var recipeDetailResponse in recipeDetailResponses)
                {
                    try
                    {
                        var recipe = recipeDetailResponse.ToRecipe();
                        if (recipe == null || await _duplicateFinder.IsDuplicateAsync(recipe!, language))
                        {
                            LogStep($"Skipping recipe '{recipeDetailResponse.Id}': null or duplicate.");
                            continue;
                        }

                        LogStep($"Enriching recipe '{recipe.Id}' with AI.");
                        var enriched = await _aIEnricher.EnrichRecipeAsync(recipe);
                        LogStep($"Translating recipe '{enriched.Id}' to '{language}'.");
                        var translated = await _recipeTranslationService.TranslateRecipeAsync(enriched, language);
                        AddTranslatedRecipe(language, enriched, translated);
                        LogStep($"Linking catalog ingredients for recipe '{enriched.Id}'.");
                        await LinkIngredientsAsync(enriched);

                        LogStep($"Inserting recipe '{enriched.Id}' into the repository.");
                        await _recipeRepository.InsertRecipeAsync(enriched);
                        LogStep($"Marking pending request '{ingredientSearchPending.Id}' as completed.");
                        await _ingredientSearchPendingService.MarkCompletedAsync(ingredientSearchPending.Id);
                    }
                    catch (Exception exception)
                    {
                        // Don't let one bad recipe (e.g. an AI enrichment failure) stop the rest of the batch.
                        LogStep($"Recipe '{recipeDetailResponse.Id}' failed and will be skipped: {exception.Message}");
                        Console.WriteLine($"[RecipeETLService] Recipe '{recipeDetailResponse.Id}' failed and will be skipped: {exception.Message}");
                    }
                }
            }
            catch(Exception exception)
            {
                LogStep($"Pending request '{ingredientSearchPending.Id}' failed: {exception.Message}");
                await _ingredientSearchPendingService.MarkFailedAsync(ingredientSearchPending.Id, exception.Message);
            }
        }

        /// <summary>Resolves each ingredient line against the ingredient catalog, creating AI-enriched catalog entries for unmatched names.</summary>
        private async Task LinkIngredientsAsync(Domain.Models.Recipe recipe)
        {
            if (recipe.Translations == null)
                return;

            foreach (var translation in recipe.Translations.Values)
            {
                if (translation.Ingredients == null)
                    continue;

                foreach (var ingredient in translation.Ingredients)
                {
                    if (string.IsNullOrWhiteSpace(ingredient.Name))
                        continue;

                    LogStep($"Looking up catalog ingredient '{ingredient.Name}'.");
                    var catalogIngredient = await _ingredientRepository.GetByNameAsync(ingredient.Name);
                    if (catalogIngredient == null)
                    {
                        LogStep($"Catalog ingredient '{ingredient.Name}' not found. Creating it.");
                        catalogIngredient = await CreateCatalogIngredientAsync(ingredient);
                    }
                    else
                    {
                        LogStep($"Catalog ingredient '{ingredient.Name}' found with id '{catalogIngredient.Id}'.");
                    }
                    ingredient.IngredientId = catalogIngredient.Id;
                }
            }
        }

        /// <summary>Ingredient wasn't found in the catalog: ask the AI for its details and persist a new catalog entry.</summary>
        private async Task<Domain.Models.Inventory.Ingredient> CreateCatalogIngredientAsync(Domain.Models.Ingredient ingredient)
        {
            LogStep($"Enriching ingredient '{ingredient.Name}' with AI.");
            var enriched = await _ingredientEnricher.EnrichIngredientAsync(ingredient.Name!);
            var now = DateTime.UtcNow;
            var newIngredient = new Domain.Models.Inventory.Ingredient
            {
                Id = Guid.NewGuid(),
                Name = ingredient.Name!,
                Description = enriched?.Description,
                FoodCategory = enriched?.FoodCategory ?? ingredient.FoodCategory,
                Image = enriched?.Image ?? ingredient.Image,
                CreatedDateTime = now,
                UpdatedDateTime = now
            };
            LogStep($"Creating catalog ingredient '{newIngredient.Name}' with id '{newIngredient.Id}'.");
            return await _ingredientRepository.CreateAsync(newIngredient);
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

                    LogStep($"Searching recipes from source '{recipeSourceType}'.");
                    var response = await _recipeSearchUseCase.ExecuteAsync(recipeRequest, recipeSourceType);
                    if (response != null && response.Any())
                    {
                        LogStep($"Source '{recipeSourceType}' returned {response.Count()} result(s).");
                        foreach(var r in response)
                        {
                            LogStep($"Fetching recipe details for '{r.Id}' from source '{recipeSourceType}'.");
                            var responseDetails = await _getRecipeUseCase.ExecuteAsync(r.Id.ToString(), recipeSourceType, language);
                            if (responseDetails != null)
                            {
                                recipeResponse.Add(responseDetails);
                            }
                        }
                    }
                    else
                    {
                        LogStep($"Source '{recipeSourceType}' returned no results.");
                    }
                }
                catch (ArgumentException ex)
                {
                    LogStep($"ArgumentException while fetching from source '{recipeSourceType}': {ex.Message}");
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    LogStep($"Exception while fetching from source '{recipeSourceType}': {ex.Message}");
                    Console.WriteLine(ex.Message);
                }
            }
            return recipeResponse;
        }
    }
}
