namespace Recipe.Infrastructure.Services.Edamame;

public class EdamameAPISettings
{
    public required string AppId { get; set; }
    public required string AppKey { get; set; }
    public string Uri { get; set; } = "https://api.edamam.com/";
    public string Beta { get; set; } = "true";
    public string Type { get; set; } = "public";
}
