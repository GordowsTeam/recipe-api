using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Core.Models;
using Recipe.Infrastructure;
using Recipe.Infrastructure.Services;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
        builder.Services.AddScoped<IRecipeService, RecipeService>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapPost("/recipe", async (RecipeRequest request, IRecipeService recipeService) => 
        {
            return await recipeService.GetRecipesAsync(request);
        })
        .WithName("GetRecipe")
        .WithOpenApi();;

        app.Run();
    }
}