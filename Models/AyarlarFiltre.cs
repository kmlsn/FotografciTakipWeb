using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class AyarlarFiltre
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public string GunlukIsler { get; set; }
        public string RezervasyonListesi { get; set; }
        public string RezervasyonTeklifleri { get; set; }
        public string Randevular { get; set; }
        public string GelirlerGiderler { get; set; }
        public string AlinanGelecekOdemeler { get; set; }
        public string CariHesapTakibi { get; set; }
        public string Kasa { get; set; }
        public string PersonelIsTakibi { get; set; }
        public string PersonelIzinTakibi { get; set; }
        public string PersonelOdemeleri { get; set; }
        public string MusteriHesapTakibi { get; set; }
        public string Siparisler { get; set; }
        public bool PersonelListesiPasifGizle { get; set; }
        public bool MusteriListesiPasifGizle { get; set; }
        public bool CariListesiPasifGizle { get; set; }
        public bool KullaniciListesiPasifGizle { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
