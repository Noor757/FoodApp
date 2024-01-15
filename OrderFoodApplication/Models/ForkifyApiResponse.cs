namespace OrderFoodApplication.Models
{
    public class ForkifyApiResponse
    {
        public ForkifyApiData data { get; set; }
        public int ? results { get; set; }
        public string ? status { get; set; }
    }

    public class ForkifyApiData
    {
        public List<Recipe>  recipes { get; set; }
        public OrderRecipeDetails recipe { get; set; }
    }
}
