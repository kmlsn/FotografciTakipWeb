using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FotografciTakipWeb.Models;
using System.Web.Security;
using System.Net.Mail;
using FotografciTakipWeb.App_Settings;
using System.IO;

namespace FotografciTakipWeb.Areas.Admin.Controllers
{
    public class GirisController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();

        // GET: Otomasyon/GirisYap
        public ActionResult GirisYap(string ReturnUrl = "")
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult GirisYap(Kullanici kl, string BeniHatirla, string ReturnUrl)
        {
            string SifreliText = "";
            MD5Sifreleme sifrele = new MD5Sifreleme();
            SifreliText = sifrele.Sifrele(kl.SifreHash);

            Kullanici kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.Email == kl.Email && x.SifreHash == SifreliText); // Kullanıcı kontrolü

            if (kullanici != null)
            {
                if (kullanici.Aktif)
                {
                    if (kullanici.RolId==1)
                    {
                        Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == kullanici.FirmaId); // Giriş Yapan kullanıcının firması
                        Lisanslar lisans = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == frm.Id); // firma lisan süresi kontrolü
                        if (DateTime.Now <= lisans.LisansBitisTarihi) // lisans bitiş tarihi bu günden büyükse
                        {
                            // lisan süresi dolmamış ise kullanıcı giriş yapabilir.
                            List<Sube> subeler = new List<Sube>();
                            List<SubeToKullanici> SubeKullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == kullanici.Id).ToList();
                            if (SubeKullanici.Count > 0) // Kullanıcı bir şubeye yetkilendirilmişse
                            {
                                if (BeniHatirla == "on")
                                    FormsAuthentication.RedirectFromLoginPage(kullanici.Email, true);
                                else
                                    FormsAuthentication.RedirectFromLoginPage(kullanici.Email, false);
                                foreach (var suku in SubeKullanici) // Giriş Yapan Kullanıcının yetkili olduğu AKTİF ŞEBELER çekiliyor.
                                {
                                    Sube sb = dbContext.Subes.FirstOrDefault(x => x.Id == suku.SubeId && x.Aktif == true && x.Sil == false);
                                    subeler.Add(sb);
                                }
                                // Giriş Yapan Kullanıcının Yetkili olduğu şubeler
                                //Session["AktifSubeAdi"] = subeler.Select(x => x).First().SubeAdi;
                                //Session["AktifSubeId"] = subeler.Select(x => x).First().Id;
                                Session["AdSoyad"] = kullanici.AdSoyad;
                                Session["Id"] = kullanici.Id;
                                Session["RolId"] = kullanici.RolId;
                                Session["FirmaId"] = kullanici.FirmaId;
                                Session["Email"] = kullanici.Email;
                                Session["ResimYol"] = kullanici.Resim.ResimAdres;
                                Session["Firma"] = kullanici.Firma.FirmaAdi;
                                Session["FirmaLogo"] = kullanici.Firma.Resim.ResimAdres;
                                // Giriş Yapan Kullanıcının Yetkili olduğu şubeler
                                //Session["AktifSubeAdi"] = subeler.Select(x => x).First().SubeAdi;
                                Session["AktifSubeId"] = 1;
                                Session["Yetkiler"] = dbContext.KullaniciYetkis.Where(x => x.FirmaId == kullanici.FirmaId && x.KullaniciId == kullanici.Id).ToList();
                                Session["Lisans"] = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == kullanici.FirmaId && x.Aktif == true && x.Sil == false);

                                if (string.IsNullOrEmpty(ReturnUrl))
                                {
                                    return RedirectToAction("Index", "Dashboard");
                                }
                                else
                                {
                                    return Redirect(ReturnUrl); // İlk giriş değilse "Otomasyon AnaSayfasına" yönlendir.
                                }
                            }
                            else // Kullanıcı bir şubeye yetkilendirilmemişse hata verecek
                            {
                                ViewBag.hata = "yetkisizsube";  // Süresi dolan firma lisan satış sayfasına yönlendirilecek.
                                return View();
                            }
                        }
                        else
                        {
                            ViewBag.hata = "lisansbitmis";  // Süresi dolan firma lisan satış sayfasına yönlendirilecek.
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.hata = "yetkisizgiris";
                        return View();
                    }
                }
                else
                {
                    ViewBag.hata = "kullanicipasif";
                    return View();
                }
            }
            else
            {
                ViewBag.hata = "kullanicisifrehatali";
                return View();
            }
        }

        public ActionResult CikisYap()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("GirisYap");
        }
    }
}