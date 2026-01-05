using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UcakBiletiRezervasyonSistemi.Services;

namespace UcakBiletiRezervasyonSistemi.Controllers
{
    // KullaniciController: Kullanıcı giriş/çıkış işlemlerini yönetir
    public class KullaniciController : Controller
    {
        private readonly KullaniciRepository _kullaniciRepo; // Kullanıcı verilerine erişim

        // Constructor: Repository başlatılır
        public KullaniciController()
        {
            _kullaniciRepo = new KullaniciRepository();
        }

        // Login sayfası (GET)
        public IActionResult Login()
        {
            // Zaten giriş yapılmışsa rolüne göre yönlendir
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Rol")))
            {
                if (HttpContext.Session.GetString("Rol") == "Admin")
                    return RedirectToAction("Index", "Admin");
                return RedirectToAction("Index", "Rezervasyon");
            }
            return View();
        }

        // Login işlemi (POST)
        [HttpPost]
        public IActionResult Login(string tcNo, string sifre)
        {
            var kullanici = _kullaniciRepo.GirisYap(tcNo, sifre);

            if (kullanici != null)
            {
                // Session bilgilerini ayarla
                HttpContext.Session.SetString("TcNo", kullanici.TcNo);
                HttpContext.Session.SetString("Rol", kullanici.RolAdiGetir());
                HttpContext.Session.SetString("AdSoyad", $"{kullanici.Ad} {kullanici.Soyad}");

                // Rolüne göre yönlendir
                if (kullanici.RolAdiGetir() == "Admin")
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Index", "Home");
            }

            // Giriş hatalıysa mesaj göster
            ViewBag.HataMesaji = "Hatalı TC veya Şifre. Tekrar deneyin.";
            return View();
        }

        // Logout işlemi
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Session temizle
            return RedirectToAction("Login", "Kullanici");
        }
    }
}
