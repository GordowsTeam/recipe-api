using Recipe.Domain.Enums;

namespace Recipe.Application.Dtos;
public class RecipeListResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public IEnumerable<Image>? Images { get; set; }
    public RecipeSourceType RecipeSourceType { get; set; }
}
