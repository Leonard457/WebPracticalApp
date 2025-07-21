// PetShop.Web.Client/Pages/Account/Login.cshtml.cs

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetShop.Core.Entities;

namespace PetShop.Web.Client.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        // BindProperty ��������� ��� ������ � ������� �� ����� POST
        [BindProperty]
        public InputModel Input { get; set; }

        // ������ ������� ����������� (Google, Facebook � �.�.), ���� ��� ���������
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // URL, �� ������� ����� ������� ������������ ����� ��������� �����
        public string ReturnUrl { get; set; }

        // ��������� �� ������, ������� ����� �������� ������������
        [TempData]
        public string ErrorMessage { get; set; }

        // ��������� ����� ��� ������ �����
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        // ����� ��� ��������� GET-������� (����� �������� ������ �����������)
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // ������� ������������ cookie, ����� ���������� ������ ������� �����
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        // ����� ��� ��������� POST-������� (����� ������������ ���������� �����)
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // ������� ����� � ������� � �������������� ������.
                // lockoutOnFailure: true - ��������� ������������ ����� ���������� ��������� �������.
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    // ���������� LocalRedirect ��� ������ �� ���� � �������� ����������������.
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // ����� ������������ ����� ��������� �� ������, ����� �� ����������,
                    // ���������� �� ������������ � ����� email.
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // ���� ������ ���������, ������ ���������� �������� ����� � �������� ���������.
            return Page();
        }
    }
}
