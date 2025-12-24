using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class MusteriMesajlariDetayMap : EntityTypeConfiguration<MusteriMesajlariDetay>
    {
        public MusteriMesajlariDetayMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Mesaj)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("MusteriMesajlariDetay");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.MusteriId).HasColumnName("MusteriId");
            this.Property(t => t.MesajId).HasColumnName("MesajId");
            this.Property(t => t.Mesaj).HasColumnName("Mesaj");
            this.Property(t => t.MusteriCevap).HasColumnName("MusteriCevap");
            this.Property(t => t.FirmaCevap).HasColumnName("FirmaCevap");
            this.Property(t => t.ResimYol).HasColumnName("ResimYol");
            this.Property(t => t.CevaplandiBit).HasColumnName("CevaplandiBit");
            this.Property(t => t.OkunduBit).HasColumnName("OkunduBit");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.CevaplayanKullaniciId).HasColumnName("CevaplayanKullaniciId");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.MusteriMesajlariDetays)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.MusteriMesajlari)
                .WithMany(t => t.MusteriMesajlariDetays)
                .HasForeignKey(d => d.MesajId);

        }
    }
}
