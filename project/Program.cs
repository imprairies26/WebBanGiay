using Microsoft.EntityFrameworkCore;
using project.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews(options =>
{
    // Đăng ký Filter tự động kiểm tra quyền đăng nhập toàn cầu
    options.Filters.Add<project.Filters.SessionAuthorizeFilter>();
})
.AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24); // Session tồn tại trong 24h
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<ShoesShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<project.Services.IEmailService, project.Services.MockEmailService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Localization configuration
var supportedCultures = new[] { "vi-VN", "en-US" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
app.UseStatusCodePagesWithReExecute("/Error/{0}");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
