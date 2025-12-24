using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class RezervasyonTurleri
    {
        public RezervasyonTurleri()
        {
            this.Randevus = new List<Randevu>();
            this.Sozlesmes = new List<Sozlesme>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public string RandevuGorunum { get; set; }
        public Nullable<long> RandevuGorunumId { get; set; }
        public string RezervasyonTuru { get; set; }
        public string FormAlan { get; set; }
        public string Aciklama { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual ICollection<Randevu> Randevus { get; set; }
        public virtual RandevuGorunum RandevuGorunum1 { get; set; }
        public virtual ICollection<Sozlesme> Sozlesmes { get; set; }
    }
}
