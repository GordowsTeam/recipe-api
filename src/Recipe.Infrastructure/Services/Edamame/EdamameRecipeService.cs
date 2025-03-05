using System.Text.Json;
using Microsoft.Extensions.Options;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;

namespace Recipe.Infrastructure.Services.Edamame;

public class EdamameRecipeService : IThirdPartyRecipeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptionSerialier;
    private readonly EdamameAPISettings _edamameApiSettings;
    private const string endpointUri = "api/recipes/v2";

    public EdamameRecipeService(IHttpClientFactory httpClientFactory, IOptions<EdamameAPISettings> edamameApiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptionSerialier = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        _edamameApiSettings = edamameApiSettings.Value;
    }

    public async Task<IEnumerable<RecipeResponse>?> GetRecipesAsync(RecipeRequest request)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("EdamameAPI");
            httpClient.BaseAddress = new Uri(_edamameApiSettings.Uri);

            var response = await httpClient.GetAsync($"{endpointUri}?" +
                $"type={_edamameApiSettings.Type}" +
                $"&beta={_edamameApiSettings.Beta}" +
                $"&q={string.Join("", request.Ingredients)}" +
                $"&app_id={_edamameApiSettings.AppId}" +
                $"&app_key={_edamameApiSettings.AppKey}");
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
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e.Message}");
            return null;
        }
    }
}

public class EdamameAPISettings
{
    public required string AppId { get; set; }
    public required string AppKey { get; set; }
    public string Uri { get; set; } = "https://api.edamam.com/";
    public string Beta { get; set; } = "true";
    public string Type { get; set; } = "public";
}

public record EdamameRecipeResponse(Source[]? Hits);

public record Source(EdamameRecipe? Recipe);

public record EdamameRecipe(string? Uri, string? Label, string? Url, string[]? IngredientLines, string? Image, EdamameIngredient[]? Ingredients, decimal Calories, decimal TotalTime, string[]? CuisineType, string[]? MealType);

public record EdamameIngredient(string Text, decimal Quantity, string Measure, string Food, decimal Weight, string Image);