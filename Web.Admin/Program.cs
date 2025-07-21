using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetShop.Core.Data;
using PetShop.Core.Entities;
using PetShop.Web.Admin.Data; // ������� ������������ ���� ��� ������ DbInitializer

var builder = WebApplication.CreateBuilder(args);

// --- 1. ��������� �������� ---

// ������ ����������� �� appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ������������ DbContext �� PetShop.Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ������������ ������� ASP.NET Core Identity
// ���������, ��� ����� ������������ ��� ApplicationUser � ����������� ����
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // ��������� ������ (����� ������� ����� ��� ����������)
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>() // ���������, ��� ��������� - ��� DbContext
    .AddDefaultTokenProviders(); // ��� ������ ������ � �.�.

// ��������� Razor Pages � �������
builder.Services.AddRazorPages();

// ������ ��������� ��� ��������������� �� �������� �����
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // ���� ���������������� ������������ �������� �����, ��� ������������ ����
    options.AccessDeniedPath = "/Account/AccessDenied";
});


var app = builder.Build();

// --- 2. ��������� ��������� ��������� HTTP-�������� (Middleware) ---

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

// �������� �������������� � �����������. ������� ���������� �����!
app.UseAuthentication(); // 1. ����������, ��� ������������ (��������� cookie).
app.UseAuthorization();  // 2. ���������, ���� �� � ������������ ������.

// ������������ �������� ��� Razor Pages
app.MapRazorPages();

// --- 3. ������������� ���� ������ � �������� ������ ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // �������� ��� ����� ��� �������� ������
        await DbInitializer.SeedAdminUser(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        string adminEmail = configuration["AdminUser:Email"];
        string newPassword = configuration.GetValue<string>("AdminUser:Password");

        // ������� ������������� ������������-��������������
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser != null && !string.IsNullOrEmpty(newPassword))
        {
            // ������� ������� ������ ������. ��� ����� �������.
            await userManager.RemovePasswordAsync(adminUser);
            // ������ ������� ������ �����
            await userManager.SetEmailAsync(adminUser, null);

            // ����� ��������� ����� ������ �� secrets.json
            var result = await userManager.AddPasswordAsync(adminUser, newPassword);
            // � ������������� ����� �����
            await userManager.SetEmailAsync(adminUser, adminEmail);

            if (result.Succeeded)
            {
                logger.LogWarning("!!! ��������: ������ �������������� ��� ������� �������. !!!");
                logger.LogWarning("!!! ����������, ������� ��������� ��� ��� ������ ������ �� Program.cs !!!");
            }
            else
            {
                // ������� ������, ���� ��� ����
                foreach (var error in result.Errors)
                {
                    logger.LogError($"������ ��� ����� ������: {error.Description}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "��������� ������ �� ����� ���������� ���������� ���� ������ ������.");
    }
}

app.Run();//Account/Login
