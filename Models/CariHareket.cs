using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class CariHareket
    {
        public CariHareket()
        {
            this.Odemelers = new List<Odemeler>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long SubeId { get; set; }
        public long CariId { get; set; }
        public System.DateTime IslemTarihi { get; set; }
        public System.DateTime OdemeTarihi { get; set; }
        public string Tip { get; set; }
        public string OdemeTuru { get; set; }
        public string OdemeYapanAdSoyad { get; set; }
        public decimal Tutar { get; set; }
        public bool TahsilatOdemeBit { get; set; }
        public string Notlar { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Cari Cari { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Sube Sube { get; set; }
        public virtual ICollection<Odemeler> Odemelers { get; set; }
    }
}
