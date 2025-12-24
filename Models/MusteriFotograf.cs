using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class MusteriFotograf
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long MusteriId { get; set; }
        public long SozlesmeId { get; set; }
        public string FotografAdi { get; set; }
        public string FotografAciklama { get; set; }
        public string FotografYol { get; set; }
        public string SecildiDurum { get; set; }
        public bool KapakFotograf { get; set; }
        public bool PosterFotograf { get; set; }
        public string Notlar { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Musteri Musteri { get; set; }
        public virtual Sozlesme Sozlesme { get; set; }
    }
}
