using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Randevu
    {
        public Randevu()
        {
            this.RandevuToPersonels = new List<RandevuToPersonel>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long SubeId { get; set; }
        public Nullable<long> SozlesmeId { get; set; }
        public string GorevliPersonellerId { get; set; }
        public Nullable<long> RandevuGorunumId { get; set; }
        public Nullable<long> RezervasyonTurId { get; set; }
        public Nullable<bool> CekimRandevu { get; set; }
        public string Baslik { get; set; }
        public string Aciklama { get; set; }
        public System.DateTime Baslangic { get; set; }
        public System.DateTime Bitis { get; set; }
        public Nullable<System.DateTime> Opsiyon { get; set; }
        public bool Iptal { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual RandevuGorunum RandevuGorunum { get; set; }
        public virtual RezervasyonTurleri RezervasyonTurleri { get; set; }
        public virtual Sozlesme Sozlesme { get; set; }
        public virtual Sube Sube { get; set; }
        public virtual ICollection<RandevuToPersonel> RandevuToPersonels { get; set; }
    }
}
