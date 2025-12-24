using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class FirmaMap : EntityTypeConfiguration<Firma>
    {
        public FirmaMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.FirmaAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Yetkili)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.VergiDairesi)
                .HasMaxLength(20);

            this.Property(t => t.VergiNo)
                .HasMaxLength(10);

            this.Property(t => t.TCKimlikNo)
                .HasMaxLength(11);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CepTel)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.SabitTel)
                .HasMaxLength(11);

            this.Property(t => t.Fax)
                .HasMaxLength(11);

            this.Property(t => t.Adres)
                .HasMaxLength(250);

            this.Property(t => t.WebSitesi)
                .HasMaxLength(250);

            this.Property(t => t.Facebook)
                .HasMaxLength(250);

            this.Property(t => t.Instagram)
                .HasMaxLength(250);

            this.Property(t => t.Twitter)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Firma");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaAdi).HasColumnName("FirmaAdi");
            this.Property(t => t.Yetkili).HasColumnName("Yetkili");
            this.Property(t => t.VergiDairesi).HasColumnName("VergiDairesi");
            this.Property(t => t.VergiNo).HasColumnName("VergiNo");
            this.Property(t => t.TCKimlikNo).HasColumnName("TCKimlikNo");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.CepTel).HasColumnName("CepTel");
            this.Property(t => t.SabitTel).HasColumnName("SabitTel");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.IlId).HasColumnName("IlId");
            this.Property(t => t.IlceId).HasColumnName("IlceId");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.WebSitesi).HasColumnName("WebSitesi");
            this.Property(t => t.Facebook).HasColumnName("Facebook");
            this.Property(t => t.Instagram).HasColumnName("Instagram");
            this.Property(t => t.Twitter).HasColumnName("Twitter");
            this.Property(t => t.FirmaHakkinda).HasColumnName("FirmaHakkinda");
            this.Property(t => t.ResimId).HasColumnName("ResimId");
            this.Property(t => t.AcilisBit).HasColumnName("AcilisBit");
            this.Property(t => t.DuyuruBit).HasColumnName("DuyuruBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasOptional(t => t.Il)
                .WithMany(t => t.Firmas)
                .HasForeignKey(d => d.IlId);
            this.HasOptional(t => t.Ilce)
                .WithMany(t => t.Firmas)
                .HasForeignKey(d => d.IlceId);
            this.HasOptional(t => t.Resim)
                .WithMany(t => t.Firmas)
                .HasForeignKey(d => d.ResimId);

        }
    }
}
