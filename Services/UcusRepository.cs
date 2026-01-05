using UcakBiletiRezervasyonSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UcakBiletiRezervasyonSistemi.Services
{
    public class UcusRepository
    {
        private static Dictionary<string, Ucus> Ucuslar = new Dictionary<string, Ucus>(); // Tüm uçuşlar

        public UcusRepository()
        {
            if (Ucuslar.Count == 0)
            {
                // Başlangıçta örnek uçuşları ekle
                Ucuslar.Add("THY101", new Ucus("THY101", "İstanbul", "Ankara", new DateTime(2025, 12, 10, 14, 0, 0), 100, 500m));
                Ucuslar.Add("PEG202", new Ucus("PEG202", "Ankara", "İzmir", new DateTime(2025, 12, 15, 09, 30, 0), 80, 450m));
                Ucuslar.Add("AFL303", new Ucus("AFL303", "İzmir", "İstanbul", new DateTime(2025, 12, 10, 18, 0, 0), 120, 550m));
            }
        }

        // Yeni uçuş ekler
        public void UcusEkle(Ucus ucus)
        {
            if (!Ucuslar.ContainsKey(ucus.UcusNo))
            {
                var yeniUcus = new Ucus(
                    ucus.UcusNo, ucus.KalkisYeri, ucus.VarisYeri, ucus.KalkisTarihiSaati, ucus.Kapasite, ucus.TemelFiyat);

                Ucuslar.Add(yeniUcus.UcusNo, yeniUcus);
            }
        }

        // Tüm uçuşları listeler (tarihe göre sıralı)
        public List<Ucus> UcusListele()
        {
            return Ucuslar.Values.OrderBy(u => u.KalkisTarihiSaati).ToList();
        }

        // Uçuş koduna göre getirir
        public Ucus UcusGetir(string ucusNo)
        {
            return Ucuslar.ContainsKey(ucusNo) ? Ucuslar[ucusNo] : null;
        }

        // Uçuş arama (kalkış, varış, tarih ve boş koltuk kontrolü)
        public List<Ucus> UcusAra(string kalkis, string varis, DateTime tarih)
        {
            return Ucuslar.Values
                .Where(u => u.KalkisYeri.Equals(kalkis, StringComparison.OrdinalIgnoreCase)
                            && u.VarisYeri.Equals(varis, StringComparison.OrdinalIgnoreCase)
                            && u.KalkisTarihiSaati.Date == tarih.Date
                            && u.BosKoltukSayisi > 0)
                .ToList();
        }

        // Uçuş iptal (repository'den siler)
        public bool UcusIptalEt(string ucusNo)
        {
            if (Ucuslar.ContainsKey(ucusNo))
            {
                Ucuslar.Remove(ucusNo);
                return true;
            }
            return false;
        }

        // Uçuş güncelleme
        public bool UcusGuncelle(Ucus guncellenmisUcus)
        {
            if (Ucuslar.ContainsKey(guncellenmisUcus.UcusNo))
            {
                Ucuslar[guncellenmisUcus.UcusNo] = guncellenmisUcus;
                return true;
            }
            return false;
        }
    }
}
