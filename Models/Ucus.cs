using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UcakBiletiRezervasyonSistemi.Models
{
    public class Ucus
    {
        [Key]
        public string UcusNo { get; set; } = string.Empty;
        public string KalkisYeri { get; set; } = string.Empty;
        public string VarisYeri { get; set; } = string.Empty;
        public DateTime KalkisTarihiSaati { get; set; }
        public int Kapasite { get; set; } 
        public int BosKoltukSayisi { get; set; } 
        public decimal TemelFiyat { get; set; }

        [NotMapped]
        public Dictionary<string, Koltuk> Koltuklar { get; set; } = new Dictionary<string, Koltuk>();

        public Ucus() { }

        public Ucus(string ucusNo, string kalkisYeri, string varisYeri, DateTime kalkisTarihiSaati, int kapasite, decimal temelFiyat)
        {
            UcusNo = ucusNo;
            KalkisYeri = kalkisYeri;
            VarisYeri = varisYeri;
            KalkisTarihiSaati = kalkisTarihiSaati;
            Kapasite = kapasite;
            BosKoltukSayisi = kapasite;
            TemelFiyat = temelFiyat;
            KoltuklariOlustur(kapasite);
        }

        public void KoltuklariOlustur(int kapasite)
        {
            Koltuklar.Clear();
            
            char[] siraHarfleri = { 'A', 'B', 'C', 'D', 'E', 'F' };
            
            int maxSira = (int)Math.Ceiling((double)kapasite / siraHarfleri.Length);

            int eklenmisSayisi = 0;

            for (int sira = 1; sira <= maxSira && eklenmisSayisi < kapasite; sira++)
            {
                foreach (char harf in siraHarfleri)
                {
                    if (eklenmisSayisi >= kapasite) break;
                    
                    string koltukNo = $"{harf}{sira}";
                    
                    KoltukTip tip = (sira <= 1) ? KoltukTip.Business : KoltukTip.Ekonomi;
                    decimal ekFiyat = (tip == KoltukTip.Business) ? 500m : 0m;
                    
                    Koltuklar.Add(koltukNo, new Koltuk 
                    { 
                        No = koltukNo, 
                        Tip = tip, 
                        EkFiyat = ekFiyat,
                        Dolu = false
                    });
                    
                    eklenmisSayisi++;
                }
            }
        }

        public bool KoltukAyir(string koltukNo)
        {
            if (Koltuklar.ContainsKey(koltukNo) && !Koltuklar[koltukNo].Dolu)
            {
                Koltuklar[koltukNo].Dolu = true;
                BosKoltukSayisi--;
                return true;
            }
            return false;
        }

        public void KoltukIadeEt(string koltukNo)
        {
            if (Koltuklar.ContainsKey(koltukNo) && Koltuklar[koltukNo].Dolu)
            {
                Koltuklar[koltukNo].Dolu = false;
                BosKoltukSayisi++;
            }
        }
    }
}