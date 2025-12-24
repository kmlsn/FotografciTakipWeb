using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class ModulSayfa
    {
        public ModulSayfa()
        {
            this.ModulSayfa1 = new List<ModulSayfa>();
        }

        public long Id { get; set; }
        public Nullable<long> ModulId { get; set; }
        public long FirmaId { get; set; }
        public long Sira { get; set; }
        public string SayfaAdi { get; set; }
        public Nullable<bool> SayfaYetkiAktif { get; set; }
        public Nullable<bool> KayitDetayiAktif { get; set; }
        public Nullable<bool> KayitEkleAktif { get; set; }
        public Nullable<bool> KayitDuzenleAktif { get; set; }
        public Nullable<bool> KayitSilAktif { get; set; }
        public Nullable<bool> Yazdirma { get; set; }
        public Nullable<bool> SMSGonder { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual ICollection<ModulSayfa> ModulSayfa1 { get; set; }
        public virtual ModulSayfa ModulSayfa2 { get; set; }
    }
}
