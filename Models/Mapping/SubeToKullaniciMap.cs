using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class SubeToKullaniciMap : EntityTypeConfiguration<SubeToKullanici>
    {
        public SubeToKullaniciMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SubeToKullanici");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.KullaniciId).HasColumnName("KullaniciId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");

            // Relationships
            this.HasRequired(t => t.Kullanici)
                .WithMany(t => t.SubeToKullanicis)
                .HasForeignKey(d => d.KullaniciId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.SubeToKullanicis)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
