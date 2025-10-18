using Recipe.Application.Dtos;

namespace Recipe.Application.Validators
{
    public static class RecipeRequestExtensions
    {
        public static bool IsValid(this RecipeRequest request, out string? errorMessage)
        {
            if (request == null)
            {
                errorMessage = "Request cannot be null.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(request.Name) ||
                (request.Ingredients?.Any() ?? false) ||
                (request.CuisineTypes?.Any() ?? false) ||
                (request.MealTypes?.Any() ?? false))
            {
                errorMessage = null;
                return true;
            }

            errorMessage = "At least one filter must be specified.";
            return false;
        }
    }
}
