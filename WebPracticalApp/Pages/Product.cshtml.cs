using EfCoreConsoleApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class ProductModel : PageModel
{
    private readonly ApplicationDbContext _context;

        // В модели Product измените свойство ImageRoute, чтобы оно всегда возвращало абсолютный путь
    public string ImageRoute => $"/images/{Product.ImageRoute}";

    public ProductModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // �������� ��� �������� ������ � ����� ������
    public Product Product { get; set; }

    // ����� OnGetAsync ������ ��������� 'id' �� ��������
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // ���� ����� � ���� ������ �� ��� ���������� ����� (ID)
        Product = await _context.Products.FindAsync(id);

        // ���� ����� � ����� ID �� ������, ���������� ����������� �������� 404 Not Found
        if (Product == null)
        {
            return NotFound();
        }

        // ���� ��� ������, ���������� ��������
        return Page();
    }
}