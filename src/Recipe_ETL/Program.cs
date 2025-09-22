using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Recipe.Infrastructure.Services.Edamame;
using MongoDB.Driver;
using Recipe.Infrastructure.Services.Spoonacular;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

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
                AddServices(services, configuration);
                var serviceProvider = services.BuildServiceProvider();

                // Run the ETL process
                var etlService = serviceProvider.GetRequiredService<IRecipeETLService>();
                //TODO: check the args for ingredients and language
                // consider get the ingredients for the search from the web/mobile app input
                var listOfIngredients = new List<string> { "chicken", "rice", "beans" };
                await etlService.ProcessRecipesAsync(listOfIngredients, "en");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"There was an exception in the main program: {ex.Message}");
            }
        }

        static void AddServices(IServiceCollection services, IConfiguration configuration) 
        {
            // Add third-party API clients
            services.AddHttpClient<EdamameRecipeService>("EdamameAPI", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });
            services.AddScoped<MockRecipeRepository>();
            services.AddScoped<EdamameRecipeService>();
            services.AddScoped<SpoonacularRecipeService>();

            // Configuration
            services.Configure<SpoonacularAPISettings>(configuration.GetSection("SpoonacularAPISettings"));
            services.AddSingleton<SpoonacularAPISettings>();

            // MongoDB settings
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            var mongodbSettings = configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>() ?? throw new ApplicationException("MongoDB connection string is not set");
            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongodbSettings.ConnectionString));
            services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongodbSettings.DatabaseName));
            services.AddScoped<IRecipeRepository, MongoRecipeRepository>();

            //AI services
            services.AddScoped<IAIEnricher, OpenAIEnricher>();
            //services.AddSingleton(sp => new OpenAIClient(new OpenAIAuthentication("your-api-key")));

            services.AddScoped<IRecipeServiceFactory, RecipeServiceFactory>();
            services.AddScoped<IRecipeSearchUseCase, RecipeSearchUseCase>();
            services.AddScoped<IRecipeETLService, RecipeETLService>();
            services.AddScoped<IGetRecipeUseCase, GetRecipeUseCase>();
            services.AddScoped<IDuplicateFinder, DuplicateFinder>();
        }
    }
}
