using UcakBiletiRezervasyonSistemi.Data;
using UcakBiletiRezervasyonSistemi.Models;
using System.Linq;

namespace UcakBiletiRezervasyonSistemi.Services
{
    public class KullaniciRepository
    {
        private readonly AppDbContext _context;

        public KullaniciRepository(AppDbContext context)
        {
            _context = context;
        }

        public Kullanici? GirisYap(string tcNo, string sifre)
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.TcNo == tcNo);
            return (kullanici != null && kullanici.SifreDogrula(sifre)) ? kullanici : null;
        }

        public void MusteriEkle(Musteri yeniMusteri)
        {
            if (_context.Kullanicilar.Any(k => k.TcNo == yeniMusteri.TcNo))
                throw new Exception("Bu TC kimlik numarası zaten kayıtlı.");
            
            _context.Kullanicilar.Add(yeniMusteri);
            _context.SaveChanges();
        }
    }
}