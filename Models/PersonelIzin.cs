using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class PersonelIzin
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long SubeId { get; set; }
        public long PersonelId { get; set; }
        public System.DateTime IzinBaslama { get; set; }
        public System.DateTime IzinBitis { get; set; }
        public System.DateTime IseBaslama { get; set; }
        public Nullable<int> KullanilanIzinGun { get; set; }
        public string Aciklama { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public string GorevliSubeler { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Personel Personel { get; set; }
        public virtual Sube Sube { get; set; }
    }
}
