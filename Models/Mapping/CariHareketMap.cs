using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class CariHareketMap : EntityTypeConfiguration<CariHareket>
    {
        public CariHareketMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Tip)
                .IsRequired()
                .HasMaxLength(6);

            this.Property(t => t.OdemeTuru)
                .HasMaxLength(20);

            this.Property(t => t.OdemeYapanAdSoyad)
                .HasMaxLength(50);

            this.Property(t => t.Notlar)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("CariHareket");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.CariId).HasColumnName("CariId");
            this.Property(t => t.IslemTarihi).HasColumnName("IslemTarihi");
            this.Property(t => t.OdemeTarihi).HasColumnName("OdemeTarihi");
            this.Property(t => t.Tip).HasColumnName("Tip");
            this.Property(t => t.OdemeTuru).HasColumnName("OdemeTuru");
            this.Property(t => t.OdemeYapanAdSoyad).HasColumnName("OdemeYapanAdSoyad");
            this.Property(t => t.Tutar).HasColumnName("Tutar");
            this.Property(t => t.TahsilatOdemeBit).HasColumnName("TahsilatOdemeBit");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Cari)
                .WithMany(t => t.CariHarekets)
                .HasForeignKey(d => d.CariId);
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.CariHarekets)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.CariHarekets)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
