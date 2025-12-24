using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class SubeToPersonel
    {
        public long Id { get; set; }
        public long PersonelId { get; set; }
        public long SubeId { get; set; }
        public virtual Personel Personel { get; set; }
        public virtual Sube Sube { get; set; }
    }
}
