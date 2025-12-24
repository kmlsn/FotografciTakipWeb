using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Sozlesme
    {
        public Sozlesme()
        {
            this.MusteriFotografs = new List<MusteriFotograf>();
            this.Odemelers = new List<Odemeler>();
            this.Randevus = new List<Randevu>();
        }

        public long Id { get; set; }
        public long SubeId { get; set; }
        public long FirmaId { get; set; }
        public string YetkiliPersonel { get; set; }
        public Nullable<long> GorevliPersonelId { get; set; }
        public string GorevliPersonellerId { get; set; }
        public long SozlesmeNo { get; set; }
        public System.DateTime SozlesmeTarihi { get; set; }
        public long RezervasyonTurId { get; set; }
        public System.DateTime RezervasyonTarihi { get; set; }
        public System.TimeSpan BaslangicSaat { get; set; }
        public System.TimeSpan BitisSaat { get; set; }
        public Nullable<long> MusteriId { get; set; }
        public string YetkiliAdSoyad { get; set; }
        public string YetkiliTel { get; set; }
        public string YetkiliEmail { get; set; }
        public string Urunler { get; set; }
        public string Modeller { get; set; }
        public string CocukAdSoyad { get; set; }
        public string AnneAd { get; set; }
        public string BabaAd { get; set; }
        public string AnneTel { get; set; }
        public string BabaTel { get; set; }
        public string AnneEmail { get; set; }
        public string BabaEmail { get; set; }
        public string GelinAd { get; set; }
        public string DamatAd { get; set; }
        public string GelinTel { get; set; }
        public string DamatTel { get; set; }
        public string GelinEmail { get; set; }
        public string DamatEmail { get; set; }
        public string Durum { get; set; }
        public Nullable<bool> TeklifBit { get; set; }
        public Nullable<bool> KesinRezervasyonBit { get; set; }
        public Nullable<bool> OpsiyonBit { get; set; }
        public Nullable<System.DateTime> OpsiyonTarihi { get; set; }
        public string PaketlerId { get; set; }
        public string Paketler { get; set; }
        public string EkHizmetlerId { get; set; }
        public string EkHizmetler { get; set; }
        public Nullable<bool> FotgrafHatirlatma { get; set; }
        public Nullable<decimal> PaketlerFiyat { get; set; }
        public Nullable<decimal> EkHizmetlerFiyat { get; set; }
        public string SureclerId { get; set; }
        public string Surecler { get; set; }
        public Nullable<decimal> Iskonto { get; set; }
        public Nullable<decimal> ToplamFiyat { get; set; }
        public Nullable<bool> KDVDahil { get; set; }
        public string Referans { get; set; }
        public string OrganizasyonYeri { get; set; }
        public string Notlar { get; set; }
        public string FotografSecimDurum { get; set; }
        public Nullable<System.DateTime> FotografSecimDurumTarihi { get; set; }
        public bool KilitBit { get; set; }
        public bool Bitti { get; set; }
        public bool Iptal { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Musteri Musteri { get; set; }
        public virtual ICollection<MusteriFotograf> MusteriFotografs { get; set; }
        public virtual ICollection<Odemeler> Odemelers { get; set; }
        public virtual Personel Personel { get; set; }
        public virtual ICollection<Randevu> Randevus { get; set; }
        public virtual RezervasyonTurleri RezervasyonTurleri { get; set; }
        public virtual Sube Sube { get; set; }
    }
}
