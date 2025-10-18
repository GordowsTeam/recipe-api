using Recipe.Domain.Enums;

namespace Recipe.Application.Dtos
{
    public class RecipeDetailResponse
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<Image>? Images { get; set; }
        public IEnumerable<Ingredient>? Ingredients { get; set; }
        public IEnumerable<string>? MissingIngredients { get; set; }//TODO:check this
        public decimal Calories { get; set; }
        public decimal TotalTime { get; set; }
        public IEnumerable<string>? CuisinTypes { get; set; }
        public IEnumerable<string>? MealTypes { get; set; }
        public IEnumerable<Direction>? Directions { get; set; }
        public RecipeSourceType RecipeSourceType { get; set; }
    }

    public static class RecipeDetailResponseExtension
    {
        public static Domain.Models.Recipe ToRecipe(this RecipeDetailResponse recipeDetailResponse)
        {
            return new Domain.Models.Recipe
            {
                Id = Guid.NewGuid(),
                Translations = new Dictionary<Core.Enums.Language, Core.Models.RecipeTranslation>
                {
                    {
                        Core.Enums.Language.English,
                        new Core.Models.RecipeTranslation
                        {
                            Language = Core.Enums.Language.English,
                            Name = recipeDetailResponse.Name,
                            CuisinTypes = recipeDetailResponse.CuisinTypes,
                            MealTypes = recipeDetailResponse.MealTypes,
                            Directions = recipeDetailResponse.Directions?.Select(d => d.ToDirection()),
                            Ingredients = recipeDetailResponse.Ingredients?.Select(i => i.ToIngredient())
                        }
                    }
                },
                Images = recipeDetailResponse.Images?.Select(i => i.ToImage()) ?? [],
                Calories = recipeDetailResponse.Calories,
                TotalTime = recipeDetailResponse.TotalTime,
                RecipeSourceType = recipeDetailResponse.RecipeSourceType
            };
        }

        public static Domain.Models.Image ToImage(this Image image) 
        {
            return new Domain.Models.Image
            {
                Url = image.Url,
                Main = image.Main
            };
        }

        public static Domain.Models.Direction ToDirection(this Direction direction) 
        {
            return new Domain.Models.Direction
            {
                Step = direction.Step,
                Image = direction.Image,
                InstructionText = direction.InstructionText
            };
        }

        public static Domain.Models.Ingredient ToIngredient(this Ingredient ingredient)
        {
            return new Domain.Models.Ingredient
            {
                Name = ingredient.Text,
                Quantity = ingredient.Quantity,
                Measure = ingredient.Measure,
                Image = ingredient.Image
            };
        }
    }
}
