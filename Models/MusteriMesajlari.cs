using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class MusteriMesajlari
    {
        public MusteriMesajlari()
        {
            this.MusteriMesajlariDetays = new List<MusteriMesajlariDetay>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long MusteriId { get; set; }
        public string Konu { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Musteri Musteri { get; set; }
        public virtual ICollection<MusteriMesajlariDetay> MusteriMesajlariDetays { get; set; }
    }
}
