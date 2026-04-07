using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Data;
using SpareKart_Website.Models;
using System.Collections.Generic;
using System.Linq;

namespace SpareKart_Website.Controllers
{
    public class CartController : Controller
    {
        // In-memory cart shared across requests
        public static List<CartItem> cart = new List<CartItem>();

        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Show the cart page
        public IActionResult Index()
        {
            // Optionally, we could map max stock here, but handling logic bounds in actions is better.
            return View(cart);
        }

        // Add an item to the cart (or increment quantity if already in cart)
        public IActionResult Add(string name, int price, string image, int qty = 1)
        {
            var product = _context.Products.FirstOrDefault(x => x.Name == name);
            int maxStock = product != null ? product.StockQty : 10;
            string dbImage = product?.ImageUrl;
            if (string.IsNullOrEmpty(dbImage)) dbImage = image;

            var item = cart.FirstOrDefault(x => x.Name == name);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    Name = name,
                    Price = price, // Ensure price is the unit price
                    Image = dbImage,
                    Quantity = (qty > maxStock) ? maxStock : qty,
                    MaxStock = maxStock
                });
            }
            else
            {
                if (item.Quantity + qty > maxStock)
                    item.Quantity = maxStock;
                else
                    item.Quantity += qty;
            }

            return RedirectToAction("Index");
        }

        // Increase quantity of a cart item by 1
        public IActionResult Increase(string name)
        {
            var item = cart.FirstOrDefault(x => x.Name == name);
            if (item != null)
            {
                var product = _context.Products.FirstOrDefault(x => x.Name == item.Name);
                int maxStock = product != null ? product.StockQty : 10;

                if (item.Quantity + 1 <= maxStock)
                {
                    item.Quantity++;
                }
            }

            return RedirectToAction("Index");
        }

        // Decrease quantity — removes item if quantity reaches 0
        public IActionResult Decrease(string name)
        {
            var item = cart.FirstOrDefault(x => x.Name == name);
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
                else
                {
                    cart.Remove(item); // Remove when quantity hits 0
                }
            }

            return RedirectToAction("Index");
        }

        // Remove item from cart entirely
        public IActionResult Remove(string name)
        {
            var item = cart.FirstOrDefault(x => x.Name == name);
            if (item != null)
                cart.Remove(item);

            return RedirectToAction("Index");
        }
    }
}