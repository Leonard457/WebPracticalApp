using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Core.Data;
using PetShop.Core.Entities;

namespace PetShop.Web.Client.Pages.Car_t
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Core.Entities.Cart? UserCart { get; set; }
        public decimal TotalPrice { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            UserCart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (UserCart != null)
            {
                TotalPrice = UserCart.Items.Sum(i => i.Quantity * i.Product.Price);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCheckoutAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userCart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (userCart == null || !userCart.Items.Any())
            {
                ErrorMessage = "Your cart is empty.";
                return RedirectToPage();
            }

            // �������� ����������
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in userCart.Items)
                {
                    var productInDb = await _context.Products.FindAsync(item.ProductId);
                    if (productInDb == null || productInDb.StockQuantity < item.Quantity)
                    {
                        // ���� ������ ��� ��� ��� �� ������� �� ������
                        await transaction.RollbackAsync();
                        ErrorMessage = $"Sorry, we don't have enough stock for {item.Product.Name}. Available: {productInDb?.StockQuantity ?? 0}.";
                        return RedirectToPage();
                    }

                    // ��������� ���������� ������ �� ������
                    productInDb.StockQuantity -= item.Quantity;
                }

                // ������� �������
                _context.CartItems.RemoveRange(userCart.Items);

                // ��������� ��� ��������� � ���� ������
                await _context.SaveChangesAsync();

                // ��������� ����������
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ErrorMessage = "An unexpected error occurred during checkout. Please try again.";
                return RedirectToPage();
            }

            return RedirectToPage("/Checkout/Success");
        }

        public async Task<IActionResult> OnPostUpdateQuantityAsync(int itemId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart.ApplicationUserId == user.Id);
            
            if (cartItem == null)
            {
                return NotFound();
            }
            
            // Проверка наличия на складе
            if (quantity > cartItem.Product.StockQuantity)
            {
                ErrorMessage = $"Невозможно добавить {quantity} шт. В наличии только {cartItem.Product.StockQuantity} шт.";
                return RedirectToPage();
            }
            
            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart.ApplicationUserId == user.Id);
            
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToPage();
        }
    }
}
