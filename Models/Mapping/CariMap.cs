using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class CariMap : EntityTypeConfiguration<Cari>
    {
        public CariMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.FirmaAdi)
                .HasMaxLength(100);

            this.Property(t => t.Yetkili)
                .HasMaxLength(50);

            this.Property(t => t.AdSoyad)
                .HasMaxLength(50);

            this.Property(t => t.SabitTel)
                .HasMaxLength(11);

            this.Property(t => t.CepTel)
                .HasMaxLength(11);

            this.Property(t => t.Fax)
                .HasMaxLength(11);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.VergiDairesi)
                .HasMaxLength(50);

            this.Property(t => t.VergiNo)
                .HasMaxLength(10);

            this.Property(t => t.TCKimlikNo)
                .HasMaxLength(11);

            this.Property(t => t.Adres)
                .HasMaxLength(250);

            this.Property(t => t.Notlar)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Cari");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.FirmaAdi).HasColumnName("FirmaAdi");
            this.Property(t => t.Yetkili).HasColumnName("Yetkili");
            this.Property(t => t.AdSoyad).HasColumnName("AdSoyad");
            this.Property(t => t.SabitTel).HasColumnName("SabitTel");
            this.Property(t => t.CepTel).HasColumnName("CepTel");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.VergiDairesi).HasColumnName("VergiDairesi");
            this.Property(t => t.VergiNo).HasColumnName("VergiNo");
            this.Property(t => t.TCKimlikNo).HasColumnName("TCKimlikNo");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.SMSKabul).HasColumnName("SMSKabul");
            this.Property(t => t.EmailKabul).HasColumnName("EmailKabul");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.Caris)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.Caris)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
