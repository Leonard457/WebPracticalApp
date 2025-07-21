using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetShop.Core.Data;
using PetShop.Core.Entities;
using PetShop.Web.Client.Data; // ������� ������������ ���� ��� ������ DbInitializer

var builder = WebApplication.CreateBuilder(args);

// --- 1. ��������� �������� ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// ������������ ��� ����� DbContext �� PetShop.Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ������������ ������� ASP.NET Core Identity ��� �������������
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // ��� ���-������� ����� ��������� ������������� �����
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // ��������� ��������� UI ��� Identity

// ��������� Razor Pages � �������
builder.Services.AddRazorPages();

// ��������� cookie ��� ��������������� �� �������� �����
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});


var app = builder.Build();

// --- 2. ��������� ��������� ��������� HTTP-�������� ---

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
app.UseStaticFiles(); // ����� ����� ��� ������ � ���������� �� wwwroot

app.UseRouting();

// �������� �������������� � �����������. ������� ���������� �����!
app.UseAuthentication();
app.UseAuthorization();

// ������������ �������� ��� Razor Pages
app.MapRazorPages();

// ��������� ��������� ��������
app.MapGet("/", context => 
{
    context.Response.Redirect("/Index");
    return Task.CompletedTask;
});

// --- 3. ������������� ���� "Customer" ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // �������� ��� ����� ��� �������� ����
        await DbInitializer.SeedCustomerRole(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the Customer role.");
    }
}


app.Run();

// --- 4. Connection Strings ---