using System;
using System.ComponentModel.DataAnnotations;

namespace Yapaydeneme.Models
{
    public class Urun
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Ürün Adı")]
        public string? UrunAdi { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        [Display(Name = "Fiyat")]
        public decimal Fiyat { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Display(Name = "Resim URL")]
        public string? ResimUrl { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Kategori")]
        public int KategoriId { get; set; }
        public virtual Kategori? Kategori { get; set; }

        [Required(ErrorMessage = "Marka zorunludur.")]
        [StringLength(50, ErrorMessage = "Marka en fazla 50 karakter olabilir.")]
        [Display(Name = "Marka")]
        public string? Marka { get; set; }
        
        [Required(ErrorMessage = "Numara zorunludur.")]
        [Display(Name = "Numara")]
        [StringLength(10, ErrorMessage = "Numara en fazla 10 karakter olabilir.")]
        public string? Numara { get; set; }

        [Required(ErrorMessage = "Renk zorunludur.")]
        [StringLength(30, ErrorMessage = "Renk en fazla 30 karakter olabilir.")]
        [Display(Name = "Renk")]
        public string? Renk { get; set; }

        [Display(Name = "Stokta Var")]
        public bool StokDurumu { get; set; } = true;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime OlusturulmaTarihi { get; set; }
    }
} 