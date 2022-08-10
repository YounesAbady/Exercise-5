using FluentValidation;
using FluentValidation.Results;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Extensions;
using Server.Protos;
using System.Text;
using System.Text.Json;

namespace RazorPages.Pages.Recipes
{
    [BindProperties]
    public class EditModel : PageModel
    {
        IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();
        [TempData]
        public string Msg { get; set; }
        [TempData]
        public string Status { get; set; }
        private IValidator<Models.Recipe> _validator;
        public List<string> Categories = new List<string>();

        public Models.Recipe Recipe { get; set; }
        public EditModel(IValidator<Models.Recipe> validator)
        {
            _validator = validator;
        }
        public async Task OnGet(Guid id)
        {
            Recipe = new();
            var channel = GrpcChannel.ForAddress(new Uri(config["BaseAddress"]));
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
            var clientForCategories = new category.categoryClient(channel);
            var requestForCategories = new GetAllCategoriesRequest();
            var responseCategories = await clientForCategories.GetAllCategoriesAsync(requestForCategories);
            if (responseCategories != null)
            {
                foreach (var category in responseCategories.Categories)
                {
                    Categories.Add(category.Title);
                }
            }
        }
        public async Task<IActionResult> OnPost(Models.Recipe recipe, Guid id)
        {
            recipe.Instructions.RemoveAll(item => item == null);
            recipe.Ingredients.RemoveAll(item => item == null);
            recipe.Categories.RemoveAll(item => item == null);
            ValidationResult result = await _validator.ValidateAsync(recipe);
            if (result.IsValid)
            {
                var ing = recipe.Ingredients[0].Split("\r\n");
                recipe.Ingredients = ing.ToList();
                var ins = recipe.Instructions[0].Split("\r\n");
                recipe.Instructions = ins.ToList();
                var channel = GrpcChannel.ForAddress(new Uri(config["BaseAddress"]));
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
                var response = await client.EditRecipeAsync(new EditRecipeRequest() { Recipe = rec, Id = id.ToString() });
                if (response.StatusCode == 200)
                {
                    Msg = "Successfully Edited!";
                    Status = "success";
                    return RedirectToPage("ListRecipes");
                }
                RedirectToPage();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Msg += ($"{error.ErrorMessage} \n");
                }
                Status = "error";
                return RedirectToPage("Edit", recipe);
            }
            return RedirectToPage();
        }
    }
}
