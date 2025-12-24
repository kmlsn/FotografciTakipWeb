using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class MusteriMap : EntityTypeConfiguration<Musteri>
    {
        public MusteriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Sifre)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.AdiSoyadi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TCKimlikNo)
                .HasMaxLength(11);

            this.Property(t => t.SabitTel)
                .HasMaxLength(11);

            this.Property(t => t.CepTel)
                .HasMaxLength(11);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Adres)
                .HasMaxLength(250);

            this.Property(t => t.Notlar)
                .HasMaxLength(250);

            this.Property(t => t.FotografSecimDurum)
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("Musteri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.MusteriKodu).HasColumnName("MusteriKodu");
            this.Property(t => t.Sifre).HasColumnName("Sifre");
            this.Property(t => t.AdiSoyadi).HasColumnName("AdiSoyadi");
            this.Property(t => t.TCKimlikNo).HasColumnName("TCKimlikNo");
            this.Property(t => t.SabitTel).HasColumnName("SabitTel");
            this.Property(t => t.CepTel).HasColumnName("CepTel");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.SMSKabul).HasColumnName("SMSKabul");
            this.Property(t => t.EmailKabul).HasColumnName("EmailKabul");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.MusteriPanelGirisYetki).HasColumnName("MusteriPanelGirisYetki");
            this.Property(t => t.FotografSecimDurum).HasColumnName("FotografSecimDurum");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.Musteris)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.Musteris)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
