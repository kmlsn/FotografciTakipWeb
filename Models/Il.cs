using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Il
    {
        public Il()
        {
            this.Firmas = new List<Firma>();
            this.Ilces = new List<Ilce>();
            this.Subes = new List<Sube>();
        }

        public int Id { get; set; }
        public string Il1 { get; set; }
        public virtual ICollection<Firma> Firmas { get; set; }
        public virtual ICollection<Ilce> Ilces { get; set; }
        public virtual ICollection<Sube> Subes { get; set; }
    }
}
