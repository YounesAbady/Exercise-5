using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Protos;

namespace RazorPages.Pages.Categories
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
        public string Category { get; set; }
        public void OnGet(string category)
        {
            Category = category;
        }
        public async Task<IActionResult> OnPost()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7106");
            var client = new category.categoryClient(channel);
            var response = await client.DeleteCategoryAsync(new DeleteCategoryRequest() { Title = Category });
            //if (request.IsSuccessStatusCode)
            //{
            //    Msg = "Successfully Deleted!";
            //    Status = "success";
            //    return RedirectToPage("ListCategories");
            //}
            return RedirectToPage();
        }
    }
}
