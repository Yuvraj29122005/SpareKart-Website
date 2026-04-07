using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Data;
using SpareKart_Website.Models;
using SpareKart_Website.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpareKart_Website.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RazorpayService _razorpay;

        public CheckoutController(ApplicationDbContext context, RazorpayService razorpay)
        {
            _context = context;
            _razorpay = razorpay;
        }

        public IActionResult Index(string name, int? price, string image, int? qty)
        {
            List<CartItem> displayCart = new List<CartItem>();
            int checkoutTotal = 0;

            if (!string.IsNullOrEmpty(name) && price.HasValue)
            {
                // Buy Now flow for specific product
                int quantity = qty ?? 1;
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
            ViewBag.RazorpayKeyId = _razorpay.KeyId;

            // Pass to view so we can persist in form if needed
            ViewBag.BuyNowName = name;
            ViewBag.BuyNowPrice = price;
            ViewBag.BuyNowImage = image;
            ViewBag.BuyNowQty = qty;

            return View();
        }

        // ========================
        // COD Order — standard form POST
        // ========================
        [HttpPost]
        public IActionResult PlaceOrder(string address, string fullname, string email, string phone, string pay, string buyNowName, int? buyNowPrice, string buyNowImage, int? buyNowQty)
        {
            List<CartItem> orderItems;
            int total = 0;

            if (!string.IsNullOrEmpty(buyNowName) && buyNowPrice.HasValue)
            {
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
                if (CartController.cart.Count == 0)
                    return RedirectToAction("Index", "Home");

                orderItems = CartController.cart.ToList();
                total = orderItems.Sum(x => x.Price * x.Quantity);
                CartController.cart.Clear();
            }

            var newOrder = new Order
            {
                OrderDate = DateTime.Now,
                TotalAmount = total,
                ShippingAddress = address,
                Status = "Processing",
                PaymentMethod = "COD",
                PaymentStatus = "Pending",
                CustomerName = fullname,
                CustomerEmail = email,
                CustomerPhone = phone
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            foreach (var item in orderItems)
            {
                var product = _context.Products.FirstOrDefault(p => p.Name == item.Name);
                if (product != null)
                {
                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = newOrder.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    });

                    // Deduct stock
                    product.StockQty -= item.Quantity;
                    if(product.StockQty < 0) product.StockQty = 0;
                    _context.Products.Update(product);
                }
            }
            _context.SaveChanges();

            return RedirectToAction("OrderConfirmation", new { id = newOrder.Id });
        }

        // ========================
        // STEP 1: Create Razorpay Order (AJAX)
        // ========================
        [HttpPost]
        public async Task<IActionResult> CreateRazorpayOrder([FromBody] RazorpayOrderRequest request)
        {
            if (request == null || request.Amount <= 0)
                return Json(new { success = false, message = "Invalid amount" });

            var receipt = $"order_{DateTime.Now.Ticks}";
            var razorpayOrderId = await _razorpay.CreateOrderAsync(request.Amount, "INR", receipt);

            if (razorpayOrderId == null)
                return Json(new { success = false, message = "Failed to create Razorpay order" });

            return Json(new
            {
                success = true,
                orderId = razorpayOrderId,
                amount = (int)(request.Amount * 100), // paise
                currency = "INR",
                keyId = _razorpay.KeyId
            });
        }

        // ========================
        // STEP 2: Verify Payment & Place Order (AJAX)
        // ========================
        [HttpPost]
        public IActionResult VerifyPayment([FromBody] RazorpayPaymentVerification verification)
        {
            if (verification == null)
                return Json(new { success = false, message = "Invalid data" });

            // Verify signature
            bool isValid = _razorpay.VerifyPaymentSignature(
                verification.RazorpayOrderId,
                verification.RazorpayPaymentId,
                verification.RazorpaySignature
            );

            if (!isValid)
                return Json(new { success = false, message = "Payment verification failed" });

            // Build the order items
            List<CartItem> orderItems;
            int total = 0;

            if (!string.IsNullOrEmpty(verification.BuyNowName) && verification.BuyNowPrice.HasValue)
            {
                int qty = verification.BuyNowQty ?? 1;
                orderItems = new List<CartItem>
                {
                    new CartItem
                    {
                        Name = verification.BuyNowName,
                        Price = verification.BuyNowPrice.Value / qty,
                        Quantity = qty,
                        Image = verification.BuyNowImage
                    }
                };
                total = verification.BuyNowPrice.Value;
            }
            else
            {
                if (CartController.cart.Count == 0)
                    return Json(new { success = false, message = "Cart is empty" });

                orderItems = CartController.cart.ToList();
                total = orderItems.Sum(x => x.Price * x.Quantity);
                CartController.cart.Clear();
            }

            var newOrder = new Order
            {
                OrderDate = DateTime.Now,
                TotalAmount = total,
                ShippingAddress = verification.Address,
                Status = "Processing",
                PaymentMethod = "Online",
                PaymentStatus = "Completed",
                RazorpayOrderId = verification.RazorpayOrderId,
                RazorpayPaymentId = verification.RazorpayPaymentId,
                RazorpaySignature = verification.RazorpaySignature,
                CustomerName = verification.FullName,
                CustomerEmail = verification.Email,
                CustomerPhone = verification.Phone
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            foreach (var item in orderItems)
            {
                var product = _context.Products.FirstOrDefault(p => p.Name == item.Name);
                if (product != null)
                {
                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = newOrder.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    });

                    // Deduct stock
                    product.StockQty -= item.Quantity;
                    if(product.StockQty < 0) product.StockQty = 0;
                    _context.Products.Update(product);
                }
            }
            _context.SaveChanges();

            return Json(new { success = true, orderId = newOrder.Id });
        }

        // ========================
        // Order Confirmation Page
        // ========================
        public IActionResult OrderConfirmation(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return RedirectToAction("MyOrders", "Order");

            ViewBag.Order = order;
            return View();
        }
    }

    // ========================
    // Request/Response DTOs
    // ========================
    public class RazorpayOrderRequest
    {
        public decimal Amount { get; set; }
    }

    public class RazorpayPaymentVerification
    {
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        
        // Buy Now fields
        public string? BuyNowName { get; set; }
        public int? BuyNowPrice { get; set; }
        public string? BuyNowImage { get; set; }
        public int? BuyNowQty { get; set; }
    }
}