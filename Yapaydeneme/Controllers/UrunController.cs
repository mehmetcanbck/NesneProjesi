using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yapaydeneme.Models;
using System.Collections.Generic;
using Yapaydeneme.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Yapaydeneme.Controllers
{
    public class UrunController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UrunController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Urun
        public async Task<IActionResult> Index()
        {
            var urunler = await _context.Urunler
                .Include(u => u.Kategori)
                .OrderByDescending(u => u.OlusturulmaTarihi)
                .ToListAsync();
            return View(urunler);
        }

        // GET: Urun/Detay/5
        public async Task<IActionResult> Detay(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunler
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        // GET: Urun/Yeni
        public async Task<IActionResult> Yeni()
        {
            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View();
        }

        // POST: Urun/Yeni
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Yeni([FromForm] Urun urun)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    urun.OlusturulmaTarihi = DateTime.Now;
                    _context.Add(urun);
                    await _context.SaveChangesAsync();
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Ürün başarıyla eklendi." });
                    }
                    
                    return RedirectToAction(nameof(Index));
                }

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, errors = errors });
                }

                ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                return View(urun);
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, errors = new[] { "Ürün eklenirken bir hata oluştu: " + ex.Message } });
                }
                
                ModelState.AddModelError("", "Ürün eklenirken bir hata oluştu.");
                ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                return View(urun);
            }
        }

        // GET: Urun/Duzenle/5
        public async Task<IActionResult> Duzenle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
            {
                return NotFound();
            }

            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View(urun);
        }

        // POST: Urun/Duzenle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(int id, Urun urun)
        {
            if (id != urun.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(urun);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UrunExists(urun.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View(urun);
        }

        // GET: Urun/Sil/5
        public async Task<IActionResult> Sil(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunler
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        // POST: Urun/Sil/5
        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilOnay(int id)
        {
            var urun = await _context.Urunler.FindAsync(id);
            if (urun != null)
            {
                _context.Urunler.Remove(urun);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Ürün listesini partial view olarak döndüren action
        public IActionResult UrunListesi()
        {
            var urunler = _context.Urunler.Include(u => u.Kategori).ToList();
            ViewBag.Kategoriler = _context.Kategoriler.ToList();
            return PartialView("_UrunListesi", urunler);
        }

        private bool UrunExists(int id)
        {
            return _context.Urunler.Any(e => e.Id == id);
        }
    }
} 