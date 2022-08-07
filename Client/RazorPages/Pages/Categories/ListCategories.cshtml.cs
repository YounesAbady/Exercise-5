using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Grpc.Net.Client;
using Server.Protos;

namespace RazorPages.Pages.Categories
{
    [BindProperties]
    public class ListCategoriesModel : PageModel
    {
        IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();
        [TempData]
        public string Msg { get; set; }
        [TempData]
        public string Status { get; set; }
        public List<string> Categories = new();
        public async Task OnGet()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7106");
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
        }
    }
}
