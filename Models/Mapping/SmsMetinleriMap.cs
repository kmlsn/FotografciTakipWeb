using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class SmsMetinleriMap : EntityTypeConfiguration<SmsMetinleri>
    {
        public SmsMetinleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.MetinAdi)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.SMSMetni)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SmsMetinleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.MetinAdi).HasColumnName("MetinAdi");
            this.Property(t => t.SMSMetni).HasColumnName("SMSMetni");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.SmsMetinleris)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
