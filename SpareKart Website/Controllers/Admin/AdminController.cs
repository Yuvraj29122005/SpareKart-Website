using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpareKart_Website.Data;
using SpareKart_Website.Models;
using System.Linq;

namespace SpareKart_Website.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewBag.TotalProducts = _context.Products.Count();
            ViewBag.TotalOrders = _context.Orders.Count();
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalRevenue = _context.Orders.Sum(o => o.TotalAmount);
            
            var allProducts = _context.Products.ToList();
            var lowStockProducts = allProducts.Where(p => p.StockQty <= 5).ToList();
            
            ViewBag.LowStockCount = lowStockProducts.Count;
            ViewBag.LowStockProducts = lowStockProducts;

            ViewBag.OrdersCompleted = _context.Orders.Count(o => o.Status == "Completed");
            ViewBag.OrdersPending = _context.Orders.Count(o => o.Status == "Pending");
            ViewBag.OrdersFailed = _context.Orders.Count(o => o.Status == "Failed");

            return View();
        }

        public IActionResult Products()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Products");
            }
            return View(product);
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction("Products");
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Products");
        }

        public IActionResult Orders()
        {
            var orders = _context.Orders.ToList();
            return View(orders);
        }

        public IActionResult OrderDetails(int id)
        {
            var o = _context.Orders
                .Include(order => order.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(order => order.Id == id);
                
            if (o == null) return NotFound();
            return View(o);
        }

        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public IActionResult RemoveUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Users");
        }

        public IActionResult Feedback()
        {
            var feedbacks = _context.Feedbacks.OrderByDescending(f => f.DateSubmitted).ToList();
            return View(feedbacks);
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var user = _context.Users.Find(userId);
            if (user == null) return RedirectToAction("Login", "Account");

            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(string name, string phone, string address, IFormFile profilePicture)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = _context.Users.Find(userId);
                if (user != null)
                {
                    user.Name = name;
                    user.Phone = phone;
                    user.Address = address;

                    if (profilePicture != null && profilePicture.Length > 0)
                    {
                        var uploadsFolder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
                        if (!System.IO.Directory.Exists(uploadsFolder))
                            System.IO.Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = System.Guid.NewGuid().ToString() + "_" + profilePicture.FileName;
                        var filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                        {
                            profilePicture.CopyTo(stream);
                        }

                        user.ProfilePictureUrl = "images/profiles/" + uniqueFileName;
                    }

                    _context.SaveChanges();
                    HttpContext.Session.SetString("UserName", user.Name);
                }
            }
            return RedirectToAction("Profile");
        }
    }
}
