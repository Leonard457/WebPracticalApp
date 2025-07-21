using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Core.Data; // ���������, ��� ��� ������������ ���� ���������

namespace PetShop.Web.Admin.Pages
{
    // �������� ������� ��������.
    // ������ ������������ � ����� "Admin" ������ �� �������.
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // �������� ��� �������� ����������, ������� �� ������� �� ��������
        public int ProductCount { get; set; }
        public int UserCount { get; set; }

        // ���� ����� ���������� ��� �������� ��������
        public async Task OnGetAsync()
        {
            // ������� ���������� ������� � �������� Products � Users
            ProductCount = await _context.Products.CountAsync();
            UserCount = await _context.Users.CountAsync();
        }
    }
}
