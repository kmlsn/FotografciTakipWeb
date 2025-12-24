using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class GonderilenEmailler
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long SubeId { get; set; }
        public string MailKonu { get; set; }
        public string AliciEposta { get; set; }
        public string MailIcerik { get; set; }
        public string TemaYol { get; set; }
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
