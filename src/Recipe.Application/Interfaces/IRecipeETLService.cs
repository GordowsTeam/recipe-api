using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipe.Application.Interfaces
{
    public interface IRecipeETLService
    {
        public Task<bool> ProcessRecipesAsync(IEnumerable<string> ingredients, string language = "en");
    }
}
