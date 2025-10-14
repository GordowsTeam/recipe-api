using Recipe.Core.Enums;
using Recipe.Domain.Models;

namespace Recipe.Core.Models
{
    public class RecipeTranslation
    {
        public Language Language { get; set; }
        public required string Name { get; set; }
        public IEnumerable<string>? CuisinTypes { get; set; }
        public IEnumerable<string>? MealTypes { get; set; }
        public IEnumerable<Direction>? Directions { get; set; }
    }
}
