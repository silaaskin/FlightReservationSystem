using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UcakBiletiRezervasyonSistemi.Models;
using UcakBiletiRezervasyonSistemi.Services;
using System;
using System.Collections.Generic;
using System.Linq;

// Controller: Müşteri uçuş arama, bilet seçimi ve rezervasyon işlemlerini yönetir
namespace UcakBiletiRezervasyonSistemi.Controllers
{
    public class RezervasyonController : Controller
    {
        // Repository ve servis nesneleri
        private readonly UcusRepository _ucusRepo = new UcusRepository();
        private readonly RezervasyonRepository _rezervasyonRepo = new RezervasyonRepository();
        private readonly FiyatlandirmaServisi _fiyatServisi = new FiyatlandirmaServisi();
        private readonly KullaniciRepository _kullaniciRepo = new KullaniciRepository();

        // Ana sayfa: Önerilen uçuşları gösterir
        public IActionResult Index()
        {
            var onerilenUcuslar = _ucusRepo.UcusListele().Take(3).ToList();
            ViewBag.FiyatServisi = _fiyatServisi; // Fiyat servisini view'e aktar
            return View(onerilenUcuslar);
        }

        // Uçuş arama işlemi
        [HttpPost]
        public IActionResult UcusAra(string kalkis, string varis, DateTime tarih)
        {
            var bulunanUcuslar = _ucusRepo.UcusAra(kalkis, varis, tarih);
            ViewBag.FiyatServisi = _fiyatServisi;
            return View(bulunanUcuslar);
        }

        // Uçuş detay ve tahmini fiyat
        public IActionResult Detay(string ucusNo)
        {
            var ucus = _ucusRepo.UcusGetir(ucusNo);
            if (ucus == null) return NotFound();

            ViewBag.TahminiFiyat = _fiyatServisi.FiyatHesapla(ucus, 1); // Tek kişi fiyat
            return View(ucus);
        }

        // Yolcu bilgileri formu
        [HttpPost]
        public IActionResult YolcuBilgileri(string ucusNo, int yolcuSayisi)
        {
            string musteriTc = HttpContext.Session.GetString("TcNo");
            if (string.IsNullOrEmpty(musteriTc)) // Oturum kontrolü
            {
                TempData["Uyarı"] = "Lütfen giriş yapın.";
                return RedirectToAction("Login", "Kullanici");
            }

            var ucus = _ucusRepo.UcusGetir(ucusNo);
            if (ucus == null || yolcuSayisi <= 0 || yolcuSayisi > ucus.BosKoltukSayisi) // Kapasite kontrolü
            {
                TempData["Hata"] = "Geçersiz uçuş veya koltuk sayısı.";
                return RedirectToAction("Index");
            }

            // Bilgileri view'e aktar
            ViewBag.UcusNo = ucusNo;
            ViewBag.YolcuSayisi = yolcuSayisi;
            ViewBag.MusteriTc = musteriTc;
            ViewBag.ToplamFiyat = _fiyatServisi.FiyatHesapla(ucus, yolcuSayisi);
            ViewBag.Koltuklar = ucus.Koltuklar;

            return View();
        }

        // Rezervasyon onaylama
        [HttpPost]
        public IActionResult Onayla(string ucusNo, int yolcuSayisi, List<Yolcu> yolcular)
        {
            string musteriTc = HttpContext.Session.GetString("TcNo");
            var ucus = _ucusRepo.UcusGetir(ucusNo);

            // Temel doğrulama
            if (string.IsNullOrEmpty(musteriTc) || ucus == null || yolcular == null || yolcuSayisi != yolcular.Count)
            {
                TempData["Hata"] = "Geçersiz işlem, eksik yolcu bilgisi veya oturum hatası.";
                return RedirectToAction("Index");
            }

            var secilenKoltuklar = yolcular.Select(y => y.KoltukNo).ToList();
            if (secilenKoltuklar.Distinct().Count() != yolcuSayisi) // Koltuk benzersizliği
            {
                TempData["Hata"] = "Her yolcu için farklı bir koltuk seçimi yapmalısınız.";
                return RedirectToAction("YolcuBilgileri", new { ucusNo = ucusNo, yolcuSayisi = yolcuSayisi });
            }

            try
            {
                decimal sonFiyat = _fiyatServisi.FiyatHesapla(ucus, yolcuSayisi);

                // Koltuk ayırma işlemi
                foreach (var yolcu in yolcular)
                {
                    if (!ucus.KoltukAyir(yolcu.KoltukNo))
                        throw new Exception($"Koltuk {yolcu.KoltukNo} az önce ayrıldı veya geçersiz.");
                }

                // Rezervasyon oluşturma ve kaydetme
                var yeniRezervasyon = new Rezervasyon
                {
                    UcusNo = ucusNo,
                    MusteriTcNo = musteriTc,
                    OdemeTutari = sonFiyat,
                    Yolcular = yolcular
                };

                _rezervasyonRepo.RezervasyonEkle(yeniRezervasyon);

                TempData["Basarili"] = $"Rezervasyonunuz {yeniRezervasyon.RezervasyonKodu} koduyla oluşturuldu.";
                return RedirectToAction("DetayGoster", new { kod = yeniRezervasyon.RezervasyonKodu });
            }
            catch (Exception ex)
            {
                TempData["Hata"] = $"Rezervasyon yapılamadı: {ex.Message}";
                return RedirectToAction("Detay", new { ucusNo = ucusNo });
            }
        }

        // Rezervasyon iptal etme
        public IActionResult IptalEt(string kod)
        {
            if (_rezervasyonRepo.RezervasyonIptalEt(kod))
                TempData["Basarili"] = $"Rezervasyon {kod} iptal edildi ve koltuklar serbest bırakıldı.";
            else
                TempData["Hata"] = "Rezervasyon bulunamadı veya iptal edilmiş.";

            return RedirectToAction("MusteriRezervasyonlari");
        }

        // Rezervasyon detay gösterme
        public IActionResult DetayGoster(string kod)
        {
            string musteriTc = HttpContext.Session.GetString("TcNo");
            if (string.IsNullOrEmpty(musteriTc)) // Oturum kontrolü
                return RedirectToAction("Login", "Kullanici");

            var rezervasyon = _rezervasyonRepo.RezervasyonGetir(kod);
            if (rezervasyon == null || rezervasyon.MusteriTcNo != musteriTc) // Kendi rezervasyonu mu?
                return Unauthorized();

            ViewBag.Ucus = _ucusRepo.UcusGetir(rezervasyon.UcusNo);
            return View(rezervasyon);
        }

        // Müşterinin tüm rezervasyonları
        public IActionResult MusteriRezervasyonlari()
        {
            string musteriTc = HttpContext.Session.GetString("TcNo");
            if (string.IsNullOrEmpty(musteriTc)) // Oturum kontrolü
                return RedirectToAction("Login", "Kullanici");

            var rezervasyonlar = _rezervasyonRepo.MusteriRezervasyonlariniListele(musteriTc);
            return View(rezervasyonlar);
        }
    }
}
