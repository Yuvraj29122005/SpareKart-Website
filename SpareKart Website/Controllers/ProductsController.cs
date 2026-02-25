using Microsoft.AspNetCore.Mvc;

namespace SpareKart_Website.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult AllProducts()
        {
            return View();
        }
    }
}