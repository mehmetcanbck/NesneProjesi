using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yapaydeneme.Models
{
    public class Siparis
    {
        public int Id { get; set; }

        [Required]
        public string? KullaniciId { get; set; }

        [Required]
        [Display(Name = "Sipariş Numarası")]
        public string? SiparisNo { get; set; }

        [Required]
        [Display(Name = "Toplam Tutar")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ToplamTutar { get; set; }

        [Required]
        [Display(Name = "Sipariş Durumu")]
        public string? SiparisDurumu { get; set; }

        [Required]
        [Display(Name = "Teslimat Adresi")]
        public string? TeslimatAdresi { get; set; }

        [Required]
        [Display(Name = "İletişim Numarası")]
        public string? TelefonNo { get; set; }

        [Display(Name = "Sipariş Tarihi")]
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? GuncellemeTarihi { get; set; }
    }
} 