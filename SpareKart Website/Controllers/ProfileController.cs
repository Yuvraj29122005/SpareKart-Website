using Microsoft.AspNetCore.Mvc;

namespace SpareKart_Website.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login", "Account");
        }
    }
}