using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UcakBiletiRezervasyonSistemi.Models;
using UcakBiletiRezervasyonSistemi.Services;

namespace UcakBiletiRezervasyonSistemi.Controllers
{
    public class AdminController : Controller
    {
        private readonly UcusRepository _repository;
        private readonly RezervasyonRepository _rezervasyonRepo;

        public AdminController(UcusRepository repository, RezervasyonRepository rezervasyonRepo)
        {
            _repository = repository;
            _rezervasyonRepo = rezervasyonRepo;
        }

        private bool AdminMi() => HttpContext.Session.GetString("Rol") == "Admin";

        public IActionResult Index()
        {
            if (!AdminMi()) return RedirectToAction("Login", "Kullanici");
            var ucuslar = _repository.UcusListele();
            return View(ucuslar);
        }

        public IActionResult UcusEkle()
        {
            if (!AdminMi()) return RedirectToAction("Login", "Kullanici");
            return View();
        }

        [HttpPost]
        public IActionResult UcusEkle(Ucus yeniUcus)
        {
            if (!AdminMi()) return RedirectToAction("Login", "Kullanici");

            try
            {
                _repository.UcusEkle(yeniUcus);
                TempData["Basarili"] = "Uçuş başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Hata"] = ex.Message;
                return View(yeniUcus);
            }
        }

        public IActionResult UcusDetay(string ucusNo)
        {
            if (!AdminMi()) return RedirectToAction("Login", "Kullanici");
            var ucus = _repository.UcusGetir(ucusNo);
            if (ucus == null) return NotFound();
            return View(ucus);
        }

        [HttpPost]
        public IActionResult UcusSil(string ucusNo)
        {
            if (!AdminMi()) return RedirectToAction("Login", "Kullanici");
            
            try
            {
                _rezervasyonRepo.UcusIptalEdildi(ucusNo);
                _repository.UcusIptalEt(ucusNo);
                TempData["Basarili"] = "Uçuş ve ilgili rezervasyonlar başarıyla iptal edildi.";
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Uçuş iptal edilirken hata: " + ex.Message;
            }
            
            return RedirectToAction("Index");
        }
    }
}
