using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Firma
    {
        public Firma()
        {
            this.AyarlarFiltres = new List<AyarlarFiltre>();
            this.AyarlarGenels = new List<AyarlarGenel>();
            this.AyarlarMailGonderims = new List<AyarlarMailGonderim>();
            this.AyarlarMailHesaps = new List<AyarlarMailHesap>();
            this.AyarlarMusteris = new List<AyarlarMusteri>();
            this.AyarlarRezervasyons = new List<AyarlarRezervasyon>();
            this.AyarlarSmsGonderims = new List<AyarlarSmsGonderim>();
            this.AyarlarSozlesmeCiktis = new List<AyarlarSozlesmeCikti>();
            this.Caris = new List<Cari>();
            this.CariHarekets = new List<CariHareket>();
            this.CekimPaketleris = new List<CekimPaketleri>();
            this.DestekTalepleris = new List<DestekTalepleri>();
            this.DestekTalepleriDetays = new List<DestekTalepleriDetay>();
            this.GelirGiders = new List<GelirGider>();
            this.GelirGiderTurleris = new List<GelirGiderTurleri>();
            this.GonderilenEmaillers = new List<GonderilenEmailler>();
            this.GonderilenSmslers = new List<GonderilenSmsler>();
            this.GunlukIsKategoris = new List<GunlukIsKategori>();
            this.GunlukIslers = new List<GunlukIsler>();
            this.HataLoglaris = new List<HataLoglari>();
            this.Kullanicis = new List<Kullanici>();
            this.KullaniciYetkis = new List<KullaniciYetki>();
            this.Lisanslars = new List<Lisanslar>();
            this.MailMetinleris = new List<MailMetinleri>();
            this.ModulSayfas = new List<ModulSayfa>();
            this.Musteris = new List<Musteri>();
            this.MusteriFotografs = new List<MusteriFotograf>();
            this.MusteriMesajs = new List<MusteriMesaj>();
            this.MusteriMesajlaris = new List<MusteriMesajlari>();
            this.MusteriMesajlariDetays = new List<MusteriMesajlariDetay>();
            this.Odemelers = new List<Odemeler>();
            this.OtomatikSmsListesis = new List<OtomatikSmsListesi>();
            this.Personels = new List<Personel>();
            this.PersonelGorevleris = new List<PersonelGorevleri>();
            this.PersonelIzins = new List<PersonelIzin>();
            this.PersonelOdemes = new List<PersonelOdeme>();
            this.Randevus = new List<Randevu>();
            this.RandevuGorunums = new List<RandevuGorunum>();
            this.RehberGrups = new List<RehberGrup>();
            this.RezervasyonEkHizmets = new List<RezervasyonEkHizmet>();
            this.RezervasyonTurleris = new List<RezervasyonTurleri>();
            this.RolYetkis = new List<RolYetki>();
            this.Siparislers = new List<Siparisler>();
            this.SmsBakiyes = new List<SmsBakiye>();
            this.SmsHarekets = new List<SmsHareket>();
            this.SmsMetinleris = new List<SmsMetinleri>();
            this.Sozlesmes = new List<Sozlesme>();
            this.SozlesmeSartlaris = new List<SozlesmeSartlari>();
            this.Subes = new List<Sube>();
            this.Sureclers = new List<Surecler>();
            this.TatilGunleris = new List<TatilGunleri>();
            this.TelefonRehberis = new List<TelefonRehberi>();
            this.Unvanlars = new List<Unvanlar>();
            this.ZamanDilimleris = new List<ZamanDilimleri>();
        }

        public long Id { get; set; }
        public string FirmaAdi { get; set; }
        public string Yetkili { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNo { get; set; }
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
        public string FirmaHakkinda { get; set; }
        public Nullable<long> ResimId { get; set; }
        public bool AcilisBit { get; set; }
        public bool DuyuruBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual ICollection<AyarlarFiltre> AyarlarFiltres { get; set; }
        public virtual ICollection<AyarlarGenel> AyarlarGenels { get; set; }
        public virtual ICollection<AyarlarMailGonderim> AyarlarMailGonderims { get; set; }
        public virtual ICollection<AyarlarMailHesap> AyarlarMailHesaps { get; set; }
        public virtual ICollection<AyarlarMusteri> AyarlarMusteris { get; set; }
        public virtual ICollection<AyarlarRezervasyon> AyarlarRezervasyons { get; set; }
        public virtual ICollection<AyarlarSmsGonderim> AyarlarSmsGonderims { get; set; }
        public virtual ICollection<AyarlarSozlesmeCikti> AyarlarSozlesmeCiktis { get; set; }
        public virtual ICollection<Cari> Caris { get; set; }
        public virtual ICollection<CariHareket> CariHarekets { get; set; }
        public virtual ICollection<CekimPaketleri> CekimPaketleris { get; set; }
        public virtual ICollection<DestekTalepleri> DestekTalepleris { get; set; }
        public virtual ICollection<DestekTalepleriDetay> DestekTalepleriDetays { get; set; }
        public virtual Il Il { get; set; }
        public virtual Ilce Ilce { get; set; }
        public virtual Resim Resim { get; set; }
        public virtual ICollection<GelirGider> GelirGiders { get; set; }
        public virtual ICollection<GelirGiderTurleri> GelirGiderTurleris { get; set; }
        public virtual ICollection<GonderilenEmailler> GonderilenEmaillers { get; set; }
        public virtual ICollection<GonderilenSmsler> GonderilenSmslers { get; set; }
        public virtual ICollection<GunlukIsKategori> GunlukIsKategoris { get; set; }
        public virtual ICollection<GunlukIsler> GunlukIslers { get; set; }
        public virtual ICollection<HataLoglari> HataLoglaris { get; set; }
        public virtual ICollection<Kullanici> Kullanicis { get; set; }
        public virtual ICollection<KullaniciYetki> KullaniciYetkis { get; set; }
        public virtual ICollection<Lisanslar> Lisanslars { get; set; }
        public virtual ICollection<MailMetinleri> MailMetinleris { get; set; }
        public virtual ICollection<ModulSayfa> ModulSayfas { get; set; }
        public virtual ICollection<Musteri> Musteris { get; set; }
        public virtual ICollection<MusteriFotograf> MusteriFotografs { get; set; }
        public virtual ICollection<MusteriMesaj> MusteriMesajs { get; set; }
        public virtual ICollection<MusteriMesajlari> MusteriMesajlaris { get; set; }
        public virtual ICollection<MusteriMesajlariDetay> MusteriMesajlariDetays { get; set; }
        public virtual ICollection<Odemeler> Odemelers { get; set; }
        public virtual ICollection<OtomatikSmsListesi> OtomatikSmsListesis { get; set; }
        public virtual ICollection<Personel> Personels { get; set; }
        public virtual ICollection<PersonelGorevleri> PersonelGorevleris { get; set; }
        public virtual ICollection<PersonelIzin> PersonelIzins { get; set; }
        public virtual ICollection<PersonelOdeme> PersonelOdemes { get; set; }
        public virtual ICollection<Randevu> Randevus { get; set; }
        public virtual ICollection<RandevuGorunum> RandevuGorunums { get; set; }
        public virtual ICollection<RehberGrup> RehberGrups { get; set; }
        public virtual ICollection<RezervasyonEkHizmet> RezervasyonEkHizmets { get; set; }
        public virtual ICollection<RezervasyonTurleri> RezervasyonTurleris { get; set; }
        public virtual ICollection<RolYetki> RolYetkis { get; set; }
        public virtual ICollection<Siparisler> Siparislers { get; set; }
        public virtual ICollection<SmsBakiye> SmsBakiyes { get; set; }
        public virtual ICollection<SmsHareket> SmsHarekets { get; set; }
        public virtual ICollection<SmsMetinleri> SmsMetinleris { get; set; }
        public virtual ICollection<Sozlesme> Sozlesmes { get; set; }
        public virtual ICollection<SozlesmeSartlari> SozlesmeSartlaris { get; set; }
        public virtual ICollection<Sube> Subes { get; set; }
        public virtual ICollection<Surecler> Sureclers { get; set; }
        public virtual ICollection<TatilGunleri> TatilGunleris { get; set; }
        public virtual ICollection<TelefonRehberi> TelefonRehberis { get; set; }
        public virtual ICollection<Unvanlar> Unvanlars { get; set; }
        public virtual ICollection<ZamanDilimleri> ZamanDilimleris { get; set; }
    }
}
