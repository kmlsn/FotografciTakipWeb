using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class IlceMap : EntityTypeConfiguration<Ilce>
    {
        public IlceMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Ilce1)
                .HasMaxLength(255);

            this.Property(t => t.IlId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            this.ToTable("Ilce");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Ilce1).HasColumnName("Ilce");
            this.Property(t => t.IlId).HasColumnName("IlId");

            // Relationships
            this.HasRequired(t => t.Il)
                .WithMany(t => t.Ilces)
                .HasForeignKey(d => d.IlId);

        }
    }
}
