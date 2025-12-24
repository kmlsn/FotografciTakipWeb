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

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class KullaniciIslemleriController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/KullaniciYetkiIslemleri
        public ActionResult KullanicilarListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Kullanici> kullanicilar = dbContext.Kullanicis.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
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
        public ActionResult KullaniciListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Kullanıcı-Yetki İşlemleri";
            ViewBag.AltMenu = "Kullanıcı Listesi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 105 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<PersonelGorevleri> gorevler = dbContext.PersonelGorevleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<Sube> subeler = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 105 && x.Aktif == true && x.Sil == false);
            ViewBag.YetkilendirSayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 106 && x.Aktif == true && x.Sil == false).SayfaYetki;
            ViewBag.Subeler = subeler;
            int kullaniciLimit = Convert.ToInt32(dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).KullaniciLimit);
            int kullaniciSayisi = dbContext.Kullanicis.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList().Count;
            ViewBag.KLimit = kullaniciSayisi < kullaniciLimit ? true : false;
            ViewBag.Ayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).KullaniciListesiPasifGizle;

            return View(gorevler);
        }
        [HttpPost]
        public ActionResult KullaniciEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            }
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            string[] SubeListesi = Request["SubeListesi"].Split(','); // SubetoKullanici tablosuna tek tek eklemek için
            string SubeListesi2 = Request["SubeListesi"]; // Kulanıcı tablosundaki YetkiliSubeler alanı için
            string AdSoyad = Request["AdSoyad"];
            string Email = Request["Email"];
            string cep = Request["CepTel"];
            long GorevId = Convert.ToInt64(Request["GorevId"]);
            string Aciklama = Request["Aciklama"];
            TelefonDüzelt td = new TelefonDüzelt();
            if (!string.IsNullOrEmpty(cep)) { cep = td.düzelt(cep); }
            long resimId;
            string resimadi = "";
            bool mailsonuc;
            ResimIsim isim = new ResimIsim();
            Resim rsm = new Resim();
            SifreOlustur sifre = new SifreOlustur();
            Kullanici kullanici = new Kullanici();
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            MD5Sifreleme sifreleme = new MD5Sifreleme();
            #region Kullanıcı Resmi ile ilgili işlemler
            if (Request.Files.Count > 0) // Kullanıcı Resmi seçilmişse
            {
                HttpPostedFileBase kullaniciresim = Request.Files[0]; //Uploaded file

                string fileName = kullaniciresim.FileName;
                resimadi = isim.resimisimlendir(kullaniciresim, AdSoyad);

                if (System.IO.File.Exists(Server.MapPath("/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/" + resimadi))) // bu resimden var mı?
                {
                    resimadi = isim.resimisimlendir(kullaniciresim, AdSoyad); // resime yeni bir isim veriyorum.
                }

                Image img = Image.FromStream(kullaniciresim.InputStream);
                img.Save(Server.MapPath("/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/" + resimadi)); // yeni logo sunucuya kaydediliyor.
                                                                                                            ////To save file, use SaveAs method
                                                                                                            //kullaniciresim.SaveAs(Server.MapPath("~/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/") + fileName); //File will be saved in application root

                rsm.ResimAdres = "/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/" + resimadi; // yeni logo veritabanına kaydediliyor.
                rsm.FirmaId = FirmaId;
                rsm.OlusturanKullaniciId = KullaniciId;
                rsm.OlusturmaTarih = DateTime.Now;
                rsm.DegistirenKullaniciId = KullaniciId;
                rsm.DegistirmeTarih = DateTime.Now;
                rsm.Aktif = true;
                rsm.Sil = false;
                dbContext.Resims.Add(rsm);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json(new { Sonuc = false, Bilgi = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
                }

                resimId = rsm.Id;
            }
            else // Kullanıcı Resmi seçilmemişse Default resim atanıyor.
            {
                resimId = 2;
            }
            #endregion
            #region Kullanıcı Kaydetme İşlemleri        
            kullanici.FirmaId = FirmaId;
            kullanici.AdSoyad = AdSoyad;
            kullanici.Email = Email;
            kullanici.CepTel = cep;
            kullanici.GorevId = GorevId;
            kullanici.Notlar = Aciklama;
            kullanici.RolId = 4;
            kullanici.GeciciSifre = "7nyGUP";
            kullanici.SifreHash = sifreleme.Sifrele(sifre.sifreolustur(6));
            kullanici.ResimId = resimId;
            kullanici.YetkiliSubeler = SubeListesi2;
            kullanici.OlusturanKullaniciId = KullaniciId;
            kullanici.OlusturmaTarih = DateTime.Now;
            kullanici.DegistirenKullaniciId = KullaniciId;
            kullanici.DegistirmeTarih = DateTime.Now;
            kullanici.Aktif = true;
            kullanici.Sil = false;
            kullanici.KilitBit = false;
            try
            {
                dbContext.Kullanicis.Add(kullanici);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Kullanıcı Ekleme";
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
            #region Trigger'e Alındı
            //// Kullanıcı Eklendikten sonra Eklenen Kullanıcıya Yetkiler veriliyor.
            //#region Kullanıcı Yetkilendirme İşlemleri
            //KullaniciYetki ky = new KullaniciYetki();
            //List<ModulSayfa> modulsayfalar = dbContext.ModulSayfas.Select(x => x).ToList();
            //foreach (var sayfa in modulsayfalar)
            //{
            //    ky.FirmaId = FirmaId;
            //    ky.KullaniciId = kullanici.Id;
            //    ky.SayfaId = sayfa.Id;
            //    ky.SayfaYetki = Convert.ToBoolean(sayfa.SayfaYetkiAktif);
            //    ky.KayitDetayi = Convert.ToBoolean(sayfa.KayitDetayiAktif);
            //    ky.KayitDuzenle = Convert.ToBoolean(sayfa.KayitDuzenleAktif);
            //    ky.KayitEkle = Convert.ToBoolean(sayfa.KayitEkleAktif);
            //    ky.KayitSil = Convert.ToBoolean(sayfa.KayitSilAktif);
            //    ky.Yazdir = Convert.ToBoolean(sayfa.Yazdirma);
            //    ky.SmsGonder = Convert.ToBoolean(sayfa.SMSGonder);
            //    ky.OlusturanKullaniciId = KullaniciId;
            //    ky.OlusturmaTarih = DateTime.Now;
            //    ky.DegistirenKullaniciId = KullaniciId;
            //    ky.DegistirmeTarih = DateTime.Now;
            //    ky.Aktif = true;
            //    ky.Sil = false;
            //    dbContext.KullaniciYetkis.Add(ky);
            //    try
            //    {
            //        dbContext.SaveChanges();
            //    }
            //    catch (Exception e)
            //    {
            //        HataLoglari hata = new HataLoglari();
            //        hata.FirmaId = FirmaId;
            //        hata.SubeId = SubeId;
            //        hata.Islem = "Kullanıcıyı Sayfaya Yetkilendirme, Sayfa Id: " + sayfa.Id.ToString() + ", Kullanıcı Id: " + kullanici.Id.ToString();
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
            //}
            //#endregion
            #endregion
            // Kullanıcı Eklendikten sonra Kullanıcı Şubelere yetkilendiriliyor.
            #region Kullanıcı Şube Yetkilendirme
            if (SubeListesi[0] != "")
            {
                SubeToKullanici subekullanici = new SubeToKullanici();
                foreach (var sube in SubeListesi)
                {
                    subekullanici.SubeId = Convert.ToInt64(sube);
                    subekullanici.KullaniciId = kullanici.Id;
                    dbContext.SubeToKullanicis.Add(subekullanici);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Kullanıcıyı Şubeye Yetkilendirme, Şube Id: " + sube + ", Kullanıcı Id: " + kullanici.Id.ToString();
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
            // Kullanıcı şifre oluşturma linkini Mail Gonder
            #region Şifre Oluşturma Linkini Mail Gönder
            string token = kullanici.Id.ToString() + "-" + kullanici.GeciciSifre + "-" + FirmaId.ToString();
            string domainname = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            string SifreOlusturmaLinki = domainname + "/Otomasyon/KullaniciIslemleri/SifreOlustur/" + token;
            string konu = "Fotoğrafçı Takip - Kullanıcı Şifre Oluşturma";
            string body = string.Empty;
            string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
            //string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");

            using (StreamReader reader = new StreamReader(Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/Kullanici_Sifre_Olustur.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{FirmaLogo}", "src=cid:firmalogo");
            //body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
            body = body.Replace("{FirmaAdi}", frm.FirmaAdi);
            body = body.Replace("{KullaniciAdi}", kullanici.AdSoyad);
            body = body.Replace("{SifreOlusturmaLinki}", SifreOlusturmaLinki);
            body = body.Replace("{FirmaWeb}", frm.WebSitesi);
            body = body.Replace("{FirmaFacebook}", frm.Facebook);
            body = body.Replace("{FirmaInstagram}", frm.Instagram);
            body = body.Replace("{FirmaTwitter}", frm.Twitter);

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
            LinkedResource firmalogo = new LinkedResource(FirmaLogo);
            firmalogo.ContentId = "firmalogo";
            //LinkedResource fotografcitakiplogo = new LinkedResource(FotografciTakipLogo);
            //fotografcitakiplogo.ContentId = "fotografcitakiplogo";
            htmlView.LinkedResources.Add(firmalogo);
            //htmlView.LinkedResources.Add(fotografcitakiplogo);
            mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, kullanici.Email, konu, body, htmlView);
            if (mailsonuc == false)
            {
                return Json(new { Sonuc = false, Mesaj = "Kullanıcıya şifre oluşturma maili gönderilemedi!<br/>Kullanıcı mail adresini veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz.", JsonRequestBehavior.AllowGet });
            }
            #endregion
            return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult KullaniciGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            }

            long KullaniciId = Convert.ToInt64(Session["Id"]);

            string AdSoyad = Request["AdSoyad"];
            string Email = Request["Email"];
            string cep = Request["CepTel"];
            long GorevId = Convert.ToInt64(Request["GorevId"]);
            string Aciklama = Request["Aciklama"];
            long resimId = Convert.ToInt64(Request["ResimId"]);
            string resimadi = "";
            string[] SubeListesi = Request["SubeListesi"].Split(','); // SubetoKullanici tablosuna tek tek eklemek için
            string SubeListesi2 = Request["SubeListesi"]; // Kulanıcı tablosundaki YetkiliSubeler alanı için
            TelefonDüzelt td = new TelefonDüzelt();
            if (!string.IsNullOrEmpty(cep)) { cep = td.düzelt(cep); }
            ResimIsim isim = new ResimIsim();
            Resim rsm = new Resim();
            SifreOlustur sifre = new SifreOlustur();
            Kullanici kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (kullanici == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            #region Kullanıcı Resmi ile ilgili işlemler
            if (Request.Files.Count > 0) // Kullanıcı Resmi seçilmişse
            {
                if (resimId != 2) // Eski resim varsayılan kullanıcı resmi ise resim silinmeyecek, değiştirilecek
                {
                    #region Eski resim sil
                    Resim eskiresim = dbContext.Resims.FirstOrDefault(x => x.Id == resimId);
                    //eskiresim.Sil = true;
                    //eskiresim.Aktif = false;
                    //eskiresim.DegistirenKullaniciId = KullaniciId;
                    //eskiresim.DegistirmeTarih = DateTime.Now;
                    dbContext.Resims.Remove(eskiresim);
                    dbContext.SaveChanges();
                    if (System.IO.File.Exists(Server.MapPath(eskiresim.ResimAdres))) // bu resimden var mı?
                    {
                        System.IO.File.Delete(Server.MapPath(eskiresim.ResimAdres)); // var olan resim siliniyor.
                    }
                    #endregion
                }

                #region Yeni resim kaydet
                HttpPostedFileBase kullaniciresim = Request.Files[0]; //Uploaded file
                string fileName = kullaniciresim.FileName;
                resimadi = isim.resimisimlendir(kullaniciresim, AdSoyad);

                if (System.IO.File.Exists(Server.MapPath("/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/" + resimadi))) // bu resimden var mı?
                {
                    resimadi = isim.resimisimlendir(kullaniciresim, AdSoyad); // resime yeni bir isim veriyorum.
                }

                Image img = Image.FromStream(kullaniciresim.InputStream);
                img.Save(Server.MapPath("/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/" + resimadi)); // yeni logo sunucuya kaydediliyor.
                                                                                                            ////To save file, use SaveAs method
                                                                                                            //kullaniciresim.SaveAs(Server.MapPath("~/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/") + fileName); //File will be saved in application root

                rsm.ResimAdres = "/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/" + resimadi; // yeni logo veritabanına kaydediliyor.
                rsm.FirmaId = FirmaId;
                rsm.OlusturanKullaniciId = KullaniciId;
                rsm.OlusturmaTarih = DateTime.Now;
                rsm.DegistirenKullaniciId = KullaniciId;
                rsm.DegistirmeTarih = DateTime.Now;
                rsm.Aktif = true;
                rsm.Sil = false;
                dbContext.Resims.Add(rsm);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json(new { Sonuc = false, Bilgi = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
                }
                resimId = rsm.Id;
                #endregion
            }
            #endregion

            kullanici.AdSoyad = AdSoyad;
            kullanici.Email = Email;
            kullanici.CepTel = cep;
            kullanici.GorevId = GorevId;
            kullanici.Notlar = Aciklama;
            kullanici.ResimId = resimId;
            kullanici.YetkiliSubeler = SubeListesi2;
            kullanici.DegistirenKullaniciId = KullaniciId;
            kullanici.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                #region Kullanıcı Şube Yetkilendirme
                // var olan yetkiler kalacak yeni eklene yetki tabloya eklenecek, KullanıcıId ve ŞubeId ile sorgu yapılacak

                SubeToKullanici subekullanici = new SubeToKullanici();
                List<SubeToKullanici> yetkilisubeler = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == id).ToList();
                // Kullanıcının herhangi bir şubeye yetkisi yoksa sayfadan gelen tüm subelere yetkilendiriliyor.
                long subeid;
                if (yetkilisubeler == null)
                {
                    foreach (var sube in SubeListesi)
                    {
                        subekullanici.SubeId = Convert.ToInt64(sube);
                        subekullanici.KullaniciId = kullanici.Id;
                        dbContext.SubeToKullanicis.Add(subekullanici);
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Kullanıcıyı Şubeye Yetkilendirme, Şube Id: " + sube + ", Kullanıcı Id: " + kullanici.Id.ToString();
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
                else // Kullanıcının herhangi bir şubeye yetkisi varsa 
                {
                    foreach (var sube in SubeListesi)
                    {
                        subeid = Convert.ToInt64(sube);
                        SubeToKullanici sb = dbContext.SubeToKullanicis.FirstOrDefault(x => x.KullaniciId == id && x.SubeId == subeid);
                        // yetkisinin olamadığı şubeye yetkilendiriliyor.
                        if (sb == null)
                        {
                            subekullanici.SubeId = Convert.ToInt64(sube);
                            subekullanici.KullaniciId = kullanici.Id;
                            dbContext.SubeToKullanicis.Add(subekullanici);
                            try
                            {
                                dbContext.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                HataLoglari hata = new HataLoglari();
                                hata.FirmaId = FirmaId;
                                hata.SubeId = SubeId;
                                hata.Islem = "Kullanıcıyı Şubeye Yetkilendirme, Şube Id: " + sube + ", Kullanıcı Id: " + kullanici.Id.ToString();
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
                #endregion
                return Json(new { Sonuc = true, Mesaj = "Güncelleme Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Kullanıcı Güncelle Kullanıcı Id: " + kullanici.Id.ToString();
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
        public ActionResult KullaniciSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            }
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long resimId;

            Kullanici kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (kullanici == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            resimId = Convert.ToInt64(kullanici.ResimId);
            #region Kullanıcı Resmi ile ilgili işlemler
            if (resimId != 2) // Eski resim varsayılan kullanıcı resmi ise resim silinmeyecek, değiştirilecek
            {
                #region Eski resim sil
                Resim eskiresim = dbContext.Resims.FirstOrDefault(x => x.Id == resimId);
                dbContext.Resims.Remove(eskiresim);
                dbContext.SaveChanges();
                if (System.IO.File.Exists(Server.MapPath(eskiresim.ResimAdres))) // bu resimden var mı?
                {
                    System.IO.File.Delete(Server.MapPath(eskiresim.ResimAdres)); // var olan logo siliniyor.
                }
                #endregion
            }
            #endregion

            kullanici.Sil = true;
            kullanici.Aktif = false;
            kullanici.ResimId = 2;  // Resim dosyası silindiği için varsayılan resim id si atanıyor.
            kullanici.DegistirenKullaniciId = KullaniciId;
            kullanici.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                #region Trigger'e Alındı
                //// Kullanıcı silindikten sonra kullanıcının yetkileri de siliniyor.
                //List<KullaniciYetki> ky = dbContext.KullaniciYetkis.Where(x => x.FirmaId == FirmaId && x.KullaniciId == KullaniciId).ToList();
                //foreach (var yetkisil in ky)
                //{
                //    yetkisil.Sil = true;
                //    yetkisil.Aktif = false;
                //    yetkisil.DegistirenKullaniciId = KullaniciId;
                //    yetkisil.DegistirmeTarih = DateTime.Now;
                //    try
                //    {
                //        dbContext.SaveChanges();
                //    }
                //    catch (Exception e)
                //    {
                //        HataLoglari hata = new HataLoglari();
                //        hata.FirmaId = FirmaId;
                //        hata.SubeId = SubeId;
                //        hata.Islem = "Kullanıcıyı Yetkileri Sil, Sayfa Id: " + yetkisil.Id.ToString() + ", Kullanıcı Id: " + kullanici.Id.ToString();
                //        hata.HataMesajı = e.Message;
                //        hata.OlusturanKullaniciId = 1;
                //        hata.OlusturmaTarih = DateTime.Now;
                //        hata.DegistirenKullaniciId = 1;
                //        hata.DegistirmeTarih = DateTime.Now;
                //        hata.Aktif = true;
                //        hata.Sil = false;
                //        dbContext.HataLoglaris.Add(hata);
                //        dbContext.SaveChanges();
                //        return Json(new { Sonuc = false, Mesaj = "Silme esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                //    }
                //}
                #endregion
                List<SubeToKullanici> subeKullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == id).ToList();
                if (subeKullanici.Count>0)
                {
                    foreach (var sube in subeKullanici)
                    {
                        dbContext.SubeToKullanicis.Remove(sube);
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Kullanıcının Yetkili Şubelerini Sil, Şube Id: " + sube.Id.ToString() + ", Kullanıcı Id: " + kullanici.Id.ToString();
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
                return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Kullanıcıyı Sil, Kullanıcı Id: " + kullanici.Id.ToString();
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
        public string KullaniciVarMi()
        {
            string Email = Request["Email"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Email == Email && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

            // email kontrolü firma bağılsız olarak yapılıyor.
            Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Email == Email && x.Aktif == true && x.Sil == false);

            if (kl != null)
                return kl.Id.ToString();
            else
                return "Yok";

        }
        [HttpPost]
        public string KullaniciDurumDegistir(long? id)
        {
            Kullanici kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.Id == id && x.Sil == false);
            if (kullanici.Aktif == true)
                kullanici.Aktif = false;
            else
                kullanici.Aktif = true;
            try
            {
                dbContext.SaveChanges();
                return "degisti";
            }
            catch (Exception)
            {
                return "hata";
            }
        }
        public ActionResult SifreOlustur(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("SifreOlusturHata", "KullaniciIslemleri");
            }
            else
            {
                string[] token = id.Split('-');
                long KullaniciId = Convert.ToInt64(token[0].ToString());
                string GeciciSifre = token[1].ToString();
                long FirmaId = Convert.ToInt64(token[2].ToString());
                Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KullaniciId && x.FirmaId == FirmaId && x.SifreHash == GeciciSifre && x.Aktif == true);
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
                    return RedirectToAction("SifreOlusturHata", "KullaniciIslemleri");
                }
            }
        }
        [HttpPost]
        public string SifreOlustur()
        {
            string Sifre = Request["Sifre"];
            long KullaniciId = Convert.ToInt64(Request["KullaniciId"]);
            MD5Sifreleme sifrele = new MD5Sifreleme();
            Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KullaniciId);
            kl.SifreHash = sifrele.Sifrele(Sifre);
            kl.DegistirenKullaniciId = KullaniciId;
            kl.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                return "basarili";
            }
            catch (Exception)
            {
                return "basarisiz";
            }
        }
        public ActionResult SifreOlusturHata()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifreSifirla(long id)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            }

            long KullaniciId = Convert.ToInt64(Session["Id"]);
            bool mailsonuc;
            SifreOlustur sifre = new SifreOlustur();
            MD5Sifreleme sifreleme = new MD5Sifreleme();
            Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            kl.SifreHash =sifreleme.Sifrele(sifre.sifreolustur(6));
            kl.DegistirenKullaniciId = KullaniciId;
            kl.DegistirmeTarih = DateTime.Now;
            dbContext.SaveChanges();
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
                //string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");
                using (StreamReader reader = new StreamReader(Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/Kullanici_Sifre_Sifirlama.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{FirmaLogo}", "src=cid:firmalogo");
                //body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
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
                //LinkedResource fotografcitakiplogo = new LinkedResource(FotografciTakipLogo);
                //fotografcitakiplogo.ContentId = "fotografcitakiplogo";
                htmlView.LinkedResources.Add(firmalogo);
                //htmlView.LinkedResources.Add(fotografcitakiplogo);
                mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, kl.Email, konu, body, htmlView);
                if (mailsonuc == false)
                {
                    return Json(new { Sonuc = false, Mesaj = "Kullanıcıya şifre oluşturma maili gönderilemedi!<br/>Kullanıcı mail adresini veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz.", JsonRequestBehavior.AllowGet });
                }
                return Json(new { Sonuc = true, Mesaj = "Şifre Sıfırlama Maili Gönderildi" }, JsonRequestBehavior.AllowGet);
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

        public ActionResult KullaniciYetkileri(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            ViewBag.UstMenu = "Kullanıcı-Yetki İşlemleri";
            ViewBag.AltMenu = "Kullanıcı Yetkileri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            }

            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 106 && x.Aktif == true && x.Sil == false);
            if (SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            else if (!SayfaYetki.SayfaYetki)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<ModulSayfa> sayfalar = dbContext.ModulSayfas.Where(x => x.Aktif == true && x.Sil == false).ToList();
            List<KullaniciYetki> ky = dbContext.KullaniciYetkis.Where(x => x.FirmaId == FirmaId && x.KullaniciId == id && x.Aktif == true && x.Sil == false).ToList(); // seçilen kullanıcının yetkileri
            if (ky == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            //return Json(new { Sonuc = false, Bilgi = "Yetkilendirilmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            ViewBag.Kullanicilar = dbContext.Kullanicis.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList(); // Kullanıcı listesi için tüm kullanıcılar
            ViewBag.Kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false); // Seçilen Kullanıcı

            ViewBag.Kullaniciyetkileri = ky;
            return View(sayfalar);
        }
        [HttpPost]
        public ActionResult KullaniciYetkileriKaydet() //Güncelle
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            }

            long KullaniciId = Convert.ToInt64(Session["Id"]);

            long SecilenKullaniciId = Convert.ToInt64(Request["Id"]);
            string[] SecilenModuluYetkilendir = Request["SecilenModuluYetkilendir"].Split(',');
            string[] SecilenSayfaGor = Request["SecilenSayfaGor"].Split(',');
            string[] SecilenKayitDetay = Request["SecilenKayitDetay"].Split(',');
            string[] SecilenKayitEkle = Request["SecilenKayitEkle"].Split(',');
            string[] SecilenKayitDuzenle = Request["SecilenKayitDuzenle"].Split(',');
            string[] SecilenKayitSil = Request["SecilenKayitSil"].Split(',');
            string[] SecilenYazdir = Request["SecilenYazdir"].Split(',');
            string[] SecilenSmsGonder = Request["SecilenSmsGonder"].Split(',');
            string[] SecilmeyenModuluYetkilendir = Request["SecilmeyenModuluYetkilendir"].Split(',');
            string[] SecilmeyenSayfaGor = Request["SecilmeyenSayfaGor"].Split(',');
            string[] SecilmeyenKayitDetay = Request["SecilmeyenKayitDetay"].Split(',');
            string[] SecilmeyenKayitEkle = Request["SecilmeyenKayitEkle"].Split(',');
            string[] SecilmeyenKayitDuzenle = Request["SecilmeyenKayitDuzenle"].Split(',');
            string[] SecilmeyenKayitSil = Request["SecilmeyenKayitSil"].Split(',');
            string[] SecilmeyenYazdir = Request["SecilmeyenYazdir"].Split(',');
            string[] SecilmeyenSmsGonder = Request["SecilmeyenSmsGonder"].Split(',');
            #region Secilen Modül Görme Yetkisi Verme
            if (SecilenModuluYetkilendir[0] != "")
            {
                foreach (string secmodul in SecilenModuluYetkilendir)
                {
                    long modul = Convert.ToInt64(secmodul);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == modul);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == modul);
                    ky.SayfaYetki = true;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilen Sayfa Görme Yetkisi Verme
            if (SecilenSayfaGor[0] != "")
            {
                foreach (string secsayf in SecilenSayfaGor)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);

                    ky.SayfaYetki = true;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilen Kayıt Detay Görme Yetkisi Verme
            if (SecilenKayitDetay[0] != "")
            {
                foreach (string secsayf in SecilenKayitDetay)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.KayitDetayi = true;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilen Kayıt Ekle Yetkisi Verme
            if (SecilenKayitEkle[0] != "")
            {
                foreach (string secsayf in SecilenKayitEkle)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.KayitEkle = true;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilen Kayıt Düzenle Yetkisi Verme
            if (SecilenKayitDuzenle[0] != "")
            {
                foreach (string secsayf in SecilenKayitDuzenle)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);

                    ky.KayitDuzenle = true;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilen Kayıt Sil Yetkisi Verme
            if (SecilenKayitSil[0] != "")
            {
                foreach (string secsayf in SecilenKayitSil)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.KayitSil = true;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilen Yazdirma Yetkisi Verme
            if (SecilenYazdir[0] != "")
            {
                foreach (string secsayf in SecilenYazdir)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.Yazdir = true;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilen Sms Gonder Yetkisi Verme
            if (SecilenSmsGonder[0] != "")
            {
                foreach (string secsayf in SecilenSmsGonder)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.SmsGonder = true;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion

            #region Secilmeyen Modül Görme Yetkisi Kaldırma
            if (SecilmeyenModuluYetkilendir[0] != "")
            {
                foreach (string secsayf in SecilmeyenModuluYetkilendir)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.SayfaYetki = false;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilmeyen Sayfa Görme Yetkisi Kaldırma
            if (SecilmeyenSayfaGor[0] != "")
            {
                foreach (string secsayf in SecilmeyenSayfaGor)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.SayfaYetki = false;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilmeyen Kayıt Detay Görme Yetkisi Kaldırma
            if (SecilmeyenKayitDetay[0] != "")
            {
                foreach (string secsayf in SecilmeyenKayitDetay)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.KayitDetayi = false;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilmeyen Kayıt Ekle Yetkisi Kaldırma
            if (SecilmeyenKayitEkle[0] != "")
            {
                foreach (string secsayf in SecilmeyenKayitEkle)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.KayitEkle = false;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilmeyen Kayıt Duzenle Yetkisi Kaldırma
            if (SecilmeyenKayitDuzenle[0] != "")
            {
                foreach (string secsayf in SecilmeyenKayitDuzenle)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.KayitDuzenle = false;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilmeyen Kayıt Sil Yetkisi Kaldırma
            if (SecilmeyenKayitSil[0] != "")
            {
                foreach (string secsayf in SecilmeyenKayitSil)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.KayitSil = false;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion

            #region Secilmeyen Yazdır Yetkisi Kaldırma
            if (SecilmeyenYazdir[0] != "")
            {
                foreach (string secsayf in SecilmeyenYazdir)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.Yazdir = false;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            #region Secilmeyen Sms Gönder Yetkisi Kaldırma
            if (SecilmeyenSmsGonder[0] != "")
            {
                foreach (string secsayf in SecilmeyenSmsGonder)
                {
                    long syf = Convert.ToInt64(secsayf);
                    KullaniciYetki ky;
                    if (KullaniciId == 1)
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    else
                        ky = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == SecilenKullaniciId && x.SayfaId == syf);
                    ky.SmsGonder = false;
                    ky.DegistirenKullaniciId = KullaniciId;
                    ky.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, mesaj = "Kayıt esnasında bit hata oluştu. Hata: " + e.Message, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            #endregion
            return Json(new { Sonuc = true, mesaj = "Kayıt Başarılı", JsonRequestBehavior.AllowGet });
        }

        public ActionResult KullaniciRolleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Kullanıcı-Yetki İşlemleri";
            ViewBag.AltMenu = "Kullanıcı Rolleri";
            return View();
        }
        public ActionResult KullaniciRolYetkileri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Kullanıcı-Yetki İşlemleri";
            ViewBag.AltMenu = "Kullanıcı-Rol Yetkileri";
            return View();
        }

        public ActionResult Profil()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            //ViewBag.UstMenu = "Dashboard";
            ViewBag.AltMenu = "Profil Bilgileri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            Kullanici kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == KullaniciId && x.Sil == false && x.Aktif == true);
            return View(kullanici);
        }
        [HttpPost]
        public ActionResult SifreDegistir()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            MD5Sifreleme sifrele = new MD5Sifreleme();
            //ViewBag.UstMenu = "Dashboard";
            long KId = Convert.ToInt64(Request["KullaniciId"]);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string EskiSifre = Request["EskiSifre"];
            string YeniSifre = Request["YeniSifre"];
            Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == KId && x.Sil == false && x.Aktif == true);

            if (kl.SifreHash != sifrele.Sifrele(EskiSifre))
            {
                return Json(new { Sonuc = false, Mesaj = "Eski şifre yanlış", JsonRequestBehavior.AllowGet });
            }

            kl.SifreHash = sifrele.Sifrele(YeniSifre);
            kl.DegistirenKullaniciId = KullaniciId;
            kl.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Şifre başarıyla değiştirildi", JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
            }

        }

        [HttpPost]
        public ActionResult ResimDegistir()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long KId = Convert.ToInt64(Request["KullaniciId"]);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long resimId;
           
            string resimadi = "";
            ResimIsim isim = new ResimIsim();
            Resim rsm = new Resim();
            Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            string AdSoyad = kl.AdSoyad;
            #region Kullanıcı Resmi ile ilgili işlemler
            if (Request.Files.Count > 0) // Kullanıcı Resmi seçilmişse
            {
                HttpPostedFileBase kullaniciresim = Request.Files[0]; //Uploaded file

                long? esliresimid = kl.ResimId;
                if (esliresimid != 2) // Eski resim varsayılan kullanıcı resmi ise resim silinmeyecek, değiştirilecek
                {
                    #region Eski resim sil
                    Resim eskiresim = dbContext.Resims.FirstOrDefault(x => x.Id == esliresimid);
                    //eskiresim.Sil = true;
                    //eskiresim.Aktif = false;
                    //eskiresim.DegistirenKullaniciId = KullaniciId;
                    //eskiresim.DegistirmeTarih = DateTime.Now;
                    dbContext.Resims.Remove(eskiresim);
                    dbContext.SaveChanges();
                    if (System.IO.File.Exists(Server.MapPath(eskiresim.ResimAdres))) // bu resimden var mı?
                    {
                        System.IO.File.Delete(Server.MapPath(eskiresim.ResimAdres)); // var olan resim siliniyor.
                    }
                    #endregion
                }

                string fileName = kullaniciresim.FileName;
                resimadi = isim.resimisimlendir(kullaniciresim, AdSoyad);
                if (System.IO.File.Exists(Server.MapPath("/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/" + resimadi))) // bu resimden var mı?
                {
                    resimadi = isim.resimisimlendir(kullaniciresim, AdSoyad); // resime yeni bir isim veriyorum.
                }
                
                Image img = Image.FromStream(kullaniciresim.InputStream);
                img.Save(Server.MapPath("/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/" + resimadi)); // yeni resim sunucuya kaydediliyor.
                                                                                                            ////To save file, use SaveAs method
                                                                                                            //kullaniciresim.SaveAs(Server.MapPath("~/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/") + fileName); //File will be saved in application root

                rsm.ResimAdres = "/Areas/Otomasyon/Dosyalar/KullaniciProfilResimleri/" + resimadi; // yeni logo veritabanına kaydediliyor.
                rsm.FirmaId = FirmaId;
                rsm.OlusturanKullaniciId = KullaniciId;
                rsm.OlusturmaTarih = DateTime.Now;
                rsm.DegistirenKullaniciId = KullaniciId;
                rsm.DegistirmeTarih = DateTime.Now;
                rsm.Aktif = true;
                rsm.Sil = false;
                dbContext.Resims.Add(rsm);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Json(new { Sonuc = false, Bilgi = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
                }
                Session["ResimYol"] = rsm.ResimAdres;
                resimId = rsm.Id;
            }
            else // Kullanıcı Resmi seçilmemişse Default resim atanıyor.
            {
                resimId = 2;
            }
            #endregion
            kl.ResimId = resimId;
            kl.DegistirenKullaniciId = KullaniciId;
            kl.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Resim başarıyla değiştirildi", JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                return Json(new { Sonuc = false, Bilgi = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
            }
        }
    }
}