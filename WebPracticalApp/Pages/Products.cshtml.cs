using EfCoreConsoleApp;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Не забудьте для ToListAsync()

// Импортируйте модели из вашего первого проекта, если они в другом namespace
// using EfCoreConsoleApp.Models; 

public class ProductsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    // Конструктор получает DbContext через Dependency Injection
    public ProductsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Свойство для хранения списка продуктов, который мы передадим на страницу
    public IList<Product> Products { get; set; }

    // Метод OnGetAsync вызывается, когда страница запрашивается методом GET
    public async Task OnGetAsync()
    {
        // Получаем все продукты из базы данных и сохраняем их в наше свойство
        Products = await _context.Products.AsNoTracking().ToListAsync();
    }
}
