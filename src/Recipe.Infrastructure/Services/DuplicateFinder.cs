using Recipe.Application.Interfaces;

namespace Recipe.Infrastructure.Services
{
    public class DuplicateFinder : IDuplicateFinder
    {
        public DuplicateFinder()
        {
            
        }

        //TODO: implement a real duplicate finder
        public Task<bool> IsDuplicateAsync(Domain.Models.Recipe recipe)
        {
            return Task.FromResult(false);
        }
    }
}
