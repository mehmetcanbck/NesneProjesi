using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Yapaydeneme.Models
{
    public class Kategori
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [Display(Name = "Kategori Adı")]
        public string KategoriAdi { get; set; }

        [Display(Name = "Açıklama")]
        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string? Aciklama { get; set; }

        public Kategori()
        {
            Urunler = new List<Urun>();
        }

        // Kategoriye ait ürünler
        public virtual ICollection<Urun> Urunler { get; set; }
    }
} 