using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.ViewModels;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;
using System.Threading.Tasks;

public class AuthController : Controller
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
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
            // Пошук користувача за електронною поштою
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            // Перевірка, чи існує користувач і чи пароль збігається
            if (user != null && user.PasswordHash == model.Password)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            // Повідомлення про помилку, якщо електронна пошта або пароль неправильні
            ViewData["ErrorMessage"] = "Невдала спроба входу.";
        }

        return View(model);
    }

    [HttpGet]
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
            // Перевірка, чи вже існує користувач з такою електронною поштою
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Ця електронна пошта вже зареєстрована.");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = model.Password // Рекомендується замінити на зашифрований пароль
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login", "Auth");
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
