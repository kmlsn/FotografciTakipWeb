using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class RandevuGorunum
    {
        public RandevuGorunum()
        {
            this.Randevus = new List<Randevu>();
            this.RezervasyonTurleris = new List<RezervasyonTurleri>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public string Gorunum { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual ICollection<Randevu> Randevus { get; set; }
        public virtual ICollection<RezervasyonTurleri> RezervasyonTurleris { get; set; }
    }
}
