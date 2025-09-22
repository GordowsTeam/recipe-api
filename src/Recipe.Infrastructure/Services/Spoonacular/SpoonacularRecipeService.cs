using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recipe.Application.Interfaces;
using Recipe.Domain.Enums;
using Recipe.Application.Dtos;
using Ingredient = Recipe.Application.Dtos.Ingredient;

namespace Recipe.Infrastructure.Services.Spoonacular;
public class SpoonacularRecipeService: IRecipeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptionSerialier;
    private readonly SpoonacularAPISettings _spoonacularApiSettings;
    private readonly ILogger<SpoonacularRecipeService> _logger;
    private const string searchEndpoint = "/recipes/complexSearch";
    private const string recipeDetailEndpoint = "/recipes";

    public SpoonacularRecipeService(IHttpClientFactory httpClientFactory, IOptions<SpoonacularAPISettings> spoonacularApiSettings, ILogger<SpoonacularRecipeService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptionSerialier = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        _spoonacularApiSettings = spoonacularApiSettings.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<RecipeListResponse>?> GetRecipesAsync(RecipeRequest request)
    {
        try
        {
            if (request == null || request.Ingredients == null || !request.Ingredients.Any() || request.Ingredients.Any(l => string.IsNullOrEmpty(l)))
            {
                throw new ArgumentException("Ingredients cannot be null or empty", nameof(request.Ingredients));
            }

            var httpClient = _httpClientFactory.CreateClient("SpoonacularAPI");
            httpClient.BaseAddress = new Uri(_spoonacularApiSettings.Uri);
            var requestUri = $"{searchEndpoint}?" +
                $"apiKey={_spoonacularApiSettings.ApiKey}" +
                $"&query={GetIngredients(request.Ingredients)}" +
                $"&number=1";
            
            var response = await httpClient.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var spoonacularRecipeResponse = JsonSerializer.Deserialize<SpoonacularRecipeSearchResponse>(responseBody, _jsonOptionSerialier);

                if (spoonacularRecipeResponse == null)
                {
                    _logger.LogError("Spoonacular API response is null");
                    return null;
                }

                return MapRecipeList(spoonacularRecipeResponse);
            }

            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }
        catch (HttpRequestException e)
        {
            _logger.LogError($"Request error in Spoonacular API Service: {e.Message}");
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError($"There was an exception while trying to consume Spoonacular API Services. Error: {e.Message}");
            return null;
        }
    }

    private string GetIngredients(IEnumerable<string> ingredients) 
    {
        return string.Join(",", ingredients.Select(i => "+" + i));
    }

    private IEnumerable<RecipeListResponse> MapRecipeList(SpoonacularRecipeSearchResponse spoonacularRecipeSearchResponses)
    {
        var result = new List<RecipeListResponse>();

        foreach (var spoonacularRecipe in spoonacularRecipeSearchResponses.results)
        {
            
            result.Add(new RecipeListResponse()
            {
                Id = spoonacularRecipe.id.ToString(),
                Name =  spoonacularRecipe.title ?? string.Empty,
                Images = [new() { Url = spoonacularRecipe.image ?? string.Empty, Main = true }],
                RecipeSourceType = RecipeSourceType.Spoonacular
            });
        }

        return result;
    }

    public async Task<RecipeDetailResponse?> GetRecipeByIdAsync(string id)
    {
        try
        {
            int.TryParse(id, out var intId);

            var httpClient = _httpClientFactory.CreateClient("SpoonacularAPI");
            httpClient.BaseAddress = new Uri(_spoonacularApiSettings.Uri);
            var requestUri = $"{recipeDetailEndpoint}/{intId}/information?" +
                $"apiKey={_spoonacularApiSettings.ApiKey}";

            var response = await httpClient.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var details = JsonSerializer.Deserialize<SpoonacularRecipeDetail>(responseBody, _jsonOptionSerialier);

                if (details == null)
                {
                    _logger.LogError("Spoonacular API response is null");
                    return null;
                }

                return Map(details);
            }

            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }
        catch (HttpRequestException e)
        {
            _logger.LogError($"Request error in Spoonacular API Service: {e.Message}");
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError($"There was an exception while trying to consume Spoonacular API Services. Error: {e.Message}");
            return null;
        }
    }


    private RecipeDetailResponse Map(SpoonacularRecipeDetail spoonacularRecipeDetail)
    {
        var result = new RecipeDetailResponse 
        {
            Id = spoonacularRecipeDetail.id.ToString(),
            Name = spoonacularRecipeDetail.title ?? string.Empty,
            Images = [new() { Url = spoonacularRecipeDetail.image ?? string.Empty, Main = true }],
            Ingredients = spoonacularRecipeDetail.extendedIngredients == null ? [] : spoonacularRecipeDetail.extendedIngredients.Select(l => new Ingredient() { Text = l.name, Quantity = (decimal)l.amount!, Measure = l.unit, Image = l.image }),
            MissingIngredients = [], //TODO
            Calories = 0,
            TotalTime = 0,
            CuisinTypes = spoonacularRecipeDetail.cuisines ?? [],
            MealTypes = spoonacularRecipeDetail.mealTypes ?? [],
            Directions = spoonacularRecipeDetail.analyzedInstructions != null && spoonacularRecipeDetail.analyzedInstructions.Any() ? 
                spoonacularRecipeDetail.analyzedInstructions.First().steps.Select(step => new Application.Dtos.Direction() { Step =  step.number.ToString(), InstructionText = step.step }) 
                : [],
            RecipeSourceType = RecipeSourceType.Spoonacular
        };

        return result;
    }
}

public record SpoonacularRecipeDetail(int id,
    string? title,
    string? image,
    IEnumerable<SpoonacularIngredient>? extendedIngredients,
    decimal? readyInMinutes,
    decimal? servings,
    IEnumerable<string>? cuisines,
    IEnumerable<string>? dishTypes,
    IEnumerable<string>? mealTypes,
    IEnumerable<string>? winePairing,
    IEnumerable<AnalyzedInstruction>? analyzedInstructions
);

public record AnalyzedInstruction(string? name, IEnumerable<Step> steps);
//public record Steps(int number, string? step, IEnumerable<SpoonacularIngredient>? ingredients, IEnumerable<Equipment>? equipment, IEnumerable<Length>? length);
public record Step(int number, string? step);
public record Equipment(string? name, string? image);
public record Length(int number, string? unit);

public record SpoonacularIngredient(string? name, string? image, decimal? amount, string? unit);

public record SpoonacularRecipeSearchResponse(SpoonacularRecipeSearchResult[] results);

public record SpoonacularRecipeSearchResult(int id, string? title, string? image);