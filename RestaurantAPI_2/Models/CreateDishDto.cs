using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI_2.Models
{
    public class CreateDishDto
    {
        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; } = 0;

        public int RestaurantId { get; set; }
    }
}