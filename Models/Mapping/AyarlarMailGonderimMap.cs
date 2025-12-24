using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class AyarlarMailGonderimMap : EntityTypeConfiguration<AyarlarMailGonderim>
    {
        public AyarlarMailGonderimMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("AyarlarMailGonderim");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.RezervasyonTarihiBilgiMaili).HasColumnName("RezervasyonTarihiBilgiMaili");
            this.Property(t => t.RezervasyonTarihiBilgiGonderimSuresi).HasColumnName("RezervasyonTarihiBilgiGonderimSuresi");
            this.Property(t => t.RezervasyonTarihiHatirlatmaMaili).HasColumnName("RezervasyonTarihiHatirlatmaMaili");
            this.Property(t => t.RezervasyonTarihiHatirlatmaGonderimSuresi).HasColumnName("RezervasyonTarihiHatirlatmaGonderimSuresi");
            this.Property(t => t.OpsiyonTarihiBilgiMaili).HasColumnName("OpsiyonTarihiBilgiMaili");
            this.Property(t => t.OpsiyonTarihiBilgiGonderimSuresi).HasColumnName("OpsiyonTarihiBilgiGonderimSuresi");
            this.Property(t => t.OpsiyonTarihiHatirlatmaMaili).HasColumnName("OpsiyonTarihiHatirlatmaMaili");
            this.Property(t => t.OpsiyonTarihiHatirlatmaGonderimSuresi).HasColumnName("OpsiyonTarihiHatirlatmaGonderimSuresi");
            this.Property(t => t.MusteriCekimRandevusuBilgiMaili).HasColumnName("MusteriCekimRandevusuBilgiMaili");
            this.Property(t => t.MusteriCekimRandevusuBilgiGonderimSuresi).HasColumnName("MusteriCekimRandevusuBilgiGonderimSuresi");
            this.Property(t => t.MusteriCekimRandevusuHatirlatmaMaili).HasColumnName("MusteriCekimRandevusuHatirlatmaMaili");
            this.Property(t => t.MusteriCekimRandevusuHatirlatmaGonderimSuresi).HasColumnName("MusteriCekimRandevusuHatirlatmaGonderimSuresi");
            this.Property(t => t.PersonelRandevuBilgiMaili).HasColumnName("PersonelRandevuBilgiMaili");
            this.Property(t => t.PersonelRandevuBilgiGonderimSuresi).HasColumnName("PersonelRandevuBilgiGonderimSuresi");
            this.Property(t => t.PersonelRandevuHatirlatmaMaili).HasColumnName("PersonelRandevuHatirlatmaMaili");
            this.Property(t => t.PersonelRandevuHatirlatmaGonderimSuresi).HasColumnName("PersonelRandevuHatirlatmaGonderimSuresi");
            this.Property(t => t.MusteriOdemeBilgiMaili).HasColumnName("MusteriOdemeBilgiMaili");
            this.Property(t => t.MusteriOdemeBilgiGonderimSuresi).HasColumnName("MusteriOdemeBilgiGonderimSuresi");
            this.Property(t => t.MusteriOdemeHatirlatmaMaili).HasColumnName("MusteriOdemeHatirlatmaMaili");
            this.Property(t => t.MusteriOdemeHatirlatmaGonderimSuresi).HasColumnName("MusteriOdemeHatirlatmaGonderimSuresi");
            this.Property(t => t.FotografSecimiHatirlatmaMaili).HasColumnName("FotografSecimiHatirlatmaMaili");
            this.Property(t => t.FotografSecimiHatirlatmaGonderimSuresi).HasColumnName("FotografSecimiHatirlatmaGonderimSuresi");
            this.Property(t => t.FotografSecimiBilgiMailiMusteri).HasColumnName("FotografSecimiBilgiMailiMusteri");
            this.Property(t => t.FotografSecimiBilgiMailiMusteriGonderimSuresi).HasColumnName("FotografSecimiBilgiMailiMusteriGonderimSuresi");
            this.Property(t => t.FotografSecimiBilgiMailiFirma).HasColumnName("FotografSecimiBilgiMailiFirma");
            this.Property(t => t.FotografSecimiBilgiMailiFirmaGonderimSuresi).HasColumnName("FotografSecimiBilgiMailiFirmaGonderimSuresi");
            this.Property(t => t.EvlilikYildonumuTebrikMaili).HasColumnName("EvlilikYildonumuTebrikMaili");
            this.Property(t => t.EvlilikYildonumuTebrikGonderimSuresi).HasColumnName("EvlilikYildonumuTebrikGonderimSuresi");
            this.Property(t => t.CariyeYapilanOdemeBilgiMaili).HasColumnName("CariyeYapilanOdemeBilgiMaili");
            this.Property(t => t.CariyeYapilanOdemeBilgiGonderimSuresi).HasColumnName("CariyeYapilanOdemeBilgiGonderimSuresi");
            this.Property(t => t.CariAlacakHatirlatmaMaili).HasColumnName("CariAlacakHatirlatmaMaili");
            this.Property(t => t.CariAlacakHatirlatmaGonderimSuresi).HasColumnName("CariAlacakHatirlatmaGonderimSuresi");
            this.Property(t => t.CariTahsilatBilgiMaili).HasColumnName("CariTahsilatBilgiMaili");
            this.Property(t => t.CariTahsilatBilgiGonderimSuresi).HasColumnName("CariTahsilatBilgiGonderimSuresi");
            this.Property(t => t.GunlukIsOdemeBilgiMaili).HasColumnName("GunlukIsOdemeBilgiMaili");
            this.Property(t => t.GunlukIsOdemeBilgiGonderimSuresi).HasColumnName("GunlukIsOdemeBilgiGonderimSuresi");
            this.Property(t => t.SurecDegisiklikBilgiMaili).HasColumnName("SurecDegisiklikBilgiMaili");
            this.Property(t => t.SurecDegisiklikBilgiGonderimSuresi).HasColumnName("SurecDegisiklikBilgiGonderimSuresi");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.AyarlarMailGonderims)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
