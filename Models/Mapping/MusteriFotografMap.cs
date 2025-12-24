using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class MusteriFotografMap : EntityTypeConfiguration<MusteriFotograf>
    {
        public MusteriFotografMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.FotografAdi)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.FotografYol)
                .IsRequired();

            this.Property(t => t.SecildiDurum)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("MusteriFotograf");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.MusteriId).HasColumnName("MusteriId");
            this.Property(t => t.SozlesmeId).HasColumnName("SozlesmeId");
            this.Property(t => t.FotografAdi).HasColumnName("FotografAdi");
            this.Property(t => t.FotografAciklama).HasColumnName("FotografAciklama");
            this.Property(t => t.FotografYol).HasColumnName("FotografYol");
            this.Property(t => t.SecildiDurum).HasColumnName("SecildiDurum");
            this.Property(t => t.KapakFotograf).HasColumnName("KapakFotograf");
            this.Property(t => t.PosterFotograf).HasColumnName("PosterFotograf");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.MusteriFotografs)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Musteri)
                .WithMany(t => t.MusteriFotografs)
                .HasForeignKey(d => d.MusteriId);
            this.HasRequired(t => t.Sozlesme)
                .WithMany(t => t.MusteriFotografs)
                .HasForeignKey(d => d.SozlesmeId);

        }
    }
}
