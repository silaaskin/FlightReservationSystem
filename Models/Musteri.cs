namespace UcakBiletiRezervasyonSistemi.Models
{
    // Kullanici'dan türetilmiş müşteri sınıfı (Inheritance)
    public class Musteri : Kullanici
    {
        // Müşteriye özel ek özellikler eklenebilir
        // Örn: public List<Rezervasyon> RezervasyonListesi { get; set; } 

        // Soyut metodu uygular (Polymorphism için)
        public override string RolAdiGetir()
        {
            return "Müşteri";
        }
    }
}
