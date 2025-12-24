using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class TatilGunleriMap : EntityTypeConfiguration<TatilGunleri>
    {
        public TatilGunleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Aciklama)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("TatilGunleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.Baslangic).HasColumnName("Baslangic");
            this.Property(t => t.Bitis).HasColumnName("Bitis");
            this.Property(t => t.Calisilacak).HasColumnName("Calisilacak");
            this.Property(t => t.IzindenDus).HasColumnName("IzindenDus");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.TatilGunleris)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
