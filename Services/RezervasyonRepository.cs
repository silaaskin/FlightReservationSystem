using UcakBiletiRezervasyonSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UcakBiletiRezervasyonSistemi.Services
{
    public class RezervasyonRepository
    {
        private static List<Rezervasyon> Rezervasyonlar = new List<Rezervasyon>(); // Statik rezervasyon listesi
        private readonly UcusRepository _ucusRepository = new UcusRepository(); // Uçuş verisine erişim

        // Yeni rezervasyon ekler
        public void RezervasyonEkle(Rezervasyon rezervasyon)
        {
            if (rezervasyon.Yolcular == null || rezervasyon.Yolcular.Count == 0)
                throw new Exception("Rezervasyon için en az bir yolcu gereklidir.");

            var ucus = _ucusRepository.UcusGetir(rezervasyon.UcusNo);
            if (ucus == null)
                throw new Exception("Rezervasyon yapılmak istenen uçuş bulunamadı.");

            Rezervasyonlar.Add(rezervasyon);
        }

        // Rezervasyon koduna göre getirir
        public Rezervasyon RezervasyonGetir(string rezervasyonKodu)
        {
            return Rezervasyonlar.FirstOrDefault(r => r.RezervasyonKodu == rezervasyonKodu);
        }

        // Belirli müşterinin tüm rezervasyonlarını listeler
        public List<Rezervasyon> MusteriRezervasyonlariniListele(string musteriTcNo)
        {
            return Rezervasyonlar
                .Where(r => r.MusteriTcNo == musteriTcNo)
                .ToList();
        }

        // Rezervasyonu iptal eder ve koltukları iade eder
        public bool RezervasyonIptalEt(string rezervasyonKodu, string mesaj = null)
        {
            var rezervasyon = RezervasyonGetir(rezervasyonKodu);
            if (rezervasyon != null && rezervasyon.AktifMi)
            {
                rezervasyon.AktifMi = false;
                rezervasyon.Mesaj = !string.IsNullOrEmpty(mesaj) 
                    ? mesaj 
                    : $"Uçuş {rezervasyon.UcusNo} iptal edilmiştir.";

                var ucus = _ucusRepository.UcusGetir(rezervasyon.UcusNo);
                if (ucus != null)
                {
                    foreach (var yolcu in rezervasyon.Yolcular)
                        ucus.KoltukIadeEt(yolcu.KoltukNo); // Koltuk iade işlemi
                }

                return true;
            }
            return false;
        }

        // Uçuş iptal edildiğinde ilgili rezervasyonları pasif yapar ve mesaj bırakır
        public void UcusIptalEdildi(string ucusNo)
        {
            var iptalRezervasyonlar = Rezervasyonlar
                .Where(r => r.UcusNo == ucusNo && r.AktifMi)
                .ToList();

            foreach (var rez in iptalRezervasyonlar)
            {
                rez.AktifMi = false;
                rez.Mesaj = $"Üzgünüz, {rez.UcusNo} numaralı uçuşunuz iptal edilmiştir.";
            }
        }
    }
}
