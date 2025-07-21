// PetShop.Web.Client/Pages/Account/Logout.cshtml.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetShop.Core.Entities;

namespace PetShop.Web.Client.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        // ����� OnPost ����� ������ ������ �� _LoginPartial.cshtml
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // ����� ������������ �� �������
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                // �������������� ������������ �� �� ��������, ������� ������� � �����.
                // � ����� ������ ��� ����� ������� ��������.
                return LocalRedirect(returnUrl);
            }
            else
            {
                // ���� �� �����-�� ������� returnUrl �� ������,
                // ������ �������������� �� ������� �������� ��� ������������.
                return RedirectToPage("/Index");
            }
        }
    }
}
