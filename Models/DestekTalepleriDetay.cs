using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class DestekTalepleriDetay
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long TalepId { get; set; }
        public string Mesaj { get; set; }
        public bool MusteriCevap { get; set; }
        public bool FirmaCevap { get; set; }
        public string ResimYol { get; set; }
        public bool CevaplandiBit { get; set; }
        public bool OkunduBit { get; set; }
        public bool KilitBit { get; set; }
        public long CevaplayanKullaniciId { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual DestekTalepleri DestekTalepleri { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Kullanici Kullanici { get; set; }
    }
}
