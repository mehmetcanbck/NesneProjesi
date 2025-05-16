using Microsoft.EntityFrameworkCore;
using Yapaydeneme.Models;

namespace Yapaydeneme.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Kategoriler zaten var mı kontrol et
            if (context.Kategoriler.Any())
            {
                return; // Veritabanı zaten doldurulmuş
            }

            // Ana kategorileri oluştur
            var kategoriler = new Kategori[]
            {
                new Kategori { KategoriAdi = "Kadın", Aciklama = "Kadın ayakkabı modelleri" },
                new Kategori { KategoriAdi = "Erkek", Aciklama = "Erkek ayakkabı modelleri" },
                new Kategori { KategoriAdi = "Çocuk", Aciklama = "Çocuk ayakkabı modelleri" }
            };

            foreach (var kategori in kategoriler)
            {
                context.Kategoriler.Add(kategori);
            }

            context.SaveChanges();
        }
    }
} 