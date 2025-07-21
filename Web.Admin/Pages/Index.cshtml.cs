using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Core.Data; // Убедитесь, что это пространство имен добавлено

namespace PetShop.Web.Admin.Pages
{
    // ЗАЩИЩАЕМ ГЛАВНУЮ СТРАНИЦУ.
    // Только пользователи с ролью "Admin" смогут ее увидеть.
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Свойства для хранения статистики, которую мы покажем на странице
        public int ProductCount { get; set; }
        public int UserCount { get; set; }

        // Этот метод выполнится при загрузке страницы
        public async Task OnGetAsync()
        {
            // Считаем количество записей в таблицах Products и Users
            ProductCount = await _context.Products.CountAsync();
            UserCount = await _context.Users.CountAsync();
        }
    }
}
