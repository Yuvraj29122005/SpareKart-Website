using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Models;
using System.Collections.Generic;
using System.Linq;

namespace SpareKart_Website.Controllers
{
    public class CartController : Controller
    {
        public static List<CartItem> cart = new List<CartItem>();

        public IActionResult Index()
        {
            return View(cart);
        }

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

        public IActionResult Remove(string name)
        {
            var item = cart.FirstOrDefault(x => x.Name == name);
            if (item != null)
                cart.Remove(item);

            return RedirectToAction("Index");
        }
    }
}