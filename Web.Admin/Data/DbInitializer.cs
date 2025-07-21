using Microsoft.AspNetCore.Identity;
using PetShop.Core.Entities;

namespace PetShop.Web.Admin.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            string roleName = "Admin";
            string adminEmail = configuration["AdminUser:Email"];

            // 1. Проверяем, существует ли роль "Admin"
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                // Если нет, создаем ее
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // 2. Проверяем, существует ли пользователь с таким email
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                // Если нет, создаем нового пользователя
                var newAdminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = configuration["AdminUser:FirstName"],
                    LastName = configuration["AdminUser:LastName"],
                    EmailConfirmed = true // Сразу подтверждаем почту
                };

                // Создаем пользователя с паролем из конфигурации
                var result = await userManager.CreateAsync(newAdminUser, configuration["AdminUser:Password"]);

                if (result.Succeeded)
                {
                    // Если пользователь успешно создан, присваиваем ему роль "Admin"
                    await userManager.AddToRoleAsync(newAdminUser, roleName);
                }
            }
        }
    }
}
