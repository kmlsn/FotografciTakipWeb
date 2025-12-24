using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class GunlukIslerMap : EntityTypeConfiguration<GunlukIsler>
    {
        public GunlukIslerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.AdSoyad)
                .HasMaxLength(50);

            this.Property(t => t.TCKimlikNo)
                .HasMaxLength(11);

            this.Property(t => t.SabitTel)
                .HasMaxLength(11);

            this.Property(t => t.CepTel)
                .HasMaxLength(11);

            this.Property(t => t.Adres)
                .HasMaxLength(250);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.OdemeTuru)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Notlar)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("GunlukIsler");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.TakipNo).HasColumnName("TakipNo");
            this.Property(t => t.Tarih).HasColumnName("Tarih");
            this.Property(t => t.KategoriId).HasColumnName("KategoriId");
            this.Property(t => t.MusteriId).HasColumnName("MusteriId");
            this.Property(t => t.AdSoyad).HasColumnName("AdSoyad");
            this.Property(t => t.TCKimlikNo).HasColumnName("TCKimlikNo");
            this.Property(t => t.SabitTel).HasColumnName("SabitTel");
            this.Property(t => t.CepTel).HasColumnName("CepTel");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.OdemeTuru).HasColumnName("OdemeTuru");
            this.Property(t => t.Adet).HasColumnName("Adet");
            this.Property(t => t.BirimUcret).HasColumnName("BirimUcret");
            this.Property(t => t.Ucret).HasColumnName("Ucret");
            this.Property(t => t.Odenen).HasColumnName("Odenen");
            this.Property(t => t.KalanBakiye).HasColumnName("KalanBakiye");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.Iptal).HasColumnName("Iptal");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.GunlukIslers)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.GunlukIsKategori)
                .WithMany(t => t.GunlukIslers)
                .HasForeignKey(d => d.KategoriId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.GunlukIslers)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
