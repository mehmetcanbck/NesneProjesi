using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Yapaydeneme.Models;

namespace Yapaydeneme.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Sepet> Sepet { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ürün-Kategori ilişkisi
            modelBuilder.Entity<Urun>()
                .HasOne(u => u.Kategori)
                .WithMany(k => k.Urunler)
                .HasForeignKey(u => u.KategoriId);

            // Sepet-Ürün ilişkisi
            modelBuilder.Entity<Sepet>()
                .HasOne(s => s.urun)
                .WithMany()
                .HasForeignKey(s => s.UrunId);
        }
    }
} 