using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class PersonelGorevleri
    {
        public PersonelGorevleri()
        {
            this.Kullanicis = new List<Kullanici>();
            this.Personels = new List<Personel>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public string Gorev { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual ICollection<Kullanici> Kullanicis { get; set; }
        public virtual ICollection<Personel> Personels { get; set; }
    }
}
