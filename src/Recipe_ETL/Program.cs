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

                //Setup Dependency Injection
                var services = new ServiceCollection();
                var serviceProvider = services.AddRecipeAppServices(configuration).BuildServiceProvider();

                //var searchPendingService = serviceProvider.GetRequiredService<IIngredientSearchPendingService>();
                //var ingredientSerachPendingList = new List<List<string>>() {
                //    new List<string>() { "chicken", "pepper" },
                //    new List<string>(){"beans"} };
                //foreach (var ingredients in ingredientSerachPendingList)
                //{
                //    await searchPendingService.AddSearchRequestAsync(ingredients);
                //}

                // Run the ETL process
                var etlService = serviceProvider.GetRequiredService<IRecipeETLService>();
                await etlService.ProcessRecipesAsync("en");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"There was an exception in the main program: {ex.Message}");
            }
        }
    }
}
