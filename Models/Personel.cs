using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Personel
    {
        public Personel()
        {
            this.PersonelIzins = new List<PersonelIzin>();
            this.PersonelOdemes = new List<PersonelOdeme>();
            this.RandevuToPersonels = new List<RandevuToPersonel>();
            this.Sozlesmes = new List<Sozlesme>();
            this.SubeToPersonels = new List<SubeToPersonel>();
        }

        public long Id { get; set; }
        public long SubeId { get; set; }
        public long FirmaId { get; set; }
        public string TCKimlikNo { get; set; }
        public string AdiSoyadi { get; set; }
        public Nullable<System.DateTime> BaslamaTarihi { get; set; }
        public Nullable<System.DateTime> BitisTarihi { get; set; }
        public string SabitTel { get; set; }
        public string CepTel { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }
        public string GorevliSubeler { get; set; }
        public long GorevId { get; set; }
        public Nullable<int> YillikIzinHakki { get; set; }
        public Nullable<int> ToplamIzin { get; set; }
        public Nullable<int> KullanilanIzin { get; set; }
        public Nullable<int> KalanIzin { get; set; }
        public string CalismaSekli { get; set; }
        public Nullable<decimal> Ucret { get; set; }
        public bool SMSKabul { get; set; }
        public bool EmailKabul { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual PersonelGorevleri PersonelGorevleri { get; set; }
        public virtual Sube Sube { get; set; }
        public virtual ICollection<PersonelIzin> PersonelIzins { get; set; }
        public virtual ICollection<PersonelOdeme> PersonelOdemes { get; set; }
        public virtual ICollection<RandevuToPersonel> RandevuToPersonels { get; set; }
        public virtual ICollection<Sozlesme> Sozlesmes { get; set; }
        public virtual ICollection<SubeToPersonel> SubeToPersonels { get; set; }
    }
}
