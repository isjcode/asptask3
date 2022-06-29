using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P225Allup.DAL;
using P225Allup.Models;
using P225Allup.ViewModels.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public void RemoveItem(int ProductId)
        {
            Console.WriteLine(ProductId);
        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            List<BasketVM> basketVMs = null;

            string coockie = HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(coockie))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

                if (basketVMs.Exists(b=>b.ProductId == id))
                {
                    basketVMs.Find(b => b.ProductId == id).Count++;
                }
                else
                {
                    BasketVM basketVM = new BasketVM
                    {
                        ProductId = (int)id,
                        Image = product.MainImage,
                        Price = product.DiscountPrice > 0 ? product.DiscountPrice : product.Price,
                        Title = product.Title,
                        Count = 1
                    };

                    basketVMs.Add(basketVM);
                }
            }
            else
            {
                basketVMs = new List<BasketVM>();

                BasketVM basketVM = new BasketVM
                {
                    ProductId = (int)id,
                    Image = product.MainImage,
                    Price = product.DiscountPrice > 0 ? product.DiscountPrice : product.Price,
                    Title = product.Title,
                    Count = 1
                };

                basketVMs.Add(basketVM);
            }

            string item = JsonConvert.SerializeObject(basketVMs);

            HttpContext.Response.Cookies.Append("basket", item);

            return RedirectToAction("index", "home");
        }

        public IActionResult GetBasket()
        {
            string coockie = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

            return Json(basketVMs);
        }
    }
}
