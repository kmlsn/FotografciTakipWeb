using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class RandevuMap : EntityTypeConfiguration<Randevu>
    {
        public RandevuMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.GorevliPersonellerId)
                .HasMaxLength(50);

            this.Property(t => t.Baslik)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Randevu");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.SozlesmeId).HasColumnName("SozlesmeId");
            this.Property(t => t.GorevliPersonellerId).HasColumnName("GorevliPersonellerId");
            this.Property(t => t.RandevuGorunumId).HasColumnName("RandevuGorunumId");
            this.Property(t => t.RezervasyonTurId).HasColumnName("RezervasyonTurId");
            this.Property(t => t.CekimRandevu).HasColumnName("CekimRandevu");
            this.Property(t => t.Baslik).HasColumnName("Baslik");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.Baslangic).HasColumnName("Baslangic");
            this.Property(t => t.Bitis).HasColumnName("Bitis");
            this.Property(t => t.Opsiyon).HasColumnName("Opsiyon");
            this.Property(t => t.Iptal).HasColumnName("Iptal");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.Randevus)
                .HasForeignKey(d => d.FirmaId);
            this.HasOptional(t => t.RandevuGorunum)
                .WithMany(t => t.Randevus)
                .HasForeignKey(d => d.RandevuGorunumId);
            this.HasOptional(t => t.RezervasyonTurleri)
                .WithMany(t => t.Randevus)
                .HasForeignKey(d => d.RezervasyonTurId);
            this.HasOptional(t => t.Sozlesme)
                .WithMany(t => t.Randevus)
                .HasForeignKey(d => d.SozlesmeId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.Randevus)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
