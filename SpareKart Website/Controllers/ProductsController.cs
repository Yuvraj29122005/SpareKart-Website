using Microsoft.AspNetCore.Mvc;

namespace SpareKart_Website.Controllers
{
    public class ProductsController : Controller
    {

        // ALL PRODUCTS PAGE
        public IActionResult AllProducts()
        {
            return View();
        }


        // 🔥 PRODUCT DETAILS PAGE (THIS IS MISSING IN YOUR PROJECT)
        [HttpGet]
        public IActionResult ProductDetails(string name, int price, string image)
        {
            ViewBag.Name = name;
            ViewBag.Price = price;
            ViewBag.Image = image;
            return View();
        }

    }
}