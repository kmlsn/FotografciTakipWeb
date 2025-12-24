using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class KullaniciYetki
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long KullaniciId { get; set; }
        public long SayfaId { get; set; }
        public bool SayfaYetki { get; set; }
        public bool KayitDetayi { get; set; }
        public bool KayitEkle { get; set; }
        public bool KayitDuzenle { get; set; }
        public bool KayitSil { get; set; }
        public bool Yazdir { get; set; }
        public bool SmsGonder { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Kullanici Kullanici { get; set; }
    }
}
