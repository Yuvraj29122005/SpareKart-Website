using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Models;
using SpareKart_Website.Data;
using SpareKart_Website.Services;
using System.Linq;
using System;

namespace SpareKart_Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AccountController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            if (ModelState.IsValid)
            {
                // Admin fast-track login
                if (user.Email == "admin@gmail.com" && user.Password == "admin123")
                {
                    var adminUser = _context.Users.FirstOrDefault(u => u.Email == "admin@gmail.com");
                    if (adminUser == null)
                    {
                        adminUser = new User { 
                            Name = "Admin", 
                            Email = "admin@gmail.com", 
                            Phone = "+91 00000 00000",
                            PasswordHash = "admin123", 
                            IsEmailVerified = true 
                        };
                        _context.Users.Add(adminUser);
                        _context.SaveChanges();
                    }
                    HttpContext.Session.SetInt32("UserId", adminUser.Id);
                    HttpContext.Session.SetString("UserName", adminUser.Name);
                    return RedirectToAction("Dashboard", "Admin");
                }
                
                // Regular database user login
                var dbUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.PasswordHash == user.Password);
                
                if (dbUser != null)
                {
                    if (!dbUser.IsEmailVerified)
                    {
                        // Generate a new OTP since they need to verify
                        dbUser.Otp = new Random().Next(100000, 999999).ToString();
                        dbUser.OtpExpiry = DateTime.Now.AddMinutes(10);
                        _context.SaveChanges();
                        
                        _emailService.SendOtpEmail(dbUser.Email, dbUser.Otp, "Verify your SpareKart account");
                        return RedirectToAction("VerifyOtp", new { email = dbUser.Email });
                    }
                    // Success (Authorize Session)
                    HttpContext.Session.SetInt32("UserId", dbUser.Id);
                    HttpContext.Session.SetString("UserName", dbUser.Name);
                    
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                }
            }
            return View(user);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Verify Email distinctiveness
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "An account with this email already exists.");
                    return View(model);
                }

                string otp = new Random().Next(100000, 999999).ToString();

                // Add to database dynamically
                var newUser = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    PasswordHash = model.Password, // For production, remember to Hash this password!
                    IsEmailVerified = false,
                    Otp = otp,
                    OtpExpiry = DateTime.Now.AddMinutes(10)
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();
                
                _emailService.SendOtpEmail(model.Email, otp, "Verify your SpareKart account");

                return RedirectToAction("VerifyOtp", new { email = model.Email });
            }
            return View(model);
        }
        
        [HttpGet]
        public IActionResult VerifyOtp(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOtp(string email, string otp)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            if (user.Otp == otp && user.OtpExpiry > DateTime.Now)
            {
                user.IsEmailVerified = true;
                user.Otp = null; // Clear OTP
                user.OtpExpiry = null;
                _context.SaveChanges();
                
                TempData["VerifySuccess"] = "true";
                return RedirectToAction("Login");
            }
            
            ModelState.AddModelError("", "Invalid or expired OTP");
            ViewBag.Email = email;
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    // Generate OTP
                    user.Otp = new Random().Next(100000, 999999).ToString();
                    user.OtpExpiry = DateTime.Now.AddMinutes(10);
                    _context.SaveChanges();
                    
                    _emailService.SendOtpEmail(user.Email, user.Otp, "Password Reset OTP from SpareKart");
                    
                    return RedirectToAction("ResetPassword", new { email = user.Email });
                }
                else 
                {
                    ModelState.AddModelError("Email", "Email not found in our records.");
                }
            }
            return View(model);
        }
        
        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string email, string otp, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            if (user.Otp == otp && user.OtpExpiry > DateTime.Now)
            {
                user.PasswordHash = newPassword;
                user.Otp = null;
                user.OtpExpiry = null;
                _context.SaveChanges();
                
                TempData["ResetSuccess"] = "true";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Invalid or expired OTP");
            ViewBag.Email = email;
            return View();
        }
    }
}