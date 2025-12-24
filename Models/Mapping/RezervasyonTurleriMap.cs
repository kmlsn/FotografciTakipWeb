using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class RezervasyonTurleriMap : EntityTypeConfiguration<RezervasyonTurleri>
    {
        public RezervasyonTurleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.RandevuGorunum)
                .HasMaxLength(100);

            this.Property(t => t.RezervasyonTuru)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.FormAlan)
                .HasMaxLength(50);

            this.Property(t => t.Aciklama)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("RezervasyonTurleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.RandevuGorunum).HasColumnName("RandevuGorunum");
            this.Property(t => t.RandevuGorunumId).HasColumnName("RandevuGorunumId");
            this.Property(t => t.RezervasyonTuru).HasColumnName("RezervasyonTuru");
            this.Property(t => t.FormAlan).HasColumnName("FormAlan");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.RezervasyonTurleris)
                .HasForeignKey(d => d.FirmaId);
            this.HasOptional(t => t.RandevuGorunum1)
                .WithMany(t => t.RezervasyonTurleris)
                .HasForeignKey(d => d.RandevuGorunumId);

        }
    }
}
