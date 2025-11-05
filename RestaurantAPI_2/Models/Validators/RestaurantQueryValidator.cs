using FluentValidation;
using RestaurantAPI_2.Entities;

namespace RestaurantAPI_2.Models.Validators
{
    public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
    {
        private int[] allowedPageSized = new int[] { 5, 10, 15 };
        public RestaurantQueryValidator(RestaurantDBContext dBContext)
        {

            string[] allowedColumn = new string[] { nameof(Restaurant.Id), nameof(Restaurant.Name), nameof(Restaurant.Description), nameof(Restaurant.HasDelivery), nameof(Restaurant.CreatedById), nameof(Restaurant.ContactNumber), nameof(Restaurant.ContactEmail), nameof(Restaurant.Category), nameof(Restaurant.AddressID), nameof(Restaurant.Address) };
            // Walidowanie czy liczba stron jest wieksza od 0, 
            RuleFor(x => x.PageNumber).GreaterThan(0);
            // Walidowanie czy liczba elementów na strone jest większa od 0
            //RuleFor(x => x.PageSize).GreaterThan(0);
            // Deklarowane niestandardowej reguły walidacji
            RuleFor(x => x.PageSize)
                .Custom((value, context) =>
                {
                    if (!allowedPageSized.Contains(value))
                    {
                        context.AddFailure("Page size", $"Page size must be one of [{string.Join(", ", allowedPageSized)}]");
                    }
                });

            RuleFor(x => x.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedColumn.Contains(value))
                .WithMessage($"SortBye must be one of [{string.Join(", ", allowedColumn)}]");

        }
    }
}
