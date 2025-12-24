using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Resim
    {
        public Resim()
        {
            this.Firmas = new List<Firma>();
            this.Kullanicis = new List<Kullanici>();
        }

        public long Id { get; set; }
        public string ResimAdres { get; set; }
        public long FirmaId { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual ICollection<Firma> Firmas { get; set; }
        public virtual ICollection<Kullanici> Kullanicis { get; set; }
    }
}
