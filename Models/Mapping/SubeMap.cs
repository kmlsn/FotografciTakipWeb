using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class SubeMap : EntityTypeConfiguration<Sube>
    {
        public SubeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SubeAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Yetkili)
                .IsRequired()
                .HasMaxLength(50);

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
                .HasMaxLength(50);

            this.Property(t => t.Facebook)
                .HasMaxLength(250);

            this.Property(t => t.Instagram)
                .HasMaxLength(250);

            this.Property(t => t.Twitter)
                .HasMaxLength(250);

            this.Property(t => t.Notlar)
                .HasMaxLength(250);

            this.Property(t => t.GorevliPersoneller)
                .HasMaxLength(50);

            this.Property(t => t.YetkiliKullanicilar)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Sube");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeAdi).HasColumnName("SubeAdi");
            this.Property(t => t.Yetkili).HasColumnName("Yetkili");
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
            this.Property(t => t.SubeHakkinda).HasColumnName("SubeHakkinda");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.GorevliPersoneller).HasColumnName("GorevliPersoneller");
            this.Property(t => t.YetkiliKullanicilar).HasColumnName("YetkiliKullanicilar");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.Subes)
                .HasForeignKey(d => d.FirmaId);
            this.HasOptional(t => t.Il)
                .WithMany(t => t.Subes)
                .HasForeignKey(d => d.IlId);
            this.HasOptional(t => t.Ilce)
                .WithMany(t => t.Subes)
                .HasForeignKey(d => d.IlceId);

        }
    }
}
