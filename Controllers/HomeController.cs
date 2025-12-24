using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FotografciTakipWeb.Models;
using System.Threading;
using FotografciTakipWeb.App_Settings;

namespace FotografciTakipWeb.Controllers
{
    public class HomeController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Home
        public ActionResult Index()
        {
            List<Il> iller = dbContext.Ils.Select(x => x).ToList();
            List<SatisFiyatlari> satisfiyatlari = dbContext.SatisFiyatlaris.Where(x => x.Aktif == true).ToList();
            ViewBag.SatisFiyatlari = satisfiyatlari;
            return View(iller);

        }

        [HttpPost]
        public ActionResult EmailKontrol()
        {
            string Email = Request["Email"];
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Email == Email);
            if (frm != null)
            {
                if (frm.Aktif == false)
                    return Json(new { Sonuc = false, Mesaj = "Firma Pasif", FirmaId = frm.Id }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Sonuc = false, Mesaj = "Email Kullanılıyor" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Sonuc = true, Mesaj = "Kayıt Yapılabilir" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FirmaAktifYap(int Id)
        {
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == Id);
            if (frm != null)
            {
                frm.Aktif = true;
                frm.DegistirenKullaniciId = 1;
                frm.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Firma Aktif oldu" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Sonuc = false, Mesaj = "Güncelleme başarısız." }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UyeOl()
        {
            string FirmaAdi = Request["FirmaAdi"];
            string Yetkili = Request["Yetkili"];
            string Email = Request["Email"];
            string CepTel = Request["CepTel"];
            int Il = Convert.ToInt32(Request["iller"]);
            int Ilce = Convert.ToInt32(Request["ilceler"]);
            string Sifre = Request["Sifre"];
            TelefonDüzelt td = new TelefonDüzelt();
            if (!string.IsNullOrEmpty(CepTel)) { CepTel = td.düzelt(CepTel); }

            #region Firma Kaydı
            Firma frm = new Firma();
            frm.FirmaAdi = FirmaAdi;
            frm.Yetkili = Yetkili;
            frm.VergiDairesi = "";
            frm.VergiNo = "";
            frm.TCKimlikNo = "";
            frm.Email = Email;
            frm.CepTel = CepTel;
            frm.SabitTel = "";
            frm.Fax = "";
            frm.IlId = Il;
            frm.IlceId = Ilce;
            frm.Adres = "";
            frm.WebSitesi = "";
            frm.Facebook = "";
            frm.Instagram = "";
            frm.Twitter = "";
            frm.FirmaHakkinda = "";
            frm.ResimId = 3;// default logo id si olacak
            frm.AcilisBit = true;
            frm.DuyuruBit = true;
            frm.OlusturanKullaniciId = 1;
            frm.OlusturmaTarih = DateTime.Now;
            frm.DegistirenKullaniciId = 1;
            frm.DegistirmeTarih = DateTime.Now;
            frm.Sil = false;
            frm.Aktif = true;
            dbContext.Firmas.Add(frm);
            dbContext.SaveChanges();
            #endregion
            #region Trigger'e Alındı
            // Firma tablosuna Firma bilgileri kaydedildikten sonran Lisanslar tablosuna demo lisans oluşturulor.
            //#region Lisans Bilgileri Kaydı
            //Lisanslar lisans = new Lisanslar();
            //lisans.FirmaId = frm.Id;
            //lisans.KayitTarihi = DateTime.Now;
            //lisans.LisansBaslangicTarihi = DateTime.Now;
            //lisans.LisansYenilemeTarihi = DateTime.Now;
            //lisans.LisansBitisTarihi = DateTime.Now.AddMonths(1);
            //lisans.Dashboard = true;
            //lisans.RezervasyonIslemleri = true;
            //lisans.RezervasyonIslemleri_Randevular = true;
            //lisans.RezervasyonIslemleri_RezervasyonListesi = true;
            //lisans.RezervasyonIslemleri_RezervasyonTakvimi = true;
            //lisans.RezervasyonIslemleri_RezervasyonTeklifleri = true;
            //lisans.Muhasebe = true;
            //lisans.Muhasebe_AlinanGelecekOdemeler = true;
            //lisans.Muhasebe_GunlukIsler = true;
            //lisans.Muhasebe_GelirGiderler = true;
            //lisans.Muhasebe_KalanBakiyeler = false;
            //lisans.Muhasebe_Kasa = true;
            //lisans.Muhasebe_Cariler = true;
            //lisans.Muhasebe_Cariler_CariHesapTakibi = true;
            //lisans.Muhasebe_Cariler_CariListesi = true;
            //lisans.Musteriler = true;
            //lisans.Musteriler_MusteriHesapTakibi = true;
            //lisans.Musteriler_MusteriListesi = true;
            //lisans.Musteriler_FotografYukle = true;
            //lisans.Musteriler_MusteriMesajlari = true;
            //lisans.Stok = false;
            //lisans.Stok_DepoIslemleri = false;
            //lisans.Stok_DepoIslemleri_DepodanYapilmisCikislar = false;
            //lisans.Stok_DepoIslemleri_DepolarArasiTransfer = false;
            //lisans.Stok_DepoIslemleri_DepoListesi = false;
            //lisans.Stok_DepoIslemleri_DepoStokDurumu = false;
            //lisans.Stok_DepoIslemleri_DepoyaYapılmışGirisler = false;
            //lisans.Stok_Iade = false;
            //lisans.Stok_StokCikisi = false;
            //lisans.Stok_StokGirisi = false;
            //lisans.Stok_StokHareketleri = false;
            //lisans.Stok_StokKartlari = false;
            //lisans.Raporlar = true;
            //lisans.Raporlar_GelecekOdemelerRaporu = true;
            //lisans.Raporlar_GelirGiderRaporlari = true;
            //lisans.Raporlar_RezervasyonIslemAdetleriRaporu = true;
            //lisans.Raporlar_RezervasyonTurleriRaporu = true;
            //lisans.TelefonRehberi = true;
            //lisans.TelefonRehberi_Rehber = true;
            //lisans.TelefonRehberi_ExceldenEkle = false;
            //lisans.Personeller = true;
            //lisans.Personeller_PersonelListesi = true;
            //lisans.Personeller_PersonelIzinleri = true;
            //lisans.Personeller_PersonelTakibi = true;
            //lisans.Tanimlar = true;
            //lisans.Tanimlar_CekimPaketleri = true;
            //lisans.Tanimlar_EmailMetinleri = true;
            //lisans.Tanimlar_GelirGiderTurleri = true;
            //lisans.Tanimlar_GunlukIsKategorileri = true;
            //lisans.Tanimlar_PersonelGorevleri = true;
            //lisans.Tanimlar_RehberGruplari = true;
            //lisans.Tanimlar_RezervasyonEkHizmetleri = true;
            //lisans.Tanimlar_RezervasyonTurleri = true;
            //lisans.Tanimlar_SmsMetinleri = true;
            //lisans.Tanimlar_SozlesmeSartlari = true;
            //lisans.Tanimlar_Sureler = true;
            //lisans.Tanimlar_TatilGunleri = true;
            //lisans.Tanimlar_ZamanDilimleri = true;
            //lisans.Ayarlar = true;
            //lisans.Ayarlar_EmailGonderimAyarlari = true;
            //lisans.Ayarlar_EmailHesapAyarlari = true;
            //lisans.Ayarlar_GenelAyarlar = true;
            //lisans.Ayarlar_ListelemeFiltreAyarlari = true;
            //lisans.Ayarlar_MusteriAyarlari = true;
            //lisans.Ayarlar_RezervasyonAyarlari = true;
            //lisans.Ayarlar_SmsGonderimAyarlari = true;
            //lisans.Ayarlar_SozlesmeCiktiAyarlari = true;
            //lisans.FirmaSubeIslemleri = true;
            //lisans.FirmaSubeIslemleri_FirmaBilgileri = true;
            //lisans.FirmaSubeIslemleri_SubeIslemleri = true;
            //lisans.KullaniciYetkiIslemleri = true;
            //lisans.KullaniciYetkiIslemleri_KullaniciListesi = true;
            //lisans.KullaniciYetkiIslemleri_KullaniciYetkilendirme = true;
            //lisans.SmsGonder = true;
            //lisans.SatinAl = true;
            //lisans.SatinAl_PaketSec = true;
            //lisans.SatinAl_Siparisler = true;
            //lisans.SubeLimit = "1";
            //lisans.KullaniciLimit = "5";
            //lisans.PersonelLimit = "5";
            //lisans.Paket = "Demo";
            //lisans.OlusturanKullaniciId = 1;
            //lisans.OlusturmaTarih = DateTime.Now;
            //lisans.DegistirenKullaniciId = 1;
            //lisans.DegistirmeTarih = DateTime.Now;
            //lisans.Aktif = true;
            //lisans.Sil = false;
            //dbContext.Lisanslars.Add(lisans);
            //dbContext.SaveChanges();
            //#endregion
            //// Yeni Firmanın yetkileri için ModülSayfa Tablosu ayarlanıyor.
            //#region Sayfa Yetkileri
            ////ModulSayfa lisansmodul = new ModulSayfa();
            ////List<ModulSayfa> modulsayfa = dbContext.ModulSayfas.Where(x => x.FirmaId == 1 && x.Aktif == true && x.Sil == false).ToList();
            ////foreach (var sayfa in modulsayfa)
            ////{
            ////    if (sayfa.ModulId==2)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Rezervasyon İşlemleri" && x.FirmaId==frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 3)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Muhasebe" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 4)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Müşteriler" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 5)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Stok" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 6)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Raporlar" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 7)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Telefon Rehberi" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 8)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Personeller" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 9)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Tanımlar" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 10)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Ayarlar" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 11)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Firma/Şube İşlemleri" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 12)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Kullanıcı/Yetki İşlemleri" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 13)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "SMS Gönder" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 14)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Satın Al" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 15)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Destek" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 16)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Ek Modül 1" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 17)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Ek Modül 2" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 18)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Ek Modül 3" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else if (sayfa.ModulId == 19)
            ////    {
            ////        long s1 = dbContext.ModulSayfas.FirstOrDefault(x => x.SayfaAdi == "Ek Modül 4" && x.FirmaId == frm.Id).Id;
            ////        lisansmodul.ModulId = s1;
            ////    }
            ////    else
            ////        lisansmodul.ModulId = sayfa.ModulId;

            ////    lisansmodul.FirmaId = frm.Id;
            ////    lisansmodul.Sira = sayfa.Sira;
            ////    lisansmodul.SayfaAdi = sayfa.SayfaAdi;
            ////    lisansmodul.SayfaYetkiAktif = sayfa.SayfaYetkiAktif;
            ////    lisansmodul.KayitDetayiAktif = sayfa.KayitDetayiAktif;
            ////    lisansmodul.KayitDuzenleAktif = sayfa.KayitDuzenleAktif;
            ////    lisansmodul.KayitEkleAktif = sayfa.KayitEkleAktif;
            ////    lisansmodul.KayitSilAktif = sayfa.KayitSilAktif;
            ////    lisansmodul.Yazdirma = sayfa.Yazdirma;
            ////    lisansmodul.SMSGonder = sayfa.SMSGonder;
            ////    lisansmodul.OlusturanKullaniciId = 1;
            ////    lisansmodul.OlusturmaTarih = DateTime.Now;
            ////    lisansmodul.DegistirenKullaniciId = 1;
            ////    lisansmodul.DegistirmeTarih = DateTime.Now;
            ////    lisansmodul.Aktif = true;
            ////    lisansmodul.Sil = false;
            ////    dbContext.ModulSayfas.Add(lisansmodul);
            ////    dbContext.SaveChanges();
            ////}
            //#endregion
            //// Hediye Sms Yüklemesi
            //#region Hediye SMS
            //SmsHareket smshareket = new SmsHareket();
            //smshareket.FirmaId = frm.Id;
            //smshareket.YuklemeTarihi = DateTime.Now;
            //smshareket.Miktar = 500;
            //smshareket.Açıklama = "İlk üyeliğe hediye 500 SMS bakiyesi";
            //smshareket.OlusturanKullaniciId = 1;
            //smshareket.OlusturmaTarih = DateTime.Now;
            //smshareket.DegistirenKullaniciId = 1;
            //smshareket.DegistirmeTarih = DateTime.Now;
            //smshareket.Aktif = true;
            //smshareket.Sil = false;
            //dbContext.SmsHarekets.Add(smshareket);

            //SmsBakiye smsbakiye = new SmsBakiye();
            //smsbakiye.FirmaId = frm.Id;
            //smsbakiye.ToplamKontur = 500;
            //smsbakiye.Bakiye = 500;
            //smsbakiye.OlusturanKullaniciId = 1;
            //smsbakiye.OlusturmaTarih = DateTime.Now;
            //smsbakiye.DegistirenKullaniciId = 1;
            //smsbakiye.DegistirmeTarih = DateTime.Now;
            //smsbakiye.Aktif = true;
            //smsbakiye.Sil = false;
            //dbContext.SmsBakiyes.Add(smsbakiye);
            //dbContext.SaveChanges();

            //#endregion
            #endregion
            // Firma tablosuna Firma bilgileri kaydedildikten sonra Firma ile aynı isimli bir ŞUBE oluşturulacak. 
            #region Sube Kaydı
            Sube sb = new Sube();
            sb.FirmaId = frm.Id;
            sb.SubeAdi = frm.FirmaAdi;
            sb.Yetkili = frm.Yetkili;
            sb.TCKimlikNo = frm.TCKimlikNo;
            sb.Email = frm.Email;
            sb.CepTel = frm.CepTel;
            sb.SabitTel = frm.SabitTel;
            sb.Fax = frm.Fax;
            sb.IlId = frm.IlId;
            sb.IlceId = frm.IlceId;
            sb.Adres = frm.Adres;
            sb.WebSitesi = frm.WebSitesi;
            sb.Facebook = frm.Facebook;
            sb.Twitter = frm.Twitter;
            sb.Instagram = frm.Instagram;
            sb.SubeHakkinda = frm.FirmaHakkinda;
            sb.OlusturanKullaniciId = 1;
            sb.OlusturmaTarih = DateTime.Now;
            sb.DegistirenKullaniciId = 1;
            sb.DegistirmeTarih = DateTime.Now;
            sb.Aktif = true;
            sb.Sil = false;
            sb.KilitBit = true;
            sb.Notlar = "Ana Şube";
            dbContext.Subes.Add(sb);
            dbContext.SaveChanges();
            #endregion
            // Firma tablosuna Firma bilgileri kaydedildikten sonra Personel Görevleri Tablosuna "Firma Sahibi" görevi ekleniyor.
            #region Trigger'e Alındı
            //#region Personel Görevi Tanımı
            //PersonelGorevleri gorev = new PersonelGorevleri();
            //gorev.FirmaId = frm.Id;
            //gorev.Gorev = "Firma Sahibi";
            //gorev.KilitBit = true;
            //gorev.OlusturanKullaniciId = 1;
            //gorev.OlusturmaTarih = DateTime.Now;
            //gorev.DegistirenKullaniciId = 1;
            //gorev.DegistirmeTarih = DateTime.Now;
            //gorev.Aktif = true;
            //gorev.Sil = false;
            //dbContext.PersonelGorevleris.Add(gorev);
            //dbContext.SaveChanges();
            //#endregion
            //#region Personel Kaydı
            //Personel pers = new Personel();
            //pers.FirmaId = frm.Id;
            //pers.GorevId = gorev.Id;
            //pers.SubeId = sb.Id;
            //pers.TCKimlikNo = frm.TCKimlikNo;
            //pers.AdiSoyadi = frm.Yetkili;
            //pers.BaslamaTarihi = DateTime.Now;
            //pers.BitisTarihi = Convert.ToDateTime("01.01.2500");
            //pers.CepTel = frm.CepTel;
            //pers.SabitTel = frm.SabitTel;
            //pers.Email = frm.Email;
            //pers.Adres = frm.Adres;
            //pers.GorevliSubeler = sb.Id.ToString();
            //pers.CalismaSekli = "Tam Zamanlı (Full Time)";
            //pers.YillikIzinHakki = 0;
            //pers.ToplamIzin = 0;
            //pers.Ucret = 0;
            //pers.SMSKabul = true;
            //pers.EmailKabul = true;
            //pers.OlusturanKullaniciId = 1;
            //pers.OlusturmaTarih = DateTime.Now;
            //pers.DegistirenKullaniciId = 1;
            //pers.DegistirmeTarih = DateTime.Now;
            //pers.Aktif = true;
            //pers.Sil = false;
            //pers.KilitBit = true;
            //dbContext.Personels.Add(pers);
            //dbContext.SaveChanges();
            //#endregion
            //#region Personel Şube Yetkilendirme
            //SubeToPersonel SubePersonel = new SubeToPersonel();
            //SubePersonel.PersonelId = pers.Id;
            //SubePersonel.SubeId = sb.Id;
            //dbContext.SubeToPersonels.Add(SubePersonel);
            //dbContext.SaveChanges();
            //#endregion
            #endregion
            // Firma tablosuna Firma bilgileri kaydedildikten sonra Kullanıcı Tablosuna kullanıcı oluşturuyor.
            long GorevId = dbContext.PersonelGorevleris.FirstOrDefault(x => x.FirmaId == frm.Id && x.Gorev == "Firma Sahibi").Id;
            #region Kullanıcı Kaydı
            MD5Sifreleme sifrele = new MD5Sifreleme();
            Kullanici kl = new Kullanici();
            kl.AdSoyad = frm.Yetkili;
            kl.OlusturanKullaniciId = 1;
            kl.OlusturmaTarih = DateTime.Now;
            kl.DegistirenKullaniciId = 1;
            kl.DegistirmeTarih = DateTime.Now;
            kl.Email = frm.Email;
            kl.CepTel = frm.CepTel;
            kl.ResimId = 2;
            kl.RolId = 2;
            kl.FirmaId = frm.Id;
            kl.GeciciSifre = "7nyGUP";
            kl.SifreHash = sifrele.Sifrele(Sifre);
            kl.Notlar = "Firma Sahibi/Yönetici";
            kl.YetkiliSubeler = sb.Id.ToString();
            kl.Aktif = true;
            kl.Sil = false;
            kl.KilitBit = true;
            kl.GorevId = GorevId;
            dbContext.Kullanicis.Add(kl);
            try
            {
                dbContext.SaveChanges();
                sb.YetkiliKullanicilar = kl.Id.ToString();
            }
            catch (Exception e)
            {

                HataLoglari hata = new HataLoglari();
                hata.FirmaId = 1;
                hata.SubeId = 1;
                hata.Islem = "Firma oluştur. Kullanıcı ekle";
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

            #endregion
            // Şube ve Kullanıcı oluşturulduktan sonra Kullanıcı ana şube için yetkilendiriliyor.
            #region Kullanıcı Şube İlişkilendirme
            SubeToKullanici SubeKullanici = new SubeToKullanici();
            SubeKullanici.KullaniciId = kl.Id;
            SubeKullanici.SubeId = sb.Id;
            dbContext.SubeToKullanicis.Add(SubeKullanici);
            dbContext.SaveChanges();
            #endregion
            #region Trigger'e Alındı
            //// Kullanıcının yetkileri tanımlanıyor.
            //#region Kullanıcı Yetkileri
            //KullaniciYetki ky = new KullaniciYetki();
            //List<ModulSayfa> modulsayfalar = dbContext.ModulSayfas.Select(x => x).ToList();  // Tüm sayfalar yükleniyor. İleride eksik sayfalar tamalandığında Kullanıcı yetki tablosuna eklenmemiş kayıtlar sorun çıkaracağı için herhangi bir kısıtlama olmadan tüm kayıtlar aynen aktarıldı.
            //foreach (var sayfa in modulsayfalar)
            //{
            //    ky.FirmaId = frm.Id;
            //    ky.KullaniciId = kl.Id;
            //    ky.SayfaId = sayfa.Id;
            //    ky.SayfaYetki = Convert.ToBoolean(sayfa.SayfaYetkiAktif);
            //    ky.KayitDetayi = Convert.ToBoolean(sayfa.KayitDetayiAktif);
            //    ky.KayitDuzenle = Convert.ToBoolean(sayfa.KayitDuzenleAktif);
            //    ky.KayitEkle = Convert.ToBoolean(sayfa.KayitEkleAktif);
            //    ky.KayitSil = Convert.ToBoolean(sayfa.KayitSilAktif);
            //    ky.Yazdir = Convert.ToBoolean(sayfa.Yazdirma);
            //    ky.SmsGonder = Convert.ToBoolean(sayfa.SMSGonder);
            //    ky.OlusturanKullaniciId = 1;
            //    ky.OlusturmaTarih = DateTime.Now;
            //    ky.DegistirenKullaniciId = 1;
            //    ky.DegistirmeTarih = DateTime.Now;
            //    ky.Aktif = true;
            //    ky.Sil = false;
            //    dbContext.KullaniciYetkis.Add(ky);
            //    dbContext.SaveChanges();
            //}
            //#endregion
            #endregion
            #region Trigger'e Alındı
            ////Süreçlerde değiştirilemeyecek süreç "Sözleşme Yapıldı" ayarlanıyor.
            //#region Süreç Tanımlama
            //Surecler surec = new Surecler();
            //surec.SurecAdi = "Sözleşme Yapıldı";
            //surec.SMSBildirim = false;
            //surec.FirmaId = frm.Id;
            //surec.Sira = 1;
            //surec.KilitBit = true;
            //surec.OlusturanKullaniciId = 1;
            //surec.OlusturmaTarih = DateTime.Now;
            //surec.DegistirenKullaniciId = 1;
            //surec.DegistirmeTarih = DateTime.Now;
            //surec.Aktif = true;
            //surec.Sil = false;
            //dbContext.Sureclers.Add(surec);
            //dbContext.SaveChanges();
            //#endregion
            //// Firma tablosuna Firma bilgileri kaydedildikten sonra SozlesmeCiktiAyarlari, FiltreAyarlari, GenelAyarlar Tablolarına varsayılan bir Ayar oluşturuyor.
            //#region Genel Ayarlar
            //AyarlarGenel genelayar = new AyarlarGenel();
            //genelayar.FirmaId = frm.Id;
            //genelayar.CalismaGunuCumartesi = true;
            //genelayar.CalismaGunuPazar = false;
            //genelayar.CariRehberKayit = true;
            //genelayar.MusteriRehberKayit = true;
            //genelayar.PersonelRehberKayit = true;
            //genelayar.GelinDamatRehberKayit = true;
            //genelayar.AnneBabaRehberKayit = true;
            //genelayar.RezervasyonYetkiliRehberKayit = true;
            //genelayar.KonturUyariVer = false;
            //genelayar.KonturUyariMiktari = 0;
            //genelayar.OlusturanKullaniciId = 1;
            //genelayar.OlusturmaTarih = DateTime.Now;
            //genelayar.DegistirenKullaniciId = 1;
            //genelayar.DegistirmeTarih = DateTime.Now;
            //genelayar.Aktif = true;
            //genelayar.Sil = false;
            //dbContext.AyarlarGenels.Add(genelayar);
            //dbContext.SaveChanges();
            //#endregion
            //#region Sözleşme Ayarlar
            //AyarlarSozlesmeCikti sozlesmeayar = new AyarlarSozlesmeCikti();
            //sozlesmeayar.FirmaId = frm.Id;
            //sozlesmeayar.LogoGoster = true;
            //sozlesmeayar.PaketlerGoster = true;
            //sozlesmeayar.EkHizmetlerGoster = true;
            //sozlesmeayar.CekimRandevulariGoster = true;
            //sozlesmeayar.YapilanOdemelerGoster = true;
            //sozlesmeayar.KalanOdemelerGoster = true;
            //sozlesmeayar.CepTelefonuGoster = true;
            //sozlesmeayar.SabitTelefonGoster = true;
            //sozlesmeayar.MusteriKoduSifreGoster = true;
            //sozlesmeayar.FaxGoster = true;
            //sozlesmeayar.OlusturanKullaniciId = 1;
            //sozlesmeayar.OlusturmaTarih = DateTime.Now;
            //sozlesmeayar.DegistirenKullaniciId = 1;
            //sozlesmeayar.DegistirmeTarih = DateTime.Now;
            //dbContext.AyarlarSozlesmeCiktis.Add(sozlesmeayar);
            //dbContext.SaveChanges();
            //#endregion
            //#region Filtre Ayarlar
            //AyarlarFiltre filterayar = new AyarlarFiltre();
            //filterayar.FirmaId = frm.Id;
            //filterayar.GunlukIsler = "0";
            //filterayar.RezervasyonListesi = "5";
            //filterayar.RezervasyonTeklifleri = "5";
            //filterayar.Randevular = "4";
            //filterayar.GelirlerGiderler = "4";
            //filterayar.AlinanGelecekOdemeler = "4";
            //filterayar.CariHesapTakibi = "4";
            //filterayar.Kasa = "0";
            //filterayar.MusteriHesapTakibi = "4";
            //filterayar.Siparisler = "5";
            //filterayar.OlusturanKullaniciId = 1;
            //filterayar.OlusturmaTarih = DateTime.Now;
            //filterayar.DegistirenKullaniciId = 1;
            //filterayar.DegistirmeTarih = DateTime.Now;
            //filterayar.Aktif = true;
            //filterayar.Sil = false;
            //dbContext.AyarlarFiltres.Add(filterayar);
            //dbContext.SaveChanges();
            //#endregion
            //#region Mail Gönderim Ayarlar
            //AyarlarMailGonderim mailgonderimayar = new AyarlarMailGonderim();
            //mailgonderimayar.FirmaId = frm.Id;
            //mailgonderimayar.RezervasyonTarihiBilgiMaili = 0;
            //mailgonderimayar.RezervasyonTarihiBilgiGonderimSuresi = 0;
            //mailgonderimayar.RezervasyonTarihiHatirlatmaMaili = 0;
            //mailgonderimayar.RezervasyonTarihiHatirlatmaGonderimSuresi = 0;
            //mailgonderimayar.OpsiyonTarihiBilgiMaili = 0;
            //mailgonderimayar.OpsiyonTarihiBilgiGonderimSuresi = 0;
            //mailgonderimayar.OpsiyonTarihiHatirlatmaMaili = 0;
            //mailgonderimayar.OpsiyonTarihiHatirlatmaGonderimSuresi = 0;
            //mailgonderimayar.MusteriCekimRandevusuBilgiMaili = 0;
            //mailgonderimayar.MusteriCekimRandevusuBilgiGonderimSuresi = 0;
            //mailgonderimayar.MusteriCekimRandevusuHatirlatmaMaili = 0;
            //mailgonderimayar.MusteriCekimRandevusuHatirlatmaGonderimSuresi = 0;
            //mailgonderimayar.PersonelRandevuBilgiMaili = 0;
            //mailgonderimayar.PersonelRandevuBilgiGonderimSuresi = 0;
            //mailgonderimayar.PersonelRandevuHatirlatmaMaili = 0;
            //mailgonderimayar.PersonelRandevuHatirlatmaGonderimSuresi = 0;
            //mailgonderimayar.MusteriOdemeBilgiMaili = 0;
            //mailgonderimayar.MusteriOdemeBilgiGonderimSuresi = 0;
            //mailgonderimayar.MusteriOdemeHatirlatmaMaili = 0;
            //mailgonderimayar.MusteriOdemeHatirlatmaGonderimSuresi = 0;
            //mailgonderimayar.FotografSecimiBilgiMailiFirma = 0;
            //mailgonderimayar.FotografSecimiBilgiMailiFirmaGonderimSuresi = 0;
            //mailgonderimayar.FotografSecimiBilgiMailiMusteri = 0;
            //mailgonderimayar.FotografSecimiBilgiMailiMusteriGonderimSuresi = 0;
            //mailgonderimayar.FotografSecimiHatirlatmaGonderimSuresi = 0;
            //mailgonderimayar.FotografSecimiHatirlatmaMaili = 0;
            //mailgonderimayar.EvlilikYildonumuTebrikGonderimSuresi = 0;
            //mailgonderimayar.EvlilikYildonumuTebrikMaili = 0;
            //mailgonderimayar.CariAlacakHatirlatmaGonderimSuresi = 0;
            //mailgonderimayar.CariAlacakHatirlatmaMaili = 0;
            //mailgonderimayar.CariTahsilatBilgiGonderimSuresi = 0;
            //mailgonderimayar.CariTahsilatBilgiMaili = 0;
            //mailgonderimayar.CariyeYapilanOdemeBilgiGonderimSuresi = 0;
            //mailgonderimayar.CariyeYapilanOdemeBilgiMaili = 0;
            //mailgonderimayar.GunlukIsOdemeBilgiGonderimSuresi = 0;
            //mailgonderimayar.GunlukIsOdemeBilgiMaili = 0;
            //mailgonderimayar.OlusturanKullaniciId = 1;
            //mailgonderimayar.OlusturmaTarih = DateTime.Now;
            //mailgonderimayar.DegistirenKullaniciId = 1;
            //mailgonderimayar.DegistirmeTarih = DateTime.Now;
            //mailgonderimayar.Aktif = true;
            //mailgonderimayar.Sil = false;
            //dbContext.AyarlarMailGonderims.Add(mailgonderimayar);
            //dbContext.SaveChanges();
            //#endregion
            //#region SMS Gönderim Ayarlar
            //AyarlarSmsGonderim smsgonderimayar = new AyarlarSmsGonderim();
            //smsgonderimayar.FirmaId = frm.Id;
            //smsgonderimayar.RezervasyonTarihiBilgiMesaji = 0;
            //smsgonderimayar.RezervasyonTarihiBilgiGonderimSuresi = 0;
            //smsgonderimayar.RezervasyonTarihiHatirlatmaMesaji = 0;
            //smsgonderimayar.RezervasyonTarihiHatirlatmaGonderimSuresi = 0;
            //smsgonderimayar.OpsiyonTarihiBilgiMesaji = 0;
            //smsgonderimayar.OpsiyonTarihiBilgiGonderimSuresi = 0;
            //smsgonderimayar.OpsiyonTarihiHatirlatmaMesaji = 0;
            //smsgonderimayar.OpsiyonTarihiHatirlatmaGonderimSuresi = 0;
            //smsgonderimayar.MusteriCekimRandevusuBilgiMesaji = 0;
            //smsgonderimayar.MusteriCekimRandevusuBilgiGonderimSuresi = 0;
            //smsgonderimayar.MusteriCekimRandevusuHatirlatmaMesaji = 0;
            //smsgonderimayar.MusteriCekimRandevusuHatirlatmaGonderimSuresi = 0;
            //smsgonderimayar.PersonelRandevuBilgiMesaji = 0;
            //smsgonderimayar.PersonelRandevuBilgiGonderimSuresi = 0;
            //smsgonderimayar.PersonelRandevuHatirlatmaMesaji = 0;
            //smsgonderimayar.PersonelRandevuHatirlatmaGonderimSuresi = 0;
            //smsgonderimayar.MusteriOdemeBilgiMesaji = 0;
            //smsgonderimayar.MusteriOdemeBilgiGonderimSuresi = 0;
            //smsgonderimayar.MusteriOdemeHatirlatmaMesaji = 0;
            //smsgonderimayar.MusteriOdemeHatirlatmaGonderimSuresi = 0;
            //smsgonderimayar.FotografSecimiBilgiMesajiFirma = 0;
            //smsgonderimayar.FotografSecimiBilgiMesajiFirmaGonderimSuresi = 0;
            //smsgonderimayar.FotografSecimiBilgiMesajiMusteri = 0;
            //smsgonderimayar.FotografSecimiBilgiMesajiMusteriGonderimSuresi = 0;
            //smsgonderimayar.FotografSecimiHatirlatmaGonderimSuresi = 0;
            //smsgonderimayar.FotografSecimiHatirlatmaMesaji = 0;
            //smsgonderimayar.EvlilikYildonumuTebrikGonderimSuresi = 0;
            //smsgonderimayar.EvlilikYildonumuTebrikMesaji = 0;
            //smsgonderimayar.CariAlacakHatirlatmaGonderimSuresi = 0;
            //smsgonderimayar.CariAlacakHatirlatmaMesaji = 0;
            //smsgonderimayar.CariTahsilatBilgiGonderimSuresi = 0;
            //smsgonderimayar.CariTahsilatBilgiMesaji = 0;
            //smsgonderimayar.CariyeYapilanOdemeBilgiGonderimSuresi = 0;
            //smsgonderimayar.CariyeYapilanOdemeBilgiMesaji = 0;
            //smsgonderimayar.GunlukIsOdemeBilgiGonderimSuresi = 0;
            //smsgonderimayar.GunlukIsOdemeBilgiMesaji = 0;
            //smsgonderimayar.OlusturanKullaniciId = 1;
            //smsgonderimayar.OlusturmaTarih = DateTime.Now;
            //smsgonderimayar.DegistirenKullaniciId = 1;
            //smsgonderimayar.DegistirmeTarih = DateTime.Now;
            //smsgonderimayar.Aktif = true;
            //smsgonderimayar.Sil = false;
            //dbContext.AyarlarSmsGonderims.Add(smsgonderimayar);
            //dbContext.SaveChanges();
            //#endregion
            //#region Müşteri Panel Ayarları
            //AyarlarMusteri musteriayar = new AyarlarMusteri();
            //musteriayar.FirmaId = frm.Id;
            //musteriayar.OdemeleriGor = true;
            //musteriayar.RezervasyonGor = true;
            //musteriayar.TeklifleriGor = true;
            //musteriayar.SozlesmeYazdir = true;
            //musteriayar.OdemeMakbuzYazdir = true;
            //musteriayar.MesajGonder = true;
            //musteriayar.FotografSecim = true;
            //musteriayar.OlusturanKullaniciId = 1;
            //musteriayar.OlusturmaTarih = DateTime.Now;
            //musteriayar.DegistirenKullaniciId = 1;
            //musteriayar.DegistirmeTarih = DateTime.Now;
            //musteriayar.Aktif = true;
            //musteriayar.Sil = false;
            //dbContext.AyarlarMusteris.Add(musteriayar);
            //dbContext.SaveChanges();
            //#endregion
            //#region Rezervasyon Ayarları
            //AyarlarRezervasyon rezervasyonayar = new AyarlarRezervasyon();
            //rezervasyonayar.FirmaId = frm.Id;
            //rezervasyonayar.PersonelGorevliTakibi = true;
            //rezervasyonayar.PersonelIzinTakibi = true;
            //rezervasyonayar.TatilGunuTakibi = true; ;
            //rezervasyonayar.GunuGecenTeklifOpsiyonIptal = true;
            //rezervasyonayar.OlusturanKullaniciId = 1;
            //rezervasyonayar.OlusturmaTarih = DateTime.Now;
            //rezervasyonayar.DegistirenKullaniciId = 1;
            //rezervasyonayar.DegistirmeTarih = DateTime.Now;
            //rezervasyonayar.Aktif = true;
            //rezervasyonayar.Sil = false;
            //dbContext.AyarlarRezervasyons.Add(rezervasyonayar);
            //dbContext.SaveChanges();
            //#endregion
            //#region Müşterisiz İşlem Varsayılan Müşteri Kaydı
            //App_Settings.SifreOlustur msifre = new App_Settings.SifreOlustur();
            //long musteriId = 0;
            //string mkod;
            //List<Musteri> mm = dbContext.Musteris.Where(x => x.FirmaId == frm.Id).Select(x => x).ToList();
            //if (mm.Count > 0)
            //{
            //    musteriId = dbContext.Musteris.Max(x => x.Id);
            //}
            //musteriId = musteriId + 1;
            //mkod = frm.Id.ToString() + "0" + musteriId.ToString();
            //Musteri musteri = new Musteri();
            //musteri.MusteriKodu = Convert.ToInt64(mkod);
            //musteri.TCKimlikNo = "11111111111";
            //musteri.AdiSoyadi = "-- Müşterisiz İşlem --";
            //musteri.Sifre = msifre.sifreolustur(8);
            //musteri.FirmaId = frm.Id;
            //musteri.SubeId = sb.Id;
            //musteri.CepTel = "00000000000";
            //musteri.SMSKabul = false;
            //musteri.KilitBit = true;
            //musteri.MusteriPanelGirisYetki = null;
            //musteri.FotografSecimDurum = null;
            //musteri.OlusturanKullaniciId = 1;
            //musteri.OlusturmaTarih = DateTime.Now;
            //musteri.DegistirenKullaniciId = 1;
            //musteri.DegistirmeTarih = DateTime.Now;
            //musteri.Aktif = true;
            //musteri.Sil = false;
            //dbContext.Musteris.Add(musteri);
            //dbContext.SaveChanges();
            //#endregion

            //#region Sözleşme Şartları
            //SozlesmeSartlari ss = new SozlesmeSartlari();
            //ss.SozlesmeSartlari1 = "<p style='margin-left:40px; text-align:center'><span style='font-size:20px'><strong>- &Ouml;RNEK S&Ouml;ZLEŞME METNİ -</strong></span></p><p style='margin-left:40px'><strong>&Ccedil;EKİM VE ALB&Uuml;M DETAYLARI</strong></p><ol>	<li>Fotoğrafı &ccedil;ekilecek &ccedil;iftin &ouml;nceden belirlenen mekan, tarih ve saatinde her şeyiyle hazır olarak bulunmaları gerekmektedir.</li><li>Alb&uuml;m pozları i&ccedil;in &ccedil;ekilen fotoğraflar d&uuml;zg&uuml;nleri ayrıldıktan sonra en iyileri retouch, renk ayarlama ve &ouml;zel işlemlerden ge&ccedil;irilerek y&uuml;ksek &ccedil;&ouml;z&uuml;n&uuml;rl&uuml;kl&uuml; halleriyle dvd veya usb bellek i&ccedil;erisinde teslim edilir. Bu s&uuml;re &ccedil;ekimden sonra en erken 15, en ge&ccedil; 45 g&uuml;nd&uuml;r.</li><li>Fotoğrafların işlenmesi, alb&uuml;m tasarımı ve son şekilleri &ccedil;ifte sunulduktan sonra alb&uuml;m &uuml;retimine ge&ccedil;ilir. Bu s&uuml;re en fazla 15 g&uuml;nd&uuml;r.</li><li>Onayı alınamayıp &uuml;retime verilemeyen hi&ccedil; bir alb&uuml;mden fotoğraf&ccedil;ı sorumlu değildir.</li><li>Fotoğraflar her işletim sisteminde sorunsuz kullanılabilen y&uuml;ksek ebatlı olarak verilir.</li><li>Fotoğraflar size teslim edildikten sonra 1 ay s&uuml;reyle arşivlerde tutulur. Sonrasında silinir. Arşivlenmesi ve saklanması talep edilen&nbsp;fotoğraflar i&ccedil;in ek &uuml;cret talep edilir.</li><li>Yağmur, olumsuz hava şartları veya plan değişikliği gibi herhangi bir nedenden &ouml;t&uuml;r&uuml; her &ccedil;ift fotoğraf &ccedil;ekimini ileriki bir tarihe erteleyebilir.&nbsp;Bu ertelemeyi fotoğraf&ccedil;ıya &ccedil;ekime 3 g&uuml;n kala bildirmek zorundadır.</li><li>&Ccedil;ift tarafından iptal edilen veya ertelenen bir &ccedil;ekimi fotoğraf&ccedil;ıya &ccedil;ekim g&uuml;n&uuml; bildirildiği takdirde &ccedil;ekim yapılmasa dahi &uuml;creti &ouml;denmek durumundadır.</li><li>Bu s&ouml;zleşmeyle birlikte fotoğrafı &ccedil;ekilen &ccedil;ift, 3-5 adet fotoğrafı fotoğraf&ccedil;ının sosyal medya ve websitesinde kullanmasına izin vermiş olur.</li></ol><p style='margin-left:40px'><strong>&Ouml;DEME BİLGİLERİ</strong></p><ol><li>&Ouml;demenin yarısı s&ouml;zleşme anında, kalanı &ccedil;ekimden &ouml;nce veya en ge&ccedil; &ccedil;ekim g&uuml;n&uuml; &ouml;denir.</li><li>Taraflar anlaşma yapıldıktan sonra tek taraflı olarak anlaşmayı iptal edemezler. Tek taraflı olarak anlaşmayı fesh eden taraf belirlenen &uuml;cretin yarısını &ouml;demek zorundadır.</li><li>Fotoğraf&ccedil;ı, s&ouml;zleşmedeki fiyatlarda s&ouml;zleşme onaylandıktan sonra hi&ccedil; bir değişiklik yapma hakkına sahip değildir.</li><li>Fotoğraf &ccedil;ekimi i&ccedil;in gidilecek mekanların giriş &uuml;cretleri, otopark &uuml;cretleri ve beklenmedik t&uuml;m giderler d&uuml;ğ&uuml;n sahibine aittir.</li></ol>";
            //ss.FirmaId = frm.Id;
            //ss.OlusturanKullaniciId = 1;
            //ss.OlusturmaTarih = DateTime.Now;
            //ss.DegistirenKullaniciId = 1;
            //ss.DegistirmeTarih = DateTime.Now;
            //ss.Aktif = true;
            //ss.Sil = false;
            //dbContext.SozlesmeSartlaris.Add(ss);
            //dbContext.SaveChanges();
            //#endregion
            //#region Varsayılan Rehber Grupları
            //string[] Grupadi = { "Anne-Baba", "Gelin-Damat", "Rezervasyon Yetkilileri", "Müşteriler", "Cariler", "Personeller" };
            //for (int i = 0; i < 6; i++)
            //{
            //    RehberGrup rehgerGrup = new RehberGrup();
            //    rehgerGrup.FirmaId = frm.Id;
            //    rehgerGrup.GrupAdi = Grupadi[i];
            //    rehgerGrup.KilitBit = true;
            //    rehgerGrup.OlusturanKullaniciId = 1;
            //    rehgerGrup.OlusturmaTarih = DateTime.Now;
            //    rehgerGrup.DegistirenKullaniciId = 1;
            //    rehgerGrup.DegistirmeTarih = DateTime.Now;
            //    rehgerGrup.Aktif = true;
            //    rehgerGrup.Sil = false;
            //    dbContext.RehberGrups.Add(rehgerGrup);
            //    dbContext.SaveChanges();
            //}
            //#endregion
            //#region Varsayılan Sms Metinleri
            //string[] smsmetinleri =
            //{
            //    "Sayın {AdSoyad}, {SozlesmeNo} numaralı sözleşmeniz oluşturulmuştur. En mutlu gününüzü ölümsüzleştirmek için bizi tercih ettiğiniz için teşekkür ederiz.",
            //    "Sayın {AdSoyad}, {SozlesmeNo} numaralı sözleşmenizin tarihini hatırlatırız. {Tarih} tarihinde görüşmek dileğiyle.",
            //    "Sayın {AdSoyad}, {SozlesmeNo} numaralı sözleşmeniz {OpsiyonTarihi} tarihine kadar opsiyonlu olarak oluşturulmuştur. En mutlu gününüzü ölümsüzleştirmek için bizi tercih ettiğiniz için teşekkür ederiz.",
            //    "Sayın {AdSoyad}, {SozlesmeNo} numaralı sözleşmenizin opsiyon tarihi yaklaşıyor. {OpsiyonTarihi} tarihine kadar sözleşmenizi kesinleştirmenizi rica ederiz.",
            //    "Sayın {AdSoyad}, çekim randevu bilgileriniz, {CekimRandevuBilgileri} olarak oluşturulmuştur. En mutlu gününüzü ölümsüzleştirmek için bizi tercih ettiğiniz için teşekkür ederiz.",
            //    "Sayın {AdSoyad}, yaklaşan çekim randevularınızı hatırlatırız. Çekim randevusu bilgileriniz, {CekimRandevuBilgileri}",
            //    "Sayın {Personel}, görevlendirildiğiniz çekim randevuları, {CekimRandevuBilgileri} olarak oluşturulmuştur.  Lütfen çekim saatinden önce müşterimiz {AdSoyad} ile iletişime geçiniz.",
            //    "Sayın {Personel}, görevlendirildiğiniz Çekim randevunuzu hatırlatırız; {CekimRandevuBilgileri}. Lütfen çekim saatinden önce müşterimiz {AdSoyad} ile iletişime geçiniz.",
            //    "Sayın {AdSoyad}, {OdemeTarihi} tarihli {Tutar} TL'lik ödemeniz için teşekkür ederiz.",
            //    "Sayın {AdSoyad}, son ödemesi {OdemeTarihi} olan {Tutar} TL'lik ödemenizi hatırlatırız.",
            //    "Sayın {AdSoyad}, {SozlesmeNo} numaralı sözleşmenize ait fotoğrafları baskı için size verilen Müşteri Kodu ve Şifreniz ile https://www.fotografcitakip.com/Musteri/Giris/GirisYap adresinden giriş yaparak seçebilirsiniz. En mutlu gününüzü ölümsüzleştirmek için bizi tercih ettiğiniz için teşekkür ederiz.",
            //    "Müşterimiz fotoğraf seçimini yapmıştır. Müşteri Kodu: {MusteriKodu}, Müşteri: {AdSoyad}, Sözleşme No: {SozlesmeNo}",
            //    "Sayın {AdSoyad}, {SozlesmeNo} numaralı sözleşmenize ait fotoğrafları baskı için size verilen Müşteri Kodu ve Şifreniz ile https://www.fotografcitakip.com/Musteri/Giris/GirisYap adresinden giriş yaparak seçebilirsiniz. En mutlu gününüzü ölümsüzleştirmek için bizi tercih ettiğiniz için teşekkür ederiz.",
            //    "Bu dünyanın gördüğü en güzel aşk hikayesine tanık olmak bizleri çok mutlu etti. Mutluluğunuzun sonsuz olması dileklerimizle evlilik yıldönümüzü kutlarız. {FirmaAdi}",
            //    "Sayın {AdSoyad}, Tarafınıza olan {OdemeTarihi} tarihli {Tutar} TL'lik borcun ödemesi yapılmıştır. Bilginize...",
            //    "Sayın {AdSoyad}, son ödemesi {OdemeTarihi} olan {Tutar} TL'lik ödemenizi hatırlatırız.",
            //    "Sayın {AdSoyad}, {Tutar} TL'lik ödemeniz için teşekkür ederiz."
            //};
            //string[] smsmetinbaslik =
            //{
            //    "Rezervasyon Bilgi Mesajı",
            //    "Rezervasyon Hatırlatma Mesajı",
            //    "Opsiyonlu Rezervasyon Bilgi Mesajı",
            //    "Opsiyonlu Rezervasyon Hatırlatma Mesajı",
            //    "Çekim Randevusu Bilgi Mesajı (Müşteri)",
            //    "Çekim Randevusu Hatırlatma Mesajı (Müşteri)",
            //    "Çekim Randevusu Bilgi Mesajı (Personel)",
            //    "Çekim Randevusu Hatırlatma Mesajı (Personel)",
            //    "Müşteri Ödeme Bilgi Mesajı",
            //    "Müşteri Ödeme Hatırlatma Mesajı",
            //    "Fotoğraf Seçim Bilgi Mesajı (Müşteri)",
            //    "Fotoğraf Seçim Bilgi Mesajı (Firma)",
            //    "Fotoğraf Seçimi Hatırlatma Mesajı",
            //    "Yıldönümü Kutlaması",
            //    "Yapılan Ödeme Bilgilendirme",
            //    "Borç Bilgilendirmesi",
            //    "Alınan Ödeme Bilgilendirme"
            //};
            //SmsMetinleri smsm = new SmsMetinleri();
            //for (int i = 0; i < smsmetinleri.Length; i++)
            //{
            //    smsm.FirmaId = frm.Id;
            //    smsm.MetinAdi = smsmetinbaslik[i];
            //    smsm.SMSMetni = smsmetinleri[i];
            //    smsm.KilitBit = false;
            //    smsm.OlusturanKullaniciId = 1;
            //    smsm.OlusturmaTarih = DateTime.Now;
            //    smsm.DegistirenKullaniciId = 1;
            //    smsm.DegistirmeTarih = DateTime.Now;
            //    smsm.Aktif = true;
            //    smsm.Sil = false;
            //    dbContext.SmsMetinleris.Add(smsm);
            //    dbContext.SaveChanges();
            //}
            //#endregion
            #endregion
            // Sistem yöneticisine sms gönder
            string sonuc_uyeol_sms = "";
            string smsmetin = "Yeni Firma kaydı yapıldı. Firma Id: " + frm.Id + ", Firma adı: " + frm.FirmaAdi + ", Yetkili: " + frm.Yetkili + ".";
            sonuc_uyeol_sms = SMSGonder.Gonder_AtakSms(smsmetin, "05332625560", 1, 1, 1);

            return Json(new { Sonuc = true, Mesaj = "Üyelik işlemi tamamlandı." }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("GirisYap", "Giris", new { area = "Otomasyon" });
        }

        [HttpPost]
        public JsonResult IlceGetir(int id)
        {
            List<Ilce> ilceler = dbContext.Ilces.Where(x => x.IlId == id).ToList();
            var ilce = ilceler.Select(m => new { Id = m.Id, IlceAd = m.Ilce1 }).OrderBy(x => x.IlceAd); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(ilce, JsonRequestBehavior.AllowGet);
        }
    }
}