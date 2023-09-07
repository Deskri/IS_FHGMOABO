using IS_FHGMOABO.DBConection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Путь к форме входа
});

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration
    .GetConnectionString("ApplicationConnectionString")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Identity.Application";
    options.DefaultSignInScheme = "Identity.External";
})
.AddCookie("Identity.Application", options =>
{
    // Настройки обработчика аутентификации для Identity.Application
})
.AddCookie("Identity.External", options =>
{
    // Настройки обработчика аутентификации для Identity.External
}).AddCookie("Identity.TwoFactorUserId", options =>
{
    // Настройки обработчика аутентификации для Identity.TwoFactorUserId
});

// Добавление сервисов Identity
builder.Services.AddIdentityCore<IdentityUser>(options =>
options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddSignInManager<SignInManager<IdentityUser>>()
    .AddUserManager<UserManager<IdentityUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Добавление аутентификации и авторизации
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
