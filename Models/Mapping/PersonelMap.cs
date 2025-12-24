using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class PersonelMap : EntityTypeConfiguration<Personel>
    {
        public PersonelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.TCKimlikNo)
                .HasMaxLength(11);

            this.Property(t => t.AdiSoyadi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SabitTel)
                .HasMaxLength(11);

            this.Property(t => t.CepTel)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Adres)
                .HasMaxLength(250);

            this.Property(t => t.GorevliSubeler)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CalismaSekli)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Personel");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.TCKimlikNo).HasColumnName("TCKimlikNo");
            this.Property(t => t.AdiSoyadi).HasColumnName("AdiSoyadi");
            this.Property(t => t.BaslamaTarihi).HasColumnName("BaslamaTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
            this.Property(t => t.SabitTel).HasColumnName("SabitTel");
            this.Property(t => t.CepTel).HasColumnName("CepTel");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.GorevliSubeler).HasColumnName("GorevliSubeler");
            this.Property(t => t.GorevId).HasColumnName("GorevId");
            this.Property(t => t.YillikIzinHakki).HasColumnName("YillikIzinHakki");
            this.Property(t => t.ToplamIzin).HasColumnName("ToplamIzin");
            this.Property(t => t.KullanilanIzin).HasColumnName("KullanilanIzin");
            this.Property(t => t.KalanIzin).HasColumnName("KalanIzin");
            this.Property(t => t.CalismaSekli).HasColumnName("CalismaSekli");
            this.Property(t => t.Ucret).HasColumnName("Ucret");
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
                .WithMany(t => t.Personels)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.PersonelGorevleri)
                .WithMany(t => t.Personels)
                .HasForeignKey(d => d.GorevId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.Personels)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
