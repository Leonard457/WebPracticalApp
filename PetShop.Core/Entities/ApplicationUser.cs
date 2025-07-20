// PetShop.Core/Entities/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PetShop.Core.Entities
{
    // Наследуемся от IdentityUser, чтобы получить все стандартные поля 
    // для аутентификации (Email, PasswordHash, PhoneNumber и т.д.).
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Навигационное свойство для связи "один-к-одному" с корзиной.
        // У каждого пользователя может быть одна корзина.
        public Cart? Cart { get; set; }
    }
}
