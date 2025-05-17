using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yapaydeneme.Models;
using Yapaydeneme.Data;
using System.Security.Claims;

namespace Yapaydeneme.Controllers
{
    public class SepetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SepetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sepet içeriğini görüntüleme
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var sepetItems = await _context.Sepet
                .Include(s => s.urun)
                .Where(s => s.KullaniciId == userId)
                .ToListAsync();

            decimal toplamTutar = sepetItems.Sum(item => item.urun.Fiyat * item.Adet);
            ViewBag.ToplamTutar = toplamTutar;

            return View(sepetItems);
        }

        // Sepete ürün ekleme
        [HttpPost]
        public async Task<IActionResult> UrunEkle(int urunId, int adet)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var urun = await _context.Urunler.FindAsync(urunId);
            if (urun == null)
            {
                return NotFound();
            }

            var mevcutSepetItem = await _context.Sepet
                .FirstOrDefaultAsync(s => s.UrunId == urunId && s.KullaniciId == userId);

            if (mevcutSepetItem != null)
            {
                mevcutSepetItem.Adet += adet;
                if (mevcutSepetItem.Adet > 10)
                {
                    mevcutSepetItem.Adet = 10;
                }
            }
            else
            {
                var yeniSepetItem = new Sepet
                {
                    KullaniciId = userId,
                    UrunId = urunId,
                    Adet = adet,
                    OlusturulmaTarihi = DateTime.Now
                };
                _context.Sepet.Add(yeniSepetItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Sepetten ürün çıkarma
        [HttpPost]
        public async Task<IActionResult> UrunSil(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sepetItem = await _context.Sepet
                .FirstOrDefaultAsync(s => s.Id == id && s.KullaniciId == userId);

            if (sepetItem == null)
            {
                return NotFound();
            }

            _context.Sepet.Remove(sepetItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Ürün adedini güncelleme
        [HttpPost]
        public async Task<IActionResult> AdetGuncelle(int id, int yeniAdet)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sepetItem = await _context.Sepet
                .FirstOrDefaultAsync(s => s.Id == id && s.KullaniciId == userId);

            if (sepetItem == null)
            {
                return NotFound();
            }

            if (yeniAdet < 1)
                yeniAdet = 1;
            if (yeniAdet > 10)
                yeniAdet = 10;

            sepetItem.Adet = yeniAdet;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Sepeti temizleme
        [HttpPost]
        public async Task<IActionResult> SepetiTemizle()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sepetItems = await _context.Sepet
                .Where(s => s.KullaniciId == userId)
                .ToListAsync();

            _context.Sepet.RemoveRange(sepetItems);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
} 