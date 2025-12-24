using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class OtomatikSmsListesi
    {
        public long Id { get; set; }
        public Nullable<long> FirmaId { get; set; }
        public Nullable<System.DateTime> GonderimTarihi { get; set; }
        public string SmsMetni { get; set; }
        public string AliciAdSoyad { get; set; }
        public string AliciTel { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
