using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class AyarlarSozlesmeCiktiMap : EntityTypeConfiguration<AyarlarSozlesmeCikti>
    {
        public AyarlarSozlesmeCiktiMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("AyarlarSozlesmeCikti");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.LogoGoster).HasColumnName("LogoGoster");
            this.Property(t => t.FirmaAdiGoster).HasColumnName("FirmaAdiGoster");
            this.Property(t => t.PaketlerGoster).HasColumnName("PaketlerGoster");
            this.Property(t => t.EkHizmetlerGoster).HasColumnName("EkHizmetlerGoster");
            this.Property(t => t.CekimRandevulariGoster).HasColumnName("CekimRandevulariGoster");
            this.Property(t => t.YapilanOdemelerGoster).HasColumnName("YapilanOdemelerGoster");
            this.Property(t => t.KalanOdemelerGoster).HasColumnName("KalanOdemelerGoster");
            this.Property(t => t.CepTelefonuGoster).HasColumnName("CepTelefonuGoster");
            this.Property(t => t.SabitTelefonGoster).HasColumnName("SabitTelefonGoster");
            this.Property(t => t.FaxGoster).HasColumnName("FaxGoster");
            this.Property(t => t.MusteriKoduSifreGoster).HasColumnName("MusteriKoduSifreGoster");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.AyarlarSozlesmeCiktis)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
