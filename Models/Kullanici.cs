namespace UcakBiletiRezervasyonSistemi.Models
{
    // Soyut kullanıcı sınıfı: doğrudan nesne oluşturulamaz
    public abstract class Kullanici
    {
        // Temel kullanıcı bilgileri
        public string TcNo { get; set; } // TC Kimlik numarası
        public string Ad { get; set; }   // Ad
        public string Soyad { get; set; } // Soyad
        public string Sifre { get; set; } // Şifre

        // Soyut metot: Alt sınıflar kendi rolünü döndürmek zorunda
        public abstract string RolAdiGetir();

        // Ortak davranış: Şifre doğrulama
        public bool SifreDogrula(string girilenSifre)
        {
            return Sifre == girilenSifre;
        }
    }
}
