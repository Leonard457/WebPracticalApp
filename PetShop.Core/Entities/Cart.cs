// PetShop.Core/Entities/Cart.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.Core.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        // Внешний ключ для связи с пользователем.
        // Тип ключа (string) должен совпадать с типом первичного ключа в ApplicationUser (Id).
        [Required]
        public string ApplicationUserId { get; set; }

        // Навигационное свойство к пользователю
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        // Навигационное свойство "один-ко-многим".
        // В одной корзине может быть много элементов (CartItem).
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}