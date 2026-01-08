using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UcakBiletiRezervasyonSistemi.Models;
using UcakBiletiRezervasyonSistemi.Services;

namespace UcakBiletiRezervasyonSistemi.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly KullaniciRepository _repository;

        public KullaniciController(KullaniciRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string tcNo, string sifre)
        {
            var kullanici = _repository.GirisYap(tcNo, sifre);
            if (kullanici != null)
            {
                HttpContext.Session.SetString("TcNo", kullanici.TcNo);
                HttpContext.Session.SetString("AdSoyad", $"{kullanici.Ad} {kullanici.Soyad}");
                HttpContext.Session.SetString("Rol", kullanici.RolAdiGetir());

                if (kullanici is Admin)
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Rezervasyon");
            }

            ViewBag.Hata = "TC No veya Şifre hatalı!";
            return View();
        }

        public IActionResult KayitOl() => View();

        [HttpPost]
        public IActionResult KayitOl(Musteri yeniMusteri)
        {
            try
            {
                _repository.MusteriEkle(yeniMusteri);
                TempData["Mesaj"] = "Kaydınız başarıyla tamamlandı. Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = ex.Message;
                return View(yeniMusteri);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
