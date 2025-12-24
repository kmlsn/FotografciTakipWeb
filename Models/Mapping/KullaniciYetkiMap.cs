using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class KullaniciYetkiMap : EntityTypeConfiguration<KullaniciYetki>
    {
        public KullaniciYetkiMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("KullaniciYetki");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.KullaniciId).HasColumnName("KullaniciId");
            this.Property(t => t.SayfaId).HasColumnName("SayfaId");
            this.Property(t => t.SayfaYetki).HasColumnName("SayfaYetki");
            this.Property(t => t.KayitDetayi).HasColumnName("KayitDetayi");
            this.Property(t => t.KayitEkle).HasColumnName("KayitEkle");
            this.Property(t => t.KayitDuzenle).HasColumnName("KayitDuzenle");
            this.Property(t => t.KayitSil).HasColumnName("KayitSil");
            this.Property(t => t.Yazdir).HasColumnName("Yazdir");
            this.Property(t => t.SmsGonder).HasColumnName("SmsGonder");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.KullaniciYetkis)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Kullanici)
                .WithMany(t => t.KullaniciYetkis)
                .HasForeignKey(d => d.KullaniciId);

        }
    }
}
