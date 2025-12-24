using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class TempSmsGonderListe
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public string AdSoyad { get; set; }
        public string Telefon { get; set; }
        public string TelefonFormatli { get; set; }
        public string Mesaj { get; set; }
        public string Durum { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
    }
}
