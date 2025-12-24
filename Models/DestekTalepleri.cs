using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class DestekTalepleri
    {
        public DestekTalepleri()
        {
            this.DestekTalepleriDetays = new List<DestekTalepleriDetay>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long KullaniciId { get; set; }
        public string TalepTuru { get; set; }
        public string TalepBaslik { get; set; }
        public string Durum { get; set; }
        public bool KilitBit { get; set; }
        public System.DateTime TalepTarihi { get; set; }
        public System.DateTime CevapTarihi { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Kullanici Kullanici { get; set; }
        public virtual ICollection<DestekTalepleriDetay> DestekTalepleriDetays { get; set; }
    }
}
