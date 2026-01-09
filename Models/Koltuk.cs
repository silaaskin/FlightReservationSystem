namespace UcakBiletiRezervasyonSistemi.Models
{
    
    public class Koltuk
    {
        public string No { get; set; } = string.Empty; 
        public KoltukTip Tip { get; set; }
        public bool Dolu { get; set; } = false;
        public decimal EkFiyat { get; set; } = 0m;
    }
}
