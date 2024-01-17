using System.ComponentModel.DataAnnotations;

namespace OrderFoodApplication.Models
{
    public class Cart
    {
        [Key]
        // Id, RecipeId, UserId, RecipeName, Address, Price, Quantity, TotalAmount, OrderDate
        // Id, RecipeId, UserId, Image_url, Publisher, Title, Price
        public int Id { get; set; }
        [Required]
        public string? Image_url { get; set; }
        [Required]
        public string? Publisher { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public double Price { get; set; }
        public string? UserId { get; set; }
        public string? RecipeId { get; set; }

    }
}
