using EfCoreConsoleApp;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // �� �������� ��� ToListAsync()

// ������������ ������ �� ������ ������� �������, ���� ��� � ������ namespace
// using EfCoreConsoleApp.Models; 

public class ProductsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    // ����������� �������� DbContext ����� Dependency Injection
    public ProductsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // �������� ��� �������� ������ ���������, ������� �� ��������� �� ��������
    public IList<Product> Products { get; set; }

    // ����� OnGetAsync ����������, ����� �������� ������������� ������� GET
    public async Task OnGetAsync()
    {
        // �������� ��� �������� �� ���� ������ � ��������� �� � ���� ��������
        Products = await _context.Products.AsNoTracking().ToListAsync();
    }
}
