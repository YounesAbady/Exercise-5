using Grpc.Core;
using Newtonsoft.Json;
using ProtoBuf;
using Server.Protos;
using System.Text;
using System.Text.Json;
namespace Server.Services
{
    public class RecipeService : recipe.recipeBase
    {
        private List<Category> _categories { get; set; } = new();
        private List<Recipe> _recipes { get; set; } = new();
        public RecipeService()
        {
            string fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            string jsonString = File.ReadAllText(fileName);
            _recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsonString);
            fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Categories.json");
            jsonString = File.ReadAllText(fileName);
            _categories = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(jsonString);
        }
        public override Task<RecipeResponse> CreateRecipe(CreateRecipeRequest request, ServerCallContext context)
        {
            Recipe tmp = new();
            tmp.Ingredients.Add(request.Recipe.Ingredients.Where(r => !string.IsNullOrWhiteSpace(r.Ingredient_)));
            tmp.Instructions.Add(request.Recipe.Instructions.Where(r => !string.IsNullOrWhiteSpace(r.Instruction_)));
            request.Recipe.Ingredients.Clear();
            request.Recipe.Ingredients.Add(tmp.Ingredients);
            request.Recipe.Instructions.Clear();
            request.Recipe.Instructions.Add(tmp.Instructions);
            if (request.Recipe.Ingredients.Count == 0 || request.Recipe.Instructions.Count == 0 || request.Recipe.Categories.Count == 0 || string.IsNullOrWhiteSpace(request.Recipe.Title))
                throw new InvalidOperationException("Cant be empty");
            else
            {
                _recipes.Add(request.Recipe);
                Sort();
                string fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
                string jsonString = System.Text.Json.JsonSerializer.Serialize(_recipes);
                File.WriteAllText(fileName, jsonString);
                return Task.FromResult(new RecipeResponse() { StatusCode = 200 });
            }
        }
        public override Task<AllRecipes> GetAllRecipes(GetAllRecipesRequest request, ServerCallContext context)
        {
            if (_recipes.Count == 0)
                throw new InvalidOperationException("Cant be empty");
            else
            {
                AllRecipes allRecipes = new();
                foreach (var recipe in _recipes)
                {
                    allRecipes.Recipes.Add(recipe);
                }
                return Task.FromResult(allRecipes);
            }
        }
        public override Task<RecipeResponse> DeleteRecipe(DeleteRecipeRequest request, ServerCallContext context)
        {
            if (new Guid(request.Id) == Guid.Empty)
                throw new InvalidOperationException("Cant be empty");
            else
            {
                Recipe recipe = _recipes.FirstOrDefault(x => x.Id == request.Id);
                _recipes.Remove(recipe);
                Sort();
                string fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
                var jsonString = System.Text.Json.JsonSerializer.Serialize(_recipes);
                File.WriteAllText(fileName, jsonString);
                return Task.FromResult(new RecipeResponse() { StatusCode = 200 });
            }
        }
        public override Task<Recipe> GetRecipe(GetRecipeRequest request, ServerCallContext context)
        {
            if (new Guid(request.Id) == Guid.Empty)
                throw new InvalidOperationException("Cant be empty");
            else
            {
                var recipe = _recipes.FirstOrDefault(x => x.Id == request.Id);
                return Task.FromResult(new Recipe(recipe));
            }
        }
        public override Task<RecipeResponse> EditRecipe(EditRecipeRequest request, ServerCallContext context)
        {
            Recipe tmp = new();
            tmp.Ingredients.Add(request.Recipe.Ingredients.Where(r => !string.IsNullOrWhiteSpace(r.Ingredient_)));
            tmp.Instructions.Add(request.Recipe.Instructions.Where(r => !string.IsNullOrWhiteSpace(r.Instruction_)));
            request.Recipe.Ingredients.Clear();
            request.Recipe.Ingredients.Add(tmp.Ingredients);
            request.Recipe.Instructions.Clear();
            request.Recipe.Instructions.Add(tmp.Instructions);
            if (new Guid(request.Id) == Guid.Empty || string.IsNullOrEmpty(request.Recipe.Title) || request.Recipe.Ingredients.Count == 0 || request.Recipe.Instructions.Count == 0 || request.Recipe.Categories.Count == 0)
                throw new InvalidOperationException("Cant be empty");
            else
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
                Sort();
                var fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
                var jsonString = System.Text.Json.JsonSerializer.Serialize(_recipes);
                File.WriteAllText(fileName, jsonString);
                return Task.FromResult(new RecipeResponse() { StatusCode = 200 });
            }
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
