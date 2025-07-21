using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetShop.Core.Entities;

namespace PetShop.Web.Admin.Pages.Account
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

        // ћетод OnPost будет вызван, когда форма из _LoginPartial отправит запрос
        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                // ѕеренаправл€ем пользовател€ на ту страницу, котора€ указана в форме.
                // ¬ нашем случае это будет главна€ страница.
                return LocalRedirect(returnUrl);
            }
            else
            {
                // ≈сли по какой-то причине returnUrl не указан, просто перенаправл€ем на главную.
                return RedirectToPage("/Index");
            }
        }
    }
}