using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yapaydeneme.Models;
using Yapaydeneme.Data;

namespace Yapaydeneme.Controllers
{
    public class SiparisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiparisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Siparis
        public async Task<IActionResult> Index()
        {
            var kullaniciId = User.Identity.Name;
            var siparisler = await _context.Siparisler
                .Where(s => s.KullaniciId == kullaniciId)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        // GET: Siparis/Detay/5
        public async Task<IActionResult> Detay(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siparis = await _context.Siparisler
                .FirstOrDefaultAsync(m => m.Id == id);

            if (siparis == null)
            {
                return NotFound();
            }

            return View(siparis);
        }

        // POST: Siparis/SiparisOlustur
        [HttpPost]
        public async Task<IActionResult> SiparisOlustur(string teslimatAdresi, string telefonNo)
        {
            var kullaniciId = User.Identity.Name;
            
            // Sepetteki ürünleri al
            var sepetUrunleri = await _context.Sepet
                .Include(s => s.urun)
                .Where(s => s.KullaniciId == kullaniciId)
                .ToListAsync();

            if (!sepetUrunleri.Any())
            {
                return RedirectToAction("Index", "Sepet");
            }

            // Toplam tutarı hesapla
            var toplamTutar = sepetUrunleri.Sum(s => s.urun.Fiyat * s.Adet);

            // Yeni sipariş oluştur
            var siparis = new Siparis
            {
                KullaniciId = kullaniciId,
                SiparisNo = DateTime.Now.Ticks.ToString(),
                ToplamTutar = toplamTutar,
                SiparisDurumu = "Beklemede",
                TeslimatAdresi = teslimatAdresi,
                TelefonNo = telefonNo,
                SiparisTarihi = DateTime.Now
            };

            _context.Add(siparis);

            // Sepeti temizle
            _context.Sepet.RemoveRange(sepetUrunleri);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detay), new { id = siparis.Id });
        }

        // POST: Siparis/DurumGuncelle
        [HttpPost]
        public async Task<IActionResult> DurumGuncelle(int id, string durum)
        {
            var siparis = await _context.Siparisler.FindAsync(id);
            if (siparis == null)
            {
                return NotFound();
            }

            siparis.SiparisDurumu = durum;
            siparis.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detay), new { id = siparis.Id });
        }
    }
} 