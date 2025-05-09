using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;
using Recipe.Core.Enums;
using Recipe.Infrastructure.Services.Spoonacular;

namespace Recipe.Infrastructure.Services.Spoonacular;
public class SpoonacularRecipeService: IRecipeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptionSerialier;
    private readonly SpoonacularAPISettings _spoonacularApiSettings;
    private readonly ILogger<SpoonacularRecipeService> _logger;
    private const string searchEndpoint = "/recipes/complexSearch";

    public SpoonacularRecipeService(IHttpClientFactory httpClientFactory, IOptions<SpoonacularAPISettings> spoonacularApiSettings, ILogger<SpoonacularRecipeService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptionSerialier = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        _spoonacularApiSettings = spoonacularApiSettings.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<RecipeResponse>?> GetRecipesAsync(RecipeRequest request)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("SpoonacularAPI");
            httpClient.BaseAddress = new Uri(_spoonacularApiSettings.Uri);
            var requestUri = $"{searchEndpoint}?" +
                $"apiKey={_spoonacularApiSettings.ApiKey}" +
                $"&query={request.Ingredients.FirstOrDefault()}";
            
            var response = await httpClient.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var spoonacularRecipeResponse = JsonSerializer.Deserialize<SpoonacularRecipeSearchResponse[]>(responseBody, _jsonOptionSerialier);

                return Map(spoonacularRecipeResponse);
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

    private IEnumerable<RecipeResponse> Map(SpoonacularRecipeSearchResponse[] spoonacularRecipeSearchResponses)
    {
        var result = new List<RecipeResponse>();

        foreach (var spoonacularRecipe in spoonacularRecipeSearchResponses)
        {
            
            result.Add(new RecipeResponse()
            {
                Id = spoonacularRecipe.id,
                Name =  spoonacularRecipe.title ?? string.Empty,
                Images = [new() { Url = spoonacularRecipe.image ?? string.Empty, Main = true }],
                RecipeSourceType = RecipeSourceType.Spoonacular
            });
        }

        return result;

    }

    public Task<RecipeResponse> GetRecipeByIdAsync(string id)
    {
        throw new NotImplementedException();
    }
}

public record SpoonacularRecipeSearchResponse(int id, string? title, string? image);