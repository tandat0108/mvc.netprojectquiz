using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProjectQuiz.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Add DbContext for SQL Server with Scoped lifetime and enable sensitive data logging
builder.Services.AddDbContext<QuizzDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuizzDB"));

    // Chỉ bật EnableSensitiveDataLogging trong môi trường phát triển
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

// Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";  // Đường dẫn trang từ chối truy cập
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);   // Hạn cookie
        options.SlidingExpiration = true;                    // Gia hạn cookie nếu người dùng hoạt động
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Thêm route cụ thể cho ProjectMembersController
    endpoints.MapControllerRoute(
        name: "projectmembers",
        pattern: "ProjectMembers/{action=Index}/{projectId?}",
        defaults: new { controller = "ProjectMembers", action = "Index" });
});

app.Run();
