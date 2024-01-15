using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderFoodApplication.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using OrderFoodApplication.ContextDBConfig;

namespace OrderFoodApplication.Controllers
{
    public class RecipeController : Controller

    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TastyBitesDBContext context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RecipeController> _logger;

        public RecipeController(IHttpClientFactory httpClientFactory, ILogger<RecipeController> logger,
             UserManager<ApplicationUser> userManager, TastyBitesDBContext dBcontext)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _userManager = userManager;
            context = dBcontext;
        }

        private readonly string apiURL = "https://forkify-api.herokuapp.com/api/v2/recipes";
        private readonly string apiKey = "64092a23-9828-47cb-ba25-f0427a05e9d7";

        public async Task<IActionResult> Index()
        {
            // Get recipes for each category
            List<Recipe> pizzaRecepie = await GetRecipesAsync("pizza", false, "Pizza");
            List<Recipe> cakeRecepie = await GetRecipesAsync("cake", false, "Cake");
            List<Recipe> chickenRecepie = await GetRecipesAsync("chicken", false, "Chicken");
            List<Recipe> chocolateRecepie = await GetRecipesAsync("chocolate", false, "Chocolate");
            // Add similar lines for other categories

            // Combine recipes for different categories if needed
            List<Recipe> allRecipes = new List<Recipe>();
            allRecipes.AddRange(pizzaRecepie);
            allRecipes.AddRange(cakeRecepie);
            allRecipes.AddRange(chickenRecepie);
            allRecipes.AddRange(chocolateRecepie);
            // Add similar lines for other categories

            return View(allRecipes);
        }

        [HttpPost]
        public IActionResult GetRecipeCard([FromBody] List<Recipe> recipes)
        {
            return PartialView("_RecipeCard", recipes);
        }

        public async Task<IActionResult> Search([FromQuery] string recipe)
        {
            // Assuming you want to search for recipes in all categories
            List<Recipe> recipes = await GetSearchRecipesAsync(recipe, "All Recepies");
            List<Recipe> allRecipes = new List<Recipe>();
            allRecipes.AddRange(recipes);

            return View(allRecipes);
        }

        //Order
        public async Task<IActionResult> Order([FromQuery] string id)
        {
            Console.WriteLine($"GetRecipeDetailsAsync accessed for recipeId:");
            ViewBag.id = id;
            ViewData["Id"] = id;
            OrderRecipeDetails details = await GetRecipeDetailsAsync(id);
            Random random = new Random();
            ViewBag.Price = Math.Round(random.Next(150, 500)/5.0)*5;

            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.UserId = user?.Id;
            ViewBag.Address = user?.Address;
            return View(details);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Order([FromForm]Order order)
        {
            order.OrderDate = DateTime.Now;
            if (ModelState.IsValid)
            {            
                context.Orders.Add(order);
                context.SaveChanges();
                return RedirectToAction("Index", "Recipe");
            }
            return RedirectToAction("Order","Recipe", new {id=order.Id});
        }

        //ShowOrder
        [HttpPost]
        public async Task<IActionResult> GetShowOrder([FromForm] OrderRecipeDetails orderRecipeDetails)
        {
            Console.WriteLine($"GetRecipeDetailsAsync accessed for recipeId:");
            string id = ViewData["Id"] as string;

            if (!string.IsNullOrEmpty(id))
            {
                orderRecipeDetails.Id = id;
                OrderRecipeDetails details = await GetRecipeDetailsAsync(orderRecipeDetails.Id);
                if (details != null)
                {
                    return PartialView("_ShowOrder", details);
                }
                else
                {
                    _logger.LogError("Recipe details are null.");
                    return PartialView("_ShowOrder", null);
                }

            }
            else
            {
                return BadRequest("Recipe ID is missing.");
            }
        }

        //end
        private async Task<OrderRecipeDetails> GetRecipeDetailsAsync(string recipeId)
        {
            Console.WriteLine($"GetRecipeDetailsAsync accessed for recipeId:", recipeId);
            try
            {
                using (HttpClient client = _httpClientFactory.CreateClient())
                {
                    string apiUrl = $"{apiURL}/{recipeId}?key={apiKey}";
                    _logger.LogInformation($"Accessing API URL: {apiUrl}");

                    HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ForkifyApiResponse>(jsonResult);

                        if (result?.data?.recipe != null)
                        {
                            return result.data.recipe;
                        }
                        else
                        {
                            _logger.LogError("Recipe details are null in the API response.");
                            return null;
                        }
                    }
                    else
                    {
                        _logger.LogError($"API request failed with status code {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occurred: {ex.Message}");
                return null;
            }
        }


        //ends here



        private async Task<List<Recipe>> GetSearchRecipesAsync(string recipeCategory, string categoryForModel)
        {
            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                string apiUrl = $"{apiURL}?search={recipeCategory}&key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false); ;

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ForkifyApiResponse>(jsonResult);

                    // Assign the category to each recipe
                    List<Recipe> recipes = result.data.recipes.Select(r => { r.Category = categoryForModel; return r; }).ToList();

                    return recipes;
                }
                else
                {
                    throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
                }
            }
        }

        private async Task<List<Recipe>> GetRecipesAsync(string recipeCategory, bool isAllShow, string categoryForModel)
        {
            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                string apiUrl = $"{apiURL}?search={recipeCategory}&key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false); ;

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ForkifyApiResponse>(jsonResult);

                    // Assign the category to each recipe
                    List<Recipe> recipes = isAllShow
                        ? result.data.recipes.Take(8).Select(r => { r.Category = categoryForModel; return r; }).ToList()
                        : result.data.recipes.Take(8).Select(r => { r.Category = categoryForModel; return r; }).ToList();

                    return recipes;
                }
                else
                {
                    throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
                }
            }
        }
    }
}