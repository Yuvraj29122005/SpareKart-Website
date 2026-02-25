using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SpareKartAdmin.Controllers
{
    public class ProductController : Controller
    {
        static List<Product> products = new List<Product>()
        {
            new Product{Id=1,Name="Engine Oil Filter",Category="Engine",Price=499,Stock=50},
            new Product{Id=2,Name="Brake Disc Set",Category="Brakes",Price=2999,Stock=30}
        };

        public IActionResult Index()
        {
            return View(products);
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product p)
        {
            p.Id = products.Count + 1;
            products.Add(p);
            return RedirectToAction("Index");
        }

        public IActionResult EditProduct(int id)
        {
            var data = products.FirstOrDefault(x => x.Id == id);
            return View(data);
        }

        [HttpPost]
        public IActionResult EditProduct(Product p)
        {
            var data = products.FirstOrDefault(x => x.Id == p.Id);
            data.Name = p.Name;
            data.Category = p.Category;
            data.Price = p.Price;
            data.Stock = p.Stock;
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var data = products.FirstOrDefault(x => x.Id == id);
            products.Remove(data);
            return RedirectToAction("Index");
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
    }
}