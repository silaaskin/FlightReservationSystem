using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UcakBiletiRezervasyonSistemi.Models;
using UcakBiletiRezervasyonSistemi.Services;
using System;
using System.Linq;

// AdminController: Admin yetkisi gerektiren uçuş yönetimi işlevlerini yönetir.
namespace UcakBiletiRezervasyonSistemi.Controllers
{
    public class AdminController : Controller
    {
        // Repositoryler: Uçuş ve rezervasyon verilerine erişim sağlar
        private readonly UcusRepository _ucusRepo = new UcusRepository();
        private readonly RezervasyonRepository _rezRepo = new RezervasyonRepository();

        // Admin paneli ana sayfası, tüm uçuşları listeler
        public IActionResult Index()
        {
            // Sadece Admin rolü erişebilir
            if (HttpContext.Session.GetString("Rol") != "Admin")
                return RedirectToAction("Login", "Kullanici");

            var tumUcuslar = _ucusRepo.UcusListele(); // Tüm uçuşları al
            return View(tumUcuslar);
        }

        // Uçuş ekleme formunu gösterir (GET)
        public IActionResult UcusEkle()
        {
            if (HttpContext.Session.GetString("Rol") != "Admin")
                return RedirectToAction("Login", "Kullanici");

            return View(new Ucus()); // Boş uçuş modeli ile form
        }

        // Yeni uçuş ekleme işlemi (POST)
        [HttpPost]
        public IActionResult UcusEkle(Ucus yeniUcus)
        {
            if (HttpContext.Session.GetString("Rol") != "Admin")
                return RedirectToAction("Login", "Kullanici");

            // Geçmiş tarihe uçuş eklenemez
            if (yeniUcus.KalkisTarihiSaati < DateTime.Now)
            {
                TempData["Hata"] = "Geçmiş bir tarihe uçuş eklenemez.";
                return View(yeniUcus);
            }

            // Uçuş numarası benzersiz olmalı
            if (_ucusRepo.UcusGetir(yeniUcus.UcusNo) != null)
            {
                TempData["Hata"] = "Bu uçuş numarası zaten mevcut.";
                return View(yeniUcus);
            }

            if (ModelState.IsValid)
            {
                // Boş koltuk sayısı girilmemişse kapasiteye eşitle
                if (yeniUcus.BosKoltukSayisi == 0 && yeniUcus.Kapasite > 0)
                    yeniUcus.BosKoltukSayisi = yeniUcus.Kapasite;

                _ucusRepo.UcusEkle(yeniUcus); // Uçuşu ekle
                TempData["Basarili"] = $"{yeniUcus.UcusNo} numaralı uçuş başarıyla eklendi.";
                return RedirectToAction("Index"); // Ana sayfaya dön
            }

            return View(yeniUcus); // Model hatalıysa formu tekrar göster
        }

        // Seçilen uçuşu iptal eder, ilgili rezervasyonları günceller
        [HttpPost]
        public IActionResult UcusIptalEt(string ucusNo)
        {
            if (HttpContext.Session.GetString("Rol") != "Admin")
                return RedirectToAction("Login", "Kullanici");

            var ucus = _ucusRepo.UcusGetir(ucusNo);
            if (ucus == null)
            {
                TempData["Hata"] = "Uçuş bulunamadı.";
                return RedirectToAction("Index");
            }

            _ucusRepo.UcusIptalEt(ucusNo); // Uçuşu iptal et
            _rezRepo.UcusIptalEdildi(ucusNo); // Rezervasyonları iptal et

            TempData["Basarili"] = $"Uçuş {ucusNo} iptal edildi.";
            return RedirectToAction("Index");
        }

        // Belirli bir uçuşun detaylarını gösterir
        public IActionResult UcusDetay(string ucusNo)
        {
            if (HttpContext.Session.GetString("Rol") != "Admin")
                return RedirectToAction("Login", "Kullanici");

            var ucus = _ucusRepo.UcusGetir(ucusNo);
            if (ucus == null)
            {
                TempData["Hata"] = "Uçuş bulunamadı veya iptal edilmiş.";
                return RedirectToAction("Index");
            }

            return View(ucus); // Uçuş detaylarını göster
        }
    }
}
