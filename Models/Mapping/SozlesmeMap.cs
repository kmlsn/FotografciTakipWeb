using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class SozlesmeMap : EntityTypeConfiguration<Sozlesme>
    {
        public SozlesmeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.YetkiliPersonel)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.GorevliPersonellerId)
                .HasMaxLength(50);

            this.Property(t => t.YetkiliAdSoyad)
                .HasMaxLength(50);

            this.Property(t => t.YetkiliTel)
                .HasMaxLength(11);

            this.Property(t => t.YetkiliEmail)
                .HasMaxLength(50);

            this.Property(t => t.CocukAdSoyad)
                .HasMaxLength(50);

            this.Property(t => t.AnneAd)
                .HasMaxLength(50);

            this.Property(t => t.BabaAd)
                .HasMaxLength(50);

            this.Property(t => t.AnneTel)
                .HasMaxLength(11);

            this.Property(t => t.BabaTel)
                .HasMaxLength(11);

            this.Property(t => t.AnneEmail)
                .HasMaxLength(50);

            this.Property(t => t.BabaEmail)
                .HasMaxLength(50);

            this.Property(t => t.GelinAd)
                .HasMaxLength(50);

            this.Property(t => t.DamatAd)
                .HasMaxLength(50);

            this.Property(t => t.GelinTel)
                .HasMaxLength(11);

            this.Property(t => t.DamatTel)
                .HasMaxLength(11);

            this.Property(t => t.GelinEmail)
                .HasMaxLength(50);

            this.Property(t => t.DamatEmail)
                .HasMaxLength(50);

            this.Property(t => t.Durum)
                .HasMaxLength(25);

            this.Property(t => t.PaketlerId)
                .HasMaxLength(50);

            this.Property(t => t.EkHizmetlerId)
                .HasMaxLength(50);

            this.Property(t => t.SureclerId)
                .HasMaxLength(50);

            this.Property(t => t.Referans)
                .HasMaxLength(50);

            this.Property(t => t.OrganizasyonYeri)
                .HasMaxLength(250);

            this.Property(t => t.Notlar)
                .HasMaxLength(250);

            this.Property(t => t.FotografSecimDurum)
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("Sozlesme");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.YetkiliPersonel).HasColumnName("YetkiliPersonel");
            this.Property(t => t.GorevliPersonelId).HasColumnName("GorevliPersonelId");
            this.Property(t => t.GorevliPersonellerId).HasColumnName("GorevliPersonellerId");
            this.Property(t => t.SozlesmeNo).HasColumnName("SozlesmeNo");
            this.Property(t => t.SozlesmeTarihi).HasColumnName("SozlesmeTarihi");
            this.Property(t => t.RezervasyonTurId).HasColumnName("RezervasyonTurId");
            this.Property(t => t.RezervasyonTarihi).HasColumnName("RezervasyonTarihi");
            this.Property(t => t.BaslangicSaat).HasColumnName("BaslangicSaat");
            this.Property(t => t.BitisSaat).HasColumnName("BitisSaat");
            this.Property(t => t.MusteriId).HasColumnName("MusteriId");
            this.Property(t => t.YetkiliAdSoyad).HasColumnName("YetkiliAdSoyad");
            this.Property(t => t.YetkiliTel).HasColumnName("YetkiliTel");
            this.Property(t => t.YetkiliEmail).HasColumnName("YetkiliEmail");
            this.Property(t => t.Urunler).HasColumnName("Urunler");
            this.Property(t => t.Modeller).HasColumnName("Modeller");
            this.Property(t => t.CocukAdSoyad).HasColumnName("CocukAdSoyad");
            this.Property(t => t.AnneAd).HasColumnName("AnneAd");
            this.Property(t => t.BabaAd).HasColumnName("BabaAd");
            this.Property(t => t.AnneTel).HasColumnName("AnneTel");
            this.Property(t => t.BabaTel).HasColumnName("BabaTel");
            this.Property(t => t.AnneEmail).HasColumnName("AnneEmail");
            this.Property(t => t.BabaEmail).HasColumnName("BabaEmail");
            this.Property(t => t.GelinAd).HasColumnName("GelinAd");
            this.Property(t => t.DamatAd).HasColumnName("DamatAd");
            this.Property(t => t.GelinTel).HasColumnName("GelinTel");
            this.Property(t => t.DamatTel).HasColumnName("DamatTel");
            this.Property(t => t.GelinEmail).HasColumnName("GelinEmail");
            this.Property(t => t.DamatEmail).HasColumnName("DamatEmail");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.TeklifBit).HasColumnName("TeklifBit");
            this.Property(t => t.KesinRezervasyonBit).HasColumnName("KesinRezervasyonBit");
            this.Property(t => t.OpsiyonBit).HasColumnName("OpsiyonBit");
            this.Property(t => t.OpsiyonTarihi).HasColumnName("OpsiyonTarihi");
            this.Property(t => t.PaketlerId).HasColumnName("PaketlerId");
            this.Property(t => t.Paketler).HasColumnName("Paketler");
            this.Property(t => t.EkHizmetlerId).HasColumnName("EkHizmetlerId");
            this.Property(t => t.EkHizmetler).HasColumnName("EkHizmetler");
            this.Property(t => t.FotgrafHatirlatma).HasColumnName("FotgrafHatirlatma");
            this.Property(t => t.PaketlerFiyat).HasColumnName("PaketlerFiyat");
            this.Property(t => t.EkHizmetlerFiyat).HasColumnName("EkHizmetlerFiyat");
            this.Property(t => t.SureclerId).HasColumnName("SureclerId");
            this.Property(t => t.Surecler).HasColumnName("Surecler");
            this.Property(t => t.Iskonto).HasColumnName("Iskonto");
            this.Property(t => t.ToplamFiyat).HasColumnName("ToplamFiyat");
            this.Property(t => t.KDVDahil).HasColumnName("KDVDahil");
            this.Property(t => t.Referans).HasColumnName("Referans");
            this.Property(t => t.OrganizasyonYeri).HasColumnName("OrganizasyonYeri");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.FotografSecimDurum).HasColumnName("FotografSecimDurum");
            this.Property(t => t.FotografSecimDurumTarihi).HasColumnName("FotografSecimDurumTarihi");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.Bitti).HasColumnName("Bitti");
            this.Property(t => t.Iptal).HasColumnName("Iptal");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.Sozlesmes)
                .HasForeignKey(d => d.FirmaId);
            this.HasOptional(t => t.Musteri)
                .WithMany(t => t.Sozlesmes)
                .HasForeignKey(d => d.MusteriId);
            this.HasOptional(t => t.Personel)
                .WithMany(t => t.Sozlesmes)
                .HasForeignKey(d => d.GorevliPersonelId);
            this.HasRequired(t => t.RezervasyonTurleri)
                .WithMany(t => t.Sozlesmes)
                .HasForeignKey(d => d.RezervasyonTurId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.Sozlesmes)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
