using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class SubeToKullanici
    {
        public long Id { get; set; }
        public long KullaniciId { get; set; }
        public long SubeId { get; set; }
        public virtual Kullanici Kullanici { get; set; }
        public virtual Sube Sube { get; set; }
    }
}
