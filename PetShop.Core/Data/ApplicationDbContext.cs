// PetShop.Core/Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetShop.Core.Entities; // Не забудьте подключить пространство имен с вашими сущностями

namespace PetShop.Core.Data
{
    // Наследуемся от IdentityDbContext и передаем наш кастомный ApplicationUser.
    // Это автоматически добавит в контекст все таблицы Identity (Users, Roles, Claims и т.д.).
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // DbSet представляет собой коллекцию сущностей в контексте, 
        // которая обычно сопоставляется с таблицей в базе данных.
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        // Этот конструктор необходим, чтобы DI-контейнер в ваших веб-проектах 
        // мог правильно сконфигурировать и передать строку подключения.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Очень важно вызвать базовый метод, чтобы Identity мог настроить свою схему.
            base.OnModelCreating(builder);

            // Здесь можно добавить дополнительную конфигурацию с помощью Fluent API.
            // Например, настроить каскадное удаление или уникальные индексы.

            // Пример: Настройка связи "один-к-одному" между User и Cart.
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<Cart>(c => c.ApplicationUserId);
        }
    }
}