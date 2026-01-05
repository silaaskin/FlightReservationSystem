namespace UcakBiletiRezervasyonSistemi.Models
{
    public class Ucus
    {
        public string UcusNo { get; set; } // Uçuş numarası
        public string KalkisYeri { get; set; }
        public string VarisYeri { get; set; }
        public DateTime KalkisTarihiSaati { get; set; }

        public int Kapasite { get; set; } // Toplam koltuk sayısı
        public int BosKoltukSayisi { get; set; } // Boş koltuk sayısı
        public decimal TemelFiyat { get; set; }

        // Koltuklar artık Koltuk objesi olarak tutuluyor
        public Dictionary<string, Koltuk> Koltuklar { get; set; } = new Dictionary<string, Koltuk>();

        // Parametresiz Constructor
        public Ucus()
        {
        }

        // Parametreli Constructor
        public Ucus(string ucusNo, string kalkisYeri, string varisYeri, DateTime kalkisTarihiSaati, int kapasite, decimal temelFiyat)
        {
            UcusNo = ucusNo;
            KalkisYeri = kalkisYeri;
            VarisYeri = varisYeri;
            KalkisTarihiSaati = kalkisTarihiSaati;
            Kapasite = kapasite;
            BosKoltukSayisi = kapasite;
            TemelFiyat = temelFiyat;

            KoltuklariOlustur(kapasite); // Koltukları başlat
        }

        // Koltukların oluşturulması
        private void KoltuklariOlustur(int kapasite)
        {
            Koltuklar.Clear();

            char[] siraHarfleri = { 'A', 'B', 'C', 'D', 'E', 'F' };
            int maxSira = (int)Math.Ceiling((double)kapasite / siraHarfleri.Length);

            for (int sira = 1; sira <= maxSira; sira++)
            {
                foreach (char harf in siraHarfleri)
                {
                    if (Koltuklar.Count >= kapasite)
                        break;

                    string koltukNo = $"{harf}{sira}";

                    // İlk 2 sıra Business, diğerleri Ekonomi
                    KoltukTip tip = sira <= 2 ? KoltukTip.Business : KoltukTip.Ekonomi;

                    Koltuklar.Add(koltukNo, new Koltuk
                    {
                        No = koltukNo,
                        Tip = tip,
                        EkFiyat = tip == KoltukTip.Business ? 500m : 0m // Örnek ek fiyat
                    });
                }
            }
        }

        // Koltuk ayırma
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

        // Koltuk iade etme
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
