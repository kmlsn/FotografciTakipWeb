using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class GelirGiderMap : EntityTypeConfiguration<GelirGider>
    {
        public GelirGiderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Tip)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.OdemeTuru)
                .HasMaxLength(20);

            this.Property(t => t.Notlar)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("GelirGider");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.Tarih).HasColumnName("Tarih");
            this.Property(t => t.SozlesmeId).HasColumnName("SozlesmeId");
            this.Property(t => t.GisId).HasColumnName("GisId");
            this.Property(t => t.OdemeId).HasColumnName("OdemeId");
            this.Property(t => t.CariHareketId).HasColumnName("CariHareketId");
            this.Property(t => t.PersonelOdemeId).HasColumnName("PersonelOdemeId");
            this.Property(t => t.Tip).HasColumnName("Tip");
            this.Property(t => t.GelirGiderTurId).HasColumnName("GelirGiderTurId");
            this.Property(t => t.Tutar).HasColumnName("Tutar");
            this.Property(t => t.OdemeTuru).HasColumnName("OdemeTuru");
            this.Property(t => t.MakbuzNo).HasColumnName("MakbuzNo");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.GelirGiders)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.GelirGiderTurleri)
                .WithMany(t => t.GelirGiders)
                .HasForeignKey(d => d.GelirGiderTurId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.GelirGiders)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
