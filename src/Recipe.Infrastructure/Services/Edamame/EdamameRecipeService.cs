using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;

namespace Recipe.Infrastructure.Services.Edamame;

public class EdamameRecipeService : IThirdPartyRecipeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptionSerialier;
    private readonly EdamameAPISettings _edamameApiSettings;
    private readonly ILogger<EdamameRecipeService> _logger;
    private const string endpointUri = "api/recipes/v2";

    public EdamameRecipeService(IHttpClientFactory httpClientFactory, IOptions<EdamameAPISettings> edamameApiSettings, ILogger<EdamameRecipeService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptionSerialier = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        _edamameApiSettings = edamameApiSettings.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<RecipeResponse>?> GetRecipesAsync(RecipeRequest request)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("EdamameAPI");
            httpClient.BaseAddress = new Uri(_edamameApiSettings.Uri);
            var requestUri = $"{endpointUri}?" +
                $"type={_edamameApiSettings.Type}" +
                $"&beta={_edamameApiSettings.Beta}" +
                $"&q={string.Join("", request.Ingredients ?? [])}" +
                $"&app_id={_edamameApiSettings.AppId}" +
                $"&app_key={_edamameApiSettings.AppKey}";
            var response = await httpClient.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var edamameRecipeResponse = JsonSerializer.Deserialize<EdamameRecipeResponse>(responseBody, _jsonOptionSerialier);

                return edamameRecipeResponse?.Map();
            }

            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }
        catch (HttpRequestException e)
        {
            _logger.LogError($"Request error in Edamame API Service: {e.Message}");
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError($"There was an exception while trying to consume Edamame API Services. Error: {e.Message}");
            return null;
        }
    }
}

public record EdamameRecipeResponse(Source[]? Hits);

public record Source(EdamameRecipe? Recipe);

public record EdamameRecipe(string? Uri, string? Label, string? Url, string[]? IngredientLines, string? Image, EdamameIngredient[]? Ingredients, decimal Calories, decimal TotalTime, string[]? CuisineType, string[]? MealType);

public record EdamameIngredient(string Text, decimal Quantity, string Measure, string Food, decimal Weight, string Image);