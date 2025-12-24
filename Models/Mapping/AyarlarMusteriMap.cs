using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class AyarlarMusteriMap : EntityTypeConfiguration<AyarlarMusteri>
    {
        public AyarlarMusteriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("AyarlarMusteri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.OdemeleriGor).HasColumnName("OdemeleriGor");
            this.Property(t => t.RezervasyonGor).HasColumnName("RezervasyonGor");
            this.Property(t => t.TeklifleriGor).HasColumnName("TeklifleriGor");
            this.Property(t => t.SozlesmeYazdir).HasColumnName("SozlesmeYazdir");
            this.Property(t => t.OdemeMakbuzYazdir).HasColumnName("OdemeMakbuzYazdir");
            this.Property(t => t.MesajGonder).HasColumnName("MesajGonder");
            this.Property(t => t.FotografSecim).HasColumnName("FotografSecim");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.AyarlarMusteris)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
