namespace RestaurantAPI_2.Models
{
    public class DishDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; } = 0;
    }
}
