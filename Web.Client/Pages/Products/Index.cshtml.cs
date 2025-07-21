using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Core.Data;
using PetShop.Core.Entities;

namespace PetShop.Web.Client.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Products { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            var totalProducts = await _context.Products.CountAsync();
            TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

            Products = await _context.Products
                                     .OrderBy(p => p.Name)
                                     .Skip((CurrentPage - 1) * PageSize)
                                     .Take(PageSize)
                                     .ToListAsync();
        }
    }
}
