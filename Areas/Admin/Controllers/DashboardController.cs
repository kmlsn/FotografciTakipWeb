using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FotografciTakipWeb.Models;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using FotografciTakipWeb.App_Settings;

namespace FotografciTakipWeb.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/Default
        public ActionResult Index()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            //ViewBag.UstMenu = "Dashboard";
            ViewBag.UstMenu = "Dashboard";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            int kalankontor = 0;
            int konturuyarimiktari = 0;
            string kalankonturuyarimesaji = "Kontor çok";
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 1 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");

            ViewBag.KullaniciYetkileri = dbContext.KullaniciYetkis.Where(x => x.KullaniciId == KullaniciId && x.Aktif == true && x.Sil == false).ToList();
            decimal ToplamGelir = 0;
            List<Siparisler> siparisler = dbContext.Siparislers.Where(x => x.Odendi == true).ToList();
            if (siparisler == null)
                ToplamGelir = 0;
            else
                ToplamGelir = siparisler.Sum(x => x.PaketTutar);
            ViewBag.AktifFirmaSayisi = dbContext.Firmas.Count(x => x.Aktif == true) - 1;
            ViewBag.ToplamGelir = ToplamGelir;
            ViewBag.AktifKulanici = dbContext.Kullanicis.Count(x => x.Aktif == true) - 1;

            ViewBag.OnayBekleyenOdemler = dbContext.Siparislers.Where(x => x.Durum == 0).ToList();
            ViewBag.YeniFirmalar = dbContext.Firmas.Where(x => x.Aktif == true).OrderByDescending(x => x.Id).Take(10).ToList();
            #region Kalan Kontor Uyarı
            kalankontor = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).Bakiye;
            konturuyarimiktari = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).KonturUyariMiktari;
            if (kalankontor <= konturuyarimiktari)
            {
                kalankonturuyarimesaji = "Kontor az";
            }
            ViewBag.KalanKontorUyariMesaji = kalankonturuyarimesaji;
            ViewBag.KalanKontorUyariMesajiVer = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).KonturUyariVer;
            #endregion
            return View();
        }

        public PartialViewResult _LisansSuresi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            Lisanslar lisans = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId);
            //TimeSpan toplamsure = lisans.LisansBitisTarihi - lisans.LisansBaslangicTarihi;
            TimeSpan toplamsure = lisans.LisansBitisTarihi - lisans.LisansYenilemeTarihi; // Başlangıç tarihinden olduğunda toplam gün çok büyük oluyor.
            TimeSpan kalangun = lisans.LisansBitisTarihi - DateTime.Now;
            //TimeSpan gecengun = DateTime.Now - lisans.LisansBaslangicTarihi;
            TimeSpan gecengun = DateTime.Now - lisans.LisansYenilemeTarihi; // Başlangıç tarihinden olduğunda toplam gün çok büyük oluyor.
            int toplam = toplamsure.Days;
            int kalan = kalangun.Days;
            int gecen = gecengun.Days;
            ViewBag.toplam = toplam;
            ViewBag.kalan = kalan;
            ViewBag.gecen = gecen;
            return PartialView();
        }

        public PartialViewResult _LisansSuresi2()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            Lisanslar lisans = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId);
            //TimeSpan toplamsure = lisans.LisansBitisTarihi - lisans.LisansBaslangicTarihi;
            TimeSpan toplamsure = lisans.LisansBitisTarihi - lisans.LisansYenilemeTarihi; // Başlangıç tarihinden olduğunda toplam gün çok büyük oluyor.
            TimeSpan kalangun = lisans.LisansBitisTarihi - DateTime.Now;
            //TimeSpan gecengun = DateTime.Now - lisans.LisansBaslangicTarihi;
            TimeSpan gecengun = DateTime.Now - lisans.LisansYenilemeTarihi; // Başlangıç tarihinden olduğunda toplam gün çok büyük oluyor.
            int toplam = toplamsure.Days;
            int kalan = kalangun.Days;
            int gecen = gecengun.Days;
            ViewBag.toplam = toplam;
            ViewBag.kalan = kalan;
            ViewBag.gecen = gecen;
            return PartialView();
        }

        public PartialViewResult _SMSKontur()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            if (FirmaId == 1)
            {
                string username = "kamilsen";
                string password = "442Bir726";
                string PostAddress;
                try
                {
                    PostAddress = "http://panel4.ekomesaj.com:9587/user/credit";

                    WebClient wUpload = new WebClient();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PostAddress);

                    request.ContentType = "application/json; charset=utf-8";
                    request.Method = "GET";
                    string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

                    request.Headers.Add("Authorization", "Basic " + svcCredentials);

                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    string result;
                    var streamReader = new StreamReader(httpResponse.GetResponseStream());
                    result = streamReader.ReadToEnd();
                    dynamic Sonuc_Kredi = JsonConvert.DeserializeObject(result);
                    int Kredi = Convert.ToInt32(Sonuc_Kredi.data.credit);
                    ViewBag.KalanKontur = Kredi;
                    ViewBag.ToplamKontur = Kredi;
                }
                catch (Exception)
                {
                    ViewBag.KalanKontur = 0;
                    ViewBag.ToplamKontur = 0;
                }
            }
            else
            {
                SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);

                ViewBag.KalanKontur = smsbakiye.Bakiye;
                ViewBag.ToplamKontur = smsbakiye.ToplamKontur;

            }
            return PartialView();
        }

        public PartialViewResult _OkunmamisMesajlar()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            /* OkunduBit false olan ve durumu "KonuKapandı veya İptal olmayan Mesajlar"*/
            List<MusteriMesaj> okunmamismesajlar = dbContext.MusteriMesajs.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.OkunduBit == false && !(x.Durum == "5" || x.Durum == "7")).ToList();
            return PartialView(okunmamismesajlar);
        }

        public PartialViewResult _Destek()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            /* OkunduBit false olan ve durumu "KonuKapandı veya İptal olmayan Mesajlar"*/
            List<DestekTalepleriDetay> okunmamiscevaplar = dbContext.DestekTalepleriDetays.Where(x => x.Aktif == true && x.Sil == false && x.MusteriCevap == true && x.FirmaCevap == false && x.OkunduBit == false).ToList();
            return PartialView(okunmamiscevaplar);
        }
        public PartialViewResult _MenuDestekTalepleri()
        {
            /* OkunduBit false olan ve durumu "KonuKapandı veya İptal olmayan Mesajlar"*/
            List<DestekTalepleriDetay> okunmamiscevaplar = dbContext.DestekTalepleriDetays.Where(x => x.Aktif == true && x.Sil == false && x.MusteriCevap == true && x.FirmaCevap == false && x.OkunduBit == false).ToList();
            return PartialView(okunmamiscevaplar);
        }

        public ActionResult SmsGonder()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            //ViewBag.UstMenu = "Dashboard";
            ViewBag.UstMenu = "SMS Gönder";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 13 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");

            return View();
        }

        [HttpPost]
        public ActionResult HizliSmsGonder()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "SMS Gönder";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long RolId = Convert.ToInt64(Session["RolId"]);

            string SMSMetin = Request["MesajMetni"];
            SMSMetin = SMSMetin.Replace("\r\n", "");
            SMSMetin = SMSMetin.Replace("\n", "");
            string CepTel = Request["TelefonNo"];

            TelefonDüzelt td = new TelefonDüzelt();
            if (!string.IsNullOrEmpty(CepTel)) { CepTel = td.düzelt(CepTel); }

            string sonuc = SMSGonder.Gonder_AtakSms(SMSMetin, CepTel, FirmaId, 1, KullaniciId);
            return Json(new { Sonuc = true, Mesaj = sonuc }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult OdemeOnay(int? id)
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

        [HttpPost]
        public ActionResult SmsTabloOlustur()
        {
            // tabloyu sıfırla
            dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE OtomatikSmsListesi");

            // Otomatik Sms Gönderilecek kayıtların olduğu tablo hergün temizlendikten sonra yeniden doldurulacak. Bu işlem gece saatlerinde otomatik yapılacak.

            DateTime BuGun = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime BirGunSonra = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, 0, 0, 0);
            DateTime IkiGunSonra = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(2).Day, 0, 0, 0);
            DateTime UcGunSonra = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(3).Day, 0, 0, 0);
            DateTime BirHaftaSonra = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(7).Day, 0, 0, 0);
            //  0: Hiçbir Zaman Gönderme
            //  1: Tarih ile aynı gün
            //  2: 1 gün önce
            //  3: 2 gün önce
            //  4: 3 gün önce
            //  5: 1 hafta önce
            //  6: Kayıt Yapıldığında
            // ------------------------------------------------------------------------------------------------------------------  Fotoğraf Seçimi için
            DateTime BirGunOnce = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-1).Day, 0, 0, 0);
            DateTime IkiGunOnce = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-2).Day, 0, 0, 0);
            DateTime UcGunOnce = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-3).Day, 0, 0, 0);
            DateTime BirHaftaOnce = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(-7).Day, 0, 0, 0);
            //  0: Hiçbir Zaman Gönderme
            //  1: Tarih ile aynı gün
            //  2: 1 gün sonra
            //  3: 2 gün sonra
            //  4: 3 gün sonra
            //  5: 1 hafta sonra
            //  6: Kayıt Yapıldığında
            OtomatikSmsListesi SmsListesi = new OtomatikSmsListesi();
            #region RezervasyonBilgiSMS'i
            // RezervasyonTarihiBilgiSms i için Tarih ile aynı gün seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> RezervasyonTarihiBilgi_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.RezervasyonTarihiBilgiGonderimSuresi == 1).ToList();
            // RezervasyonTarihiBilgiSms i için Bir gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> RezervasyonTarihiBilgi_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.RezervasyonTarihiBilgiGonderimSuresi == 2).ToList();
            // RezervasyonTarihiBilgiSms i için İki gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> RezervasyonTarihiBilgi_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.RezervasyonTarihiBilgiGonderimSuresi == 3).ToList();
            // RezervasyonTarihiBilgiSms i için Üç gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> RezervasyonTarihiBilgi_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.RezervasyonTarihiBilgiGonderimSuresi == 4).ToList();
            // RezervasyonTarihiBilgiSms i için Bir hafta önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> RezervasyonTarihiBilgi_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.RezervasyonTarihiBilgiGonderimSuresi == 5).ToList();
            foreach (var firma in RezervasyonTarihiBilgi_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi == BuGun && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.RezervasyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in RezervasyonTarihiBilgi_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi == BirGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.RezervasyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in RezervasyonTarihiBilgi_smsGonderimIkiGunOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi == IkiGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.RezervasyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in RezervasyonTarihiBilgi_smsGonderimUcGunOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi == UcGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.RezervasyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in RezervasyonTarihiBilgi_smsGonderimBirHaftaOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi == BirHaftaSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.RezervasyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            #region RezervasyonHatırlatmaSMS'i
            List<AyarlarSmsGonderim> RezervasyonTarihiHatirlatma_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.RezervasyonTarihiHatirlatmaGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> RezervasyonTarihiHatirlatma_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.RezervasyonTarihiHatirlatmaGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> RezervasyonTarihiHatirlatma_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.RezervasyonTarihiHatirlatmaGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> RezervasyonTarihiHatirlatma_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.RezervasyonTarihiHatirlatmaGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> RezervasyonTarihiHatirlatma_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.RezervasyonTarihiHatirlatmaGonderimSuresi == 5).ToList();
            foreach (var firma in RezervasyonTarihiHatirlatma_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi == BuGun && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.RezervasyonTarihiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in RezervasyonTarihiHatirlatma_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi == BirGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.RezervasyonTarihiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in RezervasyonTarihiHatirlatma_smsGonderimIkiGunOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi == IkiGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.RezervasyonTarihiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in RezervasyonTarihiHatirlatma_smsGonderimUcGunOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi == UcGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.RezervasyonTarihiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in RezervasyonTarihiHatirlatma_smsGonderimBirHaftaOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi == BirHaftaSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.RezervasyonTarihiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            #region OpsiyonBilgiSMS'i
            // RezervasyonTarihiBilgiSms i için Tarih ile aynı gün seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> OpsiyonTarihiBilgi_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.OpsiyonTarihiBilgiGonderimSuresi == 1).ToList();
            // RezervasyonTarihiBilgiSms i için Bir gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> OpsiyonTarihiBilgi_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.OpsiyonTarihiBilgiGonderimSuresi == 2).ToList();
            // RezervasyonTarihiBilgiSms i için İki gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> OpsiyonTarihiBilgi_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.OpsiyonTarihiBilgiGonderimSuresi == 3).ToList();
            // RezervasyonTarihiBilgiSms i için Üç gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> OpsiyonTarihiBilgi_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.OpsiyonTarihiBilgiGonderimSuresi == 4).ToList();
            // RezervasyonTarihiBilgiSms i için Bir hafta önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> OpsiyonTarihiBilgi_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.OpsiyonTarihiBilgiGonderimSuresi == 5).ToList();
            foreach (var firma in OpsiyonTarihiBilgi_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.OpsiyonTarihi == BuGun && x.OpsiyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.OpsiyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in OpsiyonTarihiBilgi_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.OpsiyonTarihi == BirGunSonra && x.OpsiyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.OpsiyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in OpsiyonTarihiBilgi_smsGonderimIkiGunOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.OpsiyonTarihi == IkiGunSonra && x.OpsiyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.OpsiyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in OpsiyonTarihiBilgi_smsGonderimUcGunOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.OpsiyonTarihi == UcGunSonra && x.OpsiyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.OpsiyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in OpsiyonTarihiBilgi_smsGonderimBirHaftaOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.OpsiyonTarihi == BirHaftaSonra && x.OpsiyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.OpsiyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            #region OpsiyonHatırlatmaSMS'i
            List<AyarlarSmsGonderim> OpsiyonTarihiHatirlatma_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.OpsiyonTarihiHatirlatmaGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> OpsiyonTarihiHatirlatma_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.OpsiyonTarihiHatirlatmaGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> OpsiyonTarihiHatirlatma_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.OpsiyonTarihiHatirlatmaGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> OpsiyonTarihiHatirlatma_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.OpsiyonTarihiHatirlatmaGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> OpsiyonTarihiHatirlatma_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.OpsiyonTarihiHatirlatmaGonderimSuresi == 5).ToList();
            foreach (var firma in OpsiyonTarihiHatirlatma_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.OpsiyonTarihi == BuGun && x.OpsiyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.OpsiyonTarihiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in OpsiyonTarihiHatirlatma_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.OpsiyonTarihi == BirGunSonra && x.OpsiyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.OpsiyonTarihiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in OpsiyonTarihiHatirlatma_smsGonderimIkiGunOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.OpsiyonTarihi == IkiGunSonra && x.OpsiyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.OpsiyonTarihiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in OpsiyonTarihiHatirlatma_smsGonderimUcGunOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.OpsiyonTarihi == UcGunSonra && x.OpsiyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.OpsiyonTarihiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in OpsiyonTarihiHatirlatma_smsGonderimBirHaftaOnce_Firmalar)
            {
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.OpsiyonTarihi == BirHaftaSonra && x.OpsiyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.OpsiyonTarihiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            #region MüşteriÇekimRandevusuBilgiSms'i
            List<AyarlarSmsGonderim> MusteriCekimRandevusuBilgi_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriCekimRandevusuBilgiGonderimSuresi == 1).ToList();
            // RezervasyonTarihiBilgiSms i için Bir gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> MusteriCekimRandevusuBilgi_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriCekimRandevusuBilgiGonderimSuresi == 2).ToList();
            // RezervasyonTarihiBilgiSms i için İki gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> MusteriCekimRandevusuBilgi_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriCekimRandevusuBilgiGonderimSuresi == 3).ToList();
            // RezervasyonTarihiBilgiSms i için Üç gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> MusteriCekimRandevusuBilgi_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriCekimRandevusuBilgiGonderimSuresi == 4).ToList();
            // RezervasyonTarihiBilgiSms i için Bir hafta önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> MusteriCekimRandevusuBilgi_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriCekimRandevusuBilgiGonderimSuresi == 5).ToList();
            foreach (var firma in MusteriCekimRandevusuBilgi_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BuGun) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriCekimRandevusuBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in MusteriCekimRandevusuBilgi_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BirGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriCekimRandevusuBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in MusteriCekimRandevusuBilgi_smsGonderimIkiGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == IkiGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriCekimRandevusuBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in MusteriCekimRandevusuBilgi_smsGonderimUcGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == UcGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriCekimRandevusuBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in MusteriCekimRandevusuBilgi_smsGonderimBirHaftaOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BirHaftaSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriCekimRandevusuBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            #region MüşteriÇekimRandevusuHatırlatmaSms'i
            List<AyarlarSmsGonderim> MusteriCekimRandevusuHatirlatma_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriCekimRandevusuHatirlatmaGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> MusteriCekimRandevusuHatirlatma_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriCekimRandevusuHatirlatmaGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> MusteriCekimRandevusuHatirlatma_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriCekimRandevusuHatirlatmaGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> MusteriCekimRandevusuHatirlatma_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriCekimRandevusuHatirlatmaGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> MusteriCekimRandevusuHatirlatma_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriCekimRandevusuHatirlatmaGonderimSuresi == 5).ToList();
            foreach (var firma in MusteriCekimRandevusuHatirlatma_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BuGun) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriCekimRandevusuHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in MusteriCekimRandevusuHatirlatma_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BirGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriCekimRandevusuHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in MusteriCekimRandevusuHatirlatma_smsGonderimIkiGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == IkiGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriCekimRandevusuHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in MusteriCekimRandevusuHatirlatma_smsGonderimUcGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == UcGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriCekimRandevusuHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in MusteriCekimRandevusuHatirlatma_smsGonderimBirHaftaOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BirHaftaSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriCekimRandevusuHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            #region PersonelÇekimRandevusuBilgiSms'i
            List<AyarlarSmsGonderim> PersonelCekimRandevusuBilgi_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.PersonelRandevuBilgiGonderimSuresi == 1).ToList();
            // RezervasyonTarihiBilgiSms i için Bir gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> PersonelCekimRandevusuBilgi_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.PersonelRandevuBilgiGonderimSuresi == 2).ToList();
            // RezervasyonTarihiBilgiSms i için İki gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> PersonelCekimRandevusuBilgi_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.PersonelRandevuBilgiGonderimSuresi == 3).ToList();
            // RezervasyonTarihiBilgiSms i için Üç gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> PersonelCekimRandevusuBilgi_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.PersonelRandevuBilgiGonderimSuresi == 4).ToList();
            // RezervasyonTarihiBilgiSms i için Bir hafta önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> PersonelCekimRandevusuBilgi_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.PersonelRandevuBilgiGonderimSuresi == 5).ToList();
            foreach (var firma in PersonelCekimRandevusuBilgi_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BuGun) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }

                    //Randevudaki görevli personel sayısı kadar persoele sms gönderilecek.
                    string[] gp = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in gp)
                    {
                        long pId = Convert.ToInt64(persId);
                        Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                        bool smskabul = true, emailkabul = true;
                        if (personel == null)
                        {
                            smskabul = true; emailkabul = true;
                        }
                        else
                        {
                            smskabul = personel.SMSKabul; emailkabul = personel.EmailKabul;
                        }
                        if (personel.AdiSoyadi != "-- Müşterisiz İşlem --" && personel.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.PersonelRandevuBilgiMesaji && x.Aktif == true && x.Sil == false);
                            if (smsMetin != null)
                            {
                                string smsmetin = smsMetin.SMSMetni;
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                                if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                                if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{AdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                                if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                                if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                    smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                                if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                    smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);


                                // Geçici Tabloya Aktarılacak.
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = personel.AdiSoyadi;
                                SmsListesi.AliciTel = personel.CepTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in PersonelCekimRandevusuBilgi_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BirGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    //Randevudaki görevli personel sayısı kadar persoele sms gönderilecek.
                    string[] gp = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in gp)
                    {
                        long pId = Convert.ToInt64(persId);
                        Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                        bool smskabul = true, emailkabul = true;
                        if (personel == null)
                        {
                            smskabul = true; emailkabul = true;
                        }
                        else
                        {
                            smskabul = personel.SMSKabul; emailkabul = personel.EmailKabul;
                        }
                        if (personel.AdiSoyadi != "-- Müşterisiz İşlem --" && personel.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.PersonelRandevuBilgiMesaji && x.Aktif == true && x.Sil == false);
                            if (smsMetin != null)
                            {
                                string smsmetin = smsMetin.SMSMetni;
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                                if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                                if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{AdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                                if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                                if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                    smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                                if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                    smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                                // Geçici Tabloya Aktarılacak.
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = personel.AdiSoyadi;
                                SmsListesi.AliciTel = personel.CepTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in PersonelCekimRandevusuBilgi_smsGonderimIkiGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == IkiGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    //Randevudaki görevli personel sayısı kadar persoele sms gönderilecek.
                    string[] gp = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in gp)
                    {
                        long pId = Convert.ToInt64(persId);
                        Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                        bool smskabul = true, emailkabul = true;
                        if (personel == null)
                        {
                            smskabul = true; emailkabul = true;
                        }
                        else
                        {
                            smskabul = personel.SMSKabul; emailkabul = personel.EmailKabul;
                        }
                        if (personel.AdiSoyadi != "-- Müşterisiz İşlem --" && personel.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.PersonelRandevuBilgiMesaji && x.Aktif == true && x.Sil == false);
                            if (smsMetin != null)
                            {
                                string smsmetin = smsMetin.SMSMetni;
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                                if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                                if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{AdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                                if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                                if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                    smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                                if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                    smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                                // Geçici Tabloya Aktarılacak.
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = personel.AdiSoyadi;
                                SmsListesi.AliciTel = personel.CepTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in PersonelCekimRandevusuBilgi_smsGonderimUcGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == UcGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    //Randevudaki görevli personel sayısı kadar persoele sms gönderilecek.
                    string[] gp = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in gp)
                    {
                        long pId = Convert.ToInt64(persId);
                        Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                        bool smskabul = true, emailkabul = true;
                        if (personel == null)
                        {
                            smskabul = true; emailkabul = true;
                        }
                        else
                        {
                            smskabul = personel.SMSKabul; emailkabul = personel.EmailKabul;
                        }
                        if (personel.AdiSoyadi != "-- Müşterisiz İşlem --" && personel.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.PersonelRandevuBilgiMesaji && x.Aktif == true && x.Sil == false);
                            if (smsMetin != null)
                            {
                                string smsmetin = smsMetin.SMSMetni;
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                                if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                                if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{AdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                                if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                                if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                    smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                                if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                    smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                                // Geçici Tabloya Aktarılacak.
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = personel.AdiSoyadi;
                                SmsListesi.AliciTel = personel.CepTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in PersonelCekimRandevusuBilgi_smsGonderimBirHaftaOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BirHaftaSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    //Randevudaki görevli personel sayısı kadar persoele sms gönderilecek.
                    string[] gp = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in gp)
                    {
                        long pId = Convert.ToInt64(persId);
                        Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                        bool smskabul = true, emailkabul = true;
                        if (personel == null)
                        {
                            smskabul = true; emailkabul = true;
                        }
                        else
                        {
                            smskabul = personel.SMSKabul; emailkabul = personel.EmailKabul;
                        }
                        if (personel.AdiSoyadi != "-- Müşterisiz İşlem --" && personel.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.PersonelRandevuBilgiMesaji && x.Aktif == true && x.Sil == false);
                            if (smsMetin != null)
                            {
                                string smsmetin = smsMetin.SMSMetni;
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                                if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                                if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{AdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                                if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                                if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                    smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                                if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                    smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                                // Geçici Tabloya Aktarılacak.
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = personel.AdiSoyadi;
                                SmsListesi.AliciTel = personel.CepTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            #endregion
            #region PersonelÇekimRandevusuHatırlatmaSms'i
            List<AyarlarSmsGonderim> PersonelCekimRandevusuHatirlatma_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.PersonelRandevuHatirlatmaGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> PersonelCekimRandevusuHatirlatma_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.PersonelRandevuHatirlatmaGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> PersonelCekimRandevusuHatirlatma_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.PersonelRandevuHatirlatmaGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> PersonelCekimRandevusuHatirlatma_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.PersonelRandevuHatirlatmaGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> PersonelCekimRandevusuHatirlatma_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.PersonelRandevuHatirlatmaGonderimSuresi == 5).ToList();
            foreach (var firma in PersonelCekimRandevusuHatirlatma_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BuGun) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    //Randevudaki görevli personel sayısı kadar persoele sms gönderilecek.
                    string[] gp = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in gp)
                    {
                        long pId = Convert.ToInt64(persId);
                        Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                        bool smskabul = true, emailkabul = true;
                        if (personel == null)
                        {
                            smskabul = true; emailkabul = true;
                        }
                        else
                        {
                            smskabul = personel.SMSKabul; emailkabul = personel.EmailKabul;
                        }
                        if (personel.AdiSoyadi != "-- Müşterisiz İşlem --" && personel.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.PersonelRandevuHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                            if (smsMetin != null)
                            {
                                string smsmetin = smsMetin.SMSMetni;
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                                if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                                if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{AdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                                if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                                if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                    smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                                if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                    smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);


                                // Geçici Tabloya Aktarılacak.
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = personel.AdiSoyadi;
                                SmsListesi.AliciTel = personel.CepTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in PersonelCekimRandevusuHatirlatma_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BirGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    //Randevudaki görevli personel sayısı kadar persoele sms gönderilecek.
                    string[] gp = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in gp)
                    {
                        long pId = Convert.ToInt64(persId);
                        Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                        bool smskabul = true, emailkabul = true;
                        if (personel == null)
                        {
                            smskabul = true; emailkabul = true;
                        }
                        else
                        {
                            smskabul = personel.SMSKabul; emailkabul = personel.EmailKabul;
                        }
                        if (personel.AdiSoyadi != "-- Müşterisiz İşlem --" && personel.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.PersonelRandevuHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                            if (smsMetin != null)
                            {
                                string smsmetin = smsMetin.SMSMetni;
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                                if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                                if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{AdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                                if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                                if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                    smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                                if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                    smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                                // Geçici Tabloya Aktarılacak.
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = personel.AdiSoyadi;
                                SmsListesi.AliciTel = personel.CepTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in PersonelCekimRandevusuHatirlatma_smsGonderimIkiGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == IkiGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    //Randevudaki görevli personel sayısı kadar persoele sms gönderilecek.
                    string[] gp = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in gp)
                    {
                        long pId = Convert.ToInt64(persId);
                        Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                        bool smskabul = true, emailkabul = true;
                        if (personel == null)
                        {
                            smskabul = true; emailkabul = true;
                        }
                        else
                        {
                            smskabul = personel.SMSKabul; emailkabul = personel.EmailKabul;
                        }
                        if (personel.AdiSoyadi != "-- Müşterisiz İşlem --" && personel.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.PersonelRandevuHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                            if (smsMetin != null)
                            {
                                string smsmetin = smsMetin.SMSMetni;
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                                if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                                if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{AdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                                if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                                if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                    smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                                if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                    smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                                // Geçici Tabloya Aktarılacak.
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = personel.AdiSoyadi;
                                SmsListesi.AliciTel = personel.CepTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in PersonelCekimRandevusuHatirlatma_smsGonderimUcGunOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == UcGunSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    //Randevudaki görevli personel sayısı kadar persoele sms gönderilecek.
                    string[] gp = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in gp)
                    {
                        long pId = Convert.ToInt64(persId);
                        Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                        bool smskabul = true, emailkabul = true;
                        if (personel == null)
                        {
                            smskabul = true; emailkabul = true;
                        }
                        else
                        {
                            smskabul = personel.SMSKabul; emailkabul = personel.EmailKabul;
                        }
                        if (personel.AdiSoyadi != "-- Müşterisiz İşlem --" && personel.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.PersonelRandevuHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                            if (smsMetin != null)
                            {
                                string smsmetin = smsMetin.SMSMetni;
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                                if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                                if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{AdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                                if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                                if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                    smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                                if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                    smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                                // Geçici Tabloya Aktarılacak.
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = personel.AdiSoyadi;
                                SmsListesi.AliciTel = personel.CepTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in PersonelCekimRandevusuHatirlatma_smsGonderimBirHaftaOnce_Firmalar)
            {
                // bulunan firmalara ait randevulardan tarihi bugünden 1 gün büyük olan randevular bulunuyor.
                string randevular = "";
                List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && (new DateTime(x.Baslangic.Year, x.Baslangic.Month, x.Baslangic.Day, 0, 0, 0) == BirHaftaSonra) && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var item in randevu)
                {
                    Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == item.SubeId && x.Aktif == true && x.Sil == false);
                    randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                    if (!string.IsNullOrEmpty(item.Aciklama))
                    {
                        randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                    }
                    //Randevudaki görevli personel sayısı kadar persoele sms gönderilecek.
                    string[] gp = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in gp)
                    {
                        long pId = Convert.ToInt64(persId);
                        Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                        bool smskabul = true, emailkabul = true;
                        if (personel == null)
                        {
                            smskabul = true; emailkabul = true;
                        }
                        else
                        {
                            smskabul = personel.SMSKabul; emailkabul = personel.EmailKabul;
                        }
                        if (personel.AdiSoyadi != "-- Müşterisiz İşlem --" && personel.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.PersonelRandevuHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                            if (smsMetin != null)
                            {
                                string smsmetin = smsMetin.SMSMetni;
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                                if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                                if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                    smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{AdSoyad}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                                if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                                if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                    smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                                if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                    smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                                // Geçici Tabloya Aktarılacak.
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = personel.AdiSoyadi;
                                SmsListesi.AliciTel = personel.CepTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            #endregion
            #region MüşteriÖdemeBilgiMesajı
            // RezervasyonTarihiBilgiSms i için Tarih ile aynı gün seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> MusteriOdemeBilgi_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriOdemeBilgiGonderimSuresi == 1).ToList();
            // RezervasyonTarihiBilgiSms i için Bir gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> MusteriOdemeBilgi_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriOdemeBilgiGonderimSuresi == 2).ToList();
            // RezervasyonTarihiBilgiSms i için İki gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> MusteriOdemeBilgi_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriOdemeBilgiGonderimSuresi == 3).ToList();
            // RezervasyonTarihiBilgiSms i için Üç gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> MusteriOdemeBilgi_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriOdemeBilgiGonderimSuresi == 4).ToList();
            // RezervasyonTarihiBilgiSms i için Bir hafta önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> MusteriOdemeBilgi_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriOdemeBilgiGonderimSuresi == 5).ToList();
            foreach (var firma in MusteriOdemeBilgi_smsGonderimAyniGun_Firmalar)
            {
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == firma.Id && x.OdemeTarihi == BuGun && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var odeme in odemeler)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriOdemeBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", odeme.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.SozlesmeId == 1 ? odeme.GunlukIsler.TakipNo.ToString() : odeme.Sozlesme.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in MusteriOdemeBilgi_smsGonderimBirGunOnce_Firmalar)
            {
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == firma.Id && x.OdemeTarihi == BirGunSonra && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var odeme in odemeler)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriOdemeBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", odeme.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.SozlesmeId == 1 ? odeme.GunlukIsler.TakipNo.ToString() : odeme.Sozlesme.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in MusteriOdemeBilgi_smsGonderimIkiGunOnce_Firmalar)
            {
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == firma.Id && x.OdemeTarihi == IkiGunSonra && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var odeme in odemeler)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriOdemeBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", odeme.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.SozlesmeId == 1 ? odeme.GunlukIsler.TakipNo.ToString() : odeme.Sozlesme.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in MusteriOdemeBilgi_smsGonderimUcGunOnce_Firmalar)
            {
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == firma.Id && x.OdemeTarihi == UcGunSonra && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var odeme in odemeler)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriOdemeBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", odeme.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.SozlesmeId == 1 ? odeme.GunlukIsler.TakipNo.ToString() : odeme.Sozlesme.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in MusteriOdemeBilgi_smsGonderimBirHaftaOnce_Firmalar)
            {
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == firma.Id && x.OdemeTarihi == BirHaftaSonra && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var odeme in odemeler)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriOdemeBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", odeme.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.SozlesmeId == 1 ? odeme.GunlukIsler.TakipNo.ToString() : odeme.Sozlesme.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            #endregion
            #region MüşteriÖdemeHatırlatmaMesajı
            List<AyarlarSmsGonderim> MusteriOdemeHatirlatma_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriOdemeHatirlatmaGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> MusteriOdemeHatirlatma_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriOdemeHatirlatmaGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> MusteriOdemeHatirlatma_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriOdemeHatirlatmaGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> MusteriOdemeHatirlatma_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriOdemeHatirlatmaGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> MusteriOdemeHatirlatma_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.MusteriOdemeHatirlatmaGonderimSuresi == 5).ToList();
            foreach (var firma in MusteriOdemeHatirlatma_smsGonderimAyniGun_Firmalar)
            {
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == firma.Id && x.OdemeTarihi == BuGun && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var odeme in odemeler)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriOdemeHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", odeme.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.SozlesmeId == 1 ? odeme.GunlukIsler.TakipNo.ToString() : odeme.Sozlesme.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in MusteriOdemeHatirlatma_smsGonderimBirGunOnce_Firmalar)
            {
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == firma.Id && x.OdemeTarihi == BirGunSonra && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var odeme in odemeler)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriOdemeHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", odeme.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.SozlesmeId == 1 ? odeme.GunlukIsler.TakipNo.ToString() : odeme.Sozlesme.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in MusteriOdemeHatirlatma_smsGonderimIkiGunOnce_Firmalar)
            {
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == firma.Id && x.OdemeTarihi == IkiGunSonra && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var odeme in odemeler)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriOdemeHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", odeme.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.SozlesmeId == 1 ? odeme.GunlukIsler.TakipNo.ToString() : odeme.Sozlesme.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in MusteriOdemeHatirlatma_smsGonderimUcGunOnce_Firmalar)
            {
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == firma.Id && x.OdemeTarihi == UcGunSonra && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var odeme in odemeler)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriOdemeHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", odeme.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.SozlesmeId == 1 ? odeme.GunlukIsler.TakipNo.ToString() : odeme.Sozlesme.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in MusteriOdemeHatirlatma_smsGonderimBirHaftaOnce_Firmalar)
            {
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == firma.Id && x.OdemeTarihi == BirHaftaSonra && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                foreach (var odeme in odemeler)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.MusteriOdemeHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", odeme.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.SozlesmeId == 1 ? odeme.GunlukIsler.TakipNo.ToString() : odeme.Sozlesme.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            #endregion
            #region FotoğrafSeçimiBilgiMesajıMüşteri
            // RezervasyonTarihiBilgiSms i için Bir gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> FotografSecimiBilgiMesajiMusteri_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiBilgiMesajiMusteriGonderimSuresi == 1).ToList();
            // RezervasyonTarihiBilgiSms i için Bir gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> FotografSecimiBilgiMesajiMusteri_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiBilgiMesajiMusteriGonderimSuresi == 2).ToList();
            // RezervasyonTarihiBilgiSms i için İki gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> FotografSecimiBilgiMesajiMusteri_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiBilgiMesajiMusteriGonderimSuresi == 3).ToList();
            // RezervasyonTarihiBilgiSms i için Üç gün önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> FotografSecimiBilgiMesajiMusteri_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiBilgiMesajiMusteriGonderimSuresi == 4).ToList();
            // RezervasyonTarihiBilgiSms i için Bir hafta önce seçen firmalar bulunuyor.
            List<AyarlarSmsGonderim> FotografSecimiBilgiMesajiMusteri_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiBilgiMesajiMusteriGonderimSuresi == 5).ToList();
            //            0: İşlem Yok
            //            1: Müşteriye Seçime Gönderildi
            //            2: Müşteri Seçimi Yaptı
            //            3: Müşteriye Tekrar Göderildi
            //            4: Müşteri Seçimleri Baskı için Onayladı
            foreach (var firma in FotografSecimiBilgiMesajiMusteri_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == BuGun && (x.FotografSecimDurum == "1" || x.FotografSecimDurum == "3") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiBilgiMesajiMusteri && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiBilgiMesajiMusteri_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == BirGunOnce && (x.FotografSecimDurum == "1" || x.FotografSecimDurum == "3") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiBilgiMesajiMusteri && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiBilgiMesajiMusteri_smsGonderimIkiGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == IkiGunOnce && (x.FotografSecimDurum == "1" || x.FotografSecimDurum == "3") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiBilgiMesajiMusteri && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiBilgiMesajiMusteri_smsGonderimUcGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == UcGunOnce && (x.FotografSecimDurum == "1" || x.FotografSecimDurum == "3") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiBilgiMesajiMusteri && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiBilgiMesajiMusteri_smsGonderimBirHaftaOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == BirHaftaOnce && (x.FotografSecimDurum == "1" || x.FotografSecimDurum == "3") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiBilgiMesajiMusteri && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            //            0: İşlem Yok
            //            1: Müşteriye Seçime Gönderildi
            //            2: Müşteri Seçimi Yaptı
            //            3: Müşteriye Tekrar Göderildi
            //            4: Müşteri Seçimleri Baskı için Onayladı

            #endregion
            #region FotoğrafSeçimiBilgiMesajıFirma
            List<AyarlarSmsGonderim> FotografSecimiBilgiMesajiFirma_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiBilgiMesajiFirmaGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> FotografSecimiBilgiMesajiFirma_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiBilgiMesajiFirmaGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> FotografSecimiBilgiMesajiFirma_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiBilgiMesajiFirmaGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> FotografSecimiBilgiMesajiFirma_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiBilgiMesajiFirmaGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> FotografSecimiBilgiMesajiFirma_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiBilgiMesajiFirmaGonderimSuresi == 5).ToList();
            //            0: İşlem Yok
            //            1: Müşteriye Seçime Gönderildi
            //            2: Müşteri Seçimi Yaptı
            //            3: Müşteriye Tekrar Göderildi
            //            4: Müşteri Seçimleri Baskı için Onayladı
            foreach (var firma in FotografSecimiBilgiMesajiFirma_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == BuGun && (x.FotografSecimDurum == "2" || x.FotografSecimDurum == "4") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    // SMS Firma Yetkilisine iletilecek. Daha sonra ilgili personele iletilecek.

                    if (firma.Firma.Yetkili != "" && firma.Firma.CepTel != "")
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiBilgiMesajiFirma && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", firma.Firma.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", firma.Firma.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = firma.Firma.Yetkili;
                            SmsListesi.AliciTel = firma.Firma.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiBilgiMesajiFirma_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == BirGunOnce && (x.FotografSecimDurum == "2" || x.FotografSecimDurum == "4") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    // SMS Firma Yetkilisine iletilecek. Daha sonra ilgili personele iletilecek.

                    if (firma.Firma.Yetkili != "" && firma.Firma.CepTel != "")
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiBilgiMesajiFirma && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", firma.Firma.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", firma.Firma.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = firma.Firma.Yetkili;
                            SmsListesi.AliciTel = firma.Firma.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiBilgiMesajiFirma_smsGonderimIkiGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == IkiGunOnce && (x.FotografSecimDurum == "2" || x.FotografSecimDurum == "4") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    // SMS Firma Yetkilisine iletilecek. Daha sonra ilgili personele iletilecek.

                    if (firma.Firma.Yetkili != "" && firma.Firma.CepTel != "")
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiBilgiMesajiFirma && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", firma.Firma.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", firma.Firma.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = firma.Firma.Yetkili;
                            SmsListesi.AliciTel = firma.Firma.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiBilgiMesajiFirma_smsGonderimUcGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == UcGunOnce && (x.FotografSecimDurum == "2" || x.FotografSecimDurum == "4") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    // SMS Firma Yetkilisine iletilecek. Daha sonra ilgili personele iletilecek.

                    if (firma.Firma.Yetkili != "" && firma.Firma.CepTel != "")
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiBilgiMesajiFirma && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", firma.Firma.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", firma.Firma.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = firma.Firma.Yetkili;
                            SmsListesi.AliciTel = firma.Firma.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiBilgiMesajiFirma_smsGonderimBirHaftaOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == BirHaftaOnce && (x.FotografSecimDurum == "2" || x.FotografSecimDurum == "4") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    // SMS Firma Yetkilisine iletilecek. Daha sonra ilgili personele iletilecek.

                    if (firma.Firma.Yetkili != "" && firma.Firma.CepTel != "")
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiBilgiMesajiFirma && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", firma.Firma.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", firma.Firma.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = firma.Firma.Yetkili;
                            SmsListesi.AliciTel = firma.Firma.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            //            0: İşlem Yok
            //            1: Müşteriye Seçime Gönderildi
            //            2: Müşteri Seçimi Yaptı
            //            3: Müşteriye Tekrar Göderildi
            //            4: Müşteri Seçimleri Baskı için Onayladı

            #endregion
            #region FotoğrafSeçimiHatırlatmaMesajıMüşteri
            List<AyarlarSmsGonderim> FotografSecimiHatirlatmaMesajiMusteri_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiHatirlatmaGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> FotografSecimiHatirlatmaMesajiMusteri_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiHatirlatmaGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> FotografSecimiHatirlatmaMesajiMusteri_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiHatirlatmaGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> FotografSecimiHatirlatmaMesajiMusteri_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiHatirlatmaGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> FotografSecimiHatirlatmaMesajiMusteri_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.FotografSecimiHatirlatmaGonderimSuresi == 5).ToList();
            //            0: İşlem Yok
            //            1: Müşteriye Seçime Gönderildi
            //            2: Müşteri Seçimi Yaptı
            //            3: Müşteriye Tekrar Göderildi
            //            4: Müşteri Seçimleri Baskı için Onayladı
            foreach (var firma in FotografSecimiHatirlatmaMesajiMusteri_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == BuGun && (x.FotografSecimDurum == "1" || x.FotografSecimDurum == "3") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiHatirlatmaMesajiMusteri_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == BirGunOnce && (x.FotografSecimDurum == "1" || x.FotografSecimDurum == "3") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiHatirlatmaMesajiMusteri_smsGonderimIkiGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == IkiGunOnce && (x.FotografSecimDurum == "1" || x.FotografSecimDurum == "3") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiHatirlatmaMesajiMusteri_smsGonderimUcGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == UcGunOnce && (x.FotografSecimDurum == "1" || x.FotografSecimDurum == "3") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            foreach (var firma in FotografSecimiHatirlatmaMesajiMusteri_smsGonderimBirHaftaOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşmelerden fotoğraf seçimi 
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.FotografSecimDurumTarihi == BirHaftaOnce && (x.FotografSecimDurum == "1" || x.FotografSecimDurum == "3") && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }
                    if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.FotografSecimiHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak.
                            SmsListesi.FirmaId = firma.FirmaId;
                            SmsListesi.SmsMetni = smsmetin;
                            SmsListesi.GonderimTarihi = DateTime.Now;
                            SmsListesi.AliciAdSoyad = musteri.AdiSoyadi;
                            SmsListesi.AliciTel = musteri.CepTel;
                            dbContext.OtomatikSmsListesis.Add(SmsListesi);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            //            0: İşlem Yok
            //            1: Müşteriye Seçime Gönderildi
            //            2: Müşteri Seçimi Yaptı
            //            3: Müşteriye Tekrar Göderildi
            //            4: Müşteri Seçimleri Baskı için Onayladı

            #endregion
            #region EvlilikYıldönümüKutlamaMesajı
            List<AyarlarSmsGonderim> EvlilikYildonumuTebrik_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.EvlilikYildonumuTebrikGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> EvlilikYildonumuTebrik_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.EvlilikYildonumuTebrikGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> EvlilikYildonumuTebrik_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.EvlilikYildonumuTebrikGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> EvlilikYildonumuTebrik_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.EvlilikYildonumuTebrikGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> EvlilikYildonumuTebrik_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.EvlilikYildonumuTebrikGonderimSuresi == 5).ToList();
            foreach (var firma in EvlilikYildonumuTebrik_smsGonderimAyniGun_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi.Year < DateTime.Now.Year && x.RezervasyonTarihi.Month == DateTime.Now.Month && x.RezervasyonTarihi.Day == DateTime.Now.Day && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }

                    // Gelin Ve Damat'a SMS gönderilecek.
                    if (sz.DamatTel != "" || sz.GelinTel != "")
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.EvlilikYildonumuTebrikMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak. DAMAT
                            if (string.IsNullOrEmpty(sz.DamatTel))
                            {
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = sz.DamatAd;
                                SmsListesi.AliciTel = sz.DamatTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                            // GELİN
                            if (string.IsNullOrEmpty(sz.GelinTel))
                            {
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = sz.GelinAd;
                                SmsListesi.AliciTel = sz.GelinTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in EvlilikYildonumuTebrik_smsGonderimBirGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi.Year < DateTime.Now.Year && x.RezervasyonTarihi.Month == DateTime.Now.Month && x.RezervasyonTarihi.Day == BirGunSonra.Day && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }

                    // Gelin Ve Damat'a SMS gönderilecek.
                    if (sz.DamatTel != "" || sz.GelinTel != "")
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.EvlilikYildonumuTebrikMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak. DAMAT
                            if (string.IsNullOrEmpty(sz.DamatTel))
                            {
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = sz.DamatAd;
                                SmsListesi.AliciTel = sz.DamatTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                            // GELİN
                            if (string.IsNullOrEmpty(sz.GelinTel))
                            {
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = sz.GelinAd;
                                SmsListesi.AliciTel = sz.GelinTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in EvlilikYildonumuTebrik_smsGonderimIkiGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi.Year < DateTime.Now.Year && x.RezervasyonTarihi.Month == DateTime.Now.Month && x.RezervasyonTarihi.Day == IkiGunSonra.Day && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }

                    // Gelin Ve Damat'a SMS gönderilecek.
                    if (sz.DamatTel != "" || sz.GelinTel != "")
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.EvlilikYildonumuTebrikMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak. DAMAT
                            if (string.IsNullOrEmpty(sz.DamatTel))
                            {
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = sz.DamatAd;
                                SmsListesi.AliciTel = sz.DamatTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                            // GELİN
                            if (string.IsNullOrEmpty(sz.GelinTel))
                            {
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = sz.GelinAd;
                                SmsListesi.AliciTel = sz.GelinTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in EvlilikYildonumuTebrik_smsGonderimUcGunOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi.Year < DateTime.Now.Year && x.RezervasyonTarihi.Month == DateTime.Now.Month && x.RezervasyonTarihi.Day == UcGunSonra.Day && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }

                    // Gelin Ve Damat'a SMS gönderilecek.
                    if (sz.DamatTel != "" || sz.GelinTel != "")
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.EvlilikYildonumuTebrikMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak. DAMAT
                            if (string.IsNullOrEmpty(sz.DamatTel))
                            {
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = sz.DamatAd;
                                SmsListesi.AliciTel = sz.DamatTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                            // GELİN
                            if (string.IsNullOrEmpty(sz.GelinTel))
                            {
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = sz.GelinAd;
                                SmsListesi.AliciTel = sz.GelinTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            foreach (var firma in EvlilikYildonumuTebrik_smsGonderimBirHaftaOnce_Firmalar)
            {
                // bulunan firmalara ait sözleşme tarihi bugünden 1 gün büyük olan sözleşmeler bulunuyor.
                List<Sozlesme> sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == firma.FirmaId && x.RezervasyonTarihi.Year < DateTime.Now.Year && x.RezervasyonTarihi.Month == DateTime.Now.Month && x.RezervasyonTarihi.Day == BirHaftaSonra.Day && x.Aktif == true && x.Sil == false).ToList();
                foreach (var sz in sozlesme)
                {
                    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (musteri == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
                    }

                    // Gelin Ve Damat'a SMS gönderilecek.
                    if (sz.DamatTel != "" || sz.GelinTel != "")
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.EvlilikYildonumuTebrikMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            // Sözleşmeye göre Sms gönderim metni düzenlenecek
                            string smsmetin = smsMetin.SMSMetni;
                            #region Metne eklenecek randevular
                            string randevular = "";
                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == firma.FirmaId && x.SozlesmeId == sz.Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
                            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
                            {
                                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                                if (!string.IsNullOrEmpty(item.Aciklama))
                                {
                                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                                }
                            }
                            #endregion
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                                smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                            if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                                smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                            // Geçici Tabloya Aktarılacak. DAMAT
                            if (string.IsNullOrEmpty(sz.DamatTel))
                            {
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = sz.DamatAd;
                                SmsListesi.AliciTel = sz.DamatTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                            // GELİN
                            if (string.IsNullOrEmpty(sz.GelinTel))
                            {
                                SmsListesi.FirmaId = firma.FirmaId;
                                SmsListesi.SmsMetni = smsmetin;
                                SmsListesi.GonderimTarihi = DateTime.Now;
                                SmsListesi.AliciAdSoyad = sz.GelinAd;
                                SmsListesi.AliciTel = sz.GelinTel;
                                dbContext.OtomatikSmsListesis.Add(SmsListesi);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            #endregion
            #region CariyeYapılanÖdemeBilgiMesajı
            List<AyarlarSmsGonderim> CariyeYapilanOdemeBilgi_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariyeYapilanOdemeBilgiGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> CariyeYapilanOdemeBilgi_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariyeYapilanOdemeBilgiGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> CariyeYapilanOdemeBilgi_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariyeYapilanOdemeBilgiGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> CariyeYapilanOdemeBilgi_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariyeYapilanOdemeBilgiGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> CariyeYapilanOdemeBilgi_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariyeYapilanOdemeBilgiGonderimSuresi == 5).ToList();
            foreach (var firma in CariyeYapilanOdemeBilgi_smsGonderimAyniGun_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == true && x.Tip == "Borç" && x.OdemeTarihi == BuGun && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariyeYapilanOdemeBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariyeYapilanOdemeBilgi_smsGonderimBirGunOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == true && x.Tip == "Borç" && x.OdemeTarihi == BirGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariyeYapilanOdemeBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariyeYapilanOdemeBilgi_smsGonderimIkiGunOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == true && x.Tip == "Borç" && x.OdemeTarihi == IkiGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariyeYapilanOdemeBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariyeYapilanOdemeBilgi_smsGonderimUcGunOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == true && x.Tip == "Borç" && x.OdemeTarihi == UcGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariyeYapilanOdemeBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariyeYapilanOdemeBilgi_smsGonderimBirHaftaOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == true && x.Tip == "Borç" && x.OdemeTarihi == BirHaftaSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariyeYapilanOdemeBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            #endregion
            #region CaridenYapılanTahsilatBilgiMesajı
            List<AyarlarSmsGonderim> CariTahsilatBilgi_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariTahsilatBilgiGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> CariTahsilatBilgi_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariTahsilatBilgiGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> CariTahsilatBilgi_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariTahsilatBilgiGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> CariTahsilatBilgi_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariTahsilatBilgiGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> CariTahsilatBilgi_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariTahsilatBilgiGonderimSuresi == 5).ToList();
            foreach (var firma in CariTahsilatBilgi_smsGonderimAyniGun_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == true && x.Tip == "Alacak" && x.OdemeTarihi == BuGun && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariTahsilatBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariTahsilatBilgi_smsGonderimBirGunOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == true && x.Tip == "Alacak" && x.OdemeTarihi == BirGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariTahsilatBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariyeYapilanOdemeBilgi_smsGonderimIkiGunOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == true && x.Tip == "Alacak" && x.OdemeTarihi == IkiGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariTahsilatBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariyeYapilanOdemeBilgi_smsGonderimUcGunOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == true && x.Tip == "Alacak" && x.OdemeTarihi == UcGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariTahsilatBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariyeYapilanOdemeBilgi_smsGonderimBirHaftaOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == true && x.Tip == "Alacak" && x.OdemeTarihi == BirHaftaSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariTahsilatBilgiMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            #endregion
            #region CariAlacakHatırlatmaMesajı
            List<AyarlarSmsGonderim> CariAlacakHatirlatma_smsGonderimAyniGun_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariAlacakHatirlatmaGonderimSuresi == 1).ToList();
            List<AyarlarSmsGonderim> CariAlacakHatirlatma_smsGonderimBirGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariAlacakHatirlatmaGonderimSuresi == 2).ToList();
            List<AyarlarSmsGonderim> CariAlacakHatirlatma_smsGonderimIkiGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariAlacakHatirlatmaGonderimSuresi == 3).ToList();
            List<AyarlarSmsGonderim> CariAlacakHatirlatma_smsGonderimUcGunOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariAlacakHatirlatmaGonderimSuresi == 4).ToList();
            List<AyarlarSmsGonderim> CariAlacakHatirlatma_smsGonderimBirHaftaOnce_Firmalar = dbContext.AyarlarSmsGonderims.Where(x => x.CariAlacakHatirlatmaGonderimSuresi == 5).ToList();
            foreach (var firma in CariAlacakHatirlatma_smsGonderimAyniGun_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == false && x.Tip == "Alacak" && x.OdemeTarihi == BuGun && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariAlacakHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariAlacakHatirlatma_smsGonderimBirGunOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == false && x.Tip == "Alacak" && x.OdemeTarihi == BirGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariAlacakHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariAlacakHatirlatma_smsGonderimIkiGunOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == false && x.Tip == "Alacak" && x.OdemeTarihi == IkiGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariAlacakHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariAlacakHatirlatma_smsGonderimUcGunOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == false && x.Tip == "Alacak" && x.OdemeTarihi == UcGunSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariAlacakHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }
            foreach (var firma in CariAlacakHatirlatma_smsGonderimBirHaftaOnce_Firmalar)
            {
                List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.TahsilatOdemeBit == false && x.Tip == "Alacak" && x.OdemeTarihi == BirHaftaSonra && x.Aktif == true && x.Sil == false).ToList();
                foreach (var cariislem in carihareket)
                {
                    Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == cariislem.CariId && x.FirmaId == firma.FirmaId && x.Aktif == true && x.Sil == false);
                    Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == firma.Id && x.CariHareketId == cari.Id && x.Aktif == true && x.Sil == false);
                    bool smskabul = true, emailkabul = true;
                    if (cari == null)
                    {
                        smskabul = true; emailkabul = true;
                    }
                    else
                    {
                        smskabul = cari.SMSKabul; emailkabul = cari.EmailKabul;
                    }
                    if ((cari.Yetkili != "" || cari.AdSoyad != "") && cari.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        SmsMetinleri smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.FirmaId && x.Id == firma.CariAlacakHatirlatmaMesaji && x.Aktif == true && x.Sil == false);
                        if (smsMetin != null)
                        {
                            string smsmetin = smsMetin.SMSMetni;

                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                smsmetin = smsmetin.Replace("{AdSoyad}", cari.Yetkili != "" ? cari.Yetkili : cari.AdSoyad);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi != "" ? cari.FirmaAdi : cari.AdSoyad);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", cariislem.OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", cariislem.Tutar.ToString());
                            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                                smsmetin = smsmetin.Replace("{SozlesmeNo}", odeme.CariHareketId == 1 ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                                smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo == null ? "" : odeme.AlinanOdemeMakbuzNo.ToString());
                        }
                    }
                }
            }

            #endregion

            return Json(new { Sonuc = true, Mesaj = "Sms Bilgileri Geçici Tabloya Alındı <br/> Sms gönderilebilir." }, JsonRequestBehavior.AllowGet);
        }
    }
}