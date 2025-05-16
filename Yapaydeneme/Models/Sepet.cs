using System;
using System.ComponentModel.DataAnnotations;

namespace Yapaydeneme.Models
{
    public class Sepet
    {
        public int Id { get; set; }

        [Required]
        public string KullaniciId { get; set; }

        public int UrunId { get; set; }
        public virtual Urun Urun { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Adet 1 ile 10 arasında olmalıdır")]
        public int Adet { get; set; }

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
    }
} 