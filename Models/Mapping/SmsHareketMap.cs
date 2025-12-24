using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class SmsHareketMap : EntityTypeConfiguration<SmsHareket>
    {
        public SmsHareketMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Açıklama)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("SmsHareket");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.YuklemeTarihi).HasColumnName("YuklemeTarihi");
            this.Property(t => t.Miktar).HasColumnName("Miktar");
            this.Property(t => t.Açıklama).HasColumnName("Açıklama");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.SmsHarekets)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
