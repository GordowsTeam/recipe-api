using Recipe.Core.Enums;
using Recipe.Core.Models;
using Recipe.Domain.Enums;

namespace Recipe.Domain.Models
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public IEnumerable<Image>? Images { get; set; }
        public decimal Calories { get; set; }
        public decimal TotalTime { get; set; }// in minutes
        public RecipeSourceType RecipeSourceType { get; set; }
        public bool AIEnriched { get; set; }
        public Dictionary<Language, RecipeTranslation>? Translations { get; set; } = new();

        /// <summary>When the recipe was first created (UTC).</summary>
        public DateTime? CreatedDateTime { get; set; }

        /// <summary>When the recipe was last updated (UTC).</summary>
        public DateTime? UpdatedDateTime { get; set; }

        /// <summary>Ranking/rating score (e.g. 1-5 or 0-100).</summary>
        public int Ranked { get; set; }
    }
}
