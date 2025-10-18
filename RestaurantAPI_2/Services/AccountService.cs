using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Exceptions;
using RestaurantAPI_2.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantAPI_2.Services
{
    public interface IAccountService
    {
        string GenerateJwtToken(LoginDto userDto);
        void RegisterUser(RegisterUserDto dto);
    }

    public class AccountService : IAccountService
    {
        private readonly RestaurantDBContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _settings;
        public AccountService(RestaurantDBContext context, IPasswordHasher<User> passwordHasher, AuthenticationSettings setting) 
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _settings = setting;
        }
        /// <summary>
        /// Rejestrowanie nowego użytkownika
        /// </summary>
        /// <param name="dto"></param>
        public void RegisterUser(RegisterUserDto dto)
        {
            // Przekazanie z dto podstawowych informacji o użytkowniku
            var newUser = new User
            {
                Email = dto.Email,
                PasswordHash = dto.Password,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,               
                RoleId = dto.RoleId
            };
            // Hashowanie hasła
            var hashPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.PasswordHash = hashPassword;
            // Dodawanie użytkownika do DB
            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
        /// <summary>
        /// Tworzenie JWToken
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public string GenerateJwtToken(LoginDto userDto)
        {
            var user = _context.Users.Include(u => u.Role).
                FirstOrDefault(u => u.Email == userDto.Email);

            if (user is null)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")),
                new Claim("Nationality", user.Nationality),
            };

            // Nalezy najpierw stworzyc kluczp prywatny
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtKey));
            // Kredenciały do podpisania klucza
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_settings.JwtExpireDays);
            // Tworzenie tokena Jwt
            var token = new JwtSecurityToken(_settings.JwtIssuer, 
                _settings.JwtIssuer, 
                claims, 
                expires, 
                signingCredentials:  cred);
            // Generowanie stringa reprezentującego JwtToken
            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
