using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using Server.Protos;

namespace RazorPages.Pages.Categories
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
        public static string OldCategory;
        public static int Position;
        public string Category { get; set; }
        public void OnGet(string category, int postion)
        {
            Position = postion;
            OldCategory = category;
            Category = category;
        }
        public async Task<IActionResult> OnPost(string category)
        {
            if (category != null)
            {
                //var httpClient = HttpContext.RequestServices.GetService<IHttpClientFactory>();
                //var client = httpClient.CreateClient();
                //client.BaseAddress = new Uri(config["BaseAddress"]);
                //var jsonCategory = JsonSerializer.Serialize(category);
                //var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
                //var request = await client.PutAsync($"/api/update-category/{Postion}/{category}", content);
                var channel = GrpcChannel.ForAddress("https://localhost:7106");
                var client = new category.categoryClient(channel);
                var response = await client.EditCategoryAsync(new EditCategoryRequest() { Position = Position, NewTitle = Category });
                //if (request.IsSuccessStatusCode)
                //{
                //    Msg = "Successfully Edited!";
                //    Status = "success";
                //    return RedirectToPage("ListCategories");
                //}
                return RedirectToPage("ListCategories");
            }
            else
            {
                Msg = "cant be empty!";
                Status = "error";
                return RedirectToPage("ListCategories");
            }
            return RedirectToPage();
        }
    }
}
