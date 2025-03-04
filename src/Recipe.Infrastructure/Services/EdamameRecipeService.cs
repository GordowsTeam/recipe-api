using System.Text.Json;
using Microsoft.Extensions.Options;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;

namespace Recipe.Infrastructure.Services;

public class EdamameRecipeService: IThirdPartyRecipeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptionSerialier;
    private readonly EdamameAPISettings _edamameApiSettings;

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
            httpClient.BaseAddress = new Uri("https://api.edamam.com/");

            var response = await httpClient.GetAsync($"api/recipes/v2?type=public&beta=true&q={string.Join("", request.Ingredients)}&app_id={_edamameApiSettings.AppId}&app_key={_edamameApiSettings.AppKey}");
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
    }
}

public class EdamameAPISettings 
{
    public string AppId { get; set; }
    public string AppKey { get; set; }
}

public class EdamameRecipeResponse 
{
    public Source[] Hits { get; set; }
}

public class Source 
{
    public EdamameRecipe Recipe { get; set; }
}

public class EdamameRecipe 
{
    public string Uri { get; set; }
    public string Label { get; set; }
    public string Url { get; set; }
    public string[] IngredientLines { get; set; }
    public string Image { get; set; }
    public EdamameIngredient[] Ingredients { get; set; }
    public decimal Calories { get; set; }
    public decimal TotalTime { get; set; }
    public string[] CuisineType { get; set; }
    public string[] MealType { get; set; }
}

public class EdamameIngredient
{
    public string Text { get; set; }
    public decimal Quantity { get; set; }
    public string Measure { get; set; }
    public string Food { get; set; }
    public decimal Weight { get; set; }
    public string Image { get; set; }
}

public static class MapToRecipeResponse 
{
    public static IEnumerable<RecipeResponse> Map(this EdamameRecipeResponse response) 
    {
        var result = new List<RecipeResponse>();
        foreach (var edamameRecipe in response.Hits) 
        {
            result.Add(new RecipeResponse()
            {
                Name = edamameRecipe?.Recipe?.Label,
                Images = new List<Image>() { new() { Url = edamameRecipe.Recipe?.Image, Main = true } },
                Ingredients = edamameRecipe.Recipe.Ingredients?.Select(l => new Ingredient() { Text = l.Text, Quantity = l.Quantity, Measure = l.Measure, Food = l.Food, Weight = l.Weight, Image = l.Image }),
                MissingIngredients = new List<string>() { }, //TODO
                Calories = edamameRecipe.Recipe.Calories,
                TotalTime = edamameRecipe.Recipe.TotalTime,
                CuisinTypes = edamameRecipe.Recipe.CuisineType,
                MealTypes = edamameRecipe.Recipe.MealType,
                Directions = new List<Direction>()//TODO

            });
        }
        return result;
    }
}