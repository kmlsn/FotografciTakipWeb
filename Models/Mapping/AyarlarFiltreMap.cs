using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class AyarlarFiltreMap : EntityTypeConfiguration<AyarlarFiltre>
    {
        public AyarlarFiltreMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.GunlukIsler)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.RezervasyonListesi)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.RezervasyonTeklifleri)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Randevular)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.GelirlerGiderler)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.AlinanGelecekOdemeler)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.CariHesapTakibi)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Kasa)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PersonelIsTakibi)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PersonelIzinTakibi)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PersonelOdemeleri)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.MusteriHesapTakibi)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Siparisler)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("AyarlarFiltre");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.GunlukIsler).HasColumnName("GunlukIsler");
            this.Property(t => t.RezervasyonListesi).HasColumnName("RezervasyonListesi");
            this.Property(t => t.RezervasyonTeklifleri).HasColumnName("RezervasyonTeklifleri");
            this.Property(t => t.Randevular).HasColumnName("Randevular");
            this.Property(t => t.GelirlerGiderler).HasColumnName("GelirlerGiderler");
            this.Property(t => t.AlinanGelecekOdemeler).HasColumnName("AlinanGelecekOdemeler");
            this.Property(t => t.CariHesapTakibi).HasColumnName("CariHesapTakibi");
            this.Property(t => t.Kasa).HasColumnName("Kasa");
            this.Property(t => t.PersonelIsTakibi).HasColumnName("PersonelIsTakibi");
            this.Property(t => t.PersonelIzinTakibi).HasColumnName("PersonelIzinTakibi");
            this.Property(t => t.PersonelOdemeleri).HasColumnName("PersonelOdemeleri");
            this.Property(t => t.MusteriHesapTakibi).HasColumnName("MusteriHesapTakibi");
            this.Property(t => t.Siparisler).HasColumnName("Siparisler");
            this.Property(t => t.PersonelListesiPasifGizle).HasColumnName("PersonelListesiPasifGizle");
            this.Property(t => t.MusteriListesiPasifGizle).HasColumnName("MusteriListesiPasifGizle");
            this.Property(t => t.CariListesiPasifGizle).HasColumnName("CariListesiPasifGizle");
            this.Property(t => t.KullaniciListesiPasifGizle).HasColumnName("KullaniciListesiPasifGizle");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.AyarlarFiltres)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
