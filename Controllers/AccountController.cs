using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MOM.Controllers
{
    public class AccountController : Controller
    {
        // In-memory user storage (replace with database in production)
        // Pre-populated with test users
        private static List<User> _users = new List<User>
        {
            new User
            {
                Id = 1,
                FullName = "John Doe",
                Email = "john@example.com",
                PasswordHash = HashPasswordStatic("123456"),
                CreatedDate = DateTime.Now,
                IsActive = true
            },
            new User
            {
                Id = 2,
                FullName = "Jane Smith",
                Email = "jane@example.com",
                PasswordHash = HashPasswordStatic("password123"),
                CreatedDate = DateTime.Now,
                IsActive = true
            },
            new User
            {
                Id = 3,
                FullName = "Admin User",
                Email = "admin@example.com",
                PasswordHash = HashPasswordStatic("admin123"),
                CreatedDate = DateTime.Now,
                IsActive = true
            }
        };

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Hash the entered password
                string hashedPassword = HashPassword(model.Password);

                // Find user by email and password
                var user = _users.FirstOrDefault(u => 
                    u.Email == model.Email && 
                    u.PasswordHash == hashedPassword && 
                    u.IsActive);

                if (user != null)
                {
                    // Create claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, "User")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(1)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password");
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user already exists
                if (_users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered");
                    return View(model);
                }

                // Create new user
                var user = new User
                {
                    Id = _users.Count + 1,
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                _users.Add(user);

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // Hash password using SHA256
        private string HashPassword(string password)
        {
            return HashPasswordStatic(password);
        }

        // Static method for hashing passwords (used for test user initialization)
        private static string HashPasswordStatic(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
