using Microsoft.AspNetCore.Mvc;

namespace SpareKart_Website.Controllers
{
    public class FeedbackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(string name, string email, string message)
        {
            TempData["msg"] = "Thank you for your valuable feedback!";
            return RedirectToAction("Index");
        }
    }
}