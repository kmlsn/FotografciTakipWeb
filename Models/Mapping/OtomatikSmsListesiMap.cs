using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class OtomatikSmsListesiMap : EntityTypeConfiguration<OtomatikSmsListesi>
    {
        public OtomatikSmsListesiMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.AliciAdSoyad)
                .HasMaxLength(50);

            this.Property(t => t.AliciTel)
                .HasMaxLength(11);

            // Table & Column Mappings
            this.ToTable("OtomatikSmsListesi");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.GonderimTarihi).HasColumnName("GonderimTarihi");
            this.Property(t => t.SmsMetni).HasColumnName("SmsMetni");
            this.Property(t => t.AliciAdSoyad).HasColumnName("AliciAdSoyad");
            this.Property(t => t.AliciTel).HasColumnName("AliciTel");

            // Relationships
            this.HasOptional(t => t.Firma)
                .WithMany(t => t.OtomatikSmsListesis)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
