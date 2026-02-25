using Microsoft.AspNetCore.Mvc;

namespace SpareKart_Website.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}