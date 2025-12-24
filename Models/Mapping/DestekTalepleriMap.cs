using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class DestekTalepleriMap : EntityTypeConfiguration<DestekTalepleri>
    {
        public DestekTalepleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.TalepTuru)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.TalepBaslik)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Durum)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("DestekTalepleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.KullaniciId).HasColumnName("KullaniciId");
            this.Property(t => t.TalepTuru).HasColumnName("TalepTuru");
            this.Property(t => t.TalepBaslik).HasColumnName("TalepBaslik");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.TalepTarihi).HasColumnName("TalepTarihi");
            this.Property(t => t.CevapTarihi).HasColumnName("CevapTarihi");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.DestekTalepleris)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Kullanici)
                .WithMany(t => t.DestekTalepleris)
                .HasForeignKey(d => d.KullaniciId);

        }
    }
}
