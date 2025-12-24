using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class AyarlarMailHesapMap : EntityTypeConfiguration<AyarlarMailHesap>
    {
        public AyarlarMailHesapMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.GonderenMail)
                .HasMaxLength(100);

            this.Property(t => t.GonderenSifre)
                .IsRequired();

            this.Property(t => t.GonderenAdSoyad)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SmtpAdres)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SmtpPort)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Aciklama)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("AyarlarMailHesap");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.GonderenMail).HasColumnName("GonderenMail");
            this.Property(t => t.GonderenSifre).HasColumnName("GonderenSifre");
            this.Property(t => t.GonderenAdSoyad).HasColumnName("GonderenAdSoyad");
            this.Property(t => t.SmtpAdres).HasColumnName("SmtpAdres");
            this.Property(t => t.SmtpPort).HasColumnName("SmtpPort");
            this.Property(t => t.Ssl).HasColumnName("Ssl");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.AyarlarMailHesaps)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
