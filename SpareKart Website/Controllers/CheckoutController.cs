using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Models;
using System;
using System.Linq;

namespace SpareKart_Website.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index(string name, int price, string image)
        {
            ViewBag.Name = name;
            ViewBag.Price = price;
            ViewBag.Image = image;
            return View();
        }

        [HttpPost]
        public IActionResult PlaceOrder(string address)
        {
            if (CartController.cart.Count == 0)
                return RedirectToAction("Index", "Home");

            var newOrder = new OrderModel
            {
                OrderId = "ORD" + (OrderController.orders.Count + 1),
                Date = DateTime.Now.ToShortDateString(),
                TotalAmount = CartController.cart.Sum(x => x.Price * x.Quantity),
                Address = address,
                Items = CartController.cart.ToList()
            };

            OrderController.orders.Add(newOrder);
            CartController.cart.Clear();

            return RedirectToAction("MyOrders", "Order");
        }
    }
}