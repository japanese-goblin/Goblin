using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Goblin.Data.Models;
using Goblin.Data.ViewModels;
using Goblin.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.Controllers
{
    public class AdminController : Controller
    {
        private readonly MainContext _db;

        public AdminController(MainContext db)
        {
            _db = db;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(_db.GetUsers().OrderBy(x => x.Id).ToList());
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(model.Nick != "equus" && model.Password != "1") // TODO: очен безопасно
            {
                return View();
            }

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
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          principal);

            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Admin");
        }
    }
}