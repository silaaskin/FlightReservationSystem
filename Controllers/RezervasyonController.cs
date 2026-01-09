using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UcakBiletiRezervasyonSistemi.Models;
using UcakBiletiRezervasyonSistemi.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UcakBiletiRezervasyonSistemi.Controllers
{
    public class RezervasyonController : Controller
    {
        private readonly UcusRepository _ucusRepo;
        private readonly RezervasyonRepository _rezervasyonRepo;
        private readonly FiyatlandirmaServisi _fiyatServisi;
        private readonly KullaniciRepository _kullaniciRepo;

        public RezervasyonController(
            UcusRepository ucusRepo, 
            RezervasyonRepository rezervasyonRepo, 
            FiyatlandirmaServisi fiyatServisi, 
            KullaniciRepository kullaniciRepo)
        {
            _ucusRepo = ucusRepo;
            _rezervasyonRepo = rezervasyonRepo;
            _fiyatServisi = fiyatServisi;
            _kullaniciRepo = kullaniciRepo;
        }

        public IActionResult Index()
        {
            var onerilenUcuslar = _ucusRepo.UcusListele().Take(3).ToList();
            ViewBag.FiyatServisi = _fiyatServisi;
            return View(onerilenUcuslar);
        }

        [HttpPost]
        public IActionResult UcusAra(string kalkis, string varis, DateTime tarih)
        {
            var bulunanUcuslar = _ucusRepo.UcusAra(kalkis, varis, tarih);
            ViewBag.FiyatServisi = _fiyatServisi;
            return View(bulunanUcuslar);
        }

        public IActionResult Detay(string ucusNo)
        {
            var ucus = _ucusRepo.UcusGetir(ucusNo);
            if (ucus == null) return NotFound();

            ViewBag.TahminiFiyat = _fiyatServisi.FiyatHesapla(ucus, 1);
            return View(ucus);
        }

// YolcuBilgileri metodunu GET destekli yapın ki RedirectToAction çalışsın
[HttpGet, HttpPost]
public IActionResult YolcuBilgileri(string ucusNo, int yolcuSayisi, string? kuponKodu = null)
{
    string? musteriTc = HttpContext.Session.GetString("TcNo");
    if (string.IsNullOrEmpty(musteriTc)) return RedirectToAction("Login", "Kullanici");

    var ucus = _ucusRepo.UcusGetir(ucusNo);
    if (ucus == null || yolcuSayisi <= 0)
    {
        TempData["Hata"] = "Geçersiz uçuş bilgisi.";
        return RedirectToAction("Index");
    }

    ViewBag.UcusNo = ucusNo;
    ViewBag.YolcuSayisi = yolcuSayisi;
    ViewBag.MusteriTc = musteriTc;
    ViewBag.ToplamFiyat = _fiyatServisi.FiyatHesapla(ucus, yolcuSayisi, null, kuponKodu);
    ViewBag.Koltuklar = ucus.Koltuklar;

    return View();
}

[HttpPost]
public IActionResult Onayla(string ucusNo, int yolcuSayisi, List<Yolcu> yolcular, string? kuponKodu = null)
{
    string? musteriTc = HttpContext.Session.GetString("TcNo");
    var ucus = _ucusRepo.UcusGetir(ucusNo);

    if (string.IsNullOrEmpty(musteriTc) || ucus == null || yolcular == null || yolcular.Count == 0)
    {
        TempData["Hata"] = "Rezervasyon bilgileri eksik.";
        return RedirectToAction("Index");
    }

    // 1. KONTROL: Aynı liste içinde mükerrer TC var mı?
    var girilenTcler = yolcular.Select(y => y.TcNo).ToList();
    if (girilenTcler.Count != girilenTcler.Distinct().Count())
    {
        TempData["Hata"] = "Aynı rezervasyon içinde birden fazla yolcu için aynı TC numarası girilemez.";
        return RedirectToAction("YolcuBilgileri", new { ucusNo, yolcuSayisi });
    }

    // 2. KONTROL: Veritabanında bu uçuş için bu TC'ler zaten kayıtlı mı?
    foreach (var yolcu in yolcular)
    {
        if (_rezervasyonRepo.UcusIcinTcKayitliMi(yolcu.TcNo, ucusNo))
        {
            TempData["Hata"] = $"{yolcu.TcNo} TC numaralı yolcu için bu uçuşta zaten aktif bir bilet bulunmaktadır.";
            return RedirectToAction("YolcuBilgileri", new { ucusNo, yolcuSayisi });
        }
    }

    try
    {
        // Koltuk ayırma işlemleri
        foreach (var yolcu in yolcular)
        {
            if (!ucus.KoltukAyir(yolcu.KoltukNo))
            {
                TempData["Hata"] = $"Koltuk {yolcu.KoltukNo} artık müsait değil.";
                return RedirectToAction("YolcuBilgileri", new { ucusNo, yolcuSayisi });
            }
        }

        // Rezervasyonu kaydet
        decimal sonFiyat = _fiyatServisi.FiyatHesapla(ucus, yolcuSayisi, null, kuponKodu);
        var yeniRezervasyon = new Rezervasyon
        {
            UcusNo = ucusNo,
            MusteriTcNo = musteriTc,
            ToplamFiyat = sonFiyat,
            Yolcular = yolcular,
            RezervasyonTarihi = DateTime.Now
        };

        _rezervasyonRepo.RezervasyonEkle(yeniRezervasyon);
        _ucusRepo.UcusGuncelle(ucus);

        TempData["Basarili"] = "Rezervasyonunuz başarıyla oluşturuldu!";
        return RedirectToAction("DetayGoster", new { kod = yeniRezervasyon.RezervasyonKodu });
    }
    catch (Exception ex)
    {
        TempData["Hata"] = "Hata: " + ex.Message;
        return RedirectToAction("Index");
    }
}
        public IActionResult DetayGoster(string kod)
        {
            var rezervasyon = _rezervasyonRepo.RezervasyonGetir(kod);
            if (rezervasyon == null) return NotFound();

            ViewBag.Ucus = _ucusRepo.UcusGetir(rezervasyon.UcusNo);
            return View(rezervasyon);
        }

        public IActionResult MusteriRezervasyonlari()
        {
            string? musteriTc = HttpContext.Session.GetString("TcNo");
            if (string.IsNullOrEmpty(musteriTc)) return RedirectToAction("Login", "Kullanici");

            var rezervasyonlar = _rezervasyonRepo.MusteriRezervasyonlariniListele(musteriTc);
            return View(rezervasyonlar);
        }

        public IActionResult IptalEt(string kod)
        {
            try
            {
                var rezervasyon = _rezervasyonRepo.RezervasyonGetir(kod);
                if (rezervasyon != null && rezervasyon.AktifMi)
                {
                    var ucus = _ucusRepo.UcusGetir(rezervasyon.UcusNo);
                    if (ucus != null)
                    {
                        foreach (var yolcu in rezervasyon.Yolcular)
                        {
                            ucus.KoltukIadeEt(yolcu.KoltukNo);
                        }
                        _ucusRepo.UcusGuncelle(ucus);
                    }
                    
                    _rezervasyonRepo.RezervasyonIptalEt(kod);
                    TempData["Basarili"] = "Rezervasyonunuz başarıyla iptal edildi.";
                }
                else
                {
                    TempData["Hata"] = "Rezervasyon bulunamadı veya zaten iptal edilmiş.";
                }
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "İptal işlemi sırasında hata: " + ex.Message;
            }

            return RedirectToAction("MusteriRezervasyonlari");
        }
    }
}
