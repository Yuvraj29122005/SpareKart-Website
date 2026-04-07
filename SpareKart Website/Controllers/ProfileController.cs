using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpareKart_Website.Data;
using SpareKart_Website.Models;
using System.IO;
using System.Linq;
using System;

namespace SpareKart_Website.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(string name, string phone, IFormFile profilePicture)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = _context.Users.Find(userId);
                if (user != null)
                {
                    user.Name = name;
                    user.Phone = phone;

                    if (profilePicture != null && profilePicture.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + profilePicture.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            profilePicture.CopyTo(stream);
                        }

                        user.ProfilePictureUrl = "images/profiles/" + uniqueFileName;
                    }

                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}