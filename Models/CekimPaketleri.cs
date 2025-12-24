using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class CekimPaketleri
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public string PaketAdi { get; set; }
        public string PaketDetayi { get; set; }
        public Nullable<decimal> Fiyat { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
