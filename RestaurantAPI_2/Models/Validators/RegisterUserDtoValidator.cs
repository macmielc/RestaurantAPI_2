using FluentValidation;
using RestaurantAPI_2.Entities;

namespace RestaurantAPI_2.Models.Validators
{
    /// <summary>
    /// Klasa odpowiedzialna za waildację modelu <see cref="RegisterUserDto"/>
    /// </summary>
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {

        public RegisterUserDtoValidator(RestaurantDBContext dBContext)
        {
            // Walidowanie -czy adres mail jest pusty
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            // Walidowanie czy hasło ma odpowiednią długość
            RuleFor(x => x.Password).MinimumLength(6);
            // Walidowanie czy potwierdzenie hasła ma odpowiednią długość oraz jest zgodne z hasłem
            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password).MinimumLength(6);
            // Deklarowane niestandardowej reguły walidacji
            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse =dBContext.Users.Any(u  => u.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }
                });

        }
    }
}
