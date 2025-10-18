using AutoMapper;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Models;
using RestaurantAPI_2.Models.Validators;

namespace RestaurantAPI_2
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile() 
        {
            // Jeżeli Restautant i RestautanDto mają takie same nazwy właściwości to nie trzeba ich mapowa gdyż maper same je przepisze
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(m => m.City, c => c.MapFrom(s => s.Address.City))
                .ForMember(m => m.Street, c => c.MapFrom(s => s.Address.Street))
                .ForMember(m => m.PostalCode, c => c.MapFrom(s => s.Address.PostalCode));

            // Jeżeli Dish i Dish mają takie same nazwy właściwości to nie trzeba ich mapowa gdyż maper same je przepisze
            CreateMap<Dish, DishDto>();

            // Jeżeli Dish i Dish mają takie same nazwy właściwości to nie trzeba ich mapowa gdyż maper same je przepisze, pozostaje do przypisania mapowanie
            // właściwości Adresu tab Address
            CreateMap<CreateRestaurantDto, Restaurant>()
                .ForMember(r => r.Address, c => c.MapFrom(dto => new Address()
                {
                    City = dto.City,
                    Street = dto.Street,
                    PostalCode = dto.PostalCode
                }));


            // Jeżeli Dish i Dish mają takie same nazwy właściwości to nie trzeba ich mapować gdyż maper same je przepisze
            CreateMap<CreateDishDto, Dish>();

            //CreateMap<RegisterUserDtoValidator, User>()
            //    .ForMember(u => u.e, d => d.MapFrom(s => s))

        }
    }
}
