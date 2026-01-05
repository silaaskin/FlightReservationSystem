namespace UcakBiletiRezervasyonSistemi.Models
{
    public class Rezervasyon
    {
        public string RezervasyonKodu { get; set; } // Benzersiz rezervasyon kodu
        public string UcusNo { get; set; } 
        public string MusteriTcNo { get; set; } // Rezervasyonu yapan müşterinin TC'si
        public DateTime RezervasyonTarihi { get; set; } // Rezervasyon oluşturulma tarihi
        public decimal OdemeTutari { get; set; }
        public bool AktifMi { get; set; } // True: aktif, False: iptal edilmiş
        public string Mesaj { get; set; } // Opsiyonel mesaj alanı

        // Bir rezervasyon birden fazla yolcu içerebilir
        public List<Yolcu> Yolcular { get; set; } = new List<Yolcu>();

        public Rezervasyon()
        {
            // Nesne oluşturulduğunda otomatik benzersiz kod ve tarih ataması
            RezervasyonKodu = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); 
            RezervasyonTarihi = DateTime.Now;
            AktifMi = true;
        }
    }
}
