using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class HataLoglariMap : EntityTypeConfiguration<HataLoglari>
    {
        public HataLoglariMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.HataMesajı)
                .IsRequired();

            this.Property(t => t.Islem)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("HataLoglari");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.HataMesajı).HasColumnName("HataMesajı");
            this.Property(t => t.Islem).HasColumnName("Islem");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.HataLoglaris)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.HataLoglaris)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
