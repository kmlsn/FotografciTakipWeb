using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class OdemelerMap : EntityTypeConfiguration<Odemeler>
    {
        public OdemelerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.OdemeTuru)
                .HasMaxLength(20);

            this.Property(t => t.OdemeYapanAdSoyad)
                .HasMaxLength(50);

            this.Property(t => t.OdemeSekli)
                .HasMaxLength(20);

            this.Property(t => t.Notlar)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Odemeler");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.SozlesmeId).HasColumnName("SozlesmeId");
            this.Property(t => t.GisId).HasColumnName("GisId");
            this.Property(t => t.CariHareketId).HasColumnName("CariHareketId");
            this.Property(t => t.MusteriId).HasColumnName("MusteriId");
            this.Property(t => t.OdemeTuru).HasColumnName("OdemeTuru");
            this.Property(t => t.Tarih).HasColumnName("Tarih");
            this.Property(t => t.GelecekOdemeID).HasColumnName("GelecekOdemeID");
            this.Property(t => t.OdemeTarihi).HasColumnName("OdemeTarihi");
            this.Property(t => t.OdemeYapanAdSoyad).HasColumnName("OdemeYapanAdSoyad");
            this.Property(t => t.Tutar).HasColumnName("Tutar");
            this.Property(t => t.OdemeSekli).HasColumnName("OdemeSekli");
            this.Property(t => t.OdemeAl).HasColumnName("OdemeAl");
            this.Property(t => t.AlinanOdemeMakbuzNo).HasColumnName("AlinanOdemeMakbuzNo");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.Kapora).HasColumnName("Kapora");
            this.Property(t => t.Iptal).HasColumnName("Iptal");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasOptional(t => t.CariHareket)
                .WithMany(t => t.Odemelers)
                .HasForeignKey(d => d.CariHareketId);
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.Odemelers)
                .HasForeignKey(d => d.FirmaId);
            this.HasOptional(t => t.GunlukIsler)
                .WithMany(t => t.Odemelers)
                .HasForeignKey(d => d.GisId);
            this.HasOptional(t => t.Musteri)
                .WithMany(t => t.Odemelers)
                .HasForeignKey(d => d.MusteriId);
            this.HasOptional(t => t.Odemeler2)
                .WithMany(t => t.Odemeler1)
                .HasForeignKey(d => d.GelecekOdemeID);
            this.HasOptional(t => t.Sozlesme)
                .WithMany(t => t.Odemelers)
                .HasForeignKey(d => d.SozlesmeId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.Odemelers)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
