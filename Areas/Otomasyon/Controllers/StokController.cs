using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FotografciTakipWeb.Models;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class StokController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        public ActionResult StokKartlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Stok";
            ViewBag.AltMenu = "Stok Kartları";
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt32(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 38 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }

        public ActionResult StokGirisi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Stok";
            ViewBag.AltMenu = "Stok Girişi";
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt32(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 39 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }

        public ActionResult StokCikisi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Stok";
            ViewBag.AltMenu = "Stok Çıkışı";
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt32(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 40 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }
        public ActionResult Iade()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Stok";
            ViewBag.AltMenu = "Iade";
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt32(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 42 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }
        public ActionResult StokHareket()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Stok";
            ViewBag.AltMenu = "Stok Hareketleri";
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt32(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 41 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }

        public ActionResult DepoListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Stok";
            ViewBag.AltMenu = "Depo İşlemleri";
            ViewBag.AltMenu2 = "Depo Listesi";

            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt32(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 46 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }

        public ActionResult DepolarArasiTransfer()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Stok";
            ViewBag.AltMenu = "Depo İşlemleri";
            ViewBag.AltMenu2 = "Depolar Arası Transfer";
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt32(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 47 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }

        public ActionResult DepoStokDurumu()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Stok";
            ViewBag.AltMenu = "Depo İşlemleri";
            ViewBag.AltMenu2 = "Depo Stok Durumu";
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt32(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 48 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }

        public ActionResult DepoyaYapilmisGirisler()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Stok";
            ViewBag.AltMenu = "Depoya Yapılmış Girişler";
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt32(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 49 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }

        public ActionResult DepodanYapilmisCikisler()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Stok";
            ViewBag.AltMenu = "Depodan Yapılmış Çıkışlar";
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt32(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 50 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }
    }
}