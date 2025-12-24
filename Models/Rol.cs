using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Rol
    {
        public Rol()
        {
            this.Kullanicis = new List<Kullanici>();
            this.RolYetkis = new List<RolYetki>();
        }

        public long Id { get; set; }
        public string RolAdi { get; set; }
        public string Notlar { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual ICollection<Kullanici> Kullanicis { get; set; }
        public virtual ICollection<RolYetki> RolYetkis { get; set; }
    }
}
