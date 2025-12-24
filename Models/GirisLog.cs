using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class GirisLog
    {
        public long Id { get; set; }
        public long KullaniciId { get; set; }
        public string IpAdres { get; set; }
        public string Ulke { get; set; }
        public string Sehir { get; set; }
        public System.DateTime BaglantıZaman { get; set; }
        public virtual Kullanici Kullanici { get; set; }
    }
}
