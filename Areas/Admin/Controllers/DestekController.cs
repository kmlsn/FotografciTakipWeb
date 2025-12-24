using FotografciTakipWeb.App_Settings;
using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.Admin.Controllers
{
    public class DestekController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();

        public ActionResult DestekTalepleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Destek Talepleri";

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 107 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Models.DestekTalepleri> talepler = dbContext.DestekTalepleris.Where(x => x.Aktif == true && x.Sil == false).ToList();
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 107 && x.Aktif == true && x.Sil == false);
            return View(talepler);
        }

        #region İptal Kod
        //[HttpPost]
        //public ActionResult DestekEkle()
        //{
        //    if (Session.Count == 0)
        //        return RedirectToAction("GirisYap", "Giris");
        //    long FirmaId = Convert.ToInt64(Session["FirmaId"]);
        //    long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
        //    long KullaniciId = Convert.ToInt64(Session["Id"]);
        //    Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
        //    string TalepBasligi = Request["TalepBasligi"];
        //    string TalepMesaj = Request["TalepMesaj"];
        //    TalepMesaj = TalepMesaj.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
        //    TalepMesaj = TalepMesaj.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
        //    string TalepTuru = Request["TalepTuru"];
        //    string resimadi = "";
        //    ResimIsim isim = new ResimIsim();
        //    DestekTalepleri Talep = new DestekTalepleri();
        //    Models.DestekTalepleriDetay TalepDetay = new DestekTalepleriDetay();
        //    Kullanici kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KullaniciId);

        //    string resimisim = TalepBasligi + "_" + FirmaId.ToString();

        //    #region Destek Resmi ile ilgili işlemler
        //    if (Request.Files.Count > 0) // Kullanıcı Resmi seçilmişse
        //    {
        //        HttpPostedFileBase kullaniciresim = Request.Files[0]; //Uploaded file
        //        string fileName = kullaniciresim.FileName;
        //        resimadi = isim.resimisimlendir(kullaniciresim, resimisim);
        //        if (System.IO.File.Exists(Server.MapPath("/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/" + resimadi))) // bu resimden var mı?
        //        {
        //            resimadi = isim.resimisimlendir(kullaniciresim, resimisim); // resime yeni bir isim veriyorum.
        //        }
        //        Image img = Image.FromStream(kullaniciresim.InputStream);
        //        img.Save(Server.MapPath("/Areas/Otomasyon/Dosyalar/DestekResim/" + resimadi));
        //        TalepDetay.ResimYol = "/Areas/Otomasyon/Dosyalar/DestekResim/" + resimadi;                                                                                      //kullaniciresim.SaveAs(Server.MapPath("~/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/") + fileName); //File will be saved in application root
        //    }
        //    else
        //        TalepDetay.ResimYol = "";
        //    #endregion

        //    Talep.FirmaId = FirmaId;
        //    Talep.KullaniciId = KullaniciId;
        //    Talep.TalepTuru = TalepTuru;
        //    Talep.TalepBaslik = TalepBasligi;

        //    Talep.Durum = "6"; //1: Cevap Bekleniyor 2: Okundu 3: Cevaplandı 4: Değerlendiriliyor 5: Kapandı 6: Talep İletildi 7: İptal
        //    Talep.KilitBit = false;
        //    Talep.OlusturanKullaniciId = KullaniciId;
        //    Talep.OlusturmaTarih = DateTime.Now;
        //    Talep.DegistirenKullaniciId = KullaniciId;
        //    Talep.DegistirmeTarih = DateTime.Now;
        //    Talep.Aktif = true;
        //    Talep.Sil = false;
        //    dbContext.DestekTalepleris.Add(Talep);
        //    try
        //    {
        //        dbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        HataLoglari hata = new HataLoglari();
        //        hata.FirmaId = FirmaId;
        //        hata.SubeId = SubeId;
        //        hata.Islem = "Destek Talebi Oluşturma";
        //        hata.HataMesajı = e.Message;
        //        hata.OlusturanKullaniciId = 1;
        //        hata.OlusturmaTarih = DateTime.Now;
        //        hata.DegistirenKullaniciId = 1;
        //        hata.DegistirmeTarih = DateTime.Now;
        //        hata.Aktif = true;
        //        hata.Sil = false;
        //        dbContext.HataLoglaris.Add(hata);
        //        dbContext.SaveChanges();
        //        return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
        //    }

        //    TalepDetay.FirmaId = FirmaId;
        //    TalepDetay.TalepId = Talep.Id;
        //    TalepDetay.Mesaj = TalepMesaj;
        //    TalepDetay.MusteriCevap = true;
        //    TalepDetay.FirmaCevap = false;
        //    TalepDetay.CevaplandiBit = true;
        //    TalepDetay.OkunduBit = false;
        //    TalepDetay.KilitBit = false;
        //    TalepDetay.CevaplayanKullaniciId = KullaniciId;
        //    TalepDetay.OlusturanKullaniciId = KullaniciId;
        //    TalepDetay.OlusturmaTarih = DateTime.Now;
        //    TalepDetay.DegistirenKullaniciId = KullaniciId;
        //    TalepDetay.DegistirmeTarih = DateTime.Now;
        //    TalepDetay.Aktif = true;
        //    TalepDetay.Sil = false;
        //    dbContext.DestekTalepleriDetays.Add(TalepDetay);
        //    try
        //    {
        //        dbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        HataLoglari hata = new HataLoglari();
        //        hata.FirmaId = FirmaId;
        //        hata.SubeId = SubeId;
        //        hata.Islem = "Destek talebi detay kayıtları kaydetme";
        //        hata.HataMesajı = e.Message;
        //        hata.OlusturanKullaniciId = 1;
        //        hata.OlusturmaTarih = DateTime.Now;
        //        hata.DegistirenKullaniciId = 1;
        //        hata.DegistirmeTarih = DateTime.Now;
        //        hata.Aktif = true;
        //        hata.Sil = false;
        //        dbContext.HataLoglaris.Add(hata);
        //        dbContext.SaveChanges();
        //        return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
        //    }

        //    // Talebiniz alındı maili
        //    #region Destek Talebi Mail Gönder Musteri
        //    bool mailsonuc;
        //    string konu = "Fotoğrafçı Takip - Destek talebiniz alındı";
        //    string body = string.Empty;
        //    //string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
        //    string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");
        //    string IcerikResim = Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/image/destek-talebi-2.png");
        //    using (StreamReader reader = new StreamReader(Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/Destek_Tabebi_Alindi.html")))
        //    {
        //        body = reader.ReadToEnd();
        //    }
        //    body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
        //    body = body.Replace("{IcerikResim}", "src=cid:icerikresim");
        //    body = body.Replace("{MailBaslik}", "Destek Talebiniz Alındı");
        //    body = body.Replace("{MailMetin}", "&quot;" + TalepBasligi + "&quot; başlıklı destek talebiniz tarafımıza iletildi. <br /> En kısa sürede talebinize cevap vereceğiz.");
        //    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

        //    LinkedResource fotografcitakiplogo = new LinkedResource(FotografciTakipLogo);
        //    fotografcitakiplogo.ContentId = "fotografcitakiplogo";

        //    LinkedResource icerikresim = new LinkedResource(IcerikResim);
        //    icerikresim.ContentId = "icerikresim";

        //    htmlView.LinkedResources.Add(fotografcitakiplogo);
        //    htmlView.LinkedResources.Add(icerikresim);
        //    mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, kullanici.Email, konu, body, htmlView);
        //    if (mailsonuc == false)
        //    {
        //        return Json(new { Sonuc = false, Mesaj = "Destek talebi maili gönderilemedi!<br/>Firma mail adresinizi veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz.", JsonRequestBehavior.AllowGet });
        //    }
        //    #endregion
        //    return Json(new { Sonuc = true, Mesaj = "Destek talebiniz alındı." }, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        public ActionResult DestekDetay(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            ViewBag.UstMenu = "Destek";
            ViewBag.AltMenu = "Destek Talepleri Detayı";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 108 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 108 && x.Aktif == true && x.Sil == false);
            List<DestekTalepleriDetay> talepdetay = dbContext.DestekTalepleriDetays.Where(x => x.TalepId == id && x.Aktif == true && x.Sil == false).ToList();
            DestekTalepleri talep = dbContext.DestekTalepleris.FirstOrDefault(x => x.Id == id && x.Aktif == true && x.Sil == false);
            ViewBag.Talep = talep;
            foreach (var item in talepdetay.Where(x => x.MusteriCevap == true)) // Cevaplanmış ancak okunmamış olan mesajlar okundu yapılıyor.
            {
                item.CevaplandiBit = false;
                item.OkunduBit = true;
                item.DegistirenKullaniciId = KullaniciId;
                item.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();
            }
           
            return View(talepdetay);
        }

        [HttpPost]
        public ActionResult TalepCevapla()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId==0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.OlusturanKullaniciId == 1).Id;
            }
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            long DestekId = Convert.ToInt64(Request["TalepId"]);
            string TalepCevap = Request["TalepCevap"];
            TalepCevap = TalepCevap.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            TalepCevap = TalepCevap.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            string Durum =Request["Durum"];
            DestekTalepleriDetay TalepDetay = new DestekTalepleriDetay();
            DestekTalepleri talep = dbContext.DestekTalepleris.FirstOrDefault(x => x.Id == DestekId);
            talep.Durum = Durum;
            talep.CevapTarihi = DateTime.Now;
            talep.DegistirenKullaniciId = KullaniciId;
            talep.DegistirmeTarih = DateTime.Now;
          
            TalepDetay.FirmaId = talep.FirmaId;
            TalepDetay.TalepId = DestekId;
            TalepDetay.Mesaj = TalepCevap;
            TalepDetay.ResimYol = "";
            TalepDetay.MusteriCevap = false;
            TalepDetay.FirmaCevap = true;
            TalepDetay.CevaplandiBit = true;
            TalepDetay.OkunduBit = false;
            TalepDetay.KilitBit = false;
            TalepDetay.CevaplayanKullaniciId = KullaniciId;
            TalepDetay.OlusturanKullaniciId = KullaniciId;
            TalepDetay.OlusturmaTarih = DateTime.Now;
            TalepDetay.DegistirenKullaniciId = KullaniciId;
            TalepDetay.DegistirmeTarih = DateTime.Now;
            TalepDetay.Aktif = true;
            TalepDetay.Sil = false;
            dbContext.DestekTalepleriDetays.Add(TalepDetay);
            string TalepBasligi = talep.TalepBaslik;
            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Destek talebi detay kayıtları kaydetme";
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
            #region Destek Talebi Mail Gönder Musteri
            bool mailsonuc;
            string konu = "Fotoğrafçı Takip - Destek talebiniz yanıtlandı";
            string body = string.Empty;
            //string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
            string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");
            string IcerikResim = Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/image/destek-talebi-2.png");
            using (StreamReader reader = new StreamReader(Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/Destek_Tabebi_Alindi.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
            body = body.Replace("{IcerikResim}", "src=cid:icerikresim");
            body = body.Replace("{MailBaslik}", "Destek Talebiniz Alındı");
            body = body.Replace("{MailMetin}", "&quot;" + TalepBasligi + "&quot; başlıklı destek talebiniz yanıtlanmıştır. <br /> Destek Talepleri sayfasından erişebilirsiniz.");
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

            LinkedResource fotografcitakiplogo = new LinkedResource(FotografciTakipLogo);
            fotografcitakiplogo.ContentId = "fotografcitakiplogo";

            LinkedResource icerikresim = new LinkedResource(IcerikResim);
            icerikresim.ContentId = "icerikresim";

            htmlView.LinkedResources.Add(fotografcitakiplogo);
            htmlView.LinkedResources.Add(icerikresim);
            mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, frm.Email, konu, body, htmlView);
            if (mailsonuc == false)
            {
                return Json(new { Sonuc = false, Mesaj = "Destek talebi maili gönderilemedi!<br/>Firma mail adresinizi veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz.", JsonRequestBehavior.AllowGet });
            }
            #endregion
            return Json(new { Sonuc = true, JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public ActionResult DurumGuncelle()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.OlusturanKullaniciId == 1).Id;
            }
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            Kullanici kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KullaniciId);

            string Durum = Request["Durum"];
            long DestekId = Convert.ToInt64(Request["TalepId"]);
            DestekTalepleri talep = dbContext.DestekTalepleris.FirstOrDefault(x => x.Id == DestekId);
            talep.Durum = Durum;
            talep.DegistirenKullaniciId = KullaniciId;
            talep.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                #region Destek Talebi Mail Gönder Musteri
                bool mailsonuc;
                string konu = "Fotoğrafçı Takip - Destek talebiniz yanıtlandı";
                string body = string.Empty;
                //string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
                string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");
                string IcerikResim = Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/image/destek-talebi-2.png");
                using (StreamReader reader = new StreamReader(Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/Destek_Tabebi_Alindi.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
                body = body.Replace("{IcerikResim}", "src=cid:icerikresim");
                body = body.Replace("{MailBaslik}", "Destek Talebiniz Alındı");
                body = body.Replace("{MailMetin}", "&quot;" + talep.TalepBaslik + "&quot; başlıklı destek talebinizin durumu güncellenmiştir. <br /> Destek Talepleri sayfasından erişebilirsiniz.");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                LinkedResource fotografcitakiplogo = new LinkedResource(FotografciTakipLogo);
                fotografcitakiplogo.ContentId = "fotografcitakiplogo";

                LinkedResource icerikresim = new LinkedResource(IcerikResim);
                icerikresim.ContentId = "icerikresim";

                htmlView.LinkedResources.Add(fotografcitakiplogo);
                htmlView.LinkedResources.Add(icerikresim);
                mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, kullanici.Email, konu, body, htmlView);
                if (mailsonuc == false)
                {
                    return Json(new { Sonuc = false, Mesaj = "Destek talebi maili gönderilemedi!<br/>Firma mail adresinizi veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz.", JsonRequestBehavior.AllowGet });
                }
                #endregion
                return Json(new { Sonuc = true, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = 1;
                hata.SubeId = 1;
                hata.Islem = "ADMİN - Destek talebi durum güncelleme";
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
}