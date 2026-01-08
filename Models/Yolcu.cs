using System.ComponentModel.DataAnnotations;

namespace UcakBiletiRezervasyonSistemi.Models
{
    public class Yolcu
    {
        [Key]
        public int Id { get; set; }
        public string RezervasyonKodu { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string TcNo { get; set; } = string.Empty;
        public string KoltukNo { get; set; } = string.Empty;
        public string? KoltukTipi { get; set; }
        public decimal Fiyat { get; set; }
    }
}