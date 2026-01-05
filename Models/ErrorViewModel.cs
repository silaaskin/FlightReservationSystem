namespace UcakBiletiRezervasyonSistemi.Models;

// Hata sayfaları için kullanılan model.
public class ErrorViewModel
{
    // Hata talep kimliği.
    public string? RequestId { get; set; }

    // RequestId boş değilse true döner.
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}