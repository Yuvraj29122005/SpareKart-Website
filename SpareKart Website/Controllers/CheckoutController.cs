using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Models;
using System;
using System.Linq;

namespace SpareKart_Website.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index(string name, int? price, string image, int? qty)
        {
            List<CartItem> displayCart = new List<CartItem>();
            int checkoutTotal = 0;

            if (!string.IsNullOrEmpty(name) && price.HasValue)
            {
                // Buy Now flow for specific product
                int quantity = qty ?? 1;
                // Note: The price passed from View might be (unitPrice * qty). Let's just use it as the total for this item,
                // or assume price is total and we just show it.
                displayCart.Add(new CartItem
                {
                    Name = name,
                    Price = price.Value / quantity,
                    Quantity = quantity,
                    Image = image
                });
                checkoutTotal = price.Value;
            }
            else
            {
                // Regular cart checkout
                displayCart = CartController.cart;
                if (displayCart == null || displayCart.Count == 0)
                    return RedirectToAction("Index", "Cart");
                checkoutTotal = displayCart.Sum(x => x.Price * x.Quantity);
            }

            ViewBag.Cart = displayCart;
            ViewBag.Total = checkoutTotal;
            
            // Pass to view so we can persist in form if needed
            ViewBag.BuyNowName = name;
            ViewBag.BuyNowPrice = price;
            ViewBag.BuyNowImage = image;
            ViewBag.BuyNowQty = qty;

            return View();
        }

        [HttpPost]
        public IActionResult PlaceOrder(string address, string fullname, string email, string phone, string pay, string buyNowName, int? buyNowPrice, string buyNowImage, int? buyNowQty)
        {
            List<CartItem> orderItems;
            int total = 0;

            if (!string.IsNullOrEmpty(buyNowName) && buyNowPrice.HasValue)
            {
                // Buy Now static order
                int qty = buyNowQty ?? 1;
                orderItems = new List<CartItem>
                {
                    new CartItem
                    {
                        Name = buyNowName,
                        Price = buyNowPrice.Value / qty,
                        Quantity = qty,
                        Image = buyNowImage
                    }
                };
                total = buyNowPrice.Value;
            }
            else
            {
                // Normal cart order
                if (CartController.cart.Count == 0)
                    return RedirectToAction("Index", "Home");

                orderItems = CartController.cart.ToList();
                total = orderItems.Sum(x => x.Price * x.Quantity);
                CartController.cart.Clear();
            }

            var newOrder = new OrderModel
            {
                OrderId = "ORD" + (OrderController.orders.Count + 1),
                Date = DateTime.Now.ToShortDateString(),
                TotalAmount = total,
                Address = address,
                Items = orderItems
            };

            OrderController.orders.Add(newOrder);

            return RedirectToAction("MyOrders", "Order");
        }
    }
}