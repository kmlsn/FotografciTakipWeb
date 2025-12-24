using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class AyarlarSmsGonderimMap : EntityTypeConfiguration<AyarlarSmsGonderim>
    {
        public AyarlarSmsGonderimMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("AyarlarSmsGonderim");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.RezervasyonTarihiBilgiMesaji).HasColumnName("RezervasyonTarihiBilgiMesaji");
            this.Property(t => t.RezervasyonTarihiBilgiGonderimSuresi).HasColumnName("RezervasyonTarihiBilgiGonderimSuresi");
            this.Property(t => t.RezervasyonTarihiHatirlatmaMesaji).HasColumnName("RezervasyonTarihiHatirlatmaMesaji");
            this.Property(t => t.RezervasyonTarihiHatirlatmaGonderimSuresi).HasColumnName("RezervasyonTarihiHatirlatmaGonderimSuresi");
            this.Property(t => t.OpsiyonTarihiBilgiMesaji).HasColumnName("OpsiyonTarihiBilgiMesaji");
            this.Property(t => t.OpsiyonTarihiBilgiGonderimSuresi).HasColumnName("OpsiyonTarihiBilgiGonderimSuresi");
            this.Property(t => t.OpsiyonTarihiHatirlatmaMesaji).HasColumnName("OpsiyonTarihiHatirlatmaMesaji");
            this.Property(t => t.OpsiyonTarihiHatirlatmaGonderimSuresi).HasColumnName("OpsiyonTarihiHatirlatmaGonderimSuresi");
            this.Property(t => t.MusteriCekimRandevusuBilgiMesaji).HasColumnName("MusteriCekimRandevusuBilgiMesaji");
            this.Property(t => t.MusteriCekimRandevusuBilgiGonderimSuresi).HasColumnName("MusteriCekimRandevusuBilgiGonderimSuresi");
            this.Property(t => t.MusteriCekimRandevusuHatirlatmaMesaji).HasColumnName("MusteriCekimRandevusuHatirlatmaMesaji");
            this.Property(t => t.MusteriCekimRandevusuHatirlatmaGonderimSuresi).HasColumnName("MusteriCekimRandevusuHatirlatmaGonderimSuresi");
            this.Property(t => t.PersonelRandevuBilgiMesaji).HasColumnName("PersonelRandevuBilgiMesaji");
            this.Property(t => t.PersonelRandevuBilgiGonderimSuresi).HasColumnName("PersonelRandevuBilgiGonderimSuresi");
            this.Property(t => t.PersonelRandevuHatirlatmaMesaji).HasColumnName("PersonelRandevuHatirlatmaMesaji");
            this.Property(t => t.PersonelRandevuHatirlatmaGonderimSuresi).HasColumnName("PersonelRandevuHatirlatmaGonderimSuresi");
            this.Property(t => t.MusteriOdemeBilgiMesaji).HasColumnName("MusteriOdemeBilgiMesaji");
            this.Property(t => t.MusteriOdemeBilgiGonderimSuresi).HasColumnName("MusteriOdemeBilgiGonderimSuresi");
            this.Property(t => t.MusteriOdemeHatirlatmaMesaji).HasColumnName("MusteriOdemeHatirlatmaMesaji");
            this.Property(t => t.MusteriOdemeHatirlatmaGonderimSuresi).HasColumnName("MusteriOdemeHatirlatmaGonderimSuresi");
            this.Property(t => t.FotografSecimiHatirlatmaMesaji).HasColumnName("FotografSecimiHatirlatmaMesaji");
            this.Property(t => t.FotografSecimiHatirlatmaGonderimSuresi).HasColumnName("FotografSecimiHatirlatmaGonderimSuresi");
            this.Property(t => t.FotografSecimiBilgiMesajiMusteri).HasColumnName("FotografSecimiBilgiMesajiMusteri");
            this.Property(t => t.FotografSecimiBilgiMesajiMusteriGonderimSuresi).HasColumnName("FotografSecimiBilgiMesajiMusteriGonderimSuresi");
            this.Property(t => t.FotografSecimiBilgiMesajiFirma).HasColumnName("FotografSecimiBilgiMesajiFirma");
            this.Property(t => t.FotografSecimiBilgiMesajiFirmaGonderimSuresi).HasColumnName("FotografSecimiBilgiMesajiFirmaGonderimSuresi");
            this.Property(t => t.EvlilikYildonumuTebrikMesaji).HasColumnName("EvlilikYildonumuTebrikMesaji");
            this.Property(t => t.EvlilikYildonumuTebrikGonderimSuresi).HasColumnName("EvlilikYildonumuTebrikGonderimSuresi");
            this.Property(t => t.CariyeYapilanOdemeBilgiMesaji).HasColumnName("CariyeYapilanOdemeBilgiMesaji");
            this.Property(t => t.CariyeYapilanOdemeBilgiGonderimSuresi).HasColumnName("CariyeYapilanOdemeBilgiGonderimSuresi");
            this.Property(t => t.CariAlacakHatirlatmaMesaji).HasColumnName("CariAlacakHatirlatmaMesaji");
            this.Property(t => t.CariAlacakHatirlatmaGonderimSuresi).HasColumnName("CariAlacakHatirlatmaGonderimSuresi");
            this.Property(t => t.CariTahsilatBilgiMesaji).HasColumnName("CariTahsilatBilgiMesaji");
            this.Property(t => t.CariTahsilatBilgiGonderimSuresi).HasColumnName("CariTahsilatBilgiGonderimSuresi");
            this.Property(t => t.GunlukIsOdemeBilgiMesaji).HasColumnName("GunlukIsOdemeBilgiMesaji");
            this.Property(t => t.GunlukIsOdemeBilgiGonderimSuresi).HasColumnName("GunlukIsOdemeBilgiGonderimSuresi");
            this.Property(t => t.SurecDegisiklikBilgiMesaji).HasColumnName("SurecDegisiklikBilgiMesaji");
            this.Property(t => t.SurecDegisiklikBilgiGonderimSuresi).HasColumnName("SurecDegisiklikBilgiGonderimSuresi");
            this.Property(t => t.SmsTurkceKarakter).HasColumnName("SmsTurkceKarakter");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.AyarlarSmsGonderims)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
