using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Extensions;
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
            var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var request = await client.GetStringAsync($"/api/get-recipe/{id}");
            if (request != null)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
                };
                Recipe = JsonSerializer.Deserialize<Models.Recipe>(request, options);
            }

            request = await client.GetStringAsync("/api/list-categories");
            if (request != null)
            {
                Categories = JsonSerializer.Deserialize<List<string>>(request);
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
                var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
                var client = httpClient.CreateClient();
                client.BaseAddress = new Uri(config["BaseAddress"]);
                var jsonRecipe = JsonSerializer.Serialize(recipe);
                var content = new StringContent(jsonRecipe, Encoding.UTF8, "application/json");
                var request = await client.PutAsync($"/api/update-recipe/{jsonRecipe}/{id}", content);
                if (request.IsSuccessStatusCode)
                {
                    Msg = "Successfully Edited!";
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
                return RedirectToPage("Edit", recipe);
            }
            return RedirectToPage();
        }
    }
}
