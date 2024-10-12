using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProjectQuiz.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm DbContext cho ứng dụng, kết nối với cơ sở dữ liệu
builder.Services.AddDbContext<QuizzDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuizzDB"));
});

// Cấu hình Authentication sử dụng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Đường dẫn tới trang đăng nhập
        options.LogoutPath = "/Account/Logout"; // Đường dẫn để đăng xuất
        options.AccessDeniedPath = "/Account/AccessDenied"; // Trang bị từ chối truy cập (nếu cần)
    });

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

// Thêm Authentication Middleware
app.UseAuthentication(); // Phải có để xử lý xác thực người dùng
app.UseAuthorization();

// Định tuyến mặc định cho các controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
