using Microsoft.AspNetCore.Mvc;

namespace SpareKart_Website.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public IActionResult EditProduct()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }

        public IActionResult OrderDetails()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }
    }
}
