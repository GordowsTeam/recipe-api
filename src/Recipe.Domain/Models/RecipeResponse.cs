namespace Recipe.Core.Models;

public class RecipeResponse
{
    public string Name { get; set; }
    public IEnumerable<Image> Images { get; set; }
    public IEnumerable<Ingredient> Ingredients { get; set; }
    public IEnumerable<string> MissingIngredients { get; set; }//TODO:check this
    public decimal Calories { get; set; }
    public decimal TotalTime { get; set; }
    public IEnumerable<string> CuisinTypes { get; set; }
    public IEnumerable<string> MealTypes { get; set; }
    public IEnumerable<Direction> Directions { get; set; }
}

public class Direction
{
    public string Step { get; set; }
    public string Image { get; set; }
    public string InstructionText { get; set; }
}

public class Image
{
    public string Url { get; set; }
    public bool Main { get; set; }      
}

public class Ingredient
{
    public string Text { get; set; }//TODO: Change for Name
    public decimal Quantity { get; set; }
    public string Measure { get; set; }
    public decimal Weight { get; set; }
    public string Food { get; set; }
    public FoodCategory FoodCategory { get; set; }
    public string FoodCategoryId { get; set; }
    public string Image { get; set; }
}

public enum FoodCategory
{
    None
}