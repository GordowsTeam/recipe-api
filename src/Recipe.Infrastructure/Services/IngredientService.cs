using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;
using System.Text.Json;

namespace Recipe.Infrastructure.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        public IngredientService()
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }
        public Task<IEnumerable<InitialIngredient>> LoadFromFile(string filePath)
        {
            if(!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file at path {filePath} was not found.");
            }

            var json = File.ReadAllText(filePath);
            var ingredients = JsonSerializer.Deserialize<IEnumerable<InitialIngredient>>(json, _jsonSerializerOptions);

            return Task.FromResult(ingredients ?? Enumerable.Empty<InitialIngredient>());
        }
    }
}
