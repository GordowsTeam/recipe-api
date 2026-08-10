namespace Recipe.Domain.Enums;

public enum IngredientInstanceStatus
{
    Available = 0,
    Frozen = 1,
    Defrosting = 2,
    Uncertain = 3,
    Spoiled = 4,
    Partial = 5
}
