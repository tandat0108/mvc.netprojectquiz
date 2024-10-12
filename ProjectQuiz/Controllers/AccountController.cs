using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProjectQuiz.ViewModels;
using ProjectQuiz.Data; // Import namespace của User entity
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly QuizzDbContext _context;

    public AccountController(QuizzDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Tìm người dùng dựa trên Username
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);

            if (user != null)
            {
                // So sánh password sau khi hash với PasswordHash từ DB
                if (VerifyPassword(model.Password, user.PasswordHash))
                {
                    // Tạo claims cho người dùng
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Lưu UserId vào claims
                        new Claim(ClaimTypes.Name, user.Username) // Lưu tên người dùng
                    };

                    // Tạo identity từ danh sách claims
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Tạo principal
                    var principal = new ClaimsPrincipal(identity);

                    // Lưu thông tin người dùng vào cookie (đăng nhập)
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Chuyển hướng người dùng tới trang Project sau khi đăng nhập thành công
                    return RedirectToAction("Index", "Projects");
                }
                else
                {
                    // Thêm thông báo lỗi khi mật khẩu không chính xác
                    ModelState.AddModelError("", "Password is incorrect.");
                }
            }
            else
            {
                // Thêm thông báo lỗi khi không tìm thấy username
                ModelState.AddModelError("", "Username does not exist.");
            }
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // Đăng xuất người dùng
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        // Thực hiện hash password và so sánh với passwordHash từ DB
        // Đây chỉ là mô tả, bạn cần tích hợp với hàm hash thực tế.
        return password == passwordHash; // Thay bằng logic hash thực tế
    }
}
