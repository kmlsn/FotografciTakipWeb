using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class AyarlarRezervasyonMap : EntityTypeConfiguration<AyarlarRezervasyon>
    {
        public AyarlarRezervasyonMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("AyarlarRezervasyon");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.PersonelIzinTakibi).HasColumnName("PersonelIzinTakibi");
            this.Property(t => t.TatilGunuTakibi).HasColumnName("TatilGunuTakibi");
            this.Property(t => t.PersonelGorevliTakibi).HasColumnName("PersonelGorevliTakibi");
            this.Property(t => t.GunuGecenTeklifOpsiyonIptal).HasColumnName("GunuGecenTeklifOpsiyonIptal");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.AyarlarRezervasyons)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
