namespace Recipe.Infrastructure.Services.Spoonacular;

public class SpoonacularAPISettings
{
    public required string ApiKey { get; set; }
    public string Uri { get; set; } = "https://api.spoonacular.com";
}
