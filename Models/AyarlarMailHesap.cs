using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class AyarlarMailHesap
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public string GonderenMail { get; set; }
        public string GonderenSifre { get; set; }
        public string GonderenAdSoyad { get; set; }
        public string SmtpAdres { get; set; }
        public string SmtpPort { get; set; }
        public bool Ssl { get; set; }
        public string Aciklama { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
