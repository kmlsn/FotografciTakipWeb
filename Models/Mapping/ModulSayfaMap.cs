using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class ModulSayfaMap : EntityTypeConfiguration<ModulSayfa>
    {
        public ModulSayfaMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SayfaAdi)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("ModulSayfa");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ModulId).HasColumnName("ModulId");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.Sira).HasColumnName("Sira");
            this.Property(t => t.SayfaAdi).HasColumnName("SayfaAdi");
            this.Property(t => t.SayfaYetkiAktif).HasColumnName("SayfaYetkiAktif");
            this.Property(t => t.KayitDetayiAktif).HasColumnName("KayitDetayiAktif");
            this.Property(t => t.KayitEkleAktif).HasColumnName("KayitEkleAktif");
            this.Property(t => t.KayitDuzenleAktif).HasColumnName("KayitDuzenleAktif");
            this.Property(t => t.KayitSilAktif).HasColumnName("KayitSilAktif");
            this.Property(t => t.Yazdirma).HasColumnName("Yazdirma");
            this.Property(t => t.SMSGonder).HasColumnName("SMSGonder");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.ModulSayfas)
                .HasForeignKey(d => d.FirmaId);
            this.HasOptional(t => t.ModulSayfa2)
                .WithMany(t => t.ModulSayfa1)
                .HasForeignKey(d => d.ModulId);

        }
    }
}
