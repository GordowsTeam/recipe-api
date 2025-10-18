using Recipe.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipe.Application.Interfaces
{
    public interface IDuplicateFinder
    {
        Task<bool> IsDuplicateAsync(Domain.Models.Recipe recipe, Language language);
    }
}
