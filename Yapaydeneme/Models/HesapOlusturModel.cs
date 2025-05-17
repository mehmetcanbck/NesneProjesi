using System.ComponentModel.DataAnnotations;

namespace Yapaydeneme.Models
{
    public class HesapOlusturModel
    {
        [Required]
        [Display(Name = "Kullanıcı Adı")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Sadece sayı ve harf giriniz")]// kullanıcı adına gelen karakterler sadece A'dan Z ye rakamlar
        public string? KullaniciAdi { get; set; }

        [Required]
        [Display(Name = "Eposta")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [Display(Name = "Parola")]
        [DataType(DataType.Password)] 
        public string? Sifre { get; set; }
        [Required]
        [Display(Name = "Parola")]
        [DataType(DataType.Password)]
        [Compare("Sifre",ErrorMessage ="Parola eşleşmiyor!")]
        public string? SifreTekrar { get; set; }
    }
}
