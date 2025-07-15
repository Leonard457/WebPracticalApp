using EfCoreConsoleApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // Создаем область видимости для сервисов
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;// Получаем сервисы из области видимости
            var dbContext = services.GetRequiredService<ApplicationDbContext>();// Получаем контекст базы данных

            // --- Теперь можно работать с базой данных ---
            Console.WriteLine("Adding a new Product...");
            List<Product> products = new List<Product>
            {
                // bumaga
                // chair
                // moon
                // mountain
                // pistolet
                // porosenoc
                // shelf
                // sofa
                // table
                new Product
                {
                    Name = "Bumaga",
                    Price = 100,
                    Description = "Bumaga product description",
                    Stock = 10,
                    ImageRoute = "img/bumaga.jpg",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Chair",
                    Price = 150,
                    Description = "Chair product description",
                    Stock = 5,
                    ImageRoute = "img/chair.jpg",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Moon",
                    Price = 200,
                    Description = "Moon product description",
                    Stock = 8,
                    ImageRoute = "img/moon.jpg",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Mountain",
                    Price = 300,
                    Description = "Mountain product description",
                    Stock = 3,
                    ImageRoute = "img/mountain.jpg",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Pistolet",
                    Price = 400,
                    Description = "Pistolet product description",
                    Stock = 2,
                    ImageRoute = "img/pistolet.jpg",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Porosenoc",
                    Price = 50,
                    Description = "Porosenoc product description",
                    Stock = 20,
                    ImageRoute = "img/Porosenoc.jpg",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Shelf",
                    Price = 120,
                    Description = "Shelf product description",
                    Stock = 7,
                    ImageRoute = "img/shelf.jpg",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Sofa",
                    Price = 600,
                    Description = "Sofa product description",
                    Stock = 4,
                    ImageRoute = "img/sofa.jpg",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Name = "Table",
                    Price = 250,
                    Description = "Table product description",
                    Stock = 6,
                    ImageRoute = "img/table.jpg",
                    CreatedAt = DateTime.Now
                }
            };
            foreach (var product in products)
            {
                dbContext.Products.Add(product); // Добавляем продукт в контекст
            }

            //dbContext.Users.Add(newUser);// Добавляем пользователя в контекст
            //dbContext.Products.Add(newProduct);// Добавляем продукт в контекст
            await dbContext.SaveChangesAsync();
            Console.WriteLine("User added successfully!");

            Console.WriteLine("\nReading all users:");
            var users = await dbContext.Users.ToListAsync();
            foreach (var user in users)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Registered: {user.RegistrationDate}");
            }
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // Получаем строку подключения из appsettings.json
                string connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");

                // Регистрируем DbContext в системе внедрения зависимостей
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));
            });
}
