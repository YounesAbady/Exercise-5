using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using CurrieTechnologies.Razor.SweetAlert2;
using FluentValidation;
using Grpc.Net.Client;
using Server.Protos;

namespace RazorPages.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        [TempData]
        public string Msg { get; set; }
        [TempData]
        public string Status { get; set; }
        [Required]
        public string Category;
        public async Task<IActionResult> OnPost(string category)
        {
            if (category != null)
            {
                var channel = GrpcChannel.ForAddress("https://localhost:7106");
                var client = new category.categoryClient(channel);
                var response = await client.CreateCategoryAsync(new CreateCategoryRequest() { Title = category });
                if (response.StatusCode == 200)
                {
                    Msg = "Successfully Created!";
                    Status = "success";
                    return RedirectToPage("ListCategories");
                }
                return RedirectToPage();
            }
            else
            {
                Msg = "cant be empty!";
                Status = "error";
                return RedirectToPage();
            }
        }
    }
}
