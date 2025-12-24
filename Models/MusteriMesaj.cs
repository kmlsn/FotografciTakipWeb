using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class MusteriMesaj
    {
        public MusteriMesaj()
        {
            this.MusteriMesaj1 = new List<MusteriMesaj>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public Nullable<long> MesajId { get; set; }
        public long MusteriId { get; set; }
        public string Konu { get; set; }
        public string Mesaj { get; set; }
        public System.DateTime MesajTarihi { get; set; }
        public Nullable<System.DateTime> CevapTarihi { get; set; }
        public string Durum { get; set; }
        public bool FirmaCevapBit { get; set; }
        public bool CevaplaBit { get; set; }
        public bool OkunduBit { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Musteri Musteri { get; set; }
        public virtual ICollection<MusteriMesaj> MusteriMesaj1 { get; set; }
        public virtual MusteriMesaj MusteriMesaj2 { get; set; }
    }
}
