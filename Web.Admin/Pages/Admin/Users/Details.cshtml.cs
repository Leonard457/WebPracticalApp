using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Core.Data;
using PetShop.Core.Entities;

namespace PetShop.Web.Admin.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser? UserDetail { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) return NotFound();

            // Загружаем пользователя и его корзину, 
            // а также товары в элементах корзины.
            UserDetail = await _context.Users
                .Include(u => u.Cart)
                    .ThenInclude(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (UserDetail == null) return NotFound();

            return Page();
        }
    }
}