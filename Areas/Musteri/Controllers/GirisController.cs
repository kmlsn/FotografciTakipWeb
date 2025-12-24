using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FotografciTakipWeb.Models;
using System.Web.Security;

namespace FotografciTakipWeb.Areas.Musteri.Controllers
{
    public class GirisController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Musteri/Giris
        public ActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GirisYap(Models.Musteri musteri, string BeniHatirla)
        {
            Models.Musteri mm = dbContext.Musteris.FirstOrDefault(x => x.MusteriKodu == musteri.MusteriKodu && x.Sifre == musteri.Sifre); // Kullanıcı kontrolü
            if (mm == null)
            {
                ViewBag.hata = "Musteri Kodu veya Sifre Hatali";
                return View();
            }
            if (mm.MusteriPanelGirisYetki == false)
            {
                ViewBag.hata = "Panele Giriş Yetkiniz Yok";
                return View();
            }
            if (mm.Aktif == false)
            {
                ViewBag.hata = "Müşteri Pasif";
                return View();
            }

            if (BeniHatirla == "on")
                FormsAuthentication.RedirectFromLoginPage(mm.MusteriKodu.ToString(), true);
            else
                FormsAuthentication.RedirectFromLoginPage(mm.MusteriKodu.ToString(), false);

            Session["AdSoyad"] = mm.AdiSoyadi;
            Session["MusteriId"] = mm.Id;
            Session["MusteriKodu"] = mm.MusteriKodu;
            Session["Email"] = mm.Email;
            Session["Firma"] = mm.Firma.FirmaAdi;
            Session["FirmaId"] = mm.FirmaId;
            Session["FirmaLogo"] = mm.Firma.Resim.ResimAdres;
            return RedirectToAction("Index", "Dashboard"); // İlk giriş değilse "Otomasyon AnaSayfasına" yönlendir.
        }

        public ActionResult CikisYap()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("GirisYap");
        }
    }
}