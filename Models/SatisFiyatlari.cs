using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class SatisFiyatlari
    {
        public SatisFiyatlari()
        {
            this.Siparislers = new List<Siparisler>();
        }

        public long Id { get; set; }
        public string PaketAdi { get; set; }
        public string PaketDetayi { get; set; }
        public decimal SatisFiyati { get; set; }
        public Nullable<decimal> IndirimliFiyat { get; set; }
        public Nullable<System.DateTime> IndirimBitisTarihi { get; set; }
        public string KDVOrani { get; set; }
        public Nullable<System.DateTime> Tarih { get; set; }
        public string PaketTip { get; set; }
        public bool Dashboard { get; set; }
        public bool RezervasyonIslemleri { get; set; }
        public bool RezervasyonIslemleri_RezervasyonTakvimi { get; set; }
        public bool RezervasyonIslemleri_RezervasyonListesi { get; set; }
        public bool RezervasyonIslemleri_RezervasyonTeklifleri { get; set; }
        public bool RezervasyonIslemleri_Randevular { get; set; }
        public bool Muhasebe { get; set; }
        public bool Muhasebe_GunlukIsler { get; set; }
        public bool Muhasebe_GelirGiderler { get; set; }
        public bool Muhasebe_AlinanGelecekOdemeler { get; set; }
        public bool Muhasebe_KalanBakiyeler { get; set; }
        public bool Muhasebe_Kasa { get; set; }
        public bool Muhasebe_Cariler { get; set; }
        public bool Muhasebe_Cariler_CariListesi { get; set; }
        public bool Muhasebe_Cariler_CariHesapTakibi { get; set; }
        public bool Musteriler { get; set; }
        public bool Musteriler_MusteriListesi { get; set; }
        public bool Musteriler_MusteriMesajlari { get; set; }
        public bool Musteriler_FotografYukle { get; set; }
        public bool Musteriler_MusteriHesapTakibi { get; set; }
        public bool Stok { get; set; }
        public bool Stok_StokKartlari { get; set; }
        public bool Stok_StokGirisi { get; set; }
        public bool Stok_StokCikisi { get; set; }
        public bool Stok_StokHareketleri { get; set; }
        public bool Stok_Iade { get; set; }
        public bool Stok_DepoIslemleri { get; set; }
        public bool Stok_DepoIslemleri_DepoListesi { get; set; }
        public bool Stok_DepoIslemleri_DepolarArasiTransfer { get; set; }
        public bool Stok_DepoIslemleri_DepoStokDurumu { get; set; }
        public bool Stok_DepoIslemleri_DepoyaYapılmışGirisler { get; set; }
        public bool Stok_DepoIslemleri_DepodanYapilmisCikislar { get; set; }
        public bool Raporlar { get; set; }
        public bool Raporlar_CekimPaketleriRaporu { get; set; }
        public bool Raporlar_RezervasyonIslemAdetleriRaporu { get; set; }
        public bool Raporlar_GelirGiderRaporlari { get; set; }
        public bool Raporlar_RezervasyonTurleriRaporu { get; set; }
        public bool Raporlar_RezervasyonEkHizmetleriRaporu { get; set; }
        public bool Raporlar_GelecekOdemelerRaporu { get; set; }
        public bool Raporlar_HaftalikRapor { get; set; }
        public bool Raporlar_PersonelPerformansRaporu { get; set; }
        public bool TelefonRehberi { get; set; }
        public bool TelefonRehberi_Rehber { get; set; }
        public bool TelefonRehberi_ExceldenEkle { get; set; }
        public bool Personeller { get; set; }
        public bool Personeller_PersonelListesi { get; set; }
        public bool Personeller_PersonelOdemeleri { get; set; }
        public bool Personeller_PersonelIzinleri { get; set; }
        public bool Personeller_PersonelIsTakibi { get; set; }
        public bool Tanimlar { get; set; }
        public bool Tanimlar_CekimPaketleri { get; set; }
        public bool Tanimlar_GelirGiderTurleri { get; set; }
        public bool Tanimlar_GunlukIsKategorileri { get; set; }
        public bool Tanimlar_PersonelGorevleri { get; set; }
        public bool Tanimlar_RehberGruplari { get; set; }
        public bool Tanimlar_RezervasyonEkHizmetleri { get; set; }
        public bool Tanimlar_RezervasyonTurleri { get; set; }
        public bool Tanimlar_EmailMetinleri { get; set; }
        public bool Tanimlar_SmsMetinleri { get; set; }
        public bool Tanimlar_SozlesmeSartlari { get; set; }
        public bool Tanimlar_Sureler { get; set; }
        public bool Tanimlar_TatilGunleri { get; set; }
        public bool Tanimlar_ZamanDilimleri { get; set; }
        public bool Ayarlar { get; set; }
        public bool Ayarlar_GenelAyarlar { get; set; }
        public bool Ayarlar_EmailGonderimAyarlari { get; set; }
        public bool Ayarlar_EmailHesapAyarlari { get; set; }
        public bool Ayarlar_ListelemeFiltreAyarlari { get; set; }
        public bool Ayarlar_SmsGonderimAyarlari { get; set; }
        public bool Ayarlar_MusteriAyarlari { get; set; }
        public bool Ayarlar_RezervasyonAyarlari { get; set; }
        public bool Ayarlar_SozlesmeCiktiAyarlari { get; set; }
        public bool Ayarlar_Personeller { get; set; }
        public bool FirmaSubeIslemleri { get; set; }
        public bool FirmaSubeIslemleri_FirmaBilgileri { get; set; }
        public bool FirmaSubeIslemleri_SubeIslemleri { get; set; }
        public bool KullaniciYetkiIslemleri { get; set; }
        public bool KullaniciYetkiIslemleri_KullaniciListesi { get; set; }
        public bool KullaniciYetkiIslemleri_KullaniciYetkilendirme { get; set; }
        public bool SmsGonder { get; set; }
        public bool SatinAl { get; set; }
        public bool SatinAl_PaketSec { get; set; }
        public bool SatinAl_Siparisler { get; set; }
        public bool Destek { get; set; }
        public bool Destek_DestekTalepleri { get; set; }
        public bool Destek_DestekDetay { get; set; }
        public bool EkModul1 { get; set; }
        public bool EkModul2 { get; set; }
        public bool EkModul3 { get; set; }
        public bool EkModul4 { get; set; }
        public string SubeLimit { get; set; }
        public string PersonelLimit { get; set; }
        public string KullaniciLimit { get; set; }
        public int SMSMiktar { get; set; }
        public System.DateTime OlusturmaTarihi { get; set; }
        public Nullable<System.DateTime> DegistirmeTarihi { get; set; }
        public bool Aktif { get; set; }
        public virtual ICollection<Siparisler> Siparislers { get; set; }
    }
}
