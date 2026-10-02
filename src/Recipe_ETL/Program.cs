using Recipe.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RecipeApp.Services;
using Recipe.Application.Services;
using System.Diagnostics;

namespace Recipe_ETL
{
    public class Program
    {
        [Conditional("DEBUG")]
        private static void LogStep(string message) => Console.WriteLine(message);

        static async Task Main(string[] args)
        {
            try
            {
                // Build configuration
                LogStep("[Recipe_ETL] Building configuration...");
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()) // Needs Microsoft.Extensions.Configuration.FileExtensions
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                LogStep("[Recipe_ETL] Configuration built.");

                // Setup Dependency Injection
                LogStep("[Recipe_ETL] Setting up dependency injection...");
                var services = new ServiceCollection();
                var serviceProvider = services.AddRecipeAppServices(configuration).BuildServiceProvider();
                LogStep("[Recipe_ETL] Dependency injection configured.");

                if(configuration.GetSection("LoadInitialIngredients").Get<bool>())
                {
                    // Optional: Initial Load
                    LogStep("[Recipe_ETL] LoadInitialIngredients enabled. Starting initial load...");
                    var ingredientSearchPendingService = serviceProvider.GetRequiredService<IIngredientSearchPendingService>();
                    var ingredientService = serviceProvider.GetRequiredService<IIngredientService>();
                    await InitialLoad(ingredientSearchPendingService, ingredientService);
                    LogStep("[Recipe_ETL] Initial load finished.");
                }

                // Run the ETL process
                LogStep("[Recipe_ETL] Starting ETL process...");
                var etlService = serviceProvider.GetRequiredService<IRecipeETLService>();
                await etlService.ProcessRecipesAsync(Recipe.Core.Enums.Language.Spanish);
                LogStep("[Recipe_ETL] ETL process finished.");
            }
            catch(Exception ex)
            {
                LogStep($"[Recipe_ETL] Exception details: {ex}");
                Console.WriteLine($"There was an exception in the main program: {ex.Message}");
            }
        }

        public static async Task InitialLoad(IIngredientSearchPendingService ingredientSearchPendingService, IIngredientService ingredientService)
        {
            //Read from json file or other source
            var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Initial_ingredients.json");
            LogStep($"[Recipe_ETL] Loading initial ingredients from '{path}'...");
            var ingredients = await ingredientService.LoadFromFile(path);
            LogStep($"[Recipe_ETL] Loaded {ingredients.Count()} ingredient(s) from file.");
            foreach(var ingredient in ingredients)
            {
                if (string.IsNullOrEmpty(ingredient.Name))
                {
                    LogStep("[Recipe_ETL] Skipping ingredient with empty name.");
                    continue;
                }
                LogStep($"[Recipe_ETL] Queuing search request for ingredient '{ingredient.Name}'.");
                await ingredientSearchPendingService.AddSearchRequestAsync(new List<string>() { ingredient.Name });

            }
        }
    }
}
