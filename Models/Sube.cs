using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Sube
    {
        public Sube()
        {
            this.Caris = new List<Cari>();
            this.CariHarekets = new List<CariHareket>();
            this.GelirGiders = new List<GelirGider>();
            this.GonderilenEmaillers = new List<GonderilenEmailler>();
            this.GonderilenSmslers = new List<GonderilenSmsler>();
            this.GunlukIslers = new List<GunlukIsler>();
            this.HataLoglaris = new List<HataLoglari>();
            this.Musteris = new List<Musteri>();
            this.Odemelers = new List<Odemeler>();
            this.Personels = new List<Personel>();
            this.PersonelIzins = new List<PersonelIzin>();
            this.PersonelOdemes = new List<PersonelOdeme>();
            this.Randevus = new List<Randevu>();
            this.Sozlesmes = new List<Sozlesme>();
            this.SubeToKullanicis = new List<SubeToKullanici>();
            this.SubeToPersonels = new List<SubeToPersonel>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public string SubeAdi { get; set; }
        public string Yetkili { get; set; }
        public string TCKimlikNo { get; set; }
        public string Email { get; set; }
        public string CepTel { get; set; }
        public string SabitTel { get; set; }
        public string Fax { get; set; }
        public Nullable<int> IlId { get; set; }
        public Nullable<int> IlceId { get; set; }
        public string Adres { get; set; }
        public string WebSitesi { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string SubeHakkinda { get; set; }
        public string Notlar { get; set; }
        public string GorevliPersoneller { get; set; }
        public string YetkiliKullanicilar { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual ICollection<Cari> Caris { get; set; }
        public virtual ICollection<CariHareket> CariHarekets { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual ICollection<GelirGider> GelirGiders { get; set; }
        public virtual ICollection<GonderilenEmailler> GonderilenEmaillers { get; set; }
        public virtual ICollection<GonderilenSmsler> GonderilenSmslers { get; set; }
        public virtual ICollection<GunlukIsler> GunlukIslers { get; set; }
        public virtual ICollection<HataLoglari> HataLoglaris { get; set; }
        public virtual Il Il { get; set; }
        public virtual Ilce Ilce { get; set; }
        public virtual ICollection<Musteri> Musteris { get; set; }
        public virtual ICollection<Odemeler> Odemelers { get; set; }
        public virtual ICollection<Personel> Personels { get; set; }
        public virtual ICollection<PersonelIzin> PersonelIzins { get; set; }
        public virtual ICollection<PersonelOdeme> PersonelOdemes { get; set; }
        public virtual ICollection<Randevu> Randevus { get; set; }
        public virtual ICollection<Sozlesme> Sozlesmes { get; set; }
        public virtual ICollection<SubeToKullanici> SubeToKullanicis { get; set; }
        public virtual ICollection<SubeToPersonel> SubeToPersonels { get; set; }
    }
}
