using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P225Allup.DAL;
using P225Allup.Interfaces;
using P225Allup.ViewModels.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<BasketVM> GetBasket()
        {
            string coockie = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

            return basketVMs;
        }

        public async Task<IDictionary<string, string>> GetSettingsAsync()
        {
            IDictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(x => x.Key, x => x.Value);

            return settings;
        }
    }
}
