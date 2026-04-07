using Microsoft.AspNetCore.Mvc;

using SpareKart_Website.Data;
using SpareKart_Website.Models;

namespace SpareKart_Website.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                _context.Feedbacks.Add(feedback);
                _context.SaveChanges();
                TempData["msg"] = "Thank you for your valuable feedback!";
            }
            else
            {
                TempData["msg"] = "Please fill all required fields.";
            }

            return RedirectToAction("Index");
        }
    }
}