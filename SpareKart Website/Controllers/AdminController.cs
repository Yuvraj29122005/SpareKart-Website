using Microsoft.AspNetCore.Mvc;

namespace SpareKartAdmin.Controllers
{
    public class AdminController : Controller
    {

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        public IActionResult EditProduct()
        {
            return View();
        }

    }
}