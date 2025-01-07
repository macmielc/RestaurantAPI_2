using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI_2.Entities
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Nazwa miasta
        /// </summary>
        [MaxLength(50)]
        [Required(AllowEmptyStrings = true)]
        public string? City { get; set; }
        /// <summary>
        /// Nazwa ulicy oraz numer budynku
        /// </summary>
        [MaxLength(50)]
        [Required(AllowEmptyStrings = true)]
        public string? Street { get; set; }
        /// <summary>
        /// Kod pocztowy
        /// </summary>
        [MaxLength(100)]
        [Required(AllowEmptyStrings = true)]
        public string? PostalCode { get; set; }

        public Restaurant? Restaurant { get; set; }
    }
}