using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EfCoreConsoleApp
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }

    }
    public class User
    {
        public int Id { get; set; }//Идентификатор
        public string? Name { get; set; } //Имя
        public string? Email { get; set; } //Электронная почта
        public string? Password { get; set; } //Пароль

        public DateTime RegistrationDate { get; set; } //Дата регистрации
    }
    public class  Product
    {
        public int Id { get; set; }// Идентификатор
        public string? Name { get; set; }// Имя
        public decimal Price { get; set; }// Цена
        public string? Description { get; set; }// Описание
        public int Stock { get; set; } // Количество на складе
        public string? ImageRoute { get; set; } // Путь к изображению
        public DateTime CreatedAt { get; set; } // Дата создания

    }
}
