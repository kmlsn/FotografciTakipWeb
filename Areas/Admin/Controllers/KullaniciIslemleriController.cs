using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FotografciTakipWeb.App_Settings;
using System.IO;
using System.Net.Mail;

namespace FotografciTakipWeb.Areas.Admin.Controllers
{
    public class KullaniciIslemleriController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();

        public ActionResult KullaniciListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Kullanıcı-Yetki İşlemleri";
            ViewBag.AltMenu = "Kullanıcı Listesi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 105 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<PersonelGorevleri> gorevler = dbContext.PersonelGorevleris.Where(x => x.Aktif == true && x.Sil == false).ToList();
            List<Sube> subeler = dbContext.Subes.Where(x => x.Aktif == true && x.Sil == false).ToList();
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 105 && x.Aktif == true && x.Sil == false);
            ViewBag.YetkilendirSayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 106 && x.Aktif == true && x.Sil == false).SayfaYetki;
            ViewBag.Subeler = subeler;
            return View(gorevler);
        }
        public ActionResult KullanicilarListesi()
        {
            string abc = "pXjJwO";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Kullanici> kullanicilar = dbContext.Kullanicis.Where(x => x.Aktif == true && x.Sil == false).ToList();
            MD5Sifreleme sifreleme = new MD5Sifreleme();
            abc = sifreleme.SifreCoz("W0HLxurje/iIDOCqjZ+4qQ==");
            abc = sifreleme.SifreCoz("1LYlXM");
            //abc = sifreleme.SifreCoz("pXjJwO");
            //abc = sifreleme.SifreCoz("SxNwtw");
            //abc = sifreleme.SifreCoz("8voET6");


            var klist = kullanicilar.Select(m => new
            {
                Id = m.Id,
                RolAdi = m.Rol.RolAdi,
                FirmaAdi = m.Firma.FirmaAdi,
                AdSoyad = m.AdSoyad,
                GorevId = m.GorevId,
                Gorev = m.PersonelGorevleri.Gorev,
                ResimId = m.ResimId,
                ResimAdres = m.Resim.ResimAdres,
                Email = m.Email,
                Sifre = sifreleme.SifreCoz(m.SifreHash),
                CepTel = m.CepTel,
                KilitBit = m.KilitBit,
                Aciklama = m.Notlar,
                Subeler = m.YetkiliSubeler,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            });
            return Json(new { data = klist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SifreSifirla(long id)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.OlusturanKullaniciId == 1).Id;
            }
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            bool mailsonuc;
            SifreOlustur sifre = new SifreOlustur();
            Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Id == id && x.Aktif == true && x.Sil == false);
            kl.SifreHash = sifre.sifreolustur(6);
            kl.DegistirenKullaniciId = KullaniciId;
            kl.DegistirmeTarih = DateTime.Now;
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
            try
            {
                #region Şifre Oluşturma Linkini Mail Gönder
                AyarlarMailHesap ayar = dbContext.AyarlarMailHesaps.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                string token = kl.Id.ToString() + "-" + kl.SifreHash + "-" + FirmaId.ToString();
                string domainname = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                string SifreOlusturmaLinki = domainname + "/Otomasyon/KullaniciIslemleri/SifreOlustur/" + token;
                string konu = "Fotoğrafçı Takip - Kullanıcı Şifre Oluşturma";
                string body = string.Empty;
                string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
                string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");
                using (StreamReader reader = new StreamReader(Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/Kullanici_Sifre_Sifirlama.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{FirmaLogo}", "src=cid:firmalogo");
                body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
                body = body.Replace("{FirmaAdi}", frm.FirmaAdi);
                body = body.Replace("{KullaniciAdi}", kl.AdSoyad);
                body = body.Replace("{SifreOlusturmaLinki}", SifreOlusturmaLinki);
                body = body.Replace("{FirmaWeb}", frm.WebSitesi);
                body = body.Replace("{FirmaFacebook}", frm.Facebook);
                body = body.Replace("{FirmaInstagram}", frm.Instagram);
                body = body.Replace("{FirmaTwitter}", frm.Twitter);
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                LinkedResource firmalogo = new LinkedResource(FirmaLogo);
                firmalogo.ContentId = "firmalogo";
                LinkedResource fotografcitakiplogo = new LinkedResource(FotografciTakipLogo);
                fotografcitakiplogo.ContentId = "fotografcitakiplogo";
                htmlView.LinkedResources.Add(firmalogo);
                htmlView.LinkedResources.Add(fotografcitakiplogo);
                mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, kl.Email, konu, body, htmlView);
                if (mailsonuc == false)
                {
                    return Json(new { Sonuc = false, Mesaj = "Kullanıcıya şifre oluşturma maili gönderilemedi!<br/>Kullanıcı mail adresini veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz.", JsonRequestBehavior.AllowGet });
                }
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                #endregion
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
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
    }
}