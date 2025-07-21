using Microsoft.AspNetCore.Identity;

namespace PetShop.Web.Client.Data
{
    public static class DbInitializer
    {
        public static async Task SeedCustomerRole(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string roleName = "Customer";

            // Проверяем, существует ли роль "Customer"
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                // Если нет, создаем ее
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
