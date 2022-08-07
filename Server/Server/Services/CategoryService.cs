using Grpc.Core;
using Server.Protos;
using System.Text.Json;

namespace Server.Services
{
    public class CategoryService : category.categoryBase
    {
        private List<Category> _categories = new();
        public CategoryService()
        {
            string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
            string jsonString = File.ReadAllText(fileName);
            _categories = JsonSerializer.Deserialize<List<Category>>(jsonString);
        }
        public override Task<AllCategories> GetAllCategories(GetAllCategoriesRequest request, ServerCallContext context)
        {
            AllCategories allCategories = new();
            foreach (var category in _categories)
            {
                allCategories.Categories.Add(category);
            }
            return Task.FromResult(allCategories);
        }
        public override Task<CategoryResponse> CreateCategory(CreateCategoryRequest request, ServerCallContext context)
        {
            _categories.Add(new Category() { Title = request.Title });
            string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
            string jsonString = JsonSerializer.Serialize(_categories);
            File.WriteAllText(fileName, jsonString);
            return Task.FromResult(new CategoryResponse());
        }
        public override Task<CategoryResponse> DeleteCategory(DeleteCategoryRequest request, ServerCallContext context)
        {
            Category category = new Category() { Title = request.Title };
            _categories.Remove(category);
            //foreach (Recipe recipe in s_recipes)
            //{
            //    if (recipe.Categories.Contains(category))
            //        recipe.Categories.Remove(category);
            //}
            string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
            string jsonString = JsonSerializer.Serialize(_categories);
            File.WriteAllText(fileName, jsonString);
            //fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            //jsonString = JsonSerializer.Serialize(s_recipes);
            //File.WriteAllText(fileName, jsonString);
            return Task.FromResult(new CategoryResponse());
        }
        public override Task<CategoryResponse> EditCategory(EditCategoryRequest request, ServerCallContext context)
        {
            //foreach (Recipe recipe in s_recipes)
            //{
            //    if (recipe.Categories.Contains(s_categoriesNames[int.Parse(position) - 1]))
            //    {
            //        recipe.Categories[recipe.Categories.IndexOf(s_categoriesNames[int.Parse(position) - 1])] = newCategory;
            //    }
            //}
            Category category = new Category() { Title = request.NewTitle };
            _categories[request.Position - 1] = category;
            string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
            string jsonString = JsonSerializer.Serialize(_categories);
            File.WriteAllText(fileName, jsonString);
            //fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            //jsonString = JsonSerializer.Serialize(s_recipes);
            //File.WriteAllText(fileName, jsonString);
            return Task.FromResult(new CategoryResponse());
        }
        public string PathCombine(string path1, string path2)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }
            return Path.Combine(path1, path2);
        }
    }

}
