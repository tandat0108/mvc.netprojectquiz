using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProjectQuiz.ViewModels;
using ProjectQuiz.Data; // Import namespace for the User entity
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
            // Find user based on Username
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);

            if (user != null)
            {
                // Compare hashed password with PasswordHash from DB
                if (VerifyPassword(model.Password, user.PasswordHash))
                {
                    // Create claims for the user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Store UserId in claims
                        new Claim(ClaimTypes.Name, user.Username), // Store Username
                        new Claim(ClaimTypes.Role, user.Role) // Store user's role (admin/member)
                    };

                    // Create identity from claims list
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Create principal
                    var principal = new ClaimsPrincipal(identity);

                    // Store user information in cookie (login)
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Redirect user to the Project page after successful login
                    return RedirectToAction("Index", "Projects");
                }
                else
                {
                    // Add error message when password is incorrect
                    ModelState.AddModelError("", "Password is incorrect.");
                }
            }
            else
            {
                // Add error message when username is not found
                ModelState.AddModelError("", "Username does not exist.");
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Get the current user ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the user from the database
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user != null)
            {
                // Verify current password
                if (VerifyPassword(model.CurrentPassword, user.PasswordHash))
                {
                    // Hash new password and save it
                    user.PasswordHash = HashPassword(model.NewPassword);
                    await _context.SaveChangesAsync();

                    // Display success message
                    ViewBag.Message = "Password changed successfully.";
                    return View("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect.");
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found.");
            }
        }
        return View(model);
    }

    // HashPassword is a sample function; replace it with an actual hash function
    private string HashPassword(string password)
    {
        // Integrate the actual hash function here
        return password; // Replace with actual hash logic
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // Log out the user
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        // Hash password and compare it with PasswordHash from DB
        // This is just a sample, integrate the actual hash function here.
        return password == passwordHash; // Replace with actual hash logic
    }
}
