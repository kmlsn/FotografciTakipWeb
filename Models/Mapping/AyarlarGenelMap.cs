using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class AyarlarGenelMap : EntityTypeConfiguration<AyarlarGenel>
    {
        public AyarlarGenelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("AyarlarGenel");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.CalismaGunuCumartesi).HasColumnName("CalismaGunuCumartesi");
            this.Property(t => t.CalismaGunuPazar).HasColumnName("CalismaGunuPazar");
            this.Property(t => t.MusteriRehberKayit).HasColumnName("MusteriRehberKayit");
            this.Property(t => t.CariRehberKayit).HasColumnName("CariRehberKayit");
            this.Property(t => t.PersonelRehberKayit).HasColumnName("PersonelRehberKayit");
            this.Property(t => t.AnneBabaRehberKayit).HasColumnName("AnneBabaRehberKayit");
            this.Property(t => t.GelinDamatRehberKayit).HasColumnName("GelinDamatRehberKayit");
            this.Property(t => t.RezervasyonYetkiliRehberKayit).HasColumnName("RezervasyonYetkiliRehberKayit");
            this.Property(t => t.KonturUyariVer).HasColumnName("KonturUyariVer");
            this.Property(t => t.KonturUyariMiktari).HasColumnName("KonturUyariMiktari");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.AyarlarGenels)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
