using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yapaydeneme.Models;
using Yapaydeneme.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        public async Task<IActionResult> Index(bool? adminSession)
        {
            // Admin kontrolü
            if (HttpContext.Session.GetString("AdminLogin") == "true" || adminSession == true)
            {
                var tumSiparisler = await _context.Siparisler
                    .OrderByDescending(s => s.SiparisTarihi)
                    .ToListAsync();
                return View("AdminSiparisler", tumSiparisler);
            }

            // Normal kullanıcı kontrolü
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var siparisler = await _context.Siparisler
                .Where(s => s.KullaniciId == kullaniciId)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        // GET: Siparis/Detay/5
        [Authorize]
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

            // Admin değilse ve kendi siparişi değilse erişimi engelle
            if (HttpContext.Session.GetString("AdminLogin") != "true" && 
                siparis.KullaniciId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            return View(siparis);
        }

        // GET: Siparis/SiparisOlustur
        [Authorize]
        public async Task<IActionResult> SiparisOlustur()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciId))
            {
                return RedirectToAction("Login", "Account");
            }

            var sepetUrunleri = await _context.Sepet
                .Include(s => s.urun)
                .Where(s => s.KullaniciId == kullaniciId)
                .ToListAsync();

            if (!sepetUrunleri.Any())
            {
                TempData["Hata"] = "Sepetiniz boş!";
                return RedirectToAction("Index", "Sepet");
            }

            ViewBag.ToplamTutar = sepetUrunleri.Sum(s => s.urun!.Fiyat * s.Adet);
            return View();
        }

        // POST: Siparis/SiparisOlustur
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SiparisOlustur(string teslimatAdresi, string telefonNo, OdemeBilgileri odemeBilgileri)
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciId))
            {
                TempData["Hata"] = "Kullanıcı girişi gerekli!";
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(teslimatAdresi) || string.IsNullOrEmpty(telefonNo))
            {
                TempData["Hata"] = "Lütfen teslimat adresi ve telefon numarasını doldurunuz!";
                return RedirectToAction("SiparisOlustur");
            }

            if (telefonNo.Length != 10 || !telefonNo.All(char.IsDigit))
            {
                TempData["Hata"] = "Lütfen geçerli bir telefon numarası giriniz (10 haneli)!";
                return RedirectToAction("SiparisOlustur");
            }

            // Ödeme bilgilerini kontrol et
            if (odemeBilgileri == null)
            {
                TempData["Hata"] = "Ödeme bilgileri bulunamadı!";
                return RedirectToAction("SiparisOlustur");
            }

            if (string.IsNullOrEmpty(odemeBilgileri.KartSahibi))
            {
                TempData["Hata"] = "Kart sahibi adı gerekli!";
                return RedirectToAction("SiparisOlustur");
            }

            if (string.IsNullOrEmpty(odemeBilgileri.KartNumarasi))
            {
                TempData["Hata"] = "Kart numarası gerekli!";
                return RedirectToAction("SiparisOlustur");
            }

            // Kart numarası kontrolü
            var kartNumarasi = odemeBilgileri.KartNumarasi.Replace(" ", "");
            if (kartNumarasi.Length != 16 || !kartNumarasi.All(char.IsDigit))
            {
                TempData["Hata"] = $"Geçersiz kart numarası! Girilen uzunluk: {kartNumarasi.Length}, Tüm karakterler rakam mı: {kartNumarasi.All(char.IsDigit)}";
                return RedirectToAction("SiparisOlustur");
            }

            if (string.IsNullOrEmpty(odemeBilgileri.SonKullanmaTarihi))
            {
                TempData["Hata"] = "Son kullanma tarihi gerekli!";
                return RedirectToAction("SiparisOlustur");
            }

            // Son kullanma tarihi kontrolü
            if (!System.Text.RegularExpressions.Regex.IsMatch(odemeBilgileri.SonKullanmaTarihi, @"^(0[1-9]|1[0-2])\/([0-9]{2})$"))
            {
                TempData["Hata"] = $"Geçersiz son kullanma tarihi formatı! Girilen değer: {odemeBilgileri.SonKullanmaTarihi}";
                return RedirectToAction("SiparisOlustur");
            }

            if (string.IsNullOrEmpty(odemeBilgileri.CVV))
            {
                TempData["Hata"] = "CVV kodu gerekli!";
                return RedirectToAction("SiparisOlustur");
            }

            // CVV kontrolü
            if (odemeBilgileri.CVV.Length != 3 || !odemeBilgileri.CVV.All(char.IsDigit))
            {
                TempData["Hata"] = $"Geçersiz CVV! Uzunluk: {odemeBilgileri.CVV.Length}, Tüm karakterler rakam mı: {odemeBilgileri.CVV.All(char.IsDigit)}";
                return RedirectToAction("SiparisOlustur");
            }

            // Sepetteki ürünleri al
            var sepetUrunleri = await _context.Sepet
                .Include(s => s.urun)
                .Where(s => s.KullaniciId == kullaniciId)
                .ToListAsync();

            if (!sepetUrunleri.Any())
            {
                TempData["Hata"] = "Sepetiniz boş!";
                return RedirectToAction("Index", "Sepet");
            }

            try
            {
                // Toplam tutarı hesapla
                var toplamTutar = sepetUrunleri.Sum(s => s.urun!.Fiyat * s.Adet);

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

                TempData["SiparisNo"] = siparis.SiparisNo;
                return RedirectToAction(nameof(SiparisTamamlandi));
            }
            catch (Exception ex)
            {
                TempData["Hata"] = $"Sipariş oluşturulurken bir hata oluştu: {ex.Message}";
                return RedirectToAction("SiparisOlustur");
            }
        }

        // GET: Siparis/SiparisTamamlandi
        [Authorize]
        public IActionResult SiparisTamamlandi()
        {
            if (TempData["SiparisNo"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Siparis/DurumGuncelle
        [HttpPost]
        public async Task<IActionResult> DurumGuncelle(int id, string durum)
        {
            // Admin kontrolü
            if (HttpContext.Session.GetString("AdminLogin") != "true")
            {
                return Forbid();
            }

            var siparis = await _context.Siparisler.FindAsync(id);
            if (siparis == null)
            {
                return NotFound();
            }

            siparis.SiparisDurumu = durum;
            siparis.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Siparis/Siparislerim
        [Authorize]
        public async Task<IActionResult> Siparislerim()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciId))
            {
                return RedirectToAction("Login", "Account");
            }

            var siparisler = await _context.Siparisler
                .Where(s => s.KullaniciId == kullaniciId)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        // POST: Siparis/SiparisIptal/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SiparisIptal(int id)
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciId))
            {
                return RedirectToAction("Login", "Account");
            }

            var siparis = await _context.Siparisler
                .FirstOrDefaultAsync(s => s.Id == id && s.KullaniciId == kullaniciId);

            if (siparis == null)
            {
                TempData["Hata"] = "Sipariş bulunamadı!";
                return RedirectToAction(nameof(Siparislerim));
            }

            // Durumu Beklemede veya Onaylandı olan siparişler iptal edilebilir
            if (siparis.SiparisDurumu != "Beklemede" && siparis.SiparisDurumu != "Onaylandı")
            {
                TempData["Hata"] = "Sadece beklemede veya yeni onaylanmış siparişler iptal edilebilir!";
                return RedirectToAction(nameof(Siparislerim));
            }

            siparis.SiparisDurumu = "İptal Edildi";
            siparis.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Basarili"] = "Siparişiniz başarıyla iptal edildi.";
            
            return RedirectToAction(nameof(Siparislerim));
        }
    }
}