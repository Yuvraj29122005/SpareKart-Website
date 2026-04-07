using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Data;
using System.Linq;

namespace SpareKart_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Pick up to 6 products for the featured section on the home page
            var featured = _context.Products.Take(6).ToList();
            return View(featured);
        }
    }
}