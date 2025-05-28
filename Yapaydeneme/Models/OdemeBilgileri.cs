using System.ComponentModel.DataAnnotations;

namespace Yapaydeneme.Models
{
    public class OdemeBilgileri
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kart sahibinin adı zorunludur.")]
        [Display(Name = "Kart Sahibinin Adı")]
        [StringLength(100)]
        public string? KartSahibi { get; set; }

        [Required(ErrorMessage = "Kart numarası zorunludur.")]
        [Display(Name = "Kart Numarası")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Kart numarası 16 haneli olmalıdır.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Geçerli bir kart numarası giriniz.")]
        public string? KartNumarasi { get; set; }

        [Required(ErrorMessage = "Son kullanma tarihi zorunludur.")]
        [Display(Name = "Son Kullanma Tarihi (AA/YY)")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Geçerli bir son kullanma tarihi giriniz (AA/YY).")]
        public string? SonKullanmaTarihi { get; set; }

        [Required(ErrorMessage = "CVV kodu zorunludur.")]
        [Display(Name = "CVV")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV 3 haneli olmalıdır.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "Geçerli bir CVV giriniz.")]
        public string? CVV { get; set; }
    }
} 