// PetShop.Core/Entities/Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.Core.Entities
{
    public class Product
    {
        [Key] // Явно указываем, что это первичный ключ (хотя EF Core догадался бы по имени "Id")
        public int Id { get; set; }

        [Required] // Обязательное поле
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Очень важно для денег использовать decimal, чтобы избежать ошибок округления
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public int StockQuantity { get; set; }

        // Навигационное свойство "один-ко-многим".
        // Один продукт может быть во многих элементах корзины.
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
