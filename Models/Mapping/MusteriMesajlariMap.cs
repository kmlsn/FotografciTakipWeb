using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class MusteriMesajlariMap : EntityTypeConfiguration<MusteriMesajlari>
    {
        public MusteriMesajlariMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Konu)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("MusteriMesajlari");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.MusteriId).HasColumnName("MusteriId");
            this.Property(t => t.Konu).HasColumnName("Konu");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.MusteriMesajlaris)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Musteri)
                .WithMany(t => t.MusteriMesajlaris)
                .HasForeignKey(d => d.MusteriId);

        }
    }
}
