using Grpc.Core;
using Newtonsoft.Json;
using Server.Protos;
using System.Text.Json;

namespace Server.Services
{
    public class CategoryService : category.categoryBase
    {
        private List<Category> _categories { get; set; } = new();
        private List<Recipe> _recipes { get; set; } = new();
        public CategoryService()
        {
            string fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            string jsonString = File.ReadAllText(fileName);
            _recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsonString);
            fileName = CategoryService.PathCombine(Environment.CurrentDirectory, @"\Categories.json");
            jsonString = File.ReadAllText(fileName);
            _categories = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(jsonString);
        }
        public override Task<AllCategories> GetAllCategories(GetAllCategoriesRequest request, ServerCallContext context)
        {
            AllCategories allCategories = new();
            if (_categories.Count == 0)
                throw new InvalidOperationException("Cant be empty");
            else
            {
                foreach (var category in _categories)
                {
                    allCategories.Categories.Add(category);
                }
                return Task.FromResult(allCategories);
            }
        }
        public override Task<CategoryResponse> CreateCategory(CreateCategoryRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Title))
                throw new InvalidOperationException("Cant be empty");
            else
            {
                _categories.Add(new Category() { Title = request.Title });
                Sort();
                string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
                string jsonString = System.Text.Json.JsonSerializer.Serialize(_categories);
                File.WriteAllText(fileName, jsonString);
                return Task.FromResult(new CategoryResponse() { StatusCode = 200 });
            }
        }
        public override Task<CategoryResponse> DeleteCategory(DeleteCategoryRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Title))
                throw new InvalidOperationException("Cant be empty");
            else
            {
                Category category = new Category() { Title = request.Title };
                _categories.Remove(category);
                Sort();
                foreach (Recipe recipe in _recipes)
                {
                    if (recipe.Categories.Contains(category))
                        recipe.Categories.Remove(category);
                }
                string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
                string jsonString = System.Text.Json.JsonSerializer.Serialize(_categories);
                File.WriteAllText(fileName, jsonString);
                fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
                jsonString = System.Text.Json.JsonSerializer.Serialize(_recipes);
                File.WriteAllText(fileName, jsonString);
                return Task.FromResult(new CategoryResponse() { StatusCode = 200 });
            }
        }
        public override Task<CategoryResponse> EditCategory(EditCategoryRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.NewTitle))
                throw new InvalidOperationException("Cant be empty");
            else
            {
                foreach (Recipe recipe in _recipes)
                {
                    if (recipe.Categories.Contains(_categories[request.Position - 1]))
                    {
                        recipe.Categories[recipe.Categories.IndexOf(_categories[(request.Position) - 1])] = new Category() { Title = request.NewTitle };
                    }
                }
                Category category = new Category() { Title = request.NewTitle };
                _categories[request.Position - 1] = category;
                Sort();
                string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
                string jsonString = System.Text.Json.JsonSerializer.Serialize(_categories);
                File.WriteAllText(fileName, jsonString);
                fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
                jsonString = System.Text.Json.JsonSerializer.Serialize(_recipes);
                File.WriteAllText(fileName, jsonString);
                return Task.FromResult(new CategoryResponse() { StatusCode = 200 });
            }
        }
        public static string PathCombine(string path1, string path2)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }
            return Path.Combine(path1, path2);
        }
        public void Sort()
        {
            int x = 0;
            do
            {
                x = 0;
                for (int i = 0; i < _categories.Count - 1; i++)
                {
                    if (char.ToUpper(_categories[i].Title[0]) > char.ToUpper(_categories[i + 1].Title[0]))
                    {
                        x++;
                        string tmp = _categories[i].Title;
                        _categories[i].Title = _categories[i + 1].Title;
                        _categories[i + 1].Title = tmp;
                    }
                }

            } while (x > 0);
        }
    }

}
