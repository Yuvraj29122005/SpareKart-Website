using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Models;
using System.Collections.Generic;
using System.Linq;

namespace SpareKart_Website.Controllers
{
    public class CartController : Controller
    {
        // In-memory cart shared across requests
        public static List<CartItem> cart = new List<CartItem>();

        // Show the cart page
        public IActionResult Index()
        {
            return View(cart);
        }

        // Add an item to the cart (or increment quantity if already in cart)
        public IActionResult Add(string name, int price, string image)
        {
            var item = cart.FirstOrDefault(x => x.Name == name);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    Name = name,
                    Price = price,
                    Image = image,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }

            return RedirectToAction("Index");
        }

        // Increase quantity of a cart item by 1
        public IActionResult Increase(string name)
        {
            var item = cart.FirstOrDefault(x => x.Name == name);
            if (item != null)
            {
                item.Quantity++;
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