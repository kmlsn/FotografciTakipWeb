using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FotografciTakipWeb.Models;
using FotografciTakipWeb.App_Settings;
using System.Net.Mail;
using System.Net;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class AyarlarController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/Ayarlar
        #region Genel Ayarlar
        public ActionResult GenelAyarlar()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Genel Ayarlar";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 90 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            AyarlarGenel ayar = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId);
            if (ayar != null)
                return View(ayar);
            else
                return View();
        }
        [HttpPost]
        public ActionResult GenelAyarlar(string Id, string CalismaGunuCumartesi, string CalismaGunuPazar, string CariRehberKayit, string MusteriRehberKayit,
                                         string PersonelRehberKayit, string GelinDamatRehberKayit, string AnneBabaRehberKayit, string RezervasyonYetkiliRehberKayit, string KonturUyariMiktari, string KonturUyariVer)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Email Gönderim Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long AyarId = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                AyarId = Convert.ToInt64(Id);
            }
            AyarlarGenel a = dbContext.AyarlarGenels.FirstOrDefault(x => x.Id == AyarId && x.FirmaId == FirmaId);
            if (AyarId != 0) // Daha önce kaydedilmiş bir ayar varsa güncellenecek
            {

                if (CalismaGunuCumartesi == "on")
                    a.CalismaGunuCumartesi = true;
                else
                    a.CalismaGunuCumartesi = false;
                if (CalismaGunuPazar == "on")
                    a.CalismaGunuPazar = true;
                else
                    a.CalismaGunuPazar = false;
                if (CariRehberKayit == "on")
                    a.CariRehberKayit = true;
                else
                    a.CariRehberKayit = false;
                if (MusteriRehberKayit == "on")
                    a.MusteriRehberKayit = true;
                else
                    a.MusteriRehberKayit = false;
                if (PersonelRehberKayit == "on")
                    a.PersonelRehberKayit = true;
                else
                    a.PersonelRehberKayit = false;
                if (GelinDamatRehberKayit == "on")
                    a.GelinDamatRehberKayit = true;
                else
                    a.GelinDamatRehberKayit = false;
                if (AnneBabaRehberKayit == "on")
                    a.AnneBabaRehberKayit = true;
                else
                    a.AnneBabaRehberKayit = false;
                if (RezervasyonYetkiliRehberKayit == "on")
                    a.RezervasyonYetkiliRehberKayit = true;
                else
                    a.RezervasyonYetkiliRehberKayit = false;
                if (KonturUyariVer == "on")
                    a.KonturUyariVer = true;
                else
                    a.KonturUyariVer = false;
                a.KonturUyariMiktari = Convert.ToInt32(KonturUyariMiktari);
                a.DegistirenKullaniciId = KullaniciId;
                a.DegistirmeTarih = DateTime.Now;

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            else // Daha önce kaydedilmiş bir ayar yoksa yeni kaydedilecek
            {
                AyarlarGenel ayar = new AyarlarGenel();
                ayar.FirmaId = FirmaId;
                if (CalismaGunuCumartesi == "on")
                    ayar.CalismaGunuCumartesi = true;
                else
                    ayar.CalismaGunuCumartesi = false;
                if (CalismaGunuPazar == "on")
                    ayar.CalismaGunuPazar = true;
                else
                    ayar.CalismaGunuPazar = false;
                if (CariRehberKayit == "on")
                    ayar.CariRehberKayit = true;
                else
                    ayar.CariRehberKayit = false;
                if (MusteriRehberKayit == "on")
                    ayar.MusteriRehberKayit = true;
                else
                    ayar.MusteriRehberKayit = false;
                if (PersonelRehberKayit == "on")
                    ayar.PersonelRehberKayit = true;
                else
                    ayar.PersonelRehberKayit = false;
                if (GelinDamatRehberKayit == "on")
                    ayar.GelinDamatRehberKayit = true;
                else
                    ayar.GelinDamatRehberKayit = false;
                if (AnneBabaRehberKayit == "on")
                    ayar.AnneBabaRehberKayit = true;
                else
                    ayar.AnneBabaRehberKayit = false;
                if (RezervasyonYetkiliRehberKayit == "on")
                    ayar.RezervasyonYetkiliRehberKayit = true;
                else
                    ayar.RezervasyonYetkiliRehberKayit = false;
                if (KonturUyariVer == "on")
                    a.KonturUyariVer = true;
                else
                    a.KonturUyariVer = false;
                ayar.KonturUyariMiktari = Convert.ToInt32(KonturUyariMiktari);
                ayar.OlusturanKullaniciId = KullaniciId;
                ayar.OlusturmaTarih = DateTime.Now;
                ayar.DegistirenKullaniciId = KullaniciId;
                ayar.DegistirmeTarih = DateTime.Now;
                ayar.Aktif = true;
                ayar.Sil = false;
                try
                {
                    dbContext.AyarlarGenels.Add(ayar);
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("GenelAyarlar");
        }

        #endregion

        #region Email ayarları
        public ActionResult EmailGonderimAyarlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Email Gönderim Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            AyarlarMailGonderim ayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            ViewBag.MailMetinleri = dbContext.MailMetinleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 91 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            if (ayar != null)
                return View(ayar);
            else
                return View();
        }
        [HttpPost]
        public ActionResult EmailGonderimAyarlari(string Id, string RezervasyonTarihiBilgiMaili, string RezervasyonTarihiBilgiGonderimSuresi, string RezervasyonTarihiHatirlatmaMaili, string RezervasyonTarihiHatirlatmaGonderimSuresi,
                                        string OpsiyonTarihiBilgiMaili, string OpsiyonTarihiBilgiGonderimSuresi, string OpsiyonTarihiHatirlatmaMaili, string OpsiyonTarihiHatirlatmaGonderimSuresi,
                                        string MusteriCekimRandevusuBilgiMaili, string MusteriCekimRandevusuBilgiGonderimSuresi, string MusteriCekimRandevusuHatirlatmaMaili, string MusteriCekimRandevusuHatirlatmaGonderimSuresi,
                                        string PersonelRandevuBilgiMaili, string PersonelRandevuBilgiGonderimSuresi, string PersonelRandevuHatirlatmaMaili, string PersonelRandevuHatirlatmaGonderimSuresi,
                                        string MusteriOdemeBilgiMaili, string MusteriOdemeBilgiGonderimSuresi, string MusteriOdemeHatirlatmaMaili, string MusteriOdemeHatirlatmaGonderimSuresi,
                                        string FotografSecimiHatirlatmaMaili, string FotografSecimiHatirlatmaGonderimSuresi, string EvlilikYildonumuTebrikMaili, string EvlilikYildonumuTebrikGonderimSuresi,
                                        string FotografSecimiBilgiMailiMusteri, string FotografSecimiBilgiMailiMusteriGonderimSuresi, string FotografSecimiBilgiMailiFirma, string FotografSecimiBilgiMailiFirmaGonderimSuresi,
                                        string CariyeYapilanOdemeBilgiMaili, string CariyeYapilanOdemeBilgiGonderimSuresi, string CariAlacakHatirlatmaMaili, string CariAlacakHatirlatmaGonderimSuresi,
                                        string CariTahsilatBilgiMaili, string CariTahsilatBilgiGonderimSuresi, string GunlukIsOdemeBilgiMaili, string GunlukIsOdemeBilgiGonderimSuresi, string SurecDegisiklikBilgiMaili, string SurecDegisiklikBilgiGonderimSuresi)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Email Gönderim Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long AyarId = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                AyarId = Convert.ToInt64(Id);
            }
            AyarlarMailGonderim a = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.Id == AyarId && x.FirmaId == FirmaId);
            if (AyarId != 0) // Daha önce kaydedilmiş bir ayar varsa güncellenecek
            {
                a.RezervasyonTarihiBilgiMaili = Convert.ToInt64(RezervasyonTarihiBilgiMaili);
                a.RezervasyonTarihiBilgiGonderimSuresi = Convert.ToInt16(RezervasyonTarihiBilgiGonderimSuresi);
                a.RezervasyonTarihiHatirlatmaMaili = Convert.ToInt64(RezervasyonTarihiHatirlatmaMaili);
                a.RezervasyonTarihiHatirlatmaGonderimSuresi = Convert.ToInt16(RezervasyonTarihiHatirlatmaGonderimSuresi);
                a.OpsiyonTarihiBilgiMaili = Convert.ToInt64(OpsiyonTarihiBilgiMaili);
                a.OpsiyonTarihiBilgiGonderimSuresi = Convert.ToInt16(OpsiyonTarihiBilgiGonderimSuresi);
                a.OpsiyonTarihiHatirlatmaMaili = Convert.ToInt64(OpsiyonTarihiHatirlatmaMaili);
                a.OpsiyonTarihiHatirlatmaGonderimSuresi = Convert.ToInt16(OpsiyonTarihiHatirlatmaGonderimSuresi);
                a.MusteriCekimRandevusuBilgiMaili = Convert.ToInt64(MusteriCekimRandevusuBilgiMaili);
                a.MusteriCekimRandevusuBilgiGonderimSuresi = Convert.ToInt16(MusteriCekimRandevusuBilgiGonderimSuresi);
                a.MusteriCekimRandevusuHatirlatmaMaili = Convert.ToInt64(MusteriCekimRandevusuHatirlatmaMaili);
                a.MusteriCekimRandevusuHatirlatmaGonderimSuresi = Convert.ToInt16(MusteriCekimRandevusuHatirlatmaGonderimSuresi);
                a.PersonelRandevuBilgiMaili = Convert.ToInt64(PersonelRandevuBilgiMaili);
                a.PersonelRandevuBilgiGonderimSuresi = Convert.ToInt16(PersonelRandevuBilgiGonderimSuresi);
                a.PersonelRandevuHatirlatmaMaili = Convert.ToInt64(PersonelRandevuHatirlatmaMaili);
                a.PersonelRandevuHatirlatmaGonderimSuresi = Convert.ToInt16(PersonelRandevuHatirlatmaGonderimSuresi);
                a.MusteriOdemeBilgiMaili = Convert.ToInt64(MusteriOdemeBilgiMaili);
                a.MusteriOdemeBilgiGonderimSuresi = Convert.ToInt16(MusteriOdemeBilgiGonderimSuresi);
                a.MusteriOdemeHatirlatmaMaili = Convert.ToInt64(MusteriOdemeHatirlatmaMaili);
                a.MusteriOdemeHatirlatmaGonderimSuresi = Convert.ToInt16(MusteriOdemeHatirlatmaGonderimSuresi);
                a.FotografSecimiHatirlatmaMaili = Convert.ToInt64(FotografSecimiHatirlatmaMaili);
                a.FotografSecimiHatirlatmaGonderimSuresi = Convert.ToInt16(FotografSecimiHatirlatmaGonderimSuresi);
                a.FotografSecimiBilgiMailiMusteri = Convert.ToInt64(FotografSecimiBilgiMailiMusteri);
                a.FotografSecimiBilgiMailiMusteriGonderimSuresi = Convert.ToInt16(FotografSecimiBilgiMailiMusteriGonderimSuresi);
                a.FotografSecimiBilgiMailiFirma = Convert.ToInt64(FotografSecimiBilgiMailiFirma);
                a.FotografSecimiBilgiMailiFirmaGonderimSuresi = Convert.ToInt16(FotografSecimiBilgiMailiFirmaGonderimSuresi);
                a.EvlilikYildonumuTebrikMaili = Convert.ToInt64(EvlilikYildonumuTebrikMaili);
                a.EvlilikYildonumuTebrikGonderimSuresi = Convert.ToInt16(EvlilikYildonumuTebrikGonderimSuresi);
                a.CariyeYapilanOdemeBilgiMaili = Convert.ToInt64(CariyeYapilanOdemeBilgiMaili);
                a.CariyeYapilanOdemeBilgiGonderimSuresi = Convert.ToInt16(CariyeYapilanOdemeBilgiGonderimSuresi);
                a.CariAlacakHatirlatmaMaili = Convert.ToInt64(CariAlacakHatirlatmaMaili);
                a.CariAlacakHatirlatmaGonderimSuresi = Convert.ToInt16(CariAlacakHatirlatmaGonderimSuresi);
                a.CariTahsilatBilgiMaili = Convert.ToInt64(CariTahsilatBilgiMaili);
                a.CariTahsilatBilgiGonderimSuresi = Convert.ToInt16(CariTahsilatBilgiGonderimSuresi);
                a.GunlukIsOdemeBilgiMaili = Convert.ToInt64(GunlukIsOdemeBilgiMaili);
                a.GunlukIsOdemeBilgiGonderimSuresi = Convert.ToInt16(GunlukIsOdemeBilgiGonderimSuresi);
                a.SurecDegisiklikBilgiMaili = Convert.ToInt64(SurecDegisiklikBilgiMaili);
                a.SurecDegisiklikBilgiGonderimSuresi = Convert.ToInt16(SurecDegisiklikBilgiGonderimSuresi);
                a.DegistirenKullaniciId = KullaniciId;
                a.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            else // Daha önce kaydedilmiş bir ayar yoksa yeni kaydedilecek
            {
                AyarlarMailGonderim ayar = new AyarlarMailGonderim();
                ayar.FirmaId = FirmaId;

                ayar.RezervasyonTarihiBilgiMaili = Convert.ToInt64(RezervasyonTarihiBilgiMaili);
                ayar.RezervasyonTarihiBilgiGonderimSuresi = Convert.ToInt16(RezervasyonTarihiBilgiGonderimSuresi);
                ayar.RezervasyonTarihiHatirlatmaMaili = Convert.ToInt64(RezervasyonTarihiHatirlatmaMaili);
                ayar.RezervasyonTarihiHatirlatmaGonderimSuresi = Convert.ToInt16(RezervasyonTarihiHatirlatmaGonderimSuresi);
                ayar.OpsiyonTarihiBilgiMaili = Convert.ToInt64(OpsiyonTarihiBilgiMaili);
                ayar.OpsiyonTarihiBilgiGonderimSuresi = Convert.ToInt16(OpsiyonTarihiBilgiGonderimSuresi);
                ayar.OpsiyonTarihiHatirlatmaMaili = Convert.ToInt64(OpsiyonTarihiHatirlatmaMaili);
                ayar.OpsiyonTarihiHatirlatmaGonderimSuresi = Convert.ToInt16(OpsiyonTarihiHatirlatmaGonderimSuresi);
                ayar.MusteriCekimRandevusuBilgiMaili = Convert.ToInt64(MusteriCekimRandevusuBilgiMaili);
                ayar.MusteriCekimRandevusuBilgiGonderimSuresi = Convert.ToInt16(MusteriCekimRandevusuBilgiGonderimSuresi);
                ayar.MusteriCekimRandevusuHatirlatmaMaili = Convert.ToInt64(MusteriCekimRandevusuHatirlatmaMaili);
                ayar.MusteriCekimRandevusuHatirlatmaGonderimSuresi = Convert.ToInt16(MusteriCekimRandevusuHatirlatmaGonderimSuresi);
                ayar.PersonelRandevuBilgiMaili = Convert.ToInt64(PersonelRandevuBilgiMaili);
                ayar.PersonelRandevuBilgiGonderimSuresi = Convert.ToInt16(PersonelRandevuBilgiGonderimSuresi);
                ayar.PersonelRandevuHatirlatmaMaili = Convert.ToInt64(PersonelRandevuHatirlatmaMaili);
                ayar.PersonelRandevuHatirlatmaGonderimSuresi = Convert.ToInt16(PersonelRandevuHatirlatmaGonderimSuresi);
                ayar.MusteriOdemeBilgiMaili = Convert.ToInt64(MusteriOdemeBilgiMaili);
                ayar.MusteriOdemeBilgiGonderimSuresi = Convert.ToInt16(MusteriOdemeBilgiGonderimSuresi);
                ayar.MusteriOdemeHatirlatmaMaili = Convert.ToInt64(MusteriOdemeHatirlatmaMaili);
                ayar.MusteriOdemeHatirlatmaGonderimSuresi = Convert.ToInt16(MusteriOdemeHatirlatmaGonderimSuresi);
                ayar.FotografSecimiHatirlatmaMaili = Convert.ToInt64(FotografSecimiHatirlatmaMaili);
                ayar.FotografSecimiHatirlatmaGonderimSuresi = Convert.ToInt16(FotografSecimiHatirlatmaGonderimSuresi);
                ayar.FotografSecimiBilgiMailiMusteri = Convert.ToInt64(FotografSecimiBilgiMailiMusteri);
                ayar.FotografSecimiBilgiMailiMusteriGonderimSuresi = Convert.ToInt16(FotografSecimiBilgiMailiMusteriGonderimSuresi);
                ayar.FotografSecimiBilgiMailiFirma = Convert.ToInt64(FotografSecimiBilgiMailiFirma);
                ayar.FotografSecimiBilgiMailiFirmaGonderimSuresi = Convert.ToInt16(FotografSecimiBilgiMailiFirmaGonderimSuresi);
                ayar.EvlilikYildonumuTebrikMaili = Convert.ToInt64(EvlilikYildonumuTebrikMaili);
                ayar.EvlilikYildonumuTebrikGonderimSuresi = Convert.ToInt16(EvlilikYildonumuTebrikGonderimSuresi);
                ayar.CariyeYapilanOdemeBilgiMaili = Convert.ToInt64(CariyeYapilanOdemeBilgiMaili);
                ayar.CariyeYapilanOdemeBilgiGonderimSuresi = Convert.ToInt16(CariyeYapilanOdemeBilgiGonderimSuresi);
                ayar.CariAlacakHatirlatmaMaili = Convert.ToInt64(CariAlacakHatirlatmaMaili);
                ayar.CariAlacakHatirlatmaGonderimSuresi = Convert.ToInt16(CariAlacakHatirlatmaGonderimSuresi);
                ayar.CariTahsilatBilgiMaili = Convert.ToInt64(CariTahsilatBilgiMaili);
                ayar.CariTahsilatBilgiGonderimSuresi = Convert.ToInt16(CariTahsilatBilgiGonderimSuresi);
                ayar.GunlukIsOdemeBilgiMaili = Convert.ToInt64(GunlukIsOdemeBilgiMaili);
                ayar.GunlukIsOdemeBilgiGonderimSuresi = Convert.ToInt16(GunlukIsOdemeBilgiGonderimSuresi);
                ayar.SurecDegisiklikBilgiMaili = Convert.ToInt64(SurecDegisiklikBilgiMaili);
                ayar.SurecDegisiklikBilgiGonderimSuresi = Convert.ToInt16(SurecDegisiklikBilgiGonderimSuresi);
                ayar.OlusturanKullaniciId = KullaniciId;
                ayar.OlusturmaTarih = DateTime.Now;
                ayar.DegistirenKullaniciId = KullaniciId;
                ayar.DegistirmeTarih = DateTime.Now;
                ayar.Aktif = true;
                ayar.Sil = false;
                try
                {
                    dbContext.AyarlarMailGonderims.Add(ayar);
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("EmailGonderimAyarlari");
        }


        public ActionResult EmailHesapAyarlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Email Hesap Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            AyarlarMailHesap ayar = dbContext.AyarlarMailHesaps.FirstOrDefault(x => x.FirmaId == FirmaId);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 92 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            if (ayar != null)
                return View(ayar);
            else
                return View();
        }
        [HttpPost]
        public ActionResult EmailHesapAyarlari(AyarlarMailHesap ayar, string SslKullan)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Email Hesap Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            MD5Sifreleme sifrele = new MD5Sifreleme();
            AyarlarMailHesap a = dbContext.AyarlarMailHesaps.FirstOrDefault(x => x.Id == ayar.Id && x.FirmaId == FirmaId);
            if (ayar.Id != 0) // Daha önce kaydedilmiş bir ayar varsa güncellenecek
            {
                a.FirmaId = ayar.FirmaId;
                a.GonderenMail = ayar.GonderenMail;
                a.GonderenSifre = sifrele.Sifrele(ayar.GonderenSifre);
                a.GonderenAdSoyad = ayar.GonderenAdSoyad;
                a.SmtpAdres = ayar.SmtpAdres;
                a.SmtpPort = ayar.SmtpPort;
                if (SslKullan == "on")
                    a.Ssl = true;
                else
                    a.Ssl = false;
                a.Aciklama = ayar.Aciklama;
                a.DegistirenKullaniciId = KullaniciId;
                a.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            else // Daha önce kaydedilmiş bir ayar yoksa yeni kaydedilecek
            {
                if (SslKullan == "on")
                    ayar.Ssl = true;
                else
                    ayar.Ssl = false;
                ayar.FirmaId = FirmaId;
                ayar.GonderenSifre = sifrele.Sifrele(ayar.GonderenSifre);
                ayar.OlusturanKullaniciId = KullaniciId;
                ayar.OlusturmaTarih = DateTime.Now;
                ayar.DegistirenKullaniciId = KullaniciId;
                ayar.DegistirmeTarih = DateTime.Now;
                ayar.Aktif = true;
                ayar.Sil = false;
                dbContext.AyarlarMailHesaps.Add(ayar);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "Email Hesap Ayarları";
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
            return RedirectToAction("EmailHesapAyarlari");
        }
        [HttpPost]
        public ActionResult MailAyarKontrol()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string Email = Request["Email"];
            string AdSoyad = Request["AdSoyad"];
            string GonderenSifre = Request["GonderenSifre"];
            string SmtpAdres = Request["SmtpAdres"];
            string SmtpPort = Request["SmtpPort"];
            string Ssl = Request["Ssl"];

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(Email, AdSoyad);
            mail.To.Add("info@fotografcitakip.com");
            mail.Subject = ("Mail Hesap Ayarları Testi");
            mail.Body = ("Test");
            //mail.AlternateViews.Add(htmlView);
            mail.IsBodyHtml = false;
            // gönderen maile ait mail ayarları
            SmtpClient smtp2 = new SmtpClient();
            smtp2.Credentials = new System.Net.NetworkCredential(Email, GonderenSifre);
            smtp2.Port = Convert.ToInt32(SmtpPort);
            smtp2.Host = SmtpAdres;
            if (Ssl=="Var")
                smtp2.EnableSsl = true;
            else if (Ssl == "Var")
                smtp2.EnableSsl = false;

            //// 2. Yöntem
            //SmtpClient smtp = new SmtpClient(SmtpAdres, Convert.ToInt32(SmtpPort));
            //smtp.UseDefaultCredentials = false;
            //smtp.Credentials = new System.Net.NetworkCredential(Email, GonderenSifre);
            //if (Ssl == "Var")
            //    smtp.EnableSsl = true;
            //else if (Ssl == "Var")
            //    smtp.EnableSsl = false;

            try
            {
                smtp2.Send(mail);
                return Json(new { Sonuc = true, Mesaj = "Test BAŞARILI.", JsonRequestBehavior.AllowGet });

            }
            catch (Exception)
            {

                return Json(new { Sonuc = false, Mesaj = "Test BAŞARISIZ. <br /> Lütfen Ayarları Kontrol Ediniz", JsonRequestBehavior.AllowGet });

            }


        }

        #endregion

        #region SMS Gönderim ayarları
        public ActionResult SmsGonderimAyarlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "SMS Gönderim Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            AyarlarSmsGonderim ayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            ViewBag.SmsMetinleri = dbContext.SmsMetinleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 94 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            if (ayar != null)
                return View(ayar);
            else
                return View();
        }
        [HttpPost]
        public ActionResult SmsGonderimAyarlari(string Id, string RezervasyonTarihiBilgiMesaji, string RezervasyonTarihiBilgiGonderimSuresi, string RezervasyonTarihiHatirlatmaMesaji, string RezervasyonTarihiHatirlatmaGonderimSuresi,
                                        string OpsiyonTarihiBilgiMesaji, string OpsiyonTarihiBilgiGonderimSuresi, string OpsiyonTarihiHatirlatmaMesaji, string OpsiyonTarihiHatirlatmaGonderimSuresi,
                                        string MusteriCekimRandevusuBilgiMesaji, string MusteriCekimRandevusuBilgiGonderimSuresi, string MusteriCekimRandevusuHatirlatmaMesaji, string MusteriCekimRandevusuHatirlatmaGonderimSuresi,
                                        string PersonelRandevuBilgiMesaji, string PersonelRandevuBilgiGonderimSuresi, string PersonelRandevuHatirlatmaMesaji, string PersonelRandevuHatirlatmaGonderimSuresi,
                                        string MusteriOdemeBilgiMesaji, string MusteriOdemeBilgiGonderimSuresi, string MusteriOdemeHatirlatmaMesaji, string MusteriOdemeHatirlatmaGonderimSuresi,
                                        string FotografSecimiHatirlatmaMesaji, string FotografSecimiHatirlatmaGonderimSuresi, string EvlilikYildonumuTebrikMesaji, string EvlilikYildonumuTebrikGonderimSuresi,
                                        string FotografSecimiBilgiMesajiMusteri, string FotografSecimiBilgiMesajiMusteriGonderimSuresi, string FotografSecimiBilgiMesajiFirma, string FotografSecimiBilgiMesajiFirmaGonderimSuresi,
                                        string CariyeYapilanOdemeBilgiMesaji, string CariyeYapilanOdemeBilgiGonderimSuresi, string CariAlacakHatirlatmaMesaji, string CariAlacakHatirlatmaGonderimSuresi,
                                        string CariTahsilatBilgiMesaji, string CariTahsilatBilgiGonderimSuresi, string GunlukIsOdemeBilgiMesaji, string GunlukIsOdemeBilgiGonderimSuresi, string SurecDegisiklikBilgiMesaji, string SurecDegisiklikBilgiGonderimSuresi, string SmsTurkceKarakter)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "SMS Gönderim Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long AyarId = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                AyarId = Convert.ToInt64(Id);
            }
            AyarlarSmsGonderim a = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.Id == AyarId && x.FirmaId == FirmaId);
            if (AyarId != 0) // Daha önce kaydedilmiş bir ayar varsa güncellenecek
            {
                a.RezervasyonTarihiBilgiMesaji = Convert.ToInt64(RezervasyonTarihiBilgiMesaji);
                a.RezervasyonTarihiBilgiGonderimSuresi = Convert.ToInt16(RezervasyonTarihiBilgiGonderimSuresi);
                a.RezervasyonTarihiHatirlatmaMesaji = Convert.ToInt64(RezervasyonTarihiHatirlatmaMesaji);
                a.RezervasyonTarihiHatirlatmaGonderimSuresi = Convert.ToInt16(RezervasyonTarihiHatirlatmaGonderimSuresi);
                a.OpsiyonTarihiBilgiMesaji = Convert.ToInt64(OpsiyonTarihiBilgiMesaji);
                a.OpsiyonTarihiBilgiGonderimSuresi = Convert.ToInt16(OpsiyonTarihiBilgiGonderimSuresi);
                a.OpsiyonTarihiHatirlatmaMesaji = Convert.ToInt64(OpsiyonTarihiHatirlatmaMesaji);
                a.OpsiyonTarihiHatirlatmaGonderimSuresi = Convert.ToInt16(OpsiyonTarihiHatirlatmaGonderimSuresi);
                a.MusteriCekimRandevusuBilgiMesaji = Convert.ToInt64(MusteriCekimRandevusuBilgiMesaji);
                a.MusteriCekimRandevusuBilgiGonderimSuresi = Convert.ToInt16(MusteriCekimRandevusuBilgiGonderimSuresi);
                a.MusteriCekimRandevusuHatirlatmaMesaji = Convert.ToInt64(MusteriCekimRandevusuHatirlatmaMesaji);
                a.MusteriCekimRandevusuHatirlatmaGonderimSuresi = Convert.ToInt16(MusteriCekimRandevusuHatirlatmaGonderimSuresi);
                a.PersonelRandevuBilgiMesaji = Convert.ToInt64(PersonelRandevuBilgiMesaji);
                a.PersonelRandevuBilgiGonderimSuresi = Convert.ToInt16(PersonelRandevuBilgiGonderimSuresi);
                a.PersonelRandevuHatirlatmaMesaji = Convert.ToInt64(PersonelRandevuHatirlatmaMesaji);
                a.PersonelRandevuHatirlatmaGonderimSuresi = Convert.ToInt16(PersonelRandevuHatirlatmaGonderimSuresi);
                a.MusteriOdemeBilgiMesaji = Convert.ToInt64(MusteriOdemeBilgiMesaji);
                a.MusteriOdemeBilgiGonderimSuresi = Convert.ToInt16(MusteriOdemeBilgiGonderimSuresi);
                a.MusteriOdemeHatirlatmaMesaji = Convert.ToInt64(MusteriOdemeHatirlatmaMesaji);
                a.MusteriOdemeHatirlatmaGonderimSuresi = Convert.ToInt16(MusteriOdemeHatirlatmaGonderimSuresi);
                a.FotografSecimiHatirlatmaMesaji = Convert.ToInt64(FotografSecimiHatirlatmaMesaji);
                a.FotografSecimiHatirlatmaGonderimSuresi = Convert.ToInt16(FotografSecimiHatirlatmaGonderimSuresi);
                a.FotografSecimiBilgiMesajiMusteri = Convert.ToInt64(FotografSecimiBilgiMesajiMusteri);
                a.FotografSecimiBilgiMesajiMusteriGonderimSuresi = Convert.ToInt16(FotografSecimiBilgiMesajiMusteriGonderimSuresi);
                a.FotografSecimiBilgiMesajiFirma = Convert.ToInt64(FotografSecimiBilgiMesajiFirma);
                a.FotografSecimiBilgiMesajiFirmaGonderimSuresi = Convert.ToInt16(FotografSecimiBilgiMesajiFirmaGonderimSuresi);
                a.EvlilikYildonumuTebrikMesaji = Convert.ToInt64(EvlilikYildonumuTebrikMesaji);
                a.EvlilikYildonumuTebrikGonderimSuresi = Convert.ToInt16(EvlilikYildonumuTebrikGonderimSuresi);
                a.CariyeYapilanOdemeBilgiMesaji = Convert.ToInt64(CariyeYapilanOdemeBilgiMesaji);
                a.CariyeYapilanOdemeBilgiGonderimSuresi = Convert.ToInt16(CariyeYapilanOdemeBilgiGonderimSuresi);
                a.CariAlacakHatirlatmaMesaji = Convert.ToInt64(CariAlacakHatirlatmaMesaji);
                a.CariAlacakHatirlatmaGonderimSuresi = Convert.ToInt16(CariAlacakHatirlatmaGonderimSuresi);
                a.CariTahsilatBilgiMesaji = Convert.ToInt64(CariTahsilatBilgiMesaji);
                a.CariTahsilatBilgiGonderimSuresi = Convert.ToInt16(CariTahsilatBilgiGonderimSuresi);
                a.GunlukIsOdemeBilgiMesaji = Convert.ToInt64(GunlukIsOdemeBilgiMesaji);
                a.GunlukIsOdemeBilgiGonderimSuresi = Convert.ToInt16(GunlukIsOdemeBilgiGonderimSuresi);
                a.SurecDegisiklikBilgiMesaji = Convert.ToInt64(SurecDegisiklikBilgiMesaji);
                a.SurecDegisiklikBilgiGonderimSuresi = Convert.ToInt16(SurecDegisiklikBilgiGonderimSuresi);
                if (SmsTurkceKarakter == "on")
                    a.SmsTurkceKarakter = true;
                else
                    a.SmsTurkceKarakter = false;
                a.DegistirenKullaniciId = KullaniciId;
                a.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            else // Daha önce kaydedilmiş bir ayar yoksa yeni kaydedilecek
            {
                AyarlarSmsGonderim ayar = new AyarlarSmsGonderim();
                ayar.FirmaId = FirmaId;

                ayar.RezervasyonTarihiBilgiMesaji = Convert.ToInt64(RezervasyonTarihiBilgiMesaji);
                ayar.RezervasyonTarihiBilgiGonderimSuresi = Convert.ToInt16(RezervasyonTarihiBilgiGonderimSuresi);
                ayar.RezervasyonTarihiHatirlatmaMesaji = Convert.ToInt64(RezervasyonTarihiHatirlatmaMesaji);
                ayar.RezervasyonTarihiHatirlatmaGonderimSuresi = Convert.ToInt16(RezervasyonTarihiHatirlatmaGonderimSuresi);
                ayar.OpsiyonTarihiBilgiMesaji = Convert.ToInt64(OpsiyonTarihiBilgiMesaji);
                ayar.OpsiyonTarihiBilgiGonderimSuresi = Convert.ToInt16(OpsiyonTarihiBilgiGonderimSuresi);
                ayar.OpsiyonTarihiHatirlatmaMesaji = Convert.ToInt64(OpsiyonTarihiHatirlatmaMesaji);
                ayar.OpsiyonTarihiHatirlatmaGonderimSuresi = Convert.ToInt16(OpsiyonTarihiHatirlatmaGonderimSuresi);
                ayar.MusteriCekimRandevusuBilgiMesaji = Convert.ToInt64(MusteriCekimRandevusuBilgiMesaji);
                ayar.MusteriCekimRandevusuBilgiGonderimSuresi = Convert.ToInt16(MusteriCekimRandevusuBilgiGonderimSuresi);
                ayar.MusteriCekimRandevusuHatirlatmaMesaji = Convert.ToInt64(MusteriCekimRandevusuHatirlatmaMesaji);
                ayar.MusteriCekimRandevusuHatirlatmaGonderimSuresi = Convert.ToInt16(MusteriCekimRandevusuHatirlatmaGonderimSuresi);
                ayar.PersonelRandevuBilgiMesaji = Convert.ToInt64(PersonelRandevuBilgiMesaji);
                ayar.PersonelRandevuBilgiGonderimSuresi = Convert.ToInt16(PersonelRandevuBilgiGonderimSuresi);
                ayar.PersonelRandevuHatirlatmaMesaji = Convert.ToInt64(PersonelRandevuHatirlatmaMesaji);
                ayar.PersonelRandevuHatirlatmaGonderimSuresi = Convert.ToInt16(PersonelRandevuHatirlatmaGonderimSuresi);
                ayar.MusteriOdemeBilgiMesaji = Convert.ToInt64(MusteriOdemeBilgiMesaji);
                ayar.MusteriOdemeBilgiGonderimSuresi = Convert.ToInt16(MusteriOdemeBilgiGonderimSuresi);
                ayar.MusteriOdemeHatirlatmaMesaji = Convert.ToInt64(MusteriOdemeHatirlatmaMesaji);
                ayar.MusteriOdemeHatirlatmaGonderimSuresi = Convert.ToInt16(MusteriOdemeHatirlatmaGonderimSuresi);
                ayar.FotografSecimiHatirlatmaMesaji = Convert.ToInt64(FotografSecimiHatirlatmaMesaji);
                ayar.FotografSecimiHatirlatmaGonderimSuresi = Convert.ToInt16(FotografSecimiHatirlatmaGonderimSuresi);
                ayar.FotografSecimiBilgiMesajiMusteri = Convert.ToInt64(FotografSecimiBilgiMesajiMusteri);
                ayar.FotografSecimiBilgiMesajiMusteriGonderimSuresi = Convert.ToInt16(FotografSecimiBilgiMesajiMusteriGonderimSuresi);
                ayar.FotografSecimiBilgiMesajiFirma = Convert.ToInt64(FotografSecimiBilgiMesajiFirma);
                ayar.FotografSecimiBilgiMesajiFirmaGonderimSuresi = Convert.ToInt16(FotografSecimiBilgiMesajiFirmaGonderimSuresi);
                ayar.EvlilikYildonumuTebrikMesaji = Convert.ToInt64(EvlilikYildonumuTebrikMesaji);
                ayar.EvlilikYildonumuTebrikGonderimSuresi = Convert.ToInt16(EvlilikYildonumuTebrikGonderimSuresi);
                ayar.CariyeYapilanOdemeBilgiMesaji = Convert.ToInt64(CariyeYapilanOdemeBilgiMesaji);
                ayar.CariyeYapilanOdemeBilgiGonderimSuresi = Convert.ToInt16(CariyeYapilanOdemeBilgiGonderimSuresi);
                ayar.CariAlacakHatirlatmaMesaji = Convert.ToInt64(CariAlacakHatirlatmaMesaji);
                ayar.CariAlacakHatirlatmaGonderimSuresi = Convert.ToInt16(CariAlacakHatirlatmaGonderimSuresi);
                ayar.CariTahsilatBilgiMesaji = Convert.ToInt64(CariTahsilatBilgiMesaji);
                ayar.CariTahsilatBilgiGonderimSuresi = Convert.ToInt16(CariTahsilatBilgiGonderimSuresi);
                ayar.GunlukIsOdemeBilgiMesaji = Convert.ToInt64(GunlukIsOdemeBilgiMesaji);
                ayar.GunlukIsOdemeBilgiGonderimSuresi = Convert.ToInt16(GunlukIsOdemeBilgiGonderimSuresi);
                ayar.SurecDegisiklikBilgiMesaji = Convert.ToInt64(SurecDegisiklikBilgiMesaji);
                ayar.SurecDegisiklikBilgiGonderimSuresi = Convert.ToInt16(SurecDegisiklikBilgiGonderimSuresi);
                if (SmsTurkceKarakter == "on")
                    ayar.SmsTurkceKarakter = true;
                else
                    ayar.SmsTurkceKarakter = false;
                ayar.OlusturanKullaniciId = KullaniciId;
                ayar.OlusturmaTarih = DateTime.Now;
                ayar.DegistirenKullaniciId = KullaniciId;
                ayar.DegistirmeTarih = DateTime.Now;
                ayar.Aktif = true;
                ayar.Sil = false;
                try
                {
                    dbContext.AyarlarSmsGonderims.Add(ayar);
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("SmsGonderimAyarlari");
        }


        #endregion

        #region ListeFiltre Ayarları
        public ActionResult ListelemeFiltreAyarlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Listeleme Filtre Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            AyarlarFiltre filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId);
            ViewBag.kullaniciId = KullaniciId;
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 93 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            if (filtreayar != null)
                return View(filtreayar);
            else
                return View();
        }
        [HttpPost]
        public ActionResult ListelemeFiltreAyarlariKaydet()
        {

            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string GunlukIsler = Request["GunlukIsler"];
            string RezervasyonListesi = Request["RezervasyonListesi"];
            string Randevular = Request["Randevular"];
            string RezervasyonTeklifleri = Request["RezervasyonTeklifleri"];
            string GelirlerGiderler = Request["GelirlerGiderler"];
            string AlinanGelecekOdemeler = Request["AlinanGelecekOdemeler"];
            string CariHesapTakibi = Request["CariHesapTakibi"];
            string PersonelIzinleri = Request["PersonelIzinleri"];
            string PersonelIsTakibi = Request["PersonelIsTakibi"];
            string PersonelOdemeleri = Request["PersonelOdemeleri"];
            string Kasa = Request["Kasa"];
            string MusteriHesapTakibi = Request["MusteriHesapTakibi"];
            string CariListesiPasifGizle = Request["CariListesiPasifGizle"];
            string MusteriListesiPasifGizle = Request["MusteriListesiPasifGizle"];
            string KullaniciListesiPasifGizle = Request["KullaniciListesiPasifGizle"];
            string PersonelListesiPasifGizle = Request["PersonelListesiPasifGizle"];
            AyarlarFiltre a = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId);
            if (a.Id != 0)
            {
                a.GunlukIsler = GunlukIsler;
                a.RezervasyonListesi = RezervasyonListesi;
                a.RezervasyonTeklifleri = RezervasyonTeklifleri;
                a.Randevular = Randevular;
                a.GelirlerGiderler = GelirlerGiderler;
                a.AlinanGelecekOdemeler = AlinanGelecekOdemeler;
                a.CariHesapTakibi = CariHesapTakibi;
                a.PersonelIzinTakibi = PersonelIzinleri;
                a.PersonelIsTakibi = PersonelIsTakibi;
                a.PersonelOdemeleri = PersonelOdemeleri;
                a.Kasa = Kasa;
                a.MusteriHesapTakibi = MusteriHesapTakibi;
                if (CariListesiPasifGizle == "on")
                    a.CariListesiPasifGizle = true;
                else
                    a.CariListesiPasifGizle = false;
                if (MusteriListesiPasifGizle == "on")
                    a.MusteriListesiPasifGizle = true;
                else
                    a.MusteriListesiPasifGizle = false;
                if (KullaniciListesiPasifGizle == "on")
                    a.KullaniciListesiPasifGizle = true;
                else
                    a.KullaniciListesiPasifGizle = false;
                if (PersonelListesiPasifGizle == "on")
                    a.PersonelListesiPasifGizle = true;
                else
                    a.PersonelListesiPasifGizle = false;
                a.DegistirenKullaniciId = KullaniciId;
                a.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Güncelleme Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "ListelemeFiltreAyarları kayıt güncelleme, Kayıt Id:" + a.Id.ToString();
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
            else // Eski Kayıt yoksa yeni kaydedilecek
            {
                AyarlarFiltre ayar = new AyarlarFiltre();
                ayar.GunlukIsler = GunlukIsler;
                ayar.RezervasyonListesi = RezervasyonListesi;
                ayar.RezervasyonTeklifleri = RezervasyonTeklifleri;
                a.Randevular = Randevular;
                ayar.GelirlerGiderler = GelirlerGiderler;
                ayar.AlinanGelecekOdemeler = AlinanGelecekOdemeler;
                ayar.CariHesapTakibi = CariHesapTakibi;
                ayar.PersonelIsTakibi = PersonelIsTakibi;
                ayar.PersonelIzinTakibi = PersonelIzinleri;
                ayar.PersonelOdemeleri = PersonelOdemeleri;
                ayar.Kasa = Kasa;
                ayar.MusteriHesapTakibi = MusteriHesapTakibi;
                if (CariListesiPasifGizle == "on")
                    ayar.CariListesiPasifGizle = true;
                else
                    ayar.CariListesiPasifGizle = false;
                if (MusteriListesiPasifGizle == "on")
                    ayar.MusteriListesiPasifGizle = true;
                else
                    ayar.MusteriListesiPasifGizle = false;
                if (KullaniciListesiPasifGizle == "on")
                    ayar.KullaniciListesiPasifGizle = true;
                else
                    ayar.KullaniciListesiPasifGizle = false;
                if (PersonelListesiPasifGizle == "on")
                    ayar.PersonelListesiPasifGizle = true;
                else
                    ayar.PersonelListesiPasifGizle = false;
                ayar.OlusturanKullaniciId = KullaniciId;
                ayar.OlusturmaTarih = DateTime.Now;
                ayar.DegistirenKullaniciId = KullaniciId;
                ayar.DegistirmeTarih = DateTime.Now;
                ayar.Aktif = true;
                ayar.Sil = false;
                dbContext.AyarlarFiltres.Add(ayar);
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "ListelemeFiltreAyarları yeni kayıt ekleme";
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
        }
        #endregion

        #region Müşteri Ayarları
        public ActionResult MusteriAyarlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Müşteri Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            AyarlarMusteri ayar = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 95 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            if (ayar != null)
                return View(ayar);
            else
                return View();
        }
        [HttpPost]
        public ActionResult MusteriAyarlari(string Id, string OdemeleriGor, string RezervasyonGor, string TeklifleriGor, string SozlesmeYazdir, string MesajGonder, string FotografSecim, string OdemeMakbuzYazdir)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Email Gönderim Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long AyarId = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                AyarId = Convert.ToInt64(Id);
            }
            AyarlarMusteri a = dbContext.AyarlarMusteris.FirstOrDefault(x => x.Id == AyarId && x.FirmaId == FirmaId);
            if (AyarId != 0) // Daha önce kaydedilmiş bir ayar varsa güncellenecek
            {
                if (OdemeleriGor == "on")
                    a.OdemeleriGor = true;
                else
                    a.OdemeleriGor = false;
                if (RezervasyonGor == "on")
                    a.RezervasyonGor = true;
                else
                    a.RezervasyonGor = false;
                if (TeklifleriGor == "on")
                    a.TeklifleriGor = true;
                else
                    a.TeklifleriGor = false;
                if (SozlesmeYazdir == "on")
                    a.SozlesmeYazdir = true;
                else
                    a.SozlesmeYazdir = false;
                if (OdemeMakbuzYazdir == "on")
                    a.OdemeMakbuzYazdir = true;
                else
                    a.OdemeMakbuzYazdir = false;
                if (MesajGonder == "on")
                    a.MesajGonder = true;
                else
                    a.MesajGonder = false;
                if (FotografSecim == "on")
                    a.FotografSecim = true;
                else
                    a.FotografSecim = false;

                a.DegistirenKullaniciId = KullaniciId;
                a.DegistirmeTarih = DateTime.Now;

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            else // Daha önce kaydedilmiş bir ayar yoksa yeni kaydedilecek
            {
                AyarlarMusteri ayar = new AyarlarMusteri();
                ayar.FirmaId = FirmaId;

                if (OdemeleriGor == "on")
                    ayar.OdemeleriGor = true;
                else
                    ayar.OdemeleriGor = false;
                if (RezervasyonGor == "on")
                    ayar.RezervasyonGor = true;
                else
                    ayar.RezervasyonGor = false;
                if (TeklifleriGor == "on")
                    ayar.TeklifleriGor = true;
                else
                    ayar.TeklifleriGor = false;
                if (SozlesmeYazdir == "on")
                    ayar.SozlesmeYazdir = true;
                else
                    ayar.SozlesmeYazdir = false;
                if (OdemeMakbuzYazdir == "on")
                    a.OdemeMakbuzYazdir = true;
                else
                    a.OdemeMakbuzYazdir = false;
                if (MesajGonder == "on")
                    ayar.MesajGonder = true;
                else
                    ayar.MesajGonder = false;
                if (FotografSecim == "on")
                    a.FotografSecim = true;
                else
                    a.FotografSecim = false;

                ayar.OlusturanKullaniciId = KullaniciId;
                ayar.OlusturmaTarih = DateTime.Now;
                ayar.DegistirenKullaniciId = KullaniciId;
                ayar.DegistirmeTarih = DateTime.Now;
                ayar.Aktif = true;
                ayar.Sil = false;
                try
                {
                    dbContext.AyarlarMusteris.Add(ayar);
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("MusteriAyarlari");
        }

        #endregion

        #region Sözleşme Ayarları
        public ActionResult SozlesmeCiktiAyarlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Sözleşme Çıktı Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            AyarlarSozlesmeCikti ayar = dbContext.AyarlarSozlesmeCiktis.FirstOrDefault(x => x.FirmaId == FirmaId);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 97 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            if (ayar != null)
                return View(ayar);
            else
                return View();
        }
        [HttpPost]
        public ActionResult SozlesmeCiktiAyarlari(string Id, string LogoGoster, string FirmaAdiGoster, string PaketlerGoster, string EkHizmetlerGoster, string CekimRandevulariGoster, string MusteriKoduSifreGoster,
                                                  string YapilanOdemelerGoster, string KalanOdemelerGoster, string CepTelefonuGoster, string SabitTelefonGoster, string FaxGoster)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Sözleşme Çıktı Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long AyarId = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                AyarId = Convert.ToInt64(Id);
            }
            AyarlarSozlesmeCikti a = dbContext.AyarlarSozlesmeCiktis.FirstOrDefault(x => x.Id == AyarId && x.FirmaId == FirmaId);
            if (AyarId != 0) // Daha önce kaydedilmiş bir ayar varsa güncellenecek
            {
                if (LogoGoster == "on")
                    a.LogoGoster = true;
                else
                    a.LogoGoster = false;
                if (FirmaAdiGoster == "on")
                    a.FirmaAdiGoster = true;
                else
                    a.FirmaAdiGoster = false;
                if (PaketlerGoster == "on")
                    a.PaketlerGoster = true;
                else
                    a.PaketlerGoster = false;
                if (EkHizmetlerGoster == "on")
                    a.EkHizmetlerGoster = true;
                else
                    a.EkHizmetlerGoster = false;
                if (CekimRandevulariGoster == "on")
                    a.CekimRandevulariGoster = true;
                else
                    a.CekimRandevulariGoster = false;
                if (YapilanOdemelerGoster == "on")
                    a.YapilanOdemelerGoster = true;
                else
                    a.YapilanOdemelerGoster = false;
                if (KalanOdemelerGoster == "on")
                    a.KalanOdemelerGoster = true;
                else
                    a.KalanOdemelerGoster = false;
                if (CepTelefonuGoster == "on")
                    a.CepTelefonuGoster = true;
                else
                    a.CepTelefonuGoster = false;
                if (MusteriKoduSifreGoster == "on")
                    a.MusteriKoduSifreGoster = true;
                else
                    a.MusteriKoduSifreGoster = false;
                if (SabitTelefonGoster == "on")
                    a.SabitTelefonGoster = true;
                else
                    a.SabitTelefonGoster = false;
                if (FaxGoster == "on")
                    a.FaxGoster = true;
                else
                    a.FaxGoster = false;

                a.DegistirenKullaniciId = KullaniciId;
                a.DegistirmeTarih = DateTime.Now;

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            else // Daha önce kaydedilmiş bir ayar yoksa yeni kaydedilecek
            {
                AyarlarSozlesmeCikti ayar = new AyarlarSozlesmeCikti();
                ayar.FirmaId = FirmaId;

                if (LogoGoster == "on")
                    ayar.LogoGoster = true;
                else
                    ayar.LogoGoster = false;
                if (FirmaAdiGoster == "on")
                    ayar.FirmaAdiGoster = true;
                else
                    ayar.FirmaAdiGoster = false;
                if (PaketlerGoster == "on")
                    ayar.PaketlerGoster = true;
                else
                    ayar.PaketlerGoster = false;
                if (EkHizmetlerGoster == "on")
                    ayar.EkHizmetlerGoster = true;
                else
                    ayar.EkHizmetlerGoster = false;
                if (CekimRandevulariGoster == "on")
                    ayar.CekimRandevulariGoster = true;
                else
                    ayar.CekimRandevulariGoster = false;
                if (YapilanOdemelerGoster == "on")
                    a.YapilanOdemelerGoster = true;
                else
                    a.YapilanOdemelerGoster = false;
                if (KalanOdemelerGoster == "on")
                    ayar.KalanOdemelerGoster = true;
                else
                    ayar.KalanOdemelerGoster = false;
                if (MusteriKoduSifreGoster == "on")
                    a.MusteriKoduSifreGoster = true;
                else
                    a.MusteriKoduSifreGoster = false;
                if (CepTelefonuGoster == "on")
                    a.CepTelefonuGoster = true;
                else
                    a.CepTelefonuGoster = false;
                if (SabitTelefonGoster == "on")
                    a.SabitTelefonGoster = true;
                else
                    a.SabitTelefonGoster = false;
                if (FaxGoster == "on")
                    a.FaxGoster = true;
                else
                    a.FaxGoster = false;

                ayar.OlusturanKullaniciId = KullaniciId;
                ayar.OlusturmaTarih = DateTime.Now;
                ayar.DegistirenKullaniciId = KullaniciId;
                ayar.DegistirmeTarih = DateTime.Now;
                ayar.Aktif = true;
                ayar.Sil = false;
                try
                {
                    dbContext.AyarlarSozlesmeCiktis.Add(ayar);
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("SozlesmeCiktiAyarlari");
        }

        #endregion

        #region Rezervasyon Ayarları
        public ActionResult RezervasyonAyarlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Rezervasyon Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            AyarlarRezervasyon ayar = dbContext.AyarlarRezervasyons.FirstOrDefault(x => x.FirmaId == FirmaId);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 96 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            if (ayar != null)
                return View(ayar);
            else
                return View();
        }
        [HttpPost]
        public ActionResult RezervasyonAyarlari(string Id, string PersonelIzinTakibi, string TatilGunuTakibi, string PersonelGorevliTakibi, string GunuGecenTeklifOpsiyonIptal)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Rezervasyon Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long AyarId = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                AyarId = Convert.ToInt64(Id);
            }
            AyarlarRezervasyon a = dbContext.AyarlarRezervasyons.FirstOrDefault(x => x.Id == AyarId && x.FirmaId == FirmaId);
            if (AyarId != 0) // Daha önce kaydedilmiş bir ayar varsa güncellenecek
            {

                if (PersonelIzinTakibi == "on")
                    a.PersonelIzinTakibi = true;
                else
                    a.PersonelIzinTakibi = false;
                if (TatilGunuTakibi == "on")
                    a.TatilGunuTakibi = true;
                else
                    a.TatilGunuTakibi = false;
                if (PersonelGorevliTakibi == "on")
                    a.PersonelGorevliTakibi = true;
                else
                    a.PersonelGorevliTakibi = false;
                if (GunuGecenTeklifOpsiyonIptal == "on")
                    a.GunuGecenTeklifOpsiyonIptal = true;
                else
                    a.GunuGecenTeklifOpsiyonIptal = false;

                a.DegistirenKullaniciId = KullaniciId;
                a.DegistirmeTarih = DateTime.Now;

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            else // Daha önce kaydedilmiş bir ayar yoksa yeni kaydedilecek
            {
                AyarlarRezervasyon ayar = new AyarlarRezervasyon();
                ayar.FirmaId = FirmaId;
                if (PersonelIzinTakibi == "on")
                    ayar.PersonelIzinTakibi = true;
                else
                    ayar.PersonelIzinTakibi = false;
                if (TatilGunuTakibi == "on")
                    ayar.TatilGunuTakibi = true;
                else
                    ayar.TatilGunuTakibi = false;
                if (PersonelGorevliTakibi == "on")
                    a.PersonelGorevliTakibi = true;
                else
                    a.PersonelGorevliTakibi = false;
                if (GunuGecenTeklifOpsiyonIptal == "on")
                    ayar.GunuGecenTeklifOpsiyonIptal = true;
                else
                    ayar.GunuGecenTeklifOpsiyonIptal = false;
                ayar.OlusturanKullaniciId = KullaniciId;
                ayar.OlusturmaTarih = DateTime.Now;
                ayar.DegistirenKullaniciId = KullaniciId;
                ayar.DegistirmeTarih = DateTime.Now;
                ayar.Aktif = true;
                ayar.Sil = false;
                try
                {
                    dbContext.AyarlarRezervasyons.Add(ayar);
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("RezervasyonAyarlari");
        }
        #endregion

        #region Personel Ayarları

        public ActionResult PersonelAyarlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Personel Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 98 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            AyarlarPersonel ayar = dbContext.AyarlarPersonels.FirstOrDefault(x => x.FirmaId == FirmaId);
            if (ayar != null)
                return View(ayar);
            else
                return View();

        }
        [HttpPost]
        public ActionResult PersonelAyarlari(string Id, string BaslangicIzinSuresi, string BirUcYilIzin, string UcBesYilIzin, string BesOnYilIzin, string OnOnbesYilIzin, string OnbesYirmiYilIzin, string YillikMaasArtisOrani)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Ayarlar";
            ViewBag.AltMenu = "Personel Ayarları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long AyarId = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                AyarId = Convert.ToInt64(Id);
            }
            AyarlarPersonel a = dbContext.AyarlarPersonels.FirstOrDefault(x => x.Id == AyarId && x.FirmaId == FirmaId);
            if (AyarId != 0) // Daha önce kaydedilmiş bir ayar varsa güncellenecek
            {
                a.BaslangicIzinSuresi = Convert.ToInt16(BaslangicIzinSuresi);
                a.BirUcYilIzin = Convert.ToInt16(BirUcYilIzin);
                a.UcBesYilIzin = Convert.ToInt16(UcBesYilIzin);
                a.BesOnYilIzin = Convert.ToInt16(BesOnYilIzin);
                a.OnOnbesYilIzin = Convert.ToInt16(OnOnbesYilIzin);
                a.OnbesYirmiYilIzin = Convert.ToInt16(OnbesYirmiYilIzin);
                a.YillikMaasArtisOrani = Convert.ToInt16(YillikMaasArtisOrani);
                a.DegistirenKullaniciId = KullaniciId;
                a.DegistirmeTarih = DateTime.Now;

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            else // Daha önce kaydedilmiş bir ayar yoksa yeni kaydedilecek
            {
                AyarlarPersonel ayar = new AyarlarPersonel();
                ayar.FirmaId = FirmaId;
                ayar.BaslangicIzinSuresi = Convert.ToInt16(BaslangicIzinSuresi);
                ayar.BirUcYilIzin = Convert.ToInt16(BirUcYilIzin);
                ayar.UcBesYilIzin = Convert.ToInt16(UcBesYilIzin);
                ayar.BesOnYilIzin = Convert.ToInt16(BesOnYilIzin);
                ayar.OnOnbesYilIzin = Convert.ToInt16(OnOnbesYilIzin);
                ayar.OnbesYirmiYilIzin = Convert.ToInt16(OnbesYirmiYilIzin);
                ayar.YillikMaasArtisOrani = Convert.ToInt16(YillikMaasArtisOrani);
                ayar.OlusturanKullaniciId = KullaniciId;
                ayar.OlusturmaTarih = DateTime.Now;
                ayar.DegistirenKullaniciId = KullaniciId;
                ayar.DegistirmeTarih = DateTime.Now;
                ayar.Aktif = true;
                ayar.Sil = false;
                try
                {
                    dbContext.AyarlarPersonels.Add(ayar);
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json("HATA", JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("PersonelAyarlari");
        }

        #endregion
    }
}