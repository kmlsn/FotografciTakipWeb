using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class TatilGunleri
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public System.DateTime Baslangic { get; set; }
        public System.DateTime Bitis { get; set; }
        public bool Calisilacak { get; set; }
        public bool IzindenDus { get; set; }
        public string Aciklama { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
