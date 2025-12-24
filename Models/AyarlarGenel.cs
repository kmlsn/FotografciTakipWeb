using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class AyarlarGenel
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public bool CalismaGunuCumartesi { get; set; }
        public bool CalismaGunuPazar { get; set; }
        public bool MusteriRehberKayit { get; set; }
        public bool CariRehberKayit { get; set; }
        public bool PersonelRehberKayit { get; set; }
        public bool AnneBabaRehberKayit { get; set; }
        public bool GelinDamatRehberKayit { get; set; }
        public bool RezervasyonYetkiliRehberKayit { get; set; }
        public bool KonturUyariVer { get; set; }
        public int KonturUyariMiktari { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
