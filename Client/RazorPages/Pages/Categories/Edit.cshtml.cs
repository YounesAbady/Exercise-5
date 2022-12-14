using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using Server.Protos;
using Grpc.Net.Client.Web;

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
                var channel = GrpcChannel.ForAddress(new Uri(config["BaseAddress"]), new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler())
                });
                var client = new category.categoryClient(channel);
                var response = await client.EditCategoryAsync(new EditCategoryRequest() { Position = Position, NewTitle = Category });
                if (response.StatusCode == 200)
                {
                    Msg = "Successfully Edited!";
                    Status = "success";
                    return RedirectToPage("ListCategories");
                }
                return RedirectToPage();
            }
            else
            {
                Msg = "cant be empty!";
                Status = "error";
                return RedirectToPage("ListCategories");
            }
        }
    }
}
