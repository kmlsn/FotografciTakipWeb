using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.Admin.Controllers
{
    public class SiparislerController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();

        // GET: Admin/Siparisler
        public ActionResult Siparisler()
        {

            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Siparisler";
            ViewBag.AltMenu = "SiparisListesi";

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            //KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 107 && x.Aktif == true && x.Sil == false);
            //if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
            //    return RedirectToAction("YetkisizGiris", "Hata");
            //ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 107 && x.Aktif == true && x.Sil == false);
            //List<Siparisler> siparislers = dbContext.Siparislers.Select(x => x).ToList();
            return View();

            //List<Siparisler> siparislers = dbContext.Siparislers.Select(x => x).ToList();
            //var siparisler = siparislers.Select(m => new {
            //    SiparisId = m.Id,
            //    SiparisNo = m.SiparisNo,
            //    FirmaId = m.FirmaId,
            //    FirmaAdi = m.Firma.FirmaAdi,
            //    Paket = m.Paket,
            //    PaketTutar = m.PaketTutar,
            //    LisansSuresi = m.LisansSuresi,
            //    OdemeDurum = m.Odendi,
            //    Durum = m.Durum,
            //    Tarih = m.Tarih,
            //    Iptal=m.Iptal,
            //    OdemeBildirim=m.OdemeBildirim,
            //    DosyaYol=m.Dosya,
            //    OdemeBildirimAciklama=m.OdemeBildirimAciklama,
            //    OdemeTuru=m.OdemeTuru
            //});
            //return Json(new { data = siparisler }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult SiparislerListesi()
        {
            List<Siparisler> siparislers = dbContext.Siparislers.Select(x => x).ToList();
            var siparisler = siparislers.Select(m => new
            {
                SiparisId = m.Id,
                SiparisNo = m.SiparisNo,
                FirmaId = m.FirmaId,
                FirmaAdi = m.Firma.FirmaAdi,
                Paket = m.Paket,
                PaketTutar = m.PaketTutar,
                LisansSuresi = m.LisansSuresi,
                OdemeDurum = m.Odendi,
                Durum = m.Durum,
                Tarih = m.Tarih,
                Iptal = m.Iptal,
                Sil=m.Sil,
                OdemeBildirim = m.OdemeBildirim,
                DosyaYol = m.Dosya,
                OdemeBildirimAciklama = m.OdemeBildirimAciklama,
                OdemeTuru = m.OdemeTuru
            });
            return Json(new { data = siparisler }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SiparisSil(long? id)
        {

            // Sipariş KK ile ödendi ise Lisan işlemi iptal edilecek.
            // Ödeme iade yapılacak.

            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            Models.Siparisler siparis = dbContext.Siparislers.FirstOrDefault(x => x.Id == id && x.Aktif == true && x.Sil == false);
            if (siparis == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            siparis.Sil = true;
            siparis.Aktif = false;
            siparis.DegistirenKullaniciId = 1;
            siparis.DegistirmeTarih = DateTime.Now;

            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = 1;
                hata.SubeId = 1;
                hata.Islem = "Sipariş Sil, Sipariş Id: " + id;
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public ActionResult SiparisIptal(long? id)
        {
            // Sipariş KK ile ödendi ise Lisan işlemi iptal edilecek.
            // Ödeme iade yapılacak.

            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            Models.Siparisler siparis = dbContext.Siparislers.FirstOrDefault(x => x.Id == id && x.Aktif == true && x.Sil == false);
            if (siparis == null)
                return Json(new { Sonuc = false, Bilgi = "İptal edilmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            siparis.Iptal = true;
            siparis.Sil = false;
            siparis.Aktif = true;
            siparis.DegistirenKullaniciId = 1;
            siparis.DegistirmeTarih = DateTime.Now;

            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = 1;
                hata.SubeId = 1;
                hata.Islem = "Sipariş İptal, Sipariş Id: " + id;
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }
        }

        [HttpPost]
        public ActionResult SiparisOnay(int? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            Siparisler siparis = dbContext.Siparislers.FirstOrDefault(x => x.Id == id && x.Aktif == true && x.Sil == false);
            SatisFiyatlari paket = dbContext.SatisFiyatlaris.FirstOrDefault(x => x.Id == siparis.SatisFiyatId);

            long FirmaId = siparis.FirmaId;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string PaketAdi = siparis.Paket;
            long SiparisNo = siparis.SiparisNo;
            int SmsMiktar = 0;
            int sure = siparis.LisansSuresi;

            // Sipariş durumu güncelleniyor...
            siparis.Odendi = true;
            siparis.Durum = 1; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
            siparis.DegistirenKullaniciId = KullaniciId;
            siparis.DegistirmeTarih = DateTime.Now;
            dbContext.SaveChanges();
            if (paket.PaketTip == "SMS")
            {
                // Sipariş edilen SMS miktarını bulma
                //if (sip.Paket == "1.000 SMS")
                //    SmsMiktar = 1000;
                //else if (sip.Paket == "5.000 SMS")
                //    SmsMiktar = 5000;
                //else if (sip.Paket == "10.000 SMS")
                //    SmsMiktar = 1000;
                //else if (sip.Paket == "25.000 SMS")
                //    SmsMiktar = 25000;

                SmsMiktar = paket.SMSMiktar;

                //---------
                SmsHareket smshareket = new SmsHareket();
                smshareket.FirmaId = FirmaId;
                smshareket.YuklemeTarihi = DateTime.Now;
                smshareket.Miktar = SmsMiktar;
                smshareket.Açıklama = SiparisNo + " numaralı sipariş ile alınan SMS bakiyesi";
                smshareket.OlusturanKullaniciId = KullaniciId;
                smshareket.OlusturmaTarih = DateTime.Now;
                smshareket.DegistirenKullaniciId = KullaniciId;
                smshareket.DegistirmeTarih = DateTime.Now;
                smshareket.Aktif = true;
                smshareket.Sil = false;
                dbContext.SmsHarekets.Add(smshareket);

                SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                smsbakiye.ToplamKontur = smsbakiye.Bakiye + SmsMiktar;
                smsbakiye.Bakiye = smsbakiye.Bakiye + SmsMiktar;
                smsbakiye.DegistirenKullaniciId = KullaniciId;
                smsbakiye.DegistirmeTarih = DateTime.Now;
            }
            else if (paket.PaketTip == "Lisans")
            {
                Lisanslar lisans = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId);
                //List<ModulSayfa> modulsayfa = dbContext.ModulSayfas.Where(x => x.FirmaId == FirmaId).ToList();
                // Lisans Süresi işlemleri yapılıyor...
                DateTime lbitis = lisans.LisansBitisTarihi;
                if (lbitis < DateTime.Now)
                {
                    lisans.LisansBitisTarihi = DateTime.Now.AddMonths(sure);
                }
                else
                {
                    lisans.LisansBitisTarihi = lbitis.AddMonths(sure);
                }
                lisans.LisansYenilemeTarihi = DateTime.Now;
                // Lisans Modül işlemleri yapılıyor...

                SmsMiktar = sure * paket.SMSMiktar;
                lisans.Dashboard = paket.Dashboard;
                lisans.RezervasyonIslemleri = paket.RezervasyonIslemleri;
                lisans.RezervasyonIslemleri_Randevular = paket.RezervasyonIslemleri_Randevular;
                lisans.RezervasyonIslemleri_RezervasyonListesi = paket.RezervasyonIslemleri_RezervasyonListesi;
                lisans.RezervasyonIslemleri_RezervasyonTakvimi = paket.RezervasyonIslemleri_RezervasyonTakvimi;
                lisans.RezervasyonIslemleri_RezervasyonTeklifleri = paket.RezervasyonIslemleri_RezervasyonTeklifleri;
                lisans.Muhasebe = paket.Muhasebe;
                lisans.Muhasebe_AlinanGelecekOdemeler = paket.Muhasebe_AlinanGelecekOdemeler;
                lisans.Muhasebe_GunlukIsler = paket.Muhasebe_GunlukIsler;
                lisans.Muhasebe_GelirGiderler = paket.Muhasebe_GelirGiderler;
                lisans.Muhasebe_KalanBakiyeler = paket.Muhasebe_KalanBakiyeler;
                lisans.Muhasebe_Kasa = paket.Muhasebe_Kasa;
                lisans.Muhasebe_Cariler = paket.Muhasebe_Cariler;
                lisans.Muhasebe_Cariler_CariHesapTakibi = paket.Muhasebe_Cariler_CariHesapTakibi;
                lisans.Muhasebe_Cariler_CariListesi = paket.Muhasebe_Cariler_CariListesi;
                lisans.Musteriler = paket.Musteriler;
                lisans.Musteriler_MusteriHesapTakibi = paket.Musteriler_MusteriHesapTakibi;
                lisans.Musteriler_MusteriListesi = paket.Musteriler_MusteriListesi;
                lisans.Musteriler_FotografYukle = paket.Musteriler_FotografYukle;
                lisans.Musteriler_MusteriMesajlari = paket.Musteriler_MusteriMesajlari;
                lisans.Stok = paket.Stok;
                lisans.Stok_DepoIslemleri = paket.Stok_DepoIslemleri;
                lisans.Stok_DepoIslemleri_DepodanYapilmisCikislar = paket.Stok_DepoIslemleri_DepodanYapilmisCikislar;
                lisans.Stok_DepoIslemleri_DepolarArasiTransfer = paket.Stok_DepoIslemleri_DepolarArasiTransfer;
                lisans.Stok_DepoIslemleri_DepoListesi = paket.Stok_DepoIslemleri_DepoListesi;
                lisans.Stok_DepoIslemleri_DepoStokDurumu = paket.Stok_DepoIslemleri_DepoStokDurumu;
                lisans.Stok_DepoIslemleri_DepoyaYapılmışGirisler = paket.Stok_DepoIslemleri_DepoyaYapılmışGirisler;
                lisans.Stok_Iade = paket.Stok_Iade;
                lisans.Stok_StokCikisi = paket.Stok_StokCikisi;
                lisans.Stok_StokGirisi = paket.Stok_StokGirisi;
                lisans.Stok_StokHareketleri = paket.Stok_StokHareketleri;
                lisans.Stok_StokKartlari = paket.Stok_StokKartlari;
                lisans.Raporlar = paket.Raporlar;
                lisans.Raporlar_GelecekOdemelerRaporu = paket.Raporlar_GelecekOdemelerRaporu;
                lisans.Raporlar_GelirGiderRaporlari = paket.Raporlar_GelirGiderRaporlari;
                lisans.Raporlar_RezervasyonIslemAdetleriRaporu = paket.Raporlar_RezervasyonIslemAdetleriRaporu;
                lisans.Raporlar_RezervasyonTurleriRaporu = paket.Raporlar_RezervasyonTurleriRaporu;
                lisans.Raporlar_HaftalikRapor = paket.Raporlar_HaftalikRapor;
                lisans.Raporlar_RezervasyonEkHizmetleriRaporu = paket.Raporlar_RezervasyonEkHizmetleriRaporu;
                lisans.Raporlar_CekimPaketleriRaporu = paket.Raporlar_CekimPaketleriRaporu;
                lisans.TelefonRehberi = paket.TelefonRehberi;
                lisans.TelefonRehberi_Rehber = paket.TelefonRehberi_Rehber;
                lisans.TelefonRehberi_ExceldenEkle = paket.TelefonRehberi_ExceldenEkle;
                lisans.Personeller = paket.Personeller;
                lisans.Personeller_PersonelListesi = paket.Personeller_PersonelListesi;
                lisans.Personeller_PersonelIzinleri = paket.Personeller_PersonelIzinleri;
                lisans.Personeller_PersonelOdemeleri = paket.Personeller_PersonelOdemeleri;
                lisans.Personeller_PersonelIsTakibi = paket.Personeller_PersonelIsTakibi;
                lisans.Tanimlar = paket.Tanimlar;
                lisans.Tanimlar_CekimPaketleri = paket.Tanimlar_CekimPaketleri;
                lisans.Tanimlar_EmailMetinleri = paket.Tanimlar_EmailMetinleri;
                lisans.Tanimlar_GelirGiderTurleri = paket.Tanimlar_GelirGiderTurleri;
                lisans.Tanimlar_GunlukIsKategorileri = paket.Tanimlar_GunlukIsKategorileri;
                lisans.Tanimlar_PersonelGorevleri = paket.Tanimlar_PersonelGorevleri;
                lisans.Tanimlar_RehberGruplari = paket.Tanimlar_RehberGruplari;
                lisans.Tanimlar_RezervasyonEkHizmetleri = paket.Tanimlar_RezervasyonEkHizmetleri;
                lisans.Tanimlar_RezervasyonTurleri = paket.Tanimlar_RezervasyonTurleri;
                lisans.Tanimlar_SmsMetinleri = paket.Tanimlar_SmsMetinleri;
                lisans.Tanimlar_SozlesmeSartlari = paket.Tanimlar_SozlesmeSartlari;
                lisans.Tanimlar_Sureler = paket.Tanimlar_Sureler;
                lisans.Tanimlar_TatilGunleri = paket.Tanimlar_TatilGunleri;
                lisans.Tanimlar_ZamanDilimleri = paket.Tanimlar_ZamanDilimleri;
                lisans.Ayarlar = paket.Ayarlar;
                lisans.Ayarlar_EmailGonderimAyarlari = paket.Ayarlar_EmailGonderimAyarlari;
                lisans.Ayarlar_EmailHesapAyarlari = paket.Ayarlar_EmailHesapAyarlari;
                lisans.Ayarlar_GenelAyarlar = paket.Ayarlar_GenelAyarlar;
                lisans.Ayarlar_ListelemeFiltreAyarlari = paket.Ayarlar_ListelemeFiltreAyarlari;
                lisans.Ayarlar_MusteriAyarlari = paket.Ayarlar_MusteriAyarlari;
                lisans.Ayarlar_RezervasyonAyarlari = paket.Ayarlar_RezervasyonAyarlari;
                lisans.Ayarlar_SmsGonderimAyarlari = paket.Ayarlar_SmsGonderimAyarlari;
                lisans.Ayarlar_SozlesmeCiktiAyarlari = paket.Ayarlar_SozlesmeCiktiAyarlari;
                lisans.Ayarlar_Personeller = paket.Ayarlar_Personeller;
                lisans.FirmaSubeIslemleri = paket.FirmaSubeIslemleri;
                lisans.FirmaSubeIslemleri_FirmaBilgileri = paket.FirmaSubeIslemleri_FirmaBilgileri;
                lisans.FirmaSubeIslemleri_SubeIslemleri = paket.FirmaSubeIslemleri_SubeIslemleri;
                lisans.KullaniciYetkiIslemleri = paket.KullaniciYetkiIslemleri;
                lisans.KullaniciYetkiIslemleri_KullaniciListesi = paket.KullaniciYetkiIslemleri_KullaniciListesi;
                lisans.KullaniciYetkiIslemleri_KullaniciYetkilendirme = paket.KullaniciYetkiIslemleri_KullaniciYetkilendirme;
                lisans.SmsGonder = paket.SmsGonder;
                lisans.SatinAl = paket.SatinAl;
                lisans.SatinAl_PaketSec = paket.SatinAl_PaketSec;
                lisans.SatinAl_Siparisler = paket.SatinAl_Siparisler;
                lisans.Destek = paket.Destek;
                lisans.Destek_DestekDetay = paket.Destek_DestekDetay;
                lisans.Destek_DestekTalepleri = paket.Destek_DestekTalepleri;
                lisans.SubeLimit = paket.SubeLimit;
                lisans.KullaniciLimit = paket.KullaniciLimit;
                lisans.PersonelLimit = paket.PersonelLimit;
                lisans.DegistirenKullaniciId = 1;
                lisans.DegistirmeTarih = DateTime.Now;

                #region Hediye SMS Yüklemesi
                SmsHareket smshareket = new SmsHareket();
                smshareket.FirmaId = FirmaId;
                smshareket.YuklemeTarihi = DateTime.Now;
                smshareket.Miktar = SmsMiktar;
                smshareket.Açıklama = SiparisNo + " numaralı sipariş ile alınan SMS bakiyesi";
                smshareket.OlusturanKullaniciId = KullaniciId;
                smshareket.OlusturmaTarih = DateTime.Now;
                smshareket.DegistirenKullaniciId = KullaniciId;
                smshareket.DegistirmeTarih = DateTime.Now;
                smshareket.Aktif = true;
                smshareket.Sil = false;
                dbContext.SmsHarekets.Add(smshareket);

                SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                smsbakiye.ToplamKontur = smsbakiye.Bakiye + SmsMiktar;
                smsbakiye.Bakiye = smsbakiye.Bakiye + SmsMiktar;
                smsbakiye.DegistirenKullaniciId = KullaniciId;
                smsbakiye.DegistirmeTarih = DateTime.Now;
                #endregion
            }
            else
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Lisans Güncelleme";
                hata.HataMesajı = "Lisasn güncelleme - PaketTip: Belirsiz ";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }

            try
            {
                dbContext.SaveChanges();
                siparis.Durum = 2; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                siparis.DegistirenKullaniciId = KullaniciId;
                siparis.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Ödeme onaylandı <br/> Sipariş Tamamlandı." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Lisasn Güncelleme";
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }
        }

    }
}