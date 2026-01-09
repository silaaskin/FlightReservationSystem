using System.ComponentModel.DataAnnotations;

namespace UcakBiletiRezervasyonSistemi.Models
{
    public class Rezervasyon
    {
        [Key]
        public string RezervasyonKodu { get; set; } = string.Empty;
        public string UcusNo { get; set; } = string.Empty;
        public string MusteriTcNo { get; set; } = string.Empty;
        public DateTime RezervasyonTarihi { get; set; }
        
        public decimal ToplamFiyat { get; set; } 
        
        public bool AktifMi { get; set; }
        public string? Mesaj { get; set; }

        public List<Yolcu> Yolcular { get; set; } = new List<Yolcu>();

        public Rezervasyon()
        {
            RezervasyonKodu = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); 
            RezervasyonTarihi = DateTime.Now;
            AktifMi = true;
        }
    }
}