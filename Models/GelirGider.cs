using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class GelirGider
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long SubeId { get; set; }
        public System.DateTime Tarih { get; set; }
        public Nullable<long> SozlesmeId { get; set; }
        public Nullable<long> GisId { get; set; }
        public Nullable<long> OdemeId { get; set; }
        public Nullable<long> CariHareketId { get; set; }
        public Nullable<long> PersonelOdemeId { get; set; }
        public string Tip { get; set; }
        public long GelirGiderTurId { get; set; }
        public decimal Tutar { get; set; }
        public string OdemeTuru { get; set; }
        public Nullable<long> MakbuzNo { get; set; }
        public string Notlar { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual GelirGiderTurleri GelirGiderTurleri { get; set; }
        public virtual Sube Sube { get; set; }
    }
}
