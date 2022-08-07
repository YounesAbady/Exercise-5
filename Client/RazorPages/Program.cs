using CurrieTechnologies.Razor.SweetAlert2;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Home", "");
});
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddSweetAlert2();
builder.Services.AddScoped<IValidator<RazorPages.Models.Recipe>, RazorPages.Models.Recipe.RecipeValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
