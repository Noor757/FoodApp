namespace OrderFoodApplication.Models
{
    public class OrderRecipeDetails
    {
        public string? Publisher { get; set; }
        public List<Ingredient> Ingredients { get; private set; }
        public string? Source_url { get; set; }
        public string? Image_url { get; set; }
        public string? Title { get; set; }
        public int? Servings { get; set; }
        public string? Cooking_time { get; set; }
        public string? Id { get; set; }

        public OrderRecipeDetails()
        {
            Ingredients = new List<Ingredient>();
        }
    }

    public class Ingredient
    {
        public string? Description { get; set; }
        public double? Quantity { get; set; }
        public string? Unit { get; set; }
    }
}
