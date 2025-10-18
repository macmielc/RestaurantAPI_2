using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI_2.Models
{
    public class RegisterUserDto
    {
        //[Required] - atrybuty walidacji usunięte na potrzeby RegisterUserValidato
        public string Email { get; set; }
        //[Required] - atrybuty walidacji usunięte na potrzeby RegisterUserValidato
        //[MinLength(6)] - atrybuty walidacji usunięte na potrzeby RegisterUserValidato
        public string Password { get; set; }
        //[Required] - atrybuty walidacji usunięte na potrzeby RegisterUserValidator
        public string ConfirmPassword { get; set; }

        public string Nationality { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int RoleId { get; set; } = 1;
    }
}