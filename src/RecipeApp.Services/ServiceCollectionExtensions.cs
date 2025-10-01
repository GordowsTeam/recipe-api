using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Infrastructure.MongoDBRepo;
using Recipe.Infrastructure.Services.Edamame;
using Recipe.Infrastructure.Services.Spoonacular;
using Recipe.Infrastructure.Services;

namespace RecipeApp.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRecipeAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add third-party API clients
            services.AddHttpClient<EdamameRecipeService>("EdamameAPI", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });
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
            services.AddScoped<IIngredientSearchPendingRepository, IngredientSearchPendingRepository>();

            //AI services
            services.AddScoped<IAIEnricher, OpenAIEnricher>();
            //services.AddSingleton(sp => new OpenAIClient(new OpenAIAuthentication("your-api-key")));

            // Application services
            services.AddScoped<MockRecipeRepository>();
            services.AddScoped<InternalRecipeService>();
            services.AddScoped<IRecipeServiceFactory, RecipeServiceFactory>();
            services.AddScoped<IRecipeSearchUseCase, RecipeSearchUseCase>();
            services.AddScoped<IRecipeETLService, RecipeETLService>();
            services.AddScoped<IGetRecipeUseCase, GetRecipeUseCase>();
            services.AddScoped<IDuplicateFinder, DuplicateFinder>();
            services.AddScoped<IIngredientSearchPendingService, IngredientSearchPendingService>();

            return services;//for chaining the services builder.Services.AddRecipeAppServices(mongoConnection, dbName).AddControllers().AddSwaggerGen();
        }
    }
}
