using Grpc.Core;
using Server.Protos;
using System.Text.Json;
namespace Server.Services
{
    public class RecipeService : recipe.recipeBase
    {
        private List<Category> _categories = new();
        private List<Recipe> _recipes = new();
        public RecipeService()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            string jsonString = File.ReadAllText(fileName);
            _recipes = JsonSerializer.Deserialize<List<Recipe>>(jsonString, options);
            fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Categories.json");
            jsonString = File.ReadAllText(fileName);
            _categories = JsonSerializer.Deserialize<List<Category>>(jsonString);
        }
        public override Task<RecipeResponse> CreateRecipe(CreateRecipeRequest request, ServerCallContext context)
        {
            _recipes.Add(request.Recipe);
            Sort();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            string jsonString = JsonSerializer.Serialize(_recipes, options);
            File.WriteAllText(fileName, jsonString);
            return Task.FromResult(new RecipeResponse());
        }
        public override Task<AllRecipes> GetAllRecipes(GetAllRecipesRequest request, ServerCallContext context)
        {
            AllRecipes allRecipes = new();
            foreach (var recipe in _recipes)
            {
                allRecipes.Recipes.Add(recipe);
            }
            return Task.FromResult(allRecipes);
        }
        public override Task<RecipeResponse> DeleteRecipe(DeleteRecipeRequest request, ServerCallContext context)
        {
            Recipe recipe = _recipes.FirstOrDefault(x => x.Id == request.Id);
            _recipes.Remove(recipe);
            string fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            var jsonString = JsonSerializer.Serialize(_recipes);
            File.WriteAllText(fileName, jsonString);
            return Task.FromResult(new RecipeResponse());
        }
        public override Task<Recipe> GetRecipe(GetRecipeRequest request, ServerCallContext context)
        {
            var recipe = _recipes.FirstOrDefault(x => x.Id == request.Id);
            return Task.FromResult(new Recipe(recipe));
        }
        public override Task<RecipeResponse> EditRecipe(EditRecipeRequest request, ServerCallContext context)
        {
            Recipe oldRecipe = _recipes.FirstOrDefault(x => x.Id == request.Id);
            Recipe newRecipe = request.Recipe;
            oldRecipe.Title = newRecipe.Title;
            oldRecipe.Categories.Clear();
            oldRecipe.Categories.Add(newRecipe.Categories);
            oldRecipe.Ingredients.Clear();
            oldRecipe.Ingredients.Add(newRecipe.Ingredients);
            oldRecipe.Instructions.Clear();
            oldRecipe.Instructions.Add(newRecipe.Instructions);
            var fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            var jsonString = JsonSerializer.Serialize(_recipes);
            File.WriteAllText(fileName, jsonString);
            return Task.FromResult(new RecipeResponse());
        }
        public void Sort()
        {
            int x = 0;
            do
            {
                x = 0;
                for (int i = 0; i < _recipes.Count - 1; i++)
                {
                    if (char.ToUpper(_recipes[i].Title[0]) > char.ToUpper(_recipes[i + 1].Title[0]))
                    {
                        x++;
                        Recipe tmp = _recipes[i];
                        _recipes[i] = _recipes[i + 1];
                        _recipes[i + 1] = tmp;
                    }
                }

            } while (x > 0);
        }
    }
}
