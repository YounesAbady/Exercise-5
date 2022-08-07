using FluentValidation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace RazorPages.Models
{
    public class Recipe
    {
        public Recipe()
        {
            Id = Guid.NewGuid();
        }
        public Recipe(string title, List<string> ingredients, List<string> instructions, List<string> categories)
        {
            this.Title = title;
            Ingredients = ingredients;
            Instructions = instructions;
            Categories = categories;
        }
        [JsonProperty("Id")]
        public Guid Id { get; set; }
        [JsonProperty("Title")]
        [Required]
        public string Title { get; set; } = string.Empty;
        [JsonProperty("Ingredients")]
        [Required]
        public List<string> Ingredients { get; set; } = new List<string>();
        [JsonProperty("Instructions")]
        [Required]
        public List<string> Instructions { get; set; } = new List<string>();
        [JsonProperty("Categories")]
        [Required]
        public List<string> Categories { get; set; } = new List<string>();
        public class RecipeValidator : AbstractValidator<Recipe>
        {
            public RecipeValidator()
            {
                RuleFor(x => x.Id).NotNull();
                RuleFor(x => x.Title).NotNull();
                RuleFor(x => x.Ingredients).NotNull().NotEmpty();
                RuleFor(x => x.Instructions).NotNull().NotEmpty();
                RuleFor(x => x.Categories).NotNull().NotEmpty();

            }
        }
    }
}
