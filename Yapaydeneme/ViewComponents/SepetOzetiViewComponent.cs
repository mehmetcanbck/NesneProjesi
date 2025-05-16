using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Yapaydeneme.Data;

namespace Yapaydeneme.ViewComponents
{
    public class SepetOzetiViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public SepetOzetiViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return View(0);
            }

            var sepetUrunSayisi = await _context.Sepet
                .Where(s => s.KullaniciId == userId)
                .SumAsync(s => s.Adet);

            return View(sepetUrunSayisi);
        }
    }
} 