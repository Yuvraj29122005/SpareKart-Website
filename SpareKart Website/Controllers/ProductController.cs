using Microsoft.AspNetCore.Mvc;

namespace SpareKartAdmin.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
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