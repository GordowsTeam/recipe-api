using System.ComponentModel;
using System;
using Recipe.Application.Interfaces;
using Recipe.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Recipe.Infrastructure.Services.Edamame;
using Recipe.Infrastructure.Services.Spoonacular;

namespace Recipe.Infrastructure.Services;
public class RecipeServiceFactory: IRecipeServiceFactory 
{
    private readonly IServiceProvider _serviceProvider;

    public RecipeServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IRecipeService CreateRecipeService(RecipeSourceType recipeSourceType)
    {
        return recipeSourceType switch {
            RecipeSourceType.Internal => _serviceProvider.GetRequiredService<MockRecipeRepository>(),
            RecipeSourceType.Edamame => _serviceProvider.GetRequiredService<EdamameRecipeService>(),
            RecipeSourceType.Spoonacular => _serviceProvider.GetRequiredService<SpoonacularRecipeService>(),
            _ => throw new ArgumentException("unkown source type")
        };
    }

}