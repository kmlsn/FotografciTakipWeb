using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class AyarlarRezervasyon
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public bool PersonelIzinTakibi { get; set; }
        public bool TatilGunuTakibi { get; set; }
        public bool PersonelGorevliTakibi { get; set; }
        public bool GunuGecenTeklifOpsiyonIptal { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
