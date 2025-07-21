using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetShop.Core.Data;
using PetShop.Core.Entities;
using PetShop.Web.Admin.Data; // Добавим пространство имен для нашего DbInitializer

var builder = WebApplication.CreateBuilder(args);

// --- 1. Настройка сервисов ---

// Строка подключения из appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Регистрируем DbContext из PetShop.Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Регистрируем сервисы ASP.NET Core Identity
// Указываем, что будем использовать наш ApplicationUser и стандартные роли
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Настройки пароля (можно сделать проще для разработки)
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>() // Указываем, что хранилище - наш DbContext
    .AddDefaultTokenProviders(); // Для сброса пароля и т.д.

// Добавляем Razor Pages в сервисы
builder.Services.AddRazorPages();

// Важная настройка для перенаправления на страницу входа
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Если неавторизованный пользователь пытается зайти, его перенаправит сюда
    options.AccessDeniedPath = "/Account/AccessDenied";
});


var app = builder.Build();

// --- 2. Настройка конвейера обработки HTTP-запросов (Middleware) ---

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Включаем аутентификацию и авторизацию. Порядок КРИТИЧЕСКИ ВАЖЕН!
app.UseAuthentication(); // 1. Определяет, кто пользователь (проверяет cookie).
app.UseAuthorization();  // 2. Проверяет, есть ли у пользователя доступ.

// Сопоставляем маршруты для Razor Pages
app.MapRazorPages();

// --- 3. Инициализация базы данных и создание админа ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Вызываем наш метод для создания админа
        await DbInitializer.SeedAdminUser(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


app.Run();//Account/Login
