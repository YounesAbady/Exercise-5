using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        }
        public async Task<IActionResult> OnPost()
        {
            var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
            var client = httpClient.CreateClient();
            client.BaseAddress = new Uri(config["BaseAddress"]);
            var request = await client.DeleteAsync($"/api/delete-recipe/{Recipe.Id}");
            if (request.IsSuccessStatusCode)
            {
                Msg = "Successfully Deleted!";
                Status = "success";
                return RedirectToPage("ListRecipes");
            }
            return RedirectToPage();
        }
    }
}
