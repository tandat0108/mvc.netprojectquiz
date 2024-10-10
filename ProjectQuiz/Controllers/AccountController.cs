using Microsoft.AspNetCore.Mvc;
using ProjectQuiz.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectQuiz.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuizzDbContext _context;

        public AccountController(QuizzDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Truyền một đối tượng User rỗng cho view
            return View(new User());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            if (ModelState.IsValid)
            {
                // Tìm kiếm user trong database dựa trên Username và PasswordHash
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.PasswordHash == model.PasswordHash);

                if (user != null)
                {
                    // Đăng nhập thành công, chuyển hướng người dùng đến trang Project
                    return RedirectToAction("Index", "Project");
                }
                else
                {
                    // Nếu không tìm thấy người dùng, hiển thị lỗi
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // Trả lại view với model đã nhập nếu có lỗi
            return View(model);
        }
    }
}
