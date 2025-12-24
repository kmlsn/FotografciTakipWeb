using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class SatisFiyatlariMap : EntityTypeConfiguration<SatisFiyatlari>
    {
        public SatisFiyatlariMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.PaketAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.KDVOrani)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.PaketTip)
                .HasMaxLength(10);

            this.Property(t => t.SubeLimit)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.PersonelLimit)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.KullaniciLimit)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("SatisFiyatlari");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.PaketAdi).HasColumnName("PaketAdi");
            this.Property(t => t.PaketDetayi).HasColumnName("PaketDetayi");
            this.Property(t => t.SatisFiyati).HasColumnName("SatisFiyati");
            this.Property(t => t.IndirimliFiyat).HasColumnName("IndirimliFiyat");
            this.Property(t => t.IndirimBitisTarihi).HasColumnName("IndirimBitisTarihi");
            this.Property(t => t.KDVOrani).HasColumnName("KDVOrani");
            this.Property(t => t.Tarih).HasColumnName("Tarih");
            this.Property(t => t.PaketTip).HasColumnName("PaketTip");
            this.Property(t => t.Dashboard).HasColumnName("Dashboard");
            this.Property(t => t.RezervasyonIslemleri).HasColumnName("RezervasyonIslemleri");
            this.Property(t => t.RezervasyonIslemleri_RezervasyonTakvimi).HasColumnName("RezervasyonIslemleri_RezervasyonTakvimi");
            this.Property(t => t.RezervasyonIslemleri_RezervasyonListesi).HasColumnName("RezervasyonIslemleri_RezervasyonListesi");
            this.Property(t => t.RezervasyonIslemleri_RezervasyonTeklifleri).HasColumnName("RezervasyonIslemleri_RezervasyonTeklifleri");
            this.Property(t => t.RezervasyonIslemleri_Randevular).HasColumnName("RezervasyonIslemleri_Randevular");
            this.Property(t => t.Muhasebe).HasColumnName("Muhasebe");
            this.Property(t => t.Muhasebe_GunlukIsler).HasColumnName("Muhasebe_GunlukIsler");
            this.Property(t => t.Muhasebe_GelirGiderler).HasColumnName("Muhasebe_GelirGiderler");
            this.Property(t => t.Muhasebe_AlinanGelecekOdemeler).HasColumnName("Muhasebe_AlinanGelecekOdemeler");
            this.Property(t => t.Muhasebe_KalanBakiyeler).HasColumnName("Muhasebe_KalanBakiyeler");
            this.Property(t => t.Muhasebe_Kasa).HasColumnName("Muhasebe_Kasa");
            this.Property(t => t.Muhasebe_Cariler).HasColumnName("Muhasebe_Cariler");
            this.Property(t => t.Muhasebe_Cariler_CariListesi).HasColumnName("Muhasebe_Cariler_CariListesi");
            this.Property(t => t.Muhasebe_Cariler_CariHesapTakibi).HasColumnName("Muhasebe_Cariler_CariHesapTakibi");
            this.Property(t => t.Musteriler).HasColumnName("Musteriler");
            this.Property(t => t.Musteriler_MusteriListesi).HasColumnName("Musteriler_MusteriListesi");
            this.Property(t => t.Musteriler_MusteriMesajlari).HasColumnName("Musteriler_MusteriMesajlari");
            this.Property(t => t.Musteriler_FotografYukle).HasColumnName("Musteriler_FotografYukle");
            this.Property(t => t.Musteriler_MusteriHesapTakibi).HasColumnName("Musteriler_MusteriHesapTakibi");
            this.Property(t => t.Stok).HasColumnName("Stok");
            this.Property(t => t.Stok_StokKartlari).HasColumnName("Stok_StokKartlari");
            this.Property(t => t.Stok_StokGirisi).HasColumnName("Stok_StokGirisi");
            this.Property(t => t.Stok_StokCikisi).HasColumnName("Stok_StokCikisi");
            this.Property(t => t.Stok_StokHareketleri).HasColumnName("Stok_StokHareketleri");
            this.Property(t => t.Stok_Iade).HasColumnName("Stok_Iade");
            this.Property(t => t.Stok_DepoIslemleri).HasColumnName("Stok_DepoIslemleri");
            this.Property(t => t.Stok_DepoIslemleri_DepoListesi).HasColumnName("Stok_DepoIslemleri_DepoListesi");
            this.Property(t => t.Stok_DepoIslemleri_DepolarArasiTransfer).HasColumnName("Stok_DepoIslemleri_DepolarArasiTransfer");
            this.Property(t => t.Stok_DepoIslemleri_DepoStokDurumu).HasColumnName("Stok_DepoIslemleri_DepoStokDurumu");
            this.Property(t => t.Stok_DepoIslemleri_DepoyaYapılmışGirisler).HasColumnName("Stok_DepoIslemleri_DepoyaYapılmışGirisler");
            this.Property(t => t.Stok_DepoIslemleri_DepodanYapilmisCikislar).HasColumnName("Stok_DepoIslemleri_DepodanYapilmisCikislar");
            this.Property(t => t.Raporlar).HasColumnName("Raporlar");
            this.Property(t => t.Raporlar_CekimPaketleriRaporu).HasColumnName("Raporlar_CekimPaketleriRaporu");
            this.Property(t => t.Raporlar_RezervasyonIslemAdetleriRaporu).HasColumnName("Raporlar_RezervasyonIslemAdetleriRaporu");
            this.Property(t => t.Raporlar_GelirGiderRaporlari).HasColumnName("Raporlar_GelirGiderRaporlari");
            this.Property(t => t.Raporlar_RezervasyonTurleriRaporu).HasColumnName("Raporlar_RezervasyonTurleriRaporu");
            this.Property(t => t.Raporlar_RezervasyonEkHizmetleriRaporu).HasColumnName("Raporlar_RezervasyonEkHizmetleriRaporu");
            this.Property(t => t.Raporlar_GelecekOdemelerRaporu).HasColumnName("Raporlar_GelecekOdemelerRaporu");
            this.Property(t => t.Raporlar_HaftalikRapor).HasColumnName("Raporlar_HaftalikRapor");
            this.Property(t => t.Raporlar_PersonelPerformansRaporu).HasColumnName("Raporlar_PersonelPerformansRaporu");
            this.Property(t => t.TelefonRehberi).HasColumnName("TelefonRehberi");
            this.Property(t => t.TelefonRehberi_Rehber).HasColumnName("TelefonRehberi_Rehber");
            this.Property(t => t.TelefonRehberi_ExceldenEkle).HasColumnName("TelefonRehberi_ExceldenEkle");
            this.Property(t => t.Personeller).HasColumnName("Personeller");
            this.Property(t => t.Personeller_PersonelListesi).HasColumnName("Personeller_PersonelListesi");
            this.Property(t => t.Personeller_PersonelOdemeleri).HasColumnName("Personeller_PersonelOdemeleri");
            this.Property(t => t.Personeller_PersonelIzinleri).HasColumnName("Personeller_PersonelIzinleri");
            this.Property(t => t.Personeller_PersonelIsTakibi).HasColumnName("Personeller_PersonelIsTakibi");
            this.Property(t => t.Tanimlar).HasColumnName("Tanimlar");
            this.Property(t => t.Tanimlar_CekimPaketleri).HasColumnName("Tanimlar_CekimPaketleri");
            this.Property(t => t.Tanimlar_GelirGiderTurleri).HasColumnName("Tanimlar_GelirGiderTurleri");
            this.Property(t => t.Tanimlar_GunlukIsKategorileri).HasColumnName("Tanimlar_GunlukIsKategorileri");
            this.Property(t => t.Tanimlar_PersonelGorevleri).HasColumnName("Tanimlar_PersonelGorevleri");
            this.Property(t => t.Tanimlar_RehberGruplari).HasColumnName("Tanimlar_RehberGruplari");
            this.Property(t => t.Tanimlar_RezervasyonEkHizmetleri).HasColumnName("Tanimlar_RezervasyonEkHizmetleri");
            this.Property(t => t.Tanimlar_RezervasyonTurleri).HasColumnName("Tanimlar_RezervasyonTurleri");
            this.Property(t => t.Tanimlar_EmailMetinleri).HasColumnName("Tanimlar_EmailMetinleri");
            this.Property(t => t.Tanimlar_SmsMetinleri).HasColumnName("Tanimlar_SmsMetinleri");
            this.Property(t => t.Tanimlar_SozlesmeSartlari).HasColumnName("Tanimlar_SozlesmeSartlari");
            this.Property(t => t.Tanimlar_Sureler).HasColumnName("Tanimlar_Sureler");
            this.Property(t => t.Tanimlar_TatilGunleri).HasColumnName("Tanimlar_TatilGunleri");
            this.Property(t => t.Tanimlar_ZamanDilimleri).HasColumnName("Tanimlar_ZamanDilimleri");
            this.Property(t => t.Ayarlar).HasColumnName("Ayarlar");
            this.Property(t => t.Ayarlar_GenelAyarlar).HasColumnName("Ayarlar_GenelAyarlar");
            this.Property(t => t.Ayarlar_EmailGonderimAyarlari).HasColumnName("Ayarlar_EmailGonderimAyarlari");
            this.Property(t => t.Ayarlar_EmailHesapAyarlari).HasColumnName("Ayarlar_EmailHesapAyarlari");
            this.Property(t => t.Ayarlar_ListelemeFiltreAyarlari).HasColumnName("Ayarlar_ListelemeFiltreAyarlari");
            this.Property(t => t.Ayarlar_SmsGonderimAyarlari).HasColumnName("Ayarlar_SmsGonderimAyarlari");
            this.Property(t => t.Ayarlar_MusteriAyarlari).HasColumnName("Ayarlar_MusteriAyarlari");
            this.Property(t => t.Ayarlar_RezervasyonAyarlari).HasColumnName("Ayarlar_RezervasyonAyarlari");
            this.Property(t => t.Ayarlar_SozlesmeCiktiAyarlari).HasColumnName("Ayarlar_SozlesmeCiktiAyarlari");
            this.Property(t => t.Ayarlar_Personeller).HasColumnName("Ayarlar_Personeller");
            this.Property(t => t.FirmaSubeIslemleri).HasColumnName("FirmaSubeIslemleri");
            this.Property(t => t.FirmaSubeIslemleri_FirmaBilgileri).HasColumnName("FirmaSubeIslemleri_FirmaBilgileri");
            this.Property(t => t.FirmaSubeIslemleri_SubeIslemleri).HasColumnName("FirmaSubeIslemleri_SubeIslemleri");
            this.Property(t => t.KullaniciYetkiIslemleri).HasColumnName("KullaniciYetkiIslemleri");
            this.Property(t => t.KullaniciYetkiIslemleri_KullaniciListesi).HasColumnName("KullaniciYetkiIslemleri_KullaniciListesi");
            this.Property(t => t.KullaniciYetkiIslemleri_KullaniciYetkilendirme).HasColumnName("KullaniciYetkiIslemleri_KullaniciYetkilendirme");
            this.Property(t => t.SmsGonder).HasColumnName("SmsGonder");
            this.Property(t => t.SatinAl).HasColumnName("SatinAl");
            this.Property(t => t.SatinAl_PaketSec).HasColumnName("SatinAl_PaketSec");
            this.Property(t => t.SatinAl_Siparisler).HasColumnName("SatinAl_Siparisler");
            this.Property(t => t.Destek).HasColumnName("Destek");
            this.Property(t => t.Destek_DestekTalepleri).HasColumnName("Destek_DestekTalepleri");
            this.Property(t => t.Destek_DestekDetay).HasColumnName("Destek_DestekDetay");
            this.Property(t => t.EkModul1).HasColumnName("EkModul1");
            this.Property(t => t.EkModul2).HasColumnName("EkModul2");
            this.Property(t => t.EkModul3).HasColumnName("EkModul3");
            this.Property(t => t.EkModul4).HasColumnName("EkModul4");
            this.Property(t => t.SubeLimit).HasColumnName("SubeLimit");
            this.Property(t => t.PersonelLimit).HasColumnName("PersonelLimit");
            this.Property(t => t.KullaniciLimit).HasColumnName("KullaniciLimit");
            this.Property(t => t.SMSMiktar).HasColumnName("SMSMiktar");
            this.Property(t => t.OlusturmaTarihi).HasColumnName("OlusturmaTarihi");
            this.Property(t => t.DegistirmeTarihi).HasColumnName("DegistirmeTarihi");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
        }
    }
}
