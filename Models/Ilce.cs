using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Ilce
    {
        public Ilce()
        {
            this.Firmas = new List<Firma>();
            this.Subes = new List<Sube>();
        }

        public int Id { get; set; }
        public string Ilce1 { get; set; }
        public int IlId { get; set; }
        public virtual ICollection<Firma> Firmas { get; set; }
        public virtual Il Il { get; set; }
        public virtual ICollection<Sube> Subes { get; set; }
    }
}
