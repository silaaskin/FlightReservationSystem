using UcakBiletiRezervasyonSistemi.Models;

namespace UcakBiletiRezervasyonSistemi.Services
{
    public class FiyatlandirmaServisi
    {
        public decimal FiyatHesapla(Ucus ucus, int yolcuSayisi, string? koltukNo = null, string? kuponKodu = null)
        {
            decimal biletFiyati = ucus.TemelFiyat;

            // 1. KOLTUK TİPİ ETKİSİ
            if (!string.IsNullOrEmpty(koltukNo) && ucus.Koltuklar.ContainsKey(koltukNo))
            {
                biletFiyati += ucus.Koltuklar[koltukNo].EkFiyat;
            }

            // 2. DOLULUK ORANI ETKİSİ
            decimal doluluk = (decimal)(ucus.Kapasite - ucus.BosKoltukSayisi) / ucus.Kapasite;
            if (doluluk >= 0.90m) 
                biletFiyati *= 1.30m;
            else if (doluluk >= 0.80m) 
                biletFiyati *= 1.15m;

            // 3. TARİH ETKİSİ
            TimeSpan kalanZaman = ucus.KalkisTarihiSaati - DateTime.Now;
            if (kalanZaman.TotalDays < 7) 
                biletFiyati *= 1.20m;
            else if (kalanZaman.TotalDays > 60) 
                biletFiyati *= 0.85m;

            // 4. SEZON ETKİSİ
            int ay = ucus.KalkisTarihiSaati.Month;
            if ((ay >= 6 && ay <= 8) || ay == 12 || ay == 1)
            {
                biletFiyati *= 1.25m;
            }
            else if (ay == 4)
            {
                biletFiyati *= 1.10m;
            }
            else if (ay == 11 || ay == 2 || ay == 3)
            {
                biletFiyati *= 0.90m;
            }

            // 5. KUPON/PROMOSYON KODU ETKİSİ
            if (!string.IsNullOrEmpty(kuponKodu))
            {
                kuponKodu = kuponKodu.ToUpper().Trim();
                
                switch (kuponKodu)
                {
                    case "YENIYIL2025":
                        biletFiyati *= 0.80m;
                        break;
                    case "ILKUCUS":
                        biletFiyati *= 0.85m;
                        break;
                    case "ERKENREZERVASYON":
                        biletFiyati *= 0.90m;
                        break;
                    case "OGRENCI":
                        biletFiyati *= 0.75m;
                        break;
                    case "GRUP":
                        if (yolcuSayisi >= 5)
                            biletFiyati *= 0.85m;
                        break;
                }
            }

            // 6. GRUP İNDİRİMİ
            if (yolcuSayisi >= 10)
            {
                biletFiyati *= 0.90m;
            }
            else if (yolcuSayisi >= 5)
            {
                biletFiyati *= 0.95m;
            }

            return Math.Round(biletFiyati * yolcuSayisi, 2);
        }
    }
}