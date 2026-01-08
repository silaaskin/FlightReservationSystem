using UcakBiletiRezervasyonSistemi.Data;
using UcakBiletiRezervasyonSistemi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UcakBiletiRezervasyonSistemi.Services
{
    public class UcusRepository
    {
        private readonly AppDbContext _context;

        public UcusRepository(AppDbContext context)
        {
            _context = context;
        }

        public void UcusEkle(Ucus ucus)
        {
            // ÖNEMLI: Aynı uçuş numarası kontrolü
            var mevcutUcus = _context.Ucuslar.FirstOrDefault(u => u.UcusNo == ucus.UcusNo);
            
            if (mevcutUcus != null)
            {
                throw new Exception($"'{ucus.UcusNo}' numaralı uçuş zaten sistemde kayıtlı. Lütfen farklı bir uçuş numarası kullanın.");
            }

            if (ucus.BosKoltukSayisi == 0) 
                ucus.BosKoltukSayisi = ucus.Kapasite;
            
            ucus.KoltuklariOlustur(ucus.Kapasite);
            
            _context.Ucuslar.Add(ucus);
            _context.SaveChanges();
        }

        public List<Ucus> UcusListele()
        {
            var ucuslar = _context.Ucuslar.OrderBy(u => u.KalkisTarihiSaati).ToList();
            
            foreach (var ucus in ucuslar)
            {
                ucus.KoltuklariOlustur(ucus.Kapasite);
                DoluKoltuklariIsaretle(ucus);
            }
            
            return ucuslar;
        }

        public Ucus? UcusGetir(string ucusNo)
        {
            var ucus = _context.Ucuslar.FirstOrDefault(u => u.UcusNo == ucusNo);
            
            if (ucus != null)
            {
                ucus.KoltuklariOlustur(ucus.Kapasite);
                DoluKoltuklariIsaretle(ucus);
            }
            
            return ucus;
        }

        private void DoluKoltuklariIsaretle(Ucus ucus)
        {
            try
            {
                var doluKoltuklar = _context.Rezervasyonlar
                    .Where(r => r.UcusNo == ucus.UcusNo && r.AktifMi)
                    .SelectMany(r => r.Yolcular.Select(y => y.KoltukNo))
                    .ToList();

                foreach (var koltukNo in doluKoltuklar)
                {
                    if (ucus.Koltuklar.ContainsKey(koltukNo))
                    {
                        ucus.Koltuklar[koltukNo].Dolu = true;
                    }
                }
            }
            catch (Exception)
            {
                // Sessizce devam et
            }
        }

        public List<Ucus> UcusAra(string kalkis, string varis, DateTime tarih)
        {
            // Geçmiş tarihe rezervasyon yapılmasını engelle
            if (tarih.Date < DateTime.Now.Date)
            {
                return new List<Ucus>();
            }

            var ucuslar = _context.Ucuslar
                .Where(u => u.KalkisYeri.ToLower() == kalkis.ToLower()
                            && u.VarisYeri.ToLower() == varis.ToLower()
                            && u.KalkisTarihiSaati.Date == tarih.Date
                            && u.BosKoltukSayisi > 0)
                .ToList();

            foreach (var ucus in ucuslar)
            {
                ucus.KoltuklariOlustur(ucus.Kapasite);
                DoluKoltuklariIsaretle(ucus);
            }

            return ucuslar;
        }

        public bool UcusIptalEt(string ucusNo)
        {
            try
            {
                var ucus = _context.Ucuslar.FirstOrDefault(u => u.UcusNo == ucusNo);
                if (ucus != null)
                {
                    _context.Ucuslar.Remove(ucus);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Uçuş silinirken hata: " + ex.Message);
            }
        }

        public bool UcusGuncelle(Ucus guncellenmisUcus)
        {
            try
            {
                var ucus = _context.Ucuslar.FirstOrDefault(u => u.UcusNo == guncellenmisUcus.UcusNo);
                if (ucus != null)
                {
                    ucus.BosKoltukSayisi = guncellenmisUcus.BosKoltukSayisi;
                    ucus.KalkisTarihiSaati = guncellenmisUcus.KalkisTarihiSaati;
                    ucus.TemelFiyat = guncellenmisUcus.TemelFiyat;
                    
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Uçuş güncellenirken hata: " + ex.Message);
            }
        }
    }
}