using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Extensions;
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
            var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var request = await client.GetStringAsync("/api/list-categories");
            if (request != null)
            {
                Categories = JsonSerializer.Deserialize<List<string>>(request);
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
                var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
                var client = httpClient.CreateClient();
                client.BaseAddress = new Uri(config["BaseAddress"]);
                var jsonRecipe = JsonSerializer.Serialize(recipe);
                var content = new StringContent(jsonRecipe, Encoding.UTF8, "application/json");
                var request = await client.PostAsync($"/api/add-recipe/{jsonRecipe}", content);
                if (request.IsSuccessStatusCode)
                {
                    Msg = "Successfully Created!";
                    Status = "success";
                    return RedirectToPage("ListRecipes");
                }
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
            return RedirectToPage();
        }
    }
}
