using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderFoodApplication.ContextDBConfig;
using OrderFoodApplication.Controllers;
using OrderFoodApplication.Models;

var builder = WebApplication.CreateBuilder(args);
var dbconnection = builder.Configuration.GetConnectionString("dbConnection");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<RecipeController>();

builder.Services.AddDbContext<TastyBitesDBContext>(options =>
options.UseSqlServer(dbconnection));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TastyBitesDBContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
