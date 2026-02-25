using Microsoft.AspNetCore.Mvc;

namespace SpareKart_Website.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index(string name, string price, string image)
        {
            ViewBag.Name = name;
            ViewBag.Price = price;
            ViewBag.Image = image;
            return View();
        }

        public IActionResult OrderConfirm(string name, string price, string image)
        {
            ViewBag.Name = name;
            ViewBag.Price = price;
            ViewBag.Image = image;
            return View();
        }
    }
}