using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class TelefonRehberi
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long RehberGrupId { get; set; }
        public Nullable<long> SozlesmeId { get; set; }
        public Nullable<long> MusteriId { get; set; }
        public Nullable<long> CariId { get; set; }
        public Nullable<long> PersonelId { get; set; }
        public string FirmaAdi { get; set; }
        public string AdSoyad { get; set; }
        public string SabitTel1 { get; set; }
        public string SabitTel2 { get; set; }
        public string CepTel1 { get; set; }
        public string CepTel2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public bool SmsKabul { get; set; }
        public bool EmailKabul { get; set; }
        public string Notlar { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual RehberGrup RehberGrup { get; set; }
    }
}
