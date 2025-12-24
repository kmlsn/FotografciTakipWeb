using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FotografciTakipWeb.Models;
using System.Web.Security;
using System.Net.Mail;
using FotografciTakipWeb.App_Settings;
using System.IO;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
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
        public ActionResult GirisYap(Kullanici kl, string BeniHatirla, string ReturnUrl, string Ip, string Ulke, string Sehir)
        {
            string SifreliText = "";
            MD5Sifreleme sifrele = new MD5Sifreleme();
            SifreliText = sifrele.Sifrele(kl.SifreHash);
            Kullanici kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.Email == kl.Email && x.SifreHash == SifreliText); // Kullanıcı kontrolü

            if (kullanici != null)
            {
                if (kullanici.Aktif)
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
                            #region GirisLogKayit
                            GirisLog log = new GirisLog();
                            log.KullaniciId = kullanici.Id;
                            log.IpAdres = Ip;
                            log.Ulke = Ulke;
                            log.Sehir = Sehir;
                            log.BaglantıZaman = DateTime.Now;
                            dbContext.GirisLogs.Add(log);
                            dbContext.SaveChanges();
                            #endregion
                            // Giriş Yapan Kullanıcının Yetkili olduğu şubeler
                            Session["AktifSubeAdi"] = subeler.Select(x => x).First().SubeAdi;
                            Session["AktifSubeId"] = subeler.Select(x => x).First().Id;
                            Session["AdSoyad"] = kullanici.AdSoyad;
                            Session["Id"] = kullanici.Id;
                            Session["RolId"] = kullanici.RolId;
                            Session["FirmaId"] = kullanici.FirmaId;
                            Session["Email"] = kullanici.Email;
                            Session["ResimYol"] = kullanici.Resim.ResimAdres;
                            Session["Firma"] = kullanici.Firma.FirmaAdi;
                            Session["FirmaLogo"] = kullanici.Firma.Resim.ResimAdres;
                            // Giriş Yapan Kullanıcının Yetkili olduğu şubeler
                        
                            bool dashboardYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == kullanici.FirmaId && x.KullaniciId == kullanici.Id && x.SayfaId == 1).SayfaYetki;
                            Session["Yetkiler"] = dbContext.KullaniciYetkis.Where(x => x.FirmaId == kullanici.FirmaId && x.KullaniciId == kullanici.Id).ToList();
                            Session["Lisans"] = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == kullanici.FirmaId && x.Aktif == true && x.Sil == false);
                            if (frm.AcilisBit == true) // Kullanıcı girişi yaptıktan sonra, Firmanın ilk defa girişi kontrol ediliyor.  
                                return RedirectToAction("FirmaBilgileri", "FirmaSubeIslemleri"); // İlk girişse "Kurulum Sihirbazına" yönlendir.
                            if (string.IsNullOrEmpty(ReturnUrl))
                            {
                                if (dashboardYetki)
                                    return RedirectToAction("Index", "Dashboard");
                                else
                                    return RedirectToAction("Index2", "Dashboard");
                            }
                            else
                            {
                                return Redirect(ReturnUrl); // İlk giriş değilse "Otomasyon AnaSayfasına" yönlendir.
                            }

                            //else
                            //{


                            //    //return RedirectToAction("Index", "Dashboard"); // İlk giriş değilse "Otomasyon AnaSayfasına" yönlendir.
                            //}
                        }
                        else // Kullanıcı bir şubeye yetkilendirilmemişse hata verecek
                        {
                            ViewBag.hata = "yetkisizsube";  // Süresi dolan firma lisan satış sayfasına yönlendirilecek.
                            return View();
                        }
                    }
                    else
                    {
                        List<Siparisler> siparisler = dbContext.Siparislers.Where(x => x.FirmaId == frm.Id && x.Durum == 0).ToList();
                        if (siparisler.Count > 0)
                            ViewBag.hata = "siparisvar";  // Onay bekleyen bir sipariş varsa bekleme uyarısı
                        else
                            ViewBag.hata = "lisansbitmis";  // Süresi dolan firma lisan satış sayfasına yönlendirilecek.
                        ViewBag.FirmaId = frm.Id;
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

        public ActionResult SifremiUnuttum()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifremiUnuttum(string Email)
        {
            bool mailsonuc;
            SifreOlustur sifre = new SifreOlustur();
            MD5Sifreleme sifreleme = new MD5Sifreleme();
            Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Email == Email);
            if (kl == null)
            {
                ViewBag.hata = "emailyok";  // Süresi dolan firma lisan satış sayfasına yönlendirilecek.
                return View();
            }
            long FirmaId = kl.FirmaId;
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);

            kl.GeciciSifre = sifre.sifreolustur(6);
            kl.DegistirenKullaniciId = kl.Id;
            kl.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                #region Şifre Oluşturma Linkini Mail Gönder
                AyarlarMailHesap ayar = dbContext.AyarlarMailHesaps.FirstOrDefault(x => x.FirmaId == 1); // destek@fotografcitakip.com
                string token = kl.Id.ToString() + "-" + kl.GeciciSifre + "-" + FirmaId.ToString();
                //string token = kl.GeciciSifre;
                string domainname = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                string SifreOlusturmaLinki = domainname + "/Otomasyon/Giris/SifreOlustur/" + token;
                string konu = "Fotoğrafçı Takip - Şifre Sıfırlama";
                string body = string.Empty;
                string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
                //string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");
                using (StreamReader reader = new StreamReader(Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/Sifremi_Unuttum.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{FirmaLogo}", "src=cid:firmalogo");
                body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
                body = body.Replace("{FirmaAdi}", frm.FirmaAdi);
                body = body.Replace("{KullaniciAdi}", kl.AdSoyad + "(" + kl.Email + ")");
                body = body.Replace("{SifreOlusturmaLinki}", SifreOlusturmaLinki);
                //body = body.Replace("{FirmaWeb}", frm.WebSitesi);
                //body = body.Replace("{FirmaFacebook}", frm.Facebook);
                //body = body.Replace("{FirmaInstagram}", frm.Instagram);
                //body = body.Replace("{FirmaTwitter}", frm.Twitter);
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                LinkedResource firmalogo = new LinkedResource(FirmaLogo);
                firmalogo.ContentId = "firmalogo";
                //LinkedResource fotografcitakiplogo = new LinkedResource(FotografciTakipLogo);
                //fotografcitakiplogo.ContentId = "fotografcitakiplogo";
                htmlView.LinkedResources.Add(firmalogo);
                //htmlView.LinkedResources.Add(fotografcitakiplogo);
                mailsonuc = MailGonder.Gonder(FirmaId, 1, 1, kl.Email, konu, body, htmlView);
                if (mailsonuc == false)
                {
                    return Json(new { Sonuc = false, Mesaj = "Kullanıcıya şifre oluşturma maili gönderilemedi!<br/>Kullanıcı mail adresini veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz.", JsonRequestBehavior.AllowGet });
                }
                ViewBag.hata = "mailgonderildi";
                return View();
                //return RedirectToAction("GirisYap");
                #endregion
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Kullanıcı Şifre Sıfırlama, Kullanıcı Id: " + kl.Id;
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Sıfırlama e-maili gönderilemedi, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }
        }
        public ActionResult SifreOlustur(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("SifreOlusturHata");
            }
            else
            {
                string[] token = id.Split('-');
                long KullaniciId = Convert.ToInt64(token[0].ToString());
                string GeciciSifre = token[1].ToString();
                long FirmaId = Convert.ToInt64(token[2].ToString());
                Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KullaniciId && x.FirmaId == FirmaId && x.GeciciSifre == GeciciSifre && x.Aktif == true);
                if (kl != null)
                {
                    ViewBag.FirmaAdi = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId).FirmaAdi;
                    ViewBag.AdSoyad = kl.AdSoyad;
                    ViewBag.Email = kl.Email;
                    ViewBag.KullaniciId = KullaniciId;
                    return View();
                }
                else
                {
                    return RedirectToAction("SifreOlusturHata");
                }
            }
        }
        public ActionResult SifreOlusturHata()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifreOlustur()
        {
            string Sifre = Request["Sifre"];
            long KullaniciId = Convert.ToInt64(Request["KullaniciId"]);

            string SifreliText = "";

            MD5Sifreleme sifrele = new MD5Sifreleme();
            SifreliText = sifrele.Sifrele(Sifre);

            Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KullaniciId && x.Aktif == true && x.Sil == false);
            if (kl == null)
                return RedirectToAction("SifreOlusturHata");
            kl.GeciciSifre = null;
            kl.SifreHash = SifreliText;
            kl.DegistirmeTarih = DateTime.Now;
            kl.DegistirenKullaniciId = KullaniciId;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Yeni şifreniz başarıyla kaydedildi. Giriş sayfasına yönlendiriliyorsunuz.", JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                return RedirectToAction("SifreOlusturHata");
            }
        }
    }
}