using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UcakBiletiRezervasyonSistemi.Models;

namespace UcakBiletiRezervasyonSistemi.Controllers;

// HomeController: Genel sayfaları (ana sayfa, gizlilik, hata) yönetir
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger; // Loglama için ILogger

    // Constructor: Logger enjekte edilir
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Ana sayfa
    public IActionResult Index()
    {
        return View();
    }

    // Gizlilik politikası sayfası
    public IActionResult Privacy()
    {
        return View();
    }

    // Hata sayfası, ResponseCache devre dışı
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Hata ViewModel ile RequestId gönderilir
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
