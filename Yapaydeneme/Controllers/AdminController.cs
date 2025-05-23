using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yapaydeneme.Data;
using Yapaydeneme.Models;

namespace Yapaydeneme.Controllers
{
  
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Basit bir admin kontrolü (gerçek uygulamada veritabanından kontrol edilmeli)
            if (username == "admin" && password == "123")
            {
                // Session'a admin bilgisini kaydet
                HttpContext.Session.SetString("AdminLogin", "true");
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            // Admin girişi yapılmış mı kontrol et
            if (HttpContext.Session.GetString("AdminLogin") != "true")
            {
                return RedirectToAction("Login");
            }

            // Kategorileri ViewBag'e ekle
            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();

            // Ürünleri getir
            var urunler = await _context.Urunler
                .Include(u => u.Kategori)
                .ToListAsync();

            return View(urunler);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLogin");
            return RedirectToAction("Login");
        }
    }
} 