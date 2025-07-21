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

        // BindProperty св€зывает эту модель с данными из формы POST
        [BindProperty]
        public InputModel Input { get; set; }

        // —писок внешних провайдеров (Google, Facebook и т.д.), если они настроены
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // URL, на который нужно вернуть пользовател€ после успешного входа
        public string ReturnUrl { get; set; }

        // —ообщение об ошибке, которое будет показано пользователю
        [TempData]
        public string ErrorMessage { get; set; }

        // ¬ложенный класс дл€ данных формы
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

        // ћетод дл€ обработки GET-запроса (когда страница просто открываетс€)
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // ќчищаем существующие cookie, чтобы обеспечить чистый процесс входа
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        // ћетод дл€ обработки POST-запроса (когда пользователь отправл€ет форму)
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // ѕопытка войти в систему с использованием парол€.
                // lockoutOnFailure: true - блокирует пользовател€ после нескольких неудачных попыток.
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    // »спользуем LocalRedirect дл€ защиты от атак с открытым перенаправлением.
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // ¬ажно использовать общее сообщение об ошибке, чтобы не раскрывать,
                    // существует ли пользователь с таким email.
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // ≈сли модель невалидна, просто отображаем страницу снова с ошибками валидации.
            return Page();
        }
    }
}
