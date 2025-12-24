using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class KullaniciMap : EntityTypeConfiguration<Kullanici>
    {
        public KullaniciMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CepTel)
                .HasMaxLength(11);

            this.Property(t => t.SifreHash)
                .IsRequired();

            this.Property(t => t.GeciciSifre)
                .HasMaxLength(16);

            this.Property(t => t.AdSoyad)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.Notlar)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.YetkiliSubeler)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Kullanici");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RolId).HasColumnName("RolId");
            this.Property(t => t.GorevId).HasColumnName("GorevId");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.CepTel).HasColumnName("CepTel");
            this.Property(t => t.SifreHash).HasColumnName("SifreHash");
            this.Property(t => t.GeciciSifre).HasColumnName("GeciciSifre");
            this.Property(t => t.AdSoyad).HasColumnName("AdSoyad");
            this.Property(t => t.ResimId).HasColumnName("ResimId");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.YetkiliSubeler).HasColumnName("YetkiliSubeler");
            this.Property(t => t.DuyuruBit).HasColumnName("DuyuruBit");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.Kullanicis)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.PersonelGorevleri)
                .WithMany(t => t.Kullanicis)
                .HasForeignKey(d => d.GorevId);
            this.HasOptional(t => t.Resim)
                .WithMany(t => t.Kullanicis)
                .HasForeignKey(d => d.ResimId);
            this.HasRequired(t => t.Rol)
                .WithMany(t => t.Kullanicis)
                .HasForeignKey(d => d.RolId);

        }
    }
}
