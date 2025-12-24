using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class IlMap : EntityTypeConfiguration<Il>
    {
        public IlMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Il1)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Il");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Il1).HasColumnName("Il");
        }
    }
}
