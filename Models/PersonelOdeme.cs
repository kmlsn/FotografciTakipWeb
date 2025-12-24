using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class PersonelOdeme
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long SubeId { get; set; }
        public long PersonelId { get; set; }
        public System.DateTime OdemeTarihi { get; set; }
        public string OdemeTuru { get; set; }
        public string OdemeSekli { get; set; }
        public decimal Tutar { get; set; }
        public string Aciklama { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Personel Personel { get; set; }
        public virtual Sube Sube { get; set; }
    }
}
