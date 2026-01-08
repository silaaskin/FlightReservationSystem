using UcakBiletiRezervasyonSistemi.Data;
using UcakBiletiRezervasyonSistemi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UcakBiletiRezervasyonSistemi.Services
{
    public class RezervasyonRepository
    {
        private readonly AppDbContext _context;

        public RezervasyonRepository(AppDbContext context)
        {
            _context = context;
        }

        public void RezervasyonEkle(Rezervasyon rezervasyon)
        {
            if (rezervasyon.Yolcular == null || rezervasyon.Yolcular.Count == 0)
                throw new Exception("Rezervasyon için en az bir yolcu gereklidir.");

            if (rezervasyon.RezervasyonTarihi == default(DateTime))
            {
                rezervasyon.RezervasyonTarihi = DateTime.Now;
            }

            _context.Rezervasyonlar.Add(rezervasyon);
            _context.SaveChanges();
        }

        public Rezervasyon? RezervasyonGetir(string rezervasyonKodu)
        {
            try
            {
                return _context.Rezervasyonlar
                    .Include(r => r.Yolcular)
                    .FirstOrDefault(r => r.RezervasyonKodu == rezervasyonKodu);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Rezervasyon> MusteriRezervasyonlariniListele(string musteriTcNo)
        {
            try
            {
                return _context.Rezervasyonlar
                    .Include(r => r.Yolcular)
                    .Where(r => r.MusteriTcNo == musteriTcNo)
                    .OrderByDescending(r => r.RezervasyonTarihi)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<Rezervasyon>();
            }
        }

        public bool RezervasyonIptalEt(string rezervasyonKodu, string? mesaj = null)
        {
            try
            {
                var rezervasyon = RezervasyonGetir(rezervasyonKodu);
                if (rezervasyon != null && rezervasyon.AktifMi)
                {
                    rezervasyon.AktifMi = false;
                    rezervasyon.Mesaj = !string.IsNullOrEmpty(mesaj) 
                        ? mesaj 
                        : $"Rezervasyon iptal edildi.";

                    var ucus = _context.Ucuslar.FirstOrDefault(u => u.UcusNo == rezervasyon.UcusNo);
                    if (ucus != null)
                    {
                        ucus.BosKoltukSayisi += rezervasyon.Yolcular.Count;
                    }

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void UcusIptalEdildi(string ucusNo)
        {
            try
            {
                var iptalRezervasyonlar = _context.Rezervasyonlar
                    .Where(r => r.UcusNo == ucusNo && r.AktifMi)
                    .ToList();

                foreach (var rez in iptalRezervasyonlar)
                {
                    rez.AktifMi = false;
                    rez.Mesaj = $"Üzgünüz, {rez.UcusNo} numaralı uçuşunuz iptal edilmiştir.";
                }
                _context.SaveChanges();
            }
            catch (Exception)
            {
                // Sessizce devam et
            }
        }
    }
}