using Microsoft.AspNetCore.Mvc;

namespace SpareKart_Website.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult ProductDetails()
        {
            return View();
        }
    }
}