using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace RazorPages.Pages
{
    public class HomeModel : PageModel
    {
        public void OnGet()
        {
            var images = new List<string>() { "Cat1.jpg", "Cat2.jpg", "Cat3.jpg", "Food1.jpg", "Food2.jpg", "Food3.jpg" };
            foreach (var img in images)
            {
                Image image = Image.Load(img);
                image.Mutate(x => x
                     .Resize(image.Width / 3, image.Height / 3)
                      );
                image.Save("wwwroot/" + img);
            }
        }
    }
}
