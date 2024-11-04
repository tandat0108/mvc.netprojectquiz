using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features; // Add namespace to configure request size limits
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

    // Only enable EnableSensitiveDataLogging in the development environment
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
        options.AccessDeniedPath = "/Account/AccessDenied";  // Path for access denied page
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);   // Cookie expiration time
        options.SlidingExpiration = true;                    // Extend cookie expiration if the user is active
    });

// Configure Form Options to increase the file upload size
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

// Configure Kestrel server options
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 104857600; // 100 MB
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

    // Add specific route for ProjectMembersController
    endpoints.MapControllerRoute(
        name: "projectmembers",
        pattern: "ProjectMembers/{action=Index}/{projectId?}",
        defaults: new { controller = "ProjectMembers", action = "Index" });
});

app.Run();
