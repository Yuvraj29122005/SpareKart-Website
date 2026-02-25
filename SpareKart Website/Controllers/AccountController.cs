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
            var validUser = UserModel.users
                .FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (validUser != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid Email or Password";
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            if (ModelState.IsValid)
            {
                UserModel.users.Add(user);
                return RedirectToAction("Login");
            }
            return View();
        }
    }
}