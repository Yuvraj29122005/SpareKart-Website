using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Data;
using SpareKart_Website.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SpareKart_Website.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // MY ORDERS PAGE
        public IActionResult MyOrders()
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();
            
            var viewOrders = orders.Select(o => new OrderModel
            {
                OrderId = o.Id.ToString(),
                Date = o.OrderDate.ToShortDateString(),
                TotalAmount = (int)o.TotalAmount,
                Address = o.ShippingAddress,
                PaymentStatus = o.PaymentStatus,
                Items = o.OrderItems.Select(oi => new CartItem 
                {
                    Name = oi.Product?.Name ?? "Product " + oi.ProductId,
                    Price = (int)oi.UnitPrice,
                    Quantity = oi.Quantity,
                    Image = oi.Product?.ImageUrl ?? ""
                }).ToList()
            }).ToList();

            return View(viewOrders);
        }

        // ORDER DETAILS PAGE
        public IActionResult Details(string id)
        {
            if (!int.TryParse(id, out int parsedId)) return NotFound();

            var o = _context.Orders
                .Include(order => order.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(order => order.Id == parsedId);

            if (o == null) return NotFound();

            var orderModel = new OrderModel
            {
                OrderId = o.Id.ToString(),
                Date = o.OrderDate.ToShortDateString(),
                TotalAmount = (int)o.TotalAmount,
                Address = o.ShippingAddress,
                PaymentStatus = o.PaymentStatus,
                Items = o.OrderItems.Select(oi => new CartItem 
                {
                    Name = oi.Product?.Name ?? "Product " + oi.ProductId,
                    Price = (int)oi.UnitPrice,
                    Quantity = oi.Quantity,
                    Image = oi.Product?.ImageUrl ?? ""
                }).ToList()
            };

            return View(orderModel);
        }

        // CLEAR HISTORY BUTTON
        public IActionResult ClearHistory()
        {
            var allOrders = _context.Orders.ToList();
            _context.Orders.RemoveRange(allOrders);
            _context.SaveChanges();
            
            return RedirectToAction("MyOrders");
        }
    }
}