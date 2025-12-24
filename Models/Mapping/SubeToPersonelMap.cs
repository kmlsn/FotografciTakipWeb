using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class SubeToPersonelMap : EntityTypeConfiguration<SubeToPersonel>
    {
        public SubeToPersonelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SubeToPersonel");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.PersonelId).HasColumnName("PersonelId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");

            // Relationships
            this.HasRequired(t => t.Personel)
                .WithMany(t => t.SubeToPersonels)
                .HasForeignKey(d => d.PersonelId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.SubeToPersonels)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
