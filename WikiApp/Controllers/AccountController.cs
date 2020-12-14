using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WikiApp.Models;
using WikiApp.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace WikiApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly WikiContext db;
        public AccountController(WikiContext db)
        {
            this.db = db;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            
            if (ModelState.IsValid)
            {
                var user = await db.Пользователиs.FirstOrDefaultAsync(u => u.Логин == model.Login);

                if (user != null)
                {
                    string passwordWithSalt = model.Password + user.Соль;
                    string hashCode = ReturnHashCode(passwordWithSalt);
                    var loggedUser = await db.Пользователиs.Include(u => u.IdРолиNavigation)
                        .FirstOrDefaultAsync(u => u.Логин == model.Login && u.Пароль == hashCode);

                    if (loggedUser != null)
                    {
                        await Authenticate(loggedUser);
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Некорректные логин и пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            
            var user = await db.Пользователиs.FirstOrDefaultAsync(u => u.Логин == model.Login);
            if (user == null)
            {
                Guid salt = Guid.NewGuid();
                string passwordWithSalt = model.Password + salt;
                string hashCode = ReturnHashCode(passwordWithSalt);

                Пользователи newUser = new Пользователи
                {
                    Логин = model.Login,
                    Пароль = hashCode,
                    Соль = salt,
                    ДатаРегистрации = DateTime.Now,
                    IdРоли = db.Рольs.FirstOrDefault(r => r.НаименованиеРоли == "Пользователь").IdРоли
                };

                db.Пользователиs.Add(newUser);
                await db.SaveChangesAsync();

                await Authenticate(newUser);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные логин и/или пароль");
            }
            return View(model);
        }

        private async Task Authenticate(Пользователи user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Логин),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.IdРолиNavigation?.НаименованиеРоли)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public string ReturnHashCode(string passwordAndSalt)
        {
            string hash = "";
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(passwordAndSalt));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                hash = sBuilder.ToString();
            }
            return hash;
        }
    }
}
