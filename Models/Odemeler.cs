using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Odemeler
    {
        public Odemeler()
        {
            this.Odemeler1 = new List<Odemeler>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long SubeId { get; set; }
        public Nullable<long> SozlesmeId { get; set; }
        public Nullable<long> GisId { get; set; }
        public Nullable<long> CariHareketId { get; set; }
        public Nullable<long> MusteriId { get; set; }
        public string OdemeTuru { get; set; }
        public System.DateTime Tarih { get; set; }
        public Nullable<long> GelecekOdemeID { get; set; }
        public System.DateTime OdemeTarihi { get; set; }
        public string OdemeYapanAdSoyad { get; set; }
        public decimal Tutar { get; set; }
        public string OdemeSekli { get; set; }
        public bool OdemeAl { get; set; }
        public Nullable<long> AlinanOdemeMakbuzNo { get; set; }
        public string Notlar { get; set; }
        public Nullable<bool> Kapora { get; set; }
        public bool Iptal { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual CariHareket CariHareket { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual GunlukIsler GunlukIsler { get; set; }
        public virtual Musteri Musteri { get; set; }
        public virtual ICollection<Odemeler> Odemeler1 { get; set; }
        public virtual Odemeler Odemeler2 { get; set; }
        public virtual Sozlesme Sozlesme { get; set; }
        public virtual Sube Sube { get; set; }
    }
}
