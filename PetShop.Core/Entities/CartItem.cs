// PetShop.Core/Entities/CartItem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.Core.Entities
{
    // Этот класс связывает Продукт и Корзину, добавляя информацию о количестве.
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int Quantity { get; set; }

        // Внешний ключ для связи с корзиной
        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart Cart { get; set; }

        // Внешний ключ для связи с продуктом
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
