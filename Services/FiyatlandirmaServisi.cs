using UcakBiletiRezervasyonSistemi.Models;
using System;

namespace UcakBiletiRezervasyonSistemi.Services
{
    // Dinamik fiyatlandırma servisi
    public class FiyatlandirmaServisi
    {
        public decimal FiyatHesapla(Ucus ucus, int yolcuSayisi)
        {
            decimal nihaiFiyat = ucus.TemelFiyat; // Temel bilet fiyatı

            // Doluluk oranına göre fiyat artırımı
            decimal dolulukOrani = (decimal)(ucus.Kapasite - ucus.BosKoltukSayisi) / ucus.Kapasite;
            if (dolulukOrani >= 0.90m) nihaiFiyat *= 1.30m; // %90+ doluluk → %30 zam
            else if (dolulukOrani >= 0.80m) nihaiFiyat *= 1.15m; // %80+ doluluk → %15 zam

            // Kalkış tarihine göre fiyat ayarlaması
            TimeSpan kalanZaman = ucus.KalkisTarihiSaati - DateTime.Now;
            if (kalanZaman.TotalDays < 7) nihaiFiyat *= 1.20m;   // 1 haftadan az → %20 zam
            else if (kalanZaman.TotalDays > 60) nihaiFiyat *= 0.80m; // 60 günden fazla → %20 indirim

            // Yolcu sayısına göre toplam fiyat ve 2 ondalık basamak
            return Math.Round(nihaiFiyat * yolcuSayisi, 2);
        }
    }
}
