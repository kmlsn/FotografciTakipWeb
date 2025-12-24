using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class GirisLogMap : EntityTypeConfiguration<GirisLog>
    {
        public GirisLogMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.IpAdres)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Ulke)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Sehir)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("GirisLog");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.KullaniciId).HasColumnName("KullaniciId");
            this.Property(t => t.IpAdres).HasColumnName("IpAdres");
            this.Property(t => t.Ulke).HasColumnName("Ulke");
            this.Property(t => t.Sehir).HasColumnName("Sehir");
            this.Property(t => t.BaglantıZaman).HasColumnName("BaglantıZaman");

            // Relationships
            this.HasRequired(t => t.Kullanici)
                .WithMany(t => t.GirisLogs)
                .HasForeignKey(d => d.KullaniciId);

        }
    }
}
