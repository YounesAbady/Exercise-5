using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Protos;
using System.Text.Json;
namespace RazorPages.Pages.Recipes
{
    [BindProperties]
    public class ListRecipesModel : PageModel
    {
        IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();
        [TempData]
        public string Msg { get; set; }
        [TempData]
        public string Status { get; set; }
        public List<Models.Recipe> Recipes { get; set; } = new();
        public async Task OnGet()
        {
            //var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            //var client = httpClient.CreateClient();
            //client.BaseAddress = new Uri(config["BaseAddress"]);
            //var request = await client.GetStringAsync("/api/list-recipes");
            //if (request != null)
            //{
            //    var options = new JsonSerializerOptions
            //    {
            //        WriteIndented = true,
            //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            //    };
            //    Recipes = JsonSerializer.Deserialize<List<Models.Recipe>>(request, options);
            //}
            var channel = GrpcChannel.ForAddress("https://localhost:7106");
            var client = new recipe.recipeClient(channel);
            var request = new GetAllRecipesRequest();
            var response = await client.GetAllRecipesAsync(request);
            if (response != null)
            {
                Models.Recipe newRecipe = new();
                foreach (var recipe in response.Recipes)
                {
                    newRecipe = new();
                    newRecipe.Title = recipe.Title;
                    newRecipe.Id = new Guid(recipe.Id);
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        newRecipe.Ingredients.Add(ingredient.Ingredient_);
                    }
                    foreach (var instruction in recipe.Instructions)
                    {
                        newRecipe.Instructions.Add(instruction.Instruction_);
                    }
                    foreach (var category in recipe.Categories)
                    {
                        newRecipe.Categories.Add(category.Title);
                    }
                    Recipes.Add(newRecipe);
                }
            }
        }
    }
}
