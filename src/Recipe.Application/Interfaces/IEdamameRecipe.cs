using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipe.Core.Models;

namespace Recipe.Application.Interfaces
{
    public interface IThirdPartyRecipeService
    {
        Task<IEnumerable<RecipeResponse?>> GetRecipesAsync(RecipeRequest request);
    }
}
