using RestaurantAPI_2.Entities;
using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI_2.Models
{
    public class CreateRestaurantDto
    {

        [Required]
        [MaxLength(25)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }

        public string? ContactNumber { get; set; }

        public string? ContactEmail { get; set; }

        public bool HasDelivery { get; set; }
        [Required]
        [MaxLength(50)]
        public string? City { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Street { get; set; }

        public string? PostalCode { get; set; }
    }
}