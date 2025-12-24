using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Cari
    {
        public Cari()
        {
            this.CariHarekets = new List<CariHareket>();
        }

        public long Id { get; set; }
        public long SubeId { get; set; }
        public long FirmaId { get; set; }
        public string FirmaAdi { get; set; }
        public string Yetkili { get; set; }
        public string AdSoyad { get; set; }
        public string SabitTel { get; set; }
        public string CepTel { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNo { get; set; }
        public string TCKimlikNo { get; set; }
        public string Adres { get; set; }
        public string Notlar { get; set; }
        public bool SMSKabul { get; set; }
        public bool EmailKabul { get; set; }
        public Nullable<bool> KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Sube Sube { get; set; }
        public virtual ICollection<CariHareket> CariHarekets { get; set; }
    }
}
