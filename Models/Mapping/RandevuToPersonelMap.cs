using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class RandevuToPersonelMap : EntityTypeConfiguration<RandevuToPersonel>
    {
        public RandevuToPersonelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("RandevuToPersonel");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RandevuId).HasColumnName("RandevuId");
            this.Property(t => t.PersonelId).HasColumnName("PersonelId");
            this.Property(t => t.Iptal).HasColumnName("Iptal");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Personel)
                .WithMany(t => t.RandevuToPersonels)
                .HasForeignKey(d => d.PersonelId);
            this.HasRequired(t => t.Randevu)
                .WithMany(t => t.RandevuToPersonels)
                .HasForeignKey(d => d.RandevuId);

        }
    }
}
