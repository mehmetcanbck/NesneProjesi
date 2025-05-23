using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yapaydeneme.Models;
using Yapaydeneme.Data;
using System.Text;

namespace Yapaydeneme.Controllers
{
    public class KategoriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KategoriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Kategori
        public async Task<IActionResult> Index(string kategori)
        {
            var query = _context.Kategoriler
                .Include(k => k.Urunler)
                .AsQueryable();

            if (!string.IsNullOrEmpty(kategori))
            {
                kategori = kategori.ToLower();
                query = query.Where(k => k.KategoriAdi.ToLower() == kategori);
            }

            var kategoriler = await query.ToListAsync();
            
            if (!string.IsNullOrEmpty(kategori))
            {
                if (!kategoriler.Any())
                {
                    kategoriler = await _context.Kategoriler
                        .Include(k => k.Urunler)
                        .ToListAsync();
                    TempData["Uyari"] = "Seçilen kategori bulunamadı. Tüm kategoriler gösteriliyor.";
                }
                else
                {
                    ViewBag.SecilenKategori = kategori;
                }
            }

            return View(kategoriler);
        }

        // GET: Kategori/Detay/5
        public async Task<IActionResult> Detay(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategori = await _context.Kategoriler
                .Include(k => k.Urunler)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kategori == null)
            {
                return NotFound();
            }

            return View(kategori);
        }

        // GET: Kategori/Yeni
        public IActionResult Yeni()
        {
            return View();
        }

        // POST: Kategori/Yeni
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Yeni([FromForm] Kategori kategori)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(kategori);
                    await _context.SaveChangesAsync();
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        // Tüm kategorileri al ve HTML oluştur
                        var kategoriler = await _context.Kategoriler.ToListAsync();
                        var menuHtml = GenerateKategoriMenuHtml(kategoriler);

                        return Json(new { 
                            success = true, 
                            message = "Kategori başarıyla eklendi.",
                            menuHtml = menuHtml
                        });
                    }

                    return RedirectToAction("KategoriListesi", "Kategori");
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, errors = errors });
                }

                return View(kategori);
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, errors = new[] { "Kategori eklenirken bir hata oluştu: " + ex.Message } });
                }
                
                ModelState.AddModelError("", "Kategori eklenirken bir hata oluştu.");
                return View(kategori);
            }
        }

        private string GenerateKategoriMenuHtml(List<Kategori> kategoriler)
        {
            var sb = new StringBuilder();
            foreach (var kategori in kategoriler)
            {
                sb.Append("<li class=\"nav-item\">");
                sb.Append($"<a class=\"nav-link text-dark\" href=\"/Kategori/Index?kategori={kategori.KategoriAdi.ToLower()}\">");
                sb.Append(kategori.KategoriAdi);
                sb.Append("</a>");
                sb.Append("</li>");
            }
            return sb.ToString();
        }

        // GET: Kategori/Duzenle/5
        public async Task<IActionResult> Duzenle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategori = await _context.Kategoriler
                .Include(k => k.Urunler)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kategori == null)
            {
                return NotFound();
            }

            return View(kategori);
        }

        // POST: Kategori/Duzenle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, Kategori kategori)
        {
            if (id != kategori.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kategori);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KategoriExists(kategori.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("KategoriListesi", "Kategori"); 
            }
            return View(kategori);
        }

        // GET: Kategori/Sil/5
        public async Task<IActionResult> Sil(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategori = await _context.Kategoriler
                .Include(k => k.Urunler)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kategori == null)
            {
                return NotFound();
            }

            return View(kategori);
        }

        // POST: Kategori/Sil/5
        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilOnay(int id)
        {
            var kategori = await _context.Kategoriler
                .Include(k => k.Urunler)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kategori != null)
            {
                _context.Kategoriler.Remove(kategori);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("KategoriListesi", "Kategori");
        }

        // Kategori listesini partial view olarak döndüren action
        public IActionResult KategoriListesi()
        {
            var kategoriler = _context.Kategoriler
                .Include(k => k.Urunler)
                .ToList();
            return PartialView("_KategoriListesi", kategoriler);
        }

        private bool KategoriExists(int id)
        {
            return _context.Kategoriler.Any(e => e.Id == id);
        }
    }
} 