using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Models;

namespace SpareKart_Website.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            if (ModelState.IsValid)
            {
                if (user.Email == "admin@gmail.com" && user.Password == "admin123")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(user);
        }
    }
}