using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Goblin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.Controllers
{
    [NonController]
    public class AdminController : Controller
    {
        private readonly MainContext db;

        public AdminController(MainContext context)
        {
            db = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewBag.Users = db.Users.OrderBy(x => x.ID).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (model.Nick != "equus" && model.Password != "1") // TODO: очен безопасно
                return View();

            // create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, model.Nick)
            };

            // create identity
            var identity = new ClaimsIdentity(claims, "cookie");

            // create principal
            var principal = new ClaimsPrincipal(identity);

            // sign-in
            await HttpContext.SignInAsync(
                scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                principal: principal);

            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Admin");
        }
    }
}