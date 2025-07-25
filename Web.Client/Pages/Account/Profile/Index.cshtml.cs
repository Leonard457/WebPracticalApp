using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetShop.Core.Entities;
using Microsoft.EntityFrameworkCore;

// это профиль пользователя, который будет отображаться на странице профиля
// сдесь должно отображаться имя FirstName ,LastNeme , email , Пароль
// c возможностью редактирования этих данных в этом же профиле 
// и кнопка для сохранения изменений
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
            [Required(ErrorMessage = "Имя обязательно для заполнения")]
            [StringLength(100, ErrorMessage = "Имя не может превышать 100 символов")]
            [Display(Name = "Имя")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
            [StringLength(100, ErrorMessage = "Фамилия не может превышать 100 символов")]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email обязателен для заполнения")]
            [EmailAddress(ErrorMessage = "Некорректный формат email")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;
        }

        public class PasswordInputModel
        {
            [DataType(DataType.Password)]
            [Display(Name = "Текущий пароль")]
            public string? CurrentPassword { get; set; }

            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "Пароль должен содержать не менее {2} символов", MinimumLength = 6)]
            [Display(Name = "Новый пароль")]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите новый пароль")]
            [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
            public string? ConfirmNewPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Попытка доступа к профилю неаутентифицированного пользователя");
                return NotFound("Пользователь не найден");
            }

            await LoadUserDataAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            if (!ModelState.IsValid)
            {
                await LoadUserDataAsync(user);
                return Page();
            }

            var hasChanges = false;

            // Проверяем изменения профиля
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

            // Проверяем изменение email
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
                StatusMessage = "Профиль успешно обновлен";
                _logger.LogInformation("Пользователь {UserId} обновил профиль", user.Id);
            }
            else
            {
                StatusMessage = "Изменения не обнаружены";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            await LoadUserDataAsync(user);

            if (string.IsNullOrEmpty(PasswordInput.CurrentPassword) || 
                string.IsNullOrEmpty(PasswordInput.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "Необходимо заполнить все поля для смены пароля");
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
            StatusMessage = "Пароль успешно изменен";
            _logger.LogInformation("Пользователь {UserId} изменил пароль", user.Id);

            // Очищаем поля пароля после успешной смены
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
