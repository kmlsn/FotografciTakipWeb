using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class GonderilenSmsler
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long SubeId { get; set; }
        public System.DateTime GonderimTarihi { get; set; }
        public string SmsMetni { get; set; }
        public string Telefon { get; set; }
        public string SmsGorevId { get; set; }
        public string Durum { get; set; }
        public string HataKodu { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Sube Sube { get; set; }
    }
}
