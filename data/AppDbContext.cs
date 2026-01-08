using Microsoft.EntityFrameworkCore;
using UcakBiletiRezervasyonSistemi.Models;

namespace UcakBiletiRezervasyonSistemi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Ucus> Ucuslar { get; set; }
        public DbSet<Rezervasyon> Rezervasyonlar { get; set; }
        public DbSet<Yolcu> Yolcular { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Kullanıcı tablosunda Admin/Müşteri ayrımı (TPH)
            modelBuilder.Entity<Kullanici>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Admin>("Admin")
                .HasValue<Musteri>("Musteri");

            // Tablo ve kolon isimlerini açıkça belirt
            modelBuilder.Entity<Ucus>(entity =>
            {
                entity.ToTable("Ucuslar");
                entity.Property(e => e.UcusNo).HasColumnName("UcusNo");
                entity.Property(e => e.KalkisYeri).HasColumnName("KalkisYeri");
                entity.Property(e => e.VarisYeri).HasColumnName("VarisYeri");
                entity.Property(e => e.KalkisTarihiSaati).HasColumnName("KalkisTarihiSaati");
                entity.Property(e => e.Kapasite).HasColumnName("Kapasite");
                entity.Property(e => e.BosKoltukSayisi).HasColumnName("BosKoltukSayisi");
                entity.Property(e => e.TemelFiyat).HasColumnName("TemelFiyat");
            });

            modelBuilder.Entity<Rezervasyon>(entity =>
            {
                entity.ToTable("Rezervasyonlar");
                entity.Property(e => e.RezervasyonKodu).HasColumnName("RezervasyonKodu");
                entity.Property(e => e.UcusNo).HasColumnName("UcusNo");
                entity.Property(e => e.MusteriTcNo).HasColumnName("MusteriTcNo");
                entity.Property(e => e.RezervasyonTarihi).HasColumnName("RezervasyonTarihi");
                entity.Property(e => e.ToplamFiyat).HasColumnName("ToplamFiyat");
                entity.Property(e => e.AktifMi).HasColumnName("AktifMi");
                entity.Property(e => e.Mesaj).HasColumnName("Mesaj");
            });

            modelBuilder.Entity<Yolcu>(entity =>
            {
                entity.ToTable("Yolcular");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.RezervasyonKodu).HasColumnName("RezervasyonKodu");
                entity.Property(e => e.Ad).HasColumnName("Ad");
                entity.Property(e => e.Soyad).HasColumnName("Soyad");
                entity.Property(e => e.TcNo).HasColumnName("TcNo");
                entity.Property(e => e.KoltukNo).HasColumnName("KoltukNo");
                entity.Property(e => e.KoltukTipi).HasColumnName("KoltukTipi");
                entity.Property(e => e.Fiyat).HasColumnName("Fiyat");
            });

            // Rezervasyon ve Yolcu arasındaki ilişki
            modelBuilder.Entity<Yolcu>()
                .HasOne<Rezervasyon>()
                .WithMany(r => r.Yolcular)
                .HasForeignKey(y => y.RezervasyonKodu)
                .HasPrincipalKey(r => r.RezervasyonKodu);

            base.OnModelCreating(modelBuilder);
        }
    }
}