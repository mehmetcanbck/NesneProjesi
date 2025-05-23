using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Yapaydeneme.Data;
using Yapaydeneme.Models;

namespace Yapaydeneme.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sonEklenenUrunler = await _context.Urunler
                .Include(u => u.Kategori)
                .OrderByDescending(u => u.OlusturulmaTarihi)
                .Take(6)
                .ToListAsync();

            return View(sonEklenenUrunler);
        }
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
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
