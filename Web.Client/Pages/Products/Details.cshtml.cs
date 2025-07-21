using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Core.Data;
using PetShop.Core.Entities;

namespace PetShop.Web.Client.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Product Product { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            Product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (Product == null) return NotFound();
            return Page();
        }

        [Authorize(Roles = "Customer")] // Только авторизованный клиент может добавить товар
        public async Task<IActionResult> OnPostAddToCartAsync(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge(); // Если пользователь не найден

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (cart == null)
            {
                // На всякий случай, если корзина не была создана при регистрации
                cart = new Cart { ApplicationUserId = user.Id };
                _context.Carts.Add(cart);
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            if (cartItem == null)
            {
                // Товара еще нет в корзине, добавляем новый
                cartItem = new CartItem { CartId = cart.Id, ProductId = productId, Quantity = 1 };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                // Товар уже в корзине, увеличиваем количество
                cartItem.Quantity++;
            }

            await _context.SaveChangesAsync();
            StatusMessage = "Product successfully added to your cart!";

            return RedirectToPage(new { id = productId });
        }
    }
}