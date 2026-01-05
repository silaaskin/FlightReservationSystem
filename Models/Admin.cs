// Models/Admin.cs
namespace UcakBiletiRezervasyonSistemi.Models
{
    // Admin sınıfı, Kullanici sınıfından türetilmiştir (Inheritance)
    public class Admin : Kullanici
    {
        // Soyut metodu override ederek Admin rolünü döndürür (Polymorphism)
        public override string RolAdiGetir()
        {
            return "Admin"; // Admin rol adı
        }

        // Admin'e özel ek metot veya özellikler buraya eklenebilir
    }
}
