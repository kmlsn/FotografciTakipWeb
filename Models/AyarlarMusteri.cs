using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class AyarlarMusteri
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public bool OdemeleriGor { get; set; }
        public bool RezervasyonGor { get; set; }
        public bool TeklifleriGor { get; set; }
        public bool SozlesmeYazdir { get; set; }
        public bool OdemeMakbuzYazdir { get; set; }
        public bool MesajGonder { get; set; }
        public bool FotografSecim { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
