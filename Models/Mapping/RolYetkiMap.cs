using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class RolYetkiMap : EntityTypeConfiguration<RolYetki>
    {
        public RolYetkiMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RolYetki");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.RolId).HasColumnName("RolId");
            this.Property(t => t.SayfaId).HasColumnName("SayfaId");
            this.Property(t => t.SayfaYetki).HasColumnName("SayfaYetki");
            this.Property(t => t.KayitDetayi).HasColumnName("KayitDetayi");
            this.Property(t => t.KayitEkle).HasColumnName("KayitEkle");
            this.Property(t => t.KayitDuzenle).HasColumnName("KayitDuzenle");
            this.Property(t => t.KayitSil).HasColumnName("KayitSil");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.RolYetkis)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Rol)
                .WithMany(t => t.RolYetkis)
                .HasForeignKey(d => d.RolId);

        }
    }
}
