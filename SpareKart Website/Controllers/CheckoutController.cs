using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Models;
using System;
using System.Linq;

namespace SpareKart_Website.Controllers
{
    public class CheckoutController : Controller
    {
        // GET - show checkout page with full cart
        public IActionResult Index()
        {
            // Pass the entire cart to the view
            var cart = CartController.cart;

            if (cart == null || cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(x => x.Price * x.Quantity);
            return View();
        }

        [HttpPost]
        public IActionResult PlaceOrder(string address, string fullname, string email, string phone, string pay)
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