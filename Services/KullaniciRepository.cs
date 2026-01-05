using UcakBiletiRezervasyonSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UcakBiletiRezervasyonSistemi.Services
{
    // Kullanıcı (Admin ve Müşteri) verilerini yöneten Repository
    public class KullaniciRepository
    {
        private static List<Kullanici> Kullanicilar = new List<Kullanici>(); // Statik kullanıcı listesi

        // Constructor: Uygulama ilk çalıştığında örnek kullanıcıları ekler
        public KullaniciRepository()
        {
            if (Kullanicilar.Count == 0)
            {
                // Admin ve Musteri nesneleri Kalıtım ile Kullanici sınıfından türetilmiştir
                Kullanicilar.Add(new Admin { TcNo = "11111111111", Ad = "Admin", Soyad = "Yonetici", Sifre = "admin123" });
                Kullanicilar.Add(new Musteri { TcNo = "22222222222", Ad = "Sila", Soyad = "Askin", Sifre = "musteri123" });
            }
        }

        // TC ve şifre ile kullanıcı doğrulaması (Giriş)
        public Kullanici GirisYap(string tcNo, string sifre)
        {
            var kullanici = Kullanicilar.FirstOrDefault(k => k.TcNo == tcNo);
            // SifreDogrula metodu Kullanici sınıfından gelir
            return (kullanici != null && kullanici.SifreDogrula(sifre)) ? kullanici : null;
        }

        // Yeni bir müşteri ekler
        public void MusteriEkle(Musteri yeniMusteri)
        {
            if (Kullanicilar.Any(k => k.TcNo == yeniMusteri.TcNo))
                throw new Exception("Bu TC kimlik numarası zaten kayıtlı.");
            Kullanicilar.Add(yeniMusteri);
        }
    }
}
