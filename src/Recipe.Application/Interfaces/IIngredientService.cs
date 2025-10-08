using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces
{
    public interface IIngredientService
    {
        Task<IEnumerable<InitialIngredient>> LoadFromFile(string filePath);
    }
}
