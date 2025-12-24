using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class DestekTalepleriDetayMap : EntityTypeConfiguration<DestekTalepleriDetay>
    {
        public DestekTalepleriDetayMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Mesaj)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("DestekTalepleriDetay");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.TalepId).HasColumnName("TalepId");
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
            this.HasRequired(t => t.DestekTalepleri)
                .WithMany(t => t.DestekTalepleriDetays)
                .HasForeignKey(d => d.TalepId);
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.DestekTalepleriDetays)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Kullanici)
                .WithMany(t => t.DestekTalepleriDetays)
                .HasForeignKey(d => d.CevaplayanKullaniciId);

        }
    }
}
