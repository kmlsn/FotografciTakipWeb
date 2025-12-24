using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class AyarlarMailGonderim
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long RezervasyonTarihiBilgiMaili { get; set; }
        public short RezervasyonTarihiBilgiGonderimSuresi { get; set; }
        public long RezervasyonTarihiHatirlatmaMaili { get; set; }
        public short RezervasyonTarihiHatirlatmaGonderimSuresi { get; set; }
        public long OpsiyonTarihiBilgiMaili { get; set; }
        public short OpsiyonTarihiBilgiGonderimSuresi { get; set; }
        public long OpsiyonTarihiHatirlatmaMaili { get; set; }
        public short OpsiyonTarihiHatirlatmaGonderimSuresi { get; set; }
        public long MusteriCekimRandevusuBilgiMaili { get; set; }
        public short MusteriCekimRandevusuBilgiGonderimSuresi { get; set; }
        public long MusteriCekimRandevusuHatirlatmaMaili { get; set; }
        public short MusteriCekimRandevusuHatirlatmaGonderimSuresi { get; set; }
        public long PersonelRandevuBilgiMaili { get; set; }
        public short PersonelRandevuBilgiGonderimSuresi { get; set; }
        public long PersonelRandevuHatirlatmaMaili { get; set; }
        public short PersonelRandevuHatirlatmaGonderimSuresi { get; set; }
        public long MusteriOdemeBilgiMaili { get; set; }
        public short MusteriOdemeBilgiGonderimSuresi { get; set; }
        public long MusteriOdemeHatirlatmaMaili { get; set; }
        public short MusteriOdemeHatirlatmaGonderimSuresi { get; set; }
        public long FotografSecimiHatirlatmaMaili { get; set; }
        public short FotografSecimiHatirlatmaGonderimSuresi { get; set; }
        public long FotografSecimiBilgiMailiMusteri { get; set; }
        public short FotografSecimiBilgiMailiMusteriGonderimSuresi { get; set; }
        public long FotografSecimiBilgiMailiFirma { get; set; }
        public short FotografSecimiBilgiMailiFirmaGonderimSuresi { get; set; }
        public long EvlilikYildonumuTebrikMaili { get; set; }
        public short EvlilikYildonumuTebrikGonderimSuresi { get; set; }
        public long CariyeYapilanOdemeBilgiMaili { get; set; }
        public short CariyeYapilanOdemeBilgiGonderimSuresi { get; set; }
        public long CariAlacakHatirlatmaMaili { get; set; }
        public short CariAlacakHatirlatmaGonderimSuresi { get; set; }
        public long CariTahsilatBilgiMaili { get; set; }
        public short CariTahsilatBilgiGonderimSuresi { get; set; }
        public long GunlukIsOdemeBilgiMaili { get; set; }
        public short GunlukIsOdemeBilgiGonderimSuresi { get; set; }
        public long SurecDegisiklikBilgiMaili { get; set; }
        public short SurecDegisiklikBilgiGonderimSuresi { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
