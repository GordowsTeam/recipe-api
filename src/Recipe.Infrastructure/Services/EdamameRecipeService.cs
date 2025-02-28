using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Core.Models;

namespace Recipe.Infrastructure.Services
{
    public class EdamameRecipeService(IHttpClientFactory httpClientFactory) : IThirdPartyRecipeService
    {
        public async Task<IEnumerable<RecipeResponse>?> GetRecipesAsync()
        {
            try 
            {
                var httpClient = httpClientFactory.CreateClient("EdamameAPI");
                httpClient.BaseAddress = new Uri("https://api.edamam.com/");

                var response = await httpClient.GetAsync("api/recipes/v2?type=public&beta=true&q=chicken&app_id=59d21aad&app_key=78c368f91093e7706550971359692a05");
                var s = response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    //TODO: map the edamame response to Internal RecipeResponse
                    return new List<RecipeResponse>();
                }

                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
            catch (HttpRequestException e) 
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }
    }
}
