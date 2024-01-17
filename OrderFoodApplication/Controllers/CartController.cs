using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrderFoodApplication.ContextDBConfig;
using OrderFoodApplication.Models;
using OrderFoodApplication.Repository;

namespace OrderFoodApplication.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IData data;
        private readonly TastyBitesDBContext context;
        private readonly string apiURL = "https://forkify-api.herokuapp.com/api/v2/recipes";
        private readonly string apiKey = "64092a23-9828-47cb-ba25-f0427a05e9d7";
        private readonly IHttpClientFactory _httpClientFactory;

        public CartController(IData data, TastyBitesDBContext context, IHttpClientFactory factory)
        {
            this.data = data;
            this.context = context;
            this._httpClientFactory = factory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> SaveCart(string recipeId)
        {
            Console.WriteLine("I came here");
            var user = await data.GetUser(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Make an API call to fetch recipe details
            var recipeDetails = await GetRecipeDetailsAsync(recipeId);

            if (recipeDetails != null)
            {
                var cart = new Cart
                {
                    UserId = user.Id,
                    RecipeId = recipeId,
                    Image_url = recipeDetails.Image_url,
                    Publisher = recipeDetails.Publisher,
                    Title = recipeDetails.Title
                };

                if (ModelState.IsValid)
                {
                    context.Carts.Add(cart);
                    context.SaveChanges();
                    return NoContent();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<OrderRecipeDetails> GetRecipeDetailsAsync(string recipeId)
        {
            Console.WriteLine($"GetRecipeDetailsAsync accessed for recipeId: {recipeId}");
            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                string apiUrl = $"{apiURL}/{recipeId}?key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ForkifyApiResponse>(jsonResult);

                    return result?.data?.recipe;
                }

                return null;
            }
        }

        //Fetch user Order from Carts Table
        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            var user = await data.GetUser(HttpContext.User);
            if (user == null)
            {
                return Unauthorized();
            }

            var cartItems = context.Carts.Where(c => c.UserId == user.Id).ToList();
            return View(cartItems);
        }

        //Remove an Item from the Cart
        [HttpPost]
        public IActionResult RemoveCartFromList(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var cart = context.Carts.Find(int.Parse(Id));
                if (cart != null)
                {
                    context.Carts.Remove(cart);
                    context.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }



    }
}
