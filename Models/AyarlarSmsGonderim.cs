using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class AyarlarSmsGonderim
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long RezervasyonTarihiBilgiMesaji { get; set; }
        public short RezervasyonTarihiBilgiGonderimSuresi { get; set; }
        public long RezervasyonTarihiHatirlatmaMesaji { get; set; }
        public short RezervasyonTarihiHatirlatmaGonderimSuresi { get; set; }
        public long OpsiyonTarihiBilgiMesaji { get; set; }
        public short OpsiyonTarihiBilgiGonderimSuresi { get; set; }
        public long OpsiyonTarihiHatirlatmaMesaji { get; set; }
        public short OpsiyonTarihiHatirlatmaGonderimSuresi { get; set; }
        public long MusteriCekimRandevusuBilgiMesaji { get; set; }
        public short MusteriCekimRandevusuBilgiGonderimSuresi { get; set; }
        public long MusteriCekimRandevusuHatirlatmaMesaji { get; set; }
        public short MusteriCekimRandevusuHatirlatmaGonderimSuresi { get; set; }
        public long PersonelRandevuBilgiMesaji { get; set; }
        public short PersonelRandevuBilgiGonderimSuresi { get; set; }
        public long PersonelRandevuHatirlatmaMesaji { get; set; }
        public short PersonelRandevuHatirlatmaGonderimSuresi { get; set; }
        public long MusteriOdemeBilgiMesaji { get; set; }
        public short MusteriOdemeBilgiGonderimSuresi { get; set; }
        public long MusteriOdemeHatirlatmaMesaji { get; set; }
        public short MusteriOdemeHatirlatmaGonderimSuresi { get; set; }
        public long FotografSecimiHatirlatmaMesaji { get; set; }
        public short FotografSecimiHatirlatmaGonderimSuresi { get; set; }
        public long FotografSecimiBilgiMesajiMusteri { get; set; }
        public short FotografSecimiBilgiMesajiMusteriGonderimSuresi { get; set; }
        public long FotografSecimiBilgiMesajiFirma { get; set; }
        public short FotografSecimiBilgiMesajiFirmaGonderimSuresi { get; set; }
        public long EvlilikYildonumuTebrikMesaji { get; set; }
        public short EvlilikYildonumuTebrikGonderimSuresi { get; set; }
        public long CariyeYapilanOdemeBilgiMesaji { get; set; }
        public short CariyeYapilanOdemeBilgiGonderimSuresi { get; set; }
        public long CariAlacakHatirlatmaMesaji { get; set; }
        public short CariAlacakHatirlatmaGonderimSuresi { get; set; }
        public long CariTahsilatBilgiMesaji { get; set; }
        public short CariTahsilatBilgiGonderimSuresi { get; set; }
        public long GunlukIsOdemeBilgiMesaji { get; set; }
        public short GunlukIsOdemeBilgiGonderimSuresi { get; set; }
        public long SurecDegisiklikBilgiMesaji { get; set; }
        public short SurecDegisiklikBilgiGonderimSuresi { get; set; }
        public bool SmsTurkceKarakter { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
