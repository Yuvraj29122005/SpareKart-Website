using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Models;
using System.Collections.Generic;
using System.Linq;

namespace SpareKart_Website.Controllers
{
    public class OrderController : Controller
    {
        // STATIC ORDER LIST
        public static List<OrderModel> orders =
            new List<OrderModel>();

        // MY ORDERS PAGE
        public IActionResult MyOrders()
        {
            return View(orders);
        }

        // ORDER DETAILS PAGE
        public IActionResult Details(string id)
        {
            var order =
                orders.FirstOrDefault(x => x.OrderId == id);

            return View(order);
        }

        // CLEAR HISTORY BUTTON
        public IActionResult ClearHistory()
        {
            orders.Clear();
            return RedirectToAction("MyOrders");
        }
    }
}