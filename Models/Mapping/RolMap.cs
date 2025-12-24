using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class RolMap : EntityTypeConfiguration<Rol>
    {
        public RolMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.RolAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Notlar)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Rol");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RolAdi).HasColumnName("RolAdi");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");
        }
    }
}
