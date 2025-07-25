using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetShop.Core.Entities;
using Microsoft.EntityFrameworkCore;

// ��� ������� ������������, ������� ����� ������������ �� �������� �������
// ����� ������ ������������ ��� FirstName ,LastNeme , email , ������
// c ������������ �������������� ���� ������ � ���� �� ������� 
// � ������ ��� ���������� ���������
namespace Web.Client.Pages.Account.Profile
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public ProfileInputModel Input { get; set; } = new();

        [BindProperty]
        public PasswordInputModel PasswordInput { get; set; } = new();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public class ProfileInputModel
        {
            [Required(ErrorMessage = "��� ����������� ��� ����������")]
            [StringLength(100, ErrorMessage = "��� �� ����� ��������� 100 ��������")]
            [Display(Name = "���")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "������� ����������� ��� ����������")]
            [StringLength(100, ErrorMessage = "������� �� ����� ��������� 100 ��������")]
            [Display(Name = "�������")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email ���������� ��� ����������")]
            [EmailAddress(ErrorMessage = "������������ ������ email")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;
        }

        public class PasswordInputModel
        {
            [DataType(DataType.Password)]
            [Display(Name = "������� ������")]
            public string? CurrentPassword { get; set; }

            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "������ ������ ��������� �� ����� {2} ��������", MinimumLength = 6)]
            [Display(Name = "����� ������")]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "����������� ����� ������")]
            [Compare("NewPassword", ErrorMessage = "������ �� ���������")]
            public string? ConfirmNewPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("������� ������� � ������� ���������������������� ������������");
                return NotFound("������������ �� ������");
            }

            await LoadUserDataAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("������������ �� ������");
            }

            if (!ModelState.IsValid)
            {
                await LoadUserDataAsync(user);
                return Page();
            }

            var hasChanges = false;

            // ��������� ��������� �������
            if (user.FirstName != Input.FirstName)
            {
                user.FirstName = Input.FirstName;
                hasChanges = true;
            }

            if (user.LastName != Input.LastName)
            {
                user.LastName = Input.LastName;
                hasChanges = true;
            }

            // ��������� ��������� email
            if (user.Email != Input.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await LoadUserDataAsync(user);
                    return Page();
                }

                var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.Email);
                if (!setUserNameResult.Succeeded)
                {
                    foreach (var error in setUserNameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await LoadUserDataAsync(user);
                    return Page();
                }
                hasChanges = true;
            }

            if (hasChanges)
            {
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await LoadUserDataAsync(user);
                    return Page();
                }

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "������� ������� ��������";
                _logger.LogInformation("������������ {UserId} ������� �������", user.Id);
            }
            else
            {
                StatusMessage = "��������� �� ����������";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("������������ �� ������");
            }

            await LoadUserDataAsync(user);

            if (string.IsNullOrEmpty(PasswordInput.CurrentPassword) || 
                string.IsNullOrEmpty(PasswordInput.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "���������� ��������� ��� ���� ��� ����� ������");
                return Page();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(
                user, 
                PasswordInput.CurrentPassword, 
                PasswordInput.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "������ ������� �������";
            _logger.LogInformation("������������ {UserId} ������� ������", user.Id);

            // ������� ���� ������ ����� �������� �����
            PasswordInput = new PasswordInputModel();

            return RedirectToPage();
        }

        private async Task LoadUserDataAsync(ApplicationUser user)
        {
            Input = new ProfileInputModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };
        }
    }
}
