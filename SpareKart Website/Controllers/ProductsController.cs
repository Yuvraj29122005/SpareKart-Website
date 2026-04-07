using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Data;
using System.Linq;

namespace SpareKart_Website.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ALL PRODUCTS PAGE
        public IActionResult AllProducts()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // PRODUCT DETAILS PAGE
        [HttpGet]
        public IActionResult ProductDetails(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            
            ViewBag.Name = product.Name;
            ViewBag.Price = product.Price;
            ViewBag.Image = product.ImageUrl;
            return View(product);
        }
    }
}