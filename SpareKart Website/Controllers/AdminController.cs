using Microsoft.AspNetCore.Mvc;

namespace SpareKartAdmin.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}