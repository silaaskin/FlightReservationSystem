using System.ComponentModel.DataAnnotations;

namespace UcakBiletiRezervasyonSistemi.Models
{
    public abstract class Kullanici
    {
        [Key] // MySQL için Birincil Anahtar
        public string TcNo { get; set; } = string.Empty;
        public string Ad { get; set; }   = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;

        public abstract string RolAdiGetir();

        public bool SifreDogrula(string girilenSifre)
        {
            return Sifre == girilenSifre;
        }
    }
}