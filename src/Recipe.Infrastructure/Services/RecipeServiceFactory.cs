using System.ComponentModel;
using System;
using Recipe.Application.Interfaces;
using Recipe.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Recipe.Infrastructure.Services.Edamame;

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
            _ => throw new ArgumentException("unkown source type")
        };
    }

}