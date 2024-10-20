using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProjectQuiz.Data;
using Newtonsoft.Json; // Đảm bảo rằng bạn đã thêm namespace này

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        // Cấu hình để bỏ qua vòng lặp tự tham chiếu
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Thêm Authentication Middleware
app.UseAuthentication(); // Middleware này phải đứng trước Authorization
app.UseAuthorization();

// Định tuyến mặc định cho các controller
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Thêm route cụ thể cho MembersController nếu cần
    endpoints.MapControllerRoute(
        name: "members",
        pattern: "Member/{action=Index}/{projectId?}",
        defaults: new { controller = "Members", action = "Index" });
});


app.Run();
