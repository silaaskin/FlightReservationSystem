namespace UcakBiletiRezervasyonSistemi.Models
{
    // Rezervasyon içindeki tek bir yolcuyu temsil eder
    public class Yolcu
    {
        public string Ad { get; set; }       // Yolcunun adı
        public string Soyad { get; set; }    // Yolcunun soyadı
        public string TcNo { get; set; }     // Yolcunun TC Kimlik numarası
        public string KoltukNo { get; set; } // Yolcuya ayrılan koltuk numarası
    }
}
