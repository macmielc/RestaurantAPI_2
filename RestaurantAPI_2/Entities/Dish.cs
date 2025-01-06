namespace RestaurantAPI_2.Entities
{
    public class Dish
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; } = 0;

        public int RestaurantId { get; set; }

        public Restaurant Restaurant { get; set; }
    }
}