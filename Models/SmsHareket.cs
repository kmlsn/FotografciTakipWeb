using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class SmsHareket
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public System.DateTime YuklemeTarihi { get; set; }
        public int Miktar { get; set; }
        public string Açıklama { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
