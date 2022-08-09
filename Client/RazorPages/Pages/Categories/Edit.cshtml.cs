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
                var channel = GrpcChannel.ForAddress("https://localhost:7106");
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
