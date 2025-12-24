using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class AyarlarSozlesmeCikti
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public bool LogoGoster { get; set; }
        public bool FirmaAdiGoster { get; set; }
        public bool PaketlerGoster { get; set; }
        public bool EkHizmetlerGoster { get; set; }
        public bool CekimRandevulariGoster { get; set; }
        public bool YapilanOdemelerGoster { get; set; }
        public bool KalanOdemelerGoster { get; set; }
        public bool CepTelefonuGoster { get; set; }
        public bool SabitTelefonGoster { get; set; }
        public bool FaxGoster { get; set; }
        public bool MusteriKoduSifreGoster { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
    }
}
