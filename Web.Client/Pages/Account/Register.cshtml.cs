using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetShop.Core.Data;
using PetShop.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace PetShop.Web.Client.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string ReturnUrl { get; set; }

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, StringLength(100)]
            public string FirstName { get; set; }

            [Required, StringLength(100)]
            public string LastName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    RegistrationDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // Присваиваем роль "Customer"
                    await _userManager.AddToRoleAsync(user, "Customer");

                    // Создаем для пользователя личную корзину
                    var cart = new Cart { ApplicationUserId = user.Id };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();

                    // Автоматически входим в систему после регистрации
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect("/");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
    }
}