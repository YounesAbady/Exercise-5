using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Protos;
using System.Text.Json;

namespace RazorPages.Pages.Recipes
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();
        [TempData]
        public string Msg { get; set; }
        [TempData]
        public string Status { get; set; }
        public Models.Recipe Recipe { get; set; }
        public async Task OnGet(Guid id)
        {
            Recipe = new();
            var channel = GrpcChannel.ForAddress("https://localhost:7106");
            var client = new recipe.recipeClient(channel);
            var response = await client.GetRecipeAsync(new GetRecipeRequest() { Id = id.ToString() });
            if (response != null)
            {
                Recipe.Title = response.Title;
                Recipe.Id = new Guid(response.Id);
                foreach (var ingredient in response.Ingredients)
                {
                    Recipe.Ingredients.Add(ingredient.Ingredient_);
                }
                foreach (var instruction in response.Instructions)
                {
                    Recipe.Instructions.Add(instruction.Instruction_);
                }
                foreach (var category in response.Categories)
                {
                    Recipe.Categories.Add(category.Title);
                }
            }
        }
        public async Task<IActionResult> OnPost()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7106");
            var client = new recipe.recipeClient(channel);
            var response = await client.DeleteRecipeAsync(new DeleteRecipeRequest() { Id = Recipe.Id.ToString() });
            if (response.StatusCode == 200)
            {
                Msg = "Successfully Deleted!";
                Status = "success";
                return RedirectToPage("ListRecipes");
            }
            return RedirectToPage();
        }
    }
}
