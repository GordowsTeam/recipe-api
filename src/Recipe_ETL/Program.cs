using Recipe.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RecipeApp.Services;
using Recipe.Application.Services;

namespace Recipe_ETL
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try 
            {
                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()) // Needs Microsoft.Extensions.Configuration.FileExtensions
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                // Setup Dependency Injection
                var services = new ServiceCollection();
                var serviceProvider = services.AddRecipeAppServices(configuration).BuildServiceProvider();
                if(configuration.GetSection("LoadInitialIngredients").Get<bool>())
                {
                    // Optional: Initial Load
                    var ingredientSearchPendingService = serviceProvider.GetRequiredService<IIngredientSearchPendingService>();
                    var ingredientService = serviceProvider.GetRequiredService<IIngredientService>();
                    await InitialLoad(ingredientSearchPendingService, ingredientService);
                }

                // Run the ETL process
                var etlService = serviceProvider.GetRequiredService<IRecipeETLService>();
                await etlService.ProcessRecipesAsync("en");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"There was an exception in the main program: {ex.Message}");
            }
        }

        public static async Task InitialLoad(IIngredientSearchPendingService ingredientSearchPendingService, IIngredientService ingredientService) 
        {
            //Read from json file or other source
            var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Initial_ingredients.json");
            var ingredients = await ingredientService.LoadFromFile(path);
            foreach(var ingredient in ingredients)
            {
                if (string.IsNullOrEmpty(ingredient.Name))
                {
                    continue;
                }
                await ingredientSearchPendingService.AddSearchRequestAsync(new List<string>() { ingredient.Name });

            }
        }
    }
}
