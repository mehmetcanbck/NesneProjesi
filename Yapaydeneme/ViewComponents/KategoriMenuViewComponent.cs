using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Yapaydeneme.Data;

namespace Yapaydeneme.ViewComponents
{
    public class KategoriMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public KategoriMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var kategoriler = await _context.Kategoriler.ToListAsync();
            return View(kategoriler);
        }
    }
} 