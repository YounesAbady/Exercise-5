using FluentValidation;
using FluentValidation.Results;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Extensions;
using Server.Protos;
using System.Text;
using System.Text.Json;

namespace RazorPages.Pages.Recipes
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        [TempData]
        public string Msg { get; set; }
        [TempData]
        public string Status { get; set; }
        IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();
        private IValidator<Models.Recipe> _validator;
        public List<string> Categories = new List<string>();
        public Models.Recipe Recipe { get; set; }
        public CreateModel(IValidator<Models.Recipe> validator)
        {
            _validator = validator;
        }
        public async Task OnGet(Models.Recipe? recipe)
        {
            var channel = GrpcChannel.ForAddress(new Uri(config["BaseAddress"]), new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            });
            var client = new category.categoryClient(channel);
            var request = new GetAllCategoriesRequest();
            var response = await client.GetAllCategoriesAsync(request);
            if (response != null)
            {
                foreach (var category in response.Categories)
                {
                    Categories.Add(category.Title);
                }
            }
            if (recipe != null)
                Recipe = recipe;
        }
        public async Task<IActionResult> OnPost(Models.Recipe recipe)
        {
            recipe.Instructions.RemoveAll(item => item == null);
            recipe.Ingredients.RemoveAll(item => item == null);
            recipe.Categories.RemoveAll(item => item == null);

            ValidationResult result = await _validator.ValidateAsync(recipe);
            if (result.IsValid)
            {
                recipe.Instructions[0] = recipe.Instructions[0].Replace("/", "-");
                recipe.Ingredients[0] = recipe.Ingredients[0].Replace("/", "-");
                var ing = recipe.Ingredients[0].Split("\r\n");
                recipe.Ingredients = ing.ToList();
                var ins = recipe.Instructions[0].Split("\r\n");
                recipe.Instructions = ins.ToList();
                var channel = GrpcChannel.ForAddress(new Uri(config["BaseAddress"]), new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler())
                });
                var client = new recipe.recipeClient(channel);
                Recipe rec = new();
                rec.Title = recipe.Title;
                rec.Id = Guid.NewGuid().ToString();
                foreach (var ingredient in recipe.Ingredients)
                {
                    rec.Ingredients.Add(new Ingredient() { Ingredient_ = ingredient });
                }
                foreach (var instruction in recipe.Instructions)
                {
                    rec.Instructions.Add(new Instruction() { Instruction_ = instruction });
                }
                foreach (var category in recipe.Categories)
                {
                    rec.Categories.Add(new Category() { Title = category });
                }
                var response = await client.CreateRecipeAsync(new CreateRecipeRequest() { Recipe = rec });
                if (response.StatusCode == 200)
                {
                    Msg = "Successfully Created!";
                    Status = "success";
                    return RedirectToPage("ListRecipes");
                }
                return RedirectToPage();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Msg += ($"{error.ErrorMessage} \n");
                }
                Status = "error";
                return RedirectToPage("Create", recipe);
            }
        }
    }
}
