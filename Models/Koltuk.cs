namespace UcakBiletiRezervasyonSistemi.Models
{
    // Uçakta her bir koltuğu temsil eden sınıf
    public class Koltuk
    {
        public string No { get; set; } // Koltuk numarası
        public KoltukTip Tip { get; set; } // Koltuk tipi (Ekonomi, Business vb.)
        public bool Dolu { get; set; } = false; // Koltuğun doluluk durumu, default boş
        public decimal EkFiyat { get; set; } = 0m; // Ekstra ücret gerekiyorsa
    }
}
