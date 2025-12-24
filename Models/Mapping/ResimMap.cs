using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class ResimMap : EntityTypeConfiguration<Resim>
    {
        public ResimMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.ResimAdres)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Resim");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ResimAdres).HasColumnName("ResimAdres");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");
        }
    }
}
