using FotografciTakipWeb.App_Settings;
using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class FirmaSubeIslemleriController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/FirmaSubeIslemleri
        public ActionResult FirmaBilgileri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Firma-Şube İşlemleri";
            ViewBag.AltMenu = "Firma Bilgileri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 101 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            List<Il> iller = dbContext.Ils.Select(x => x).ToList();
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            ViewBag.frm = frm;
            List<Ilce> ilceler = dbContext.Ilces.Where(x => x.IlId == frm.IlId).OrderBy(x => x.Ilce1).ToList();
            ViewBag.ilceler = ilceler;
            #region Firma Açılış ve Duyuru Mesajı göstergesi
            ViewBag.AcilisBit = frm.AcilisBit;
            ViewBag.DuyuruBit = frm.DuyuruBit;
            #endregion
            return View(iller);
        }
        [HttpPost]
        public ActionResult FirmaGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == id);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.OlusturanKullaniciId == 1).Id;
            }
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string FirmaAdi = Request["FirmaAdi"]; // Kilitli, Değişiklik Yapılamaz.
            string Yetkili = Request["Yetkili"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string VergiDairesi = Request["VergiDairesi"];
            string VergiNo = Request["VergiNo"];
            string Email = Request["Email"];
            int IlId = Convert.ToInt32(Request["IlId"]);
            int IlceId = Convert.ToInt32(Request["IlceId"]);
            string WebSitesi = Request["WebSitesi"];
            string Facebook = Request["Facebook"];
            string Twitter = Request["Twitter"];
            string Instagram = Request["Instagram"];
            string firmahakkinda = Request["FirmaHakkinda"];
            string adres = Request["Adres"];
            if (!string.IsNullOrEmpty(firmahakkinda))
            {
                firmahakkinda = firmahakkinda.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
                firmahakkinda = firmahakkinda.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor. 
            }
            else
                firmahakkinda = "";

            if (!string.IsNullOrEmpty(adres))
            {
                adres = adres.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
                adres = adres.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            }
            else
                adres = "";
            string cep = Request["CepTel"];
            string sabit = Request["SabitTel"];
            string fax = Request["Fax"];
            TelefonDüzelt td = new TelefonDüzelt();
            if (cep != null) { cep = td.düzelt(cep); }
            if (sabit != null) { sabit = td.düzelt(sabit); }
            if (fax != null) { fax = td.düzelt(fax); }

            long resimId;
            string resimadi = "";
            Resim rsm = new Resim();
            ResimIsim isim = new ResimIsim();
            #region Firma Logosu ile ilgili işlemler
            if (Request.Files.Count > 0) // Logo seçilmişse
            {
                HttpPostedFileBase Logo = Request.Files[0]; //Uploaded file
                if (frm.ResimId == 3) // firmanın mevcut logosu default logo ise seçilen logo veritabanına ve sunucuya kaydediliyor.
                {
                    resimadi = isim.resimisimlendir(Logo, FirmaAdi); // logoya isim veriliyor
                    if (System.IO.File.Exists(Server.MapPath("~/Areas/Otomasyon/Dosyalar/FirmaLogolar/" + resimadi))) // bu resimden var mı?
                    {
                        resimadi = isim.resimisimlendir(Logo, FirmaAdi); // logoya yeni bir isim veriyorum.
                    }
                    Image img = Image.FromStream(Logo.InputStream);
                    img.Save(Server.MapPath("~/Areas/Otomasyon/Dosyalar/FirmaLogolar/" + resimadi)); // yeni logo sunucuya kaydediliyor.

                    rsm.ResimAdres = "/Areas/Otomasyon/Dosyalar/FirmaLogolar/" + resimadi; // yeni logo veritabanına kaydediliyor.
                    rsm.FirmaId = FirmaId;
                    rsm.OlusturanKullaniciId = KullaniciId;
                    rsm.OlusturmaTarih = DateTime.Now;
                    rsm.DegistirenKullaniciId = KullaniciId;
                    rsm.DegistirmeTarih = DateTime.Now;
                    rsm.Aktif = true;
                    rsm.Sil = false;
                    try
                    {
                        dbContext.Resims.Add(rsm);
                        dbContext.SaveChanges();
                        resimId = rsm.Id;
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Varsayılan Firma Logosu Yeni Firma Logosu ile değiştirlirken yeni logo resilmer tablosuna kaydedilemedi";
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
                else // firmanın mevcut logosu default logo değilse mevcuttaki logo sunucudan siliniyor, yeni logo sunucuya kaydediliyor. 
                     //  Veri tabanındaki mevcut kaydın resim yolu güncelleniyor.
                {
                    // Eski Logoyu sunucudan sil
                    string eskilogo = frm.Resim.ResimAdres;
                    if (System.IO.File.Exists(Server.MapPath(eskilogo))) // bu resimden var mı?
                    {
                        System.IO.File.Delete(Server.MapPath(eskilogo)); // var olan logo siliniyor.
                    }
                    // Yeni logoyu sunucuya kaydet
                    resimadi = isim.resimisimlendir(Logo, FirmaAdi); // logoya isim veriliyor
                    if (System.IO.File.Exists(Server.MapPath("~/Areas/Otomasyon/Dosyalar/FirmaLogolar/" + resimadi))) // bu resimden var mı?
                    {
                        resimadi = isim.resimisimlendir(Logo, FirmaAdi); // logoya yeni bir isim veriyorum.
                    }
                    Image img = Image.FromStream(Logo.InputStream);
                    // yeni logoyu veritabanında güncelle
                    img.Save(Server.MapPath("~/Areas/Otomasyon/Dosyalar/FirmaLogolar/" + resimadi)); // yeni logo sunucuya kaydediliyor.
                    Resim logoguncelle = dbContext.Resims.FirstOrDefault(x => x.Id == frm.ResimId);
                    logoguncelle.ResimAdres = "/Areas/Otomasyon/Dosyalar/FirmaLogolar/" + resimadi;
                    logoguncelle.DegistirenKullaniciId = KullaniciId;
                    logoguncelle.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                        resimId = logoguncelle.Id;
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Mevcut Firma Logosu Yeni Firma Logosu ile değiştirlirken yeni logo resilmer tablosunda güncellenemedi.";
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
            else // Firma Logosu seçilmemişse mevcuttaki Logo ID'si Firma Tablosuna elkeniyor.
            {
                resimId = Convert.ToInt64(frm.ResimId);
            }
            #endregion

            // Firma ile birlikte ANAŞUBE nin bilgileri de güncellenecek.
            Sube sb = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true); // Oturum açan firma ile aynı isme sahip ANAŞUBE bilgileri de firma bilgileri ile birlikte güncelleniyor.
                                                                                                       // Kilit bite bakılarak şubenin ana şube olup olmadığı anlaşılıyor.
            frm.Yetkili = Yetkili;
            sb.Yetkili = Yetkili;
            frm.VergiDairesi = VergiDairesi;
            frm.VergiNo = VergiNo;
            frm.TCKimlikNo = TCKimlikNo;
            sb.TCKimlikNo = TCKimlikNo;
            //frm.Email = Email; // Değişikliğe İzin verilmediği için eski mail adresi aynen kalacak
            //sb.Email = Email;  // Değişikliğe İzin verilmediği için eski mail adresi aynen kalacak
            frm.CepTel = cep;
            sb.CepTel = cep;
            frm.SabitTel = sabit;
            sb.SabitTel = sabit;
            frm.Fax = fax;
            sb.Fax = fax;
            frm.IlId = IlId;
            sb.IlId = IlId;
            frm.IlceId = IlceId;
            sb.IlceId = IlceId;
            frm.Adres = adres;
            sb.Adres = adres;
            frm.WebSitesi = WebSitesi;
            sb.WebSitesi = WebSitesi;
            frm.Facebook = Facebook;
            sb.Facebook = Facebook;
            frm.Instagram = Instagram;
            sb.Instagram = Instagram;
            frm.Twitter = Twitter;
            sb.Twitter = Twitter;
            frm.FirmaHakkinda = firmahakkinda;
            sb.SubeHakkinda = firmahakkinda;
            frm.ResimId = resimId;
            frm.AcilisBit = false;
            frm.DegistirenKullaniciId = KullaniciId;
            sb.DegistirenKullaniciId = KullaniciId;
            frm.DegistirmeTarih = DateTime.Now;
            sb.DegistirmeTarih = DateTime.Now;
            frm.Aktif = true;
            sb.Aktif = true;
            frm.Sil = false;
            sb.Sil = false;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Güncelleme Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Firma Bilgileri Güncelle";
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
        public string AcilisBitDegistir(long? id)
        {
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == id && x.Sil == false && x.Aktif == true);

            if (frm.AcilisBit == true)
                frm.AcilisBit = false;
            else
                frm.AcilisBit = true;
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

        [HttpPost]
        public string FirmaDurumDegistir(long id)
        {
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == id);
            if (frm.Aktif == true)
            {
                frm.Aktif = false;
            }
            else
            {
                frm.Aktif = true;
            }
            try
            {
                dbContext.SaveChanges();
                return "degisti";
            }
            catch (Exception)
            {
                return "hata";
            }
        }  // Yönetici Panelinde Kullanılkacak.
        [HttpPost]
        public string FirmaSilDegistir(long id)
        {
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == id);
            if (frm.Sil == true)
            {
                frm.Sil = false;
            }
            else
            {
                frm.Sil = true;
            }
            try
            {
                dbContext.SaveChanges();
                return "degisti";
            }
            catch (Exception)
            {
                return "hata";
            }
        }  // Yönetici Panelinde Kullanılkacak.
        public ActionResult SubeIslemleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            ViewBag.UstMenu = "Firma-Şube İşlemleri";
            ViewBag.AltMenu = "Şube İşlemleri";
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 102 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = SayfaYetki.KayitDuzenle;
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            List<Sube> subeler = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true).ToList();
            if (frm == null)
                return Json(new { Sonuc = false, Bilgi = "Görüntülenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            List<Personel> personelList = dbContext.Personels.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<Kullanici> kullaniciList = dbContext.Kullanicis.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.KullaniciYetki = SayfaYetki;
            ViewBag.sb = subeler;
            ViewBag.PersonelListesi = personelList;
            ViewBag.KullaniciListesi = kullaniciList;
            //ViewBag.FirmaAdi = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId);
            List<Il> iller = dbContext.Ils.Select(x => x).ToList();
            List<Ilce> ilceler = dbContext.Ilces.Where(x => x.IlId == frm.IlId).OrderBy(x => x.Ilce1).ToList();

            ViewBag.ilceler = ilceler;

            int subeLimit = Convert.ToInt32(dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).SubeLimit);
            int subeSayisi = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList().Count;
            ViewBag.SLimit = subeSayisi < subeLimit ? true : false;

            return View(iller);
        }
        public ActionResult SubelerListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            List<Sube> subeler = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var subelist = subeler.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                SubeAdi = m.SubeAdi,
                Yetkili = m.Yetkili,
                TCKimlikNo = m.TCKimlikNo,
                Email = m.Email,
                CepTel = m.CepTel,
                SabitTel = m.SabitTel,
                Fax = m.Fax,
                IlId = m.IlId,
                IlceId = m.IlceId,
                Il = m.Il.Il1,
                Ilce = m.Ilce.Ilce1,
                Adres = m.Adres,
                WebSitesi = m.WebSitesi,
                Facebook = m.Facebook,
                Instagram = m.Instagram,
                Twitter = m.Twitter,
                SubeHakkinda = m.SubeHakkinda,
                Notlar = m.Notlar,
                GorevliPersoneller = m.GorevliPersoneller,
                YetkiliKullanicilar = m.YetkiliKullanicilar,
                KilitBit = m.KilitBit,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih.ToShortDateString(),
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih.ToShortDateString(),
                Aktif = m.Aktif,
                Sil = m.Sil
            });
            return Json(new { data = subelist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SubeEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.OlusturanKullaniciId == 1).Id;
            }
            string SubeAdi = Request["SubeAdi"];
            string Yetkili = Request["Yetkili"];
            long YetkiliPersonelId = Convert.ToInt64(Request["YetkiliPersonelId"]);
            string TCKimlikNo = Request["TCKimlikNo"];
            string CepTel = Request["CepTel"];
            string SabitTel = Request["SabitTel"];
            string Email = Request["Email"];
            string Fax = Request["Fax"];
            int illerSube = Convert.ToInt32(Request["illerSube"]);
            int ilcelerSube = Convert.ToInt32(Request["ilcelerSube"]);
            string Adres = Request["Adres"];
            string SubeHakkinda = Request["SubeHakkinda"];
            string WebSitesi = Request["WebSitesi"];
            string Facebook = Request["Facebook"];
            string Twitter = Request["Twitter"];
            string Instagram = Request["Instagram"];
            string[] PersonelListesi = Request["PersonelListesi"].Split(','); // SubetoPersonel tablosuna tek tek eklemek için
            string PersonelList = Request["PersonelListesi"];
            bool mailsonuc;
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            TelefonDüzelt td = new TelefonDüzelt();

            if (CepTel != null) { CepTel = td.düzelt(CepTel); }
            if (SabitTel != null) { SabitTel = td.düzelt(SabitTel); }
            if (Fax != null) { Fax = td.düzelt(Fax); }
            if (string.IsNullOrEmpty(Adres))
            {
                Adres = Adres.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
                Adres = Adres.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            }
            if (string.IsNullOrEmpty(SubeHakkinda))
            {
                SubeHakkinda = SubeHakkinda.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
                SubeHakkinda = SubeHakkinda.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            }

            Sube sb = new Sube();
            sb.FirmaId = FirmaId;
            sb.SubeAdi = SubeAdi;
            sb.Yetkili = Yetkili;
            sb.TCKimlikNo = TCKimlikNo;
            sb.Email = Email;
            sb.CepTel = CepTel;
            sb.SabitTel = SabitTel;
            sb.Fax = Fax;
            sb.IlId = illerSube;
            sb.IlceId = ilcelerSube;
            sb.Adres = Adres;
            sb.SubeHakkinda = SubeHakkinda;
            sb.WebSitesi = WebSitesi;
            sb.Facebook = Facebook;
            sb.Twitter = Twitter;
            sb.Instagram = Instagram;

            sb.Aktif = true;
            sb.Sil = false;
            sb.KilitBit = false;
            sb.OlusturanKullaniciId = KullaniciId;
            sb.OlusturmaTarih = DateTime.Now;
            sb.DegistirenKullaniciId = KullaniciId;
            sb.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.Subes.Add(sb);
                dbContext.SaveChanges();
                // Firma yetkilisi yeni açılan şubeye yetkilendiriliyor.
                SubeToKullanici subekullaniciyetkili = new SubeToKullanici();
                subekullaniciyetkili.KullaniciId = dbContext.Kullanicis.FirstOrDefault(x => x.FirmaId == FirmaId && x.RolId == 2).Id;
                subekullaniciyetkili.SubeId = sb.Id;
                Thread.Sleep(100);

                // Trigger kontrol ediyor.
                //Personel personel = dbContext.Personels.FirstOrDefault(x => x.FirmaId == FirmaId && x.SubeId == sb.Id && x.PersonelGorevleri.Gorev == "Şube Yetkilisi");
                //PersonelList = PersonelList + "," + personel.Id;
                //sb.GorevliPersoneller = PersonelList;
                //dbContext.SaveChanges();


                Kullanici firmayoneticikullanici = dbContext.Kullanicis.FirstOrDefault(x => x.FirmaId == FirmaId && x.RolId == 2);
                firmayoneticikullanici.YetkiliSubeler = firmayoneticikullanici.YetkiliSubeler + "," + sb.Id;

                try
                {
                    dbContext.SubeToKullanicis.Add(subekullaniciyetkili);
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Yeni açılan şubeye Firma Sahibi Yetkilendirilirken oluşan hata.";
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
                // Şube için yazılan yetkili için bir kullanıcı oluşturuluyor.

                // Şubede çalışan personeller SubetoPersonel tablosuna ekleniyor.
                SubeToPersonel subeToPersonel = new SubeToPersonel();
                foreach (var pers in PersonelListesi)
                {
                    subeToPersonel.PersonelId = Convert.ToInt64(pers);
                    subeToPersonel.SubeId = sb.Id;
                    try
                    {
                        dbContext.SubeToPersonels.Add(subeToPersonel);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Yeni açılan şubede çalışan personel kaydı yapılırken oluşan hata.";
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
                personelSubeGuncelle(KullaniciId, FirmaId);
                SifreOlustur sifre = new SifreOlustur();
                // Şube için yazılan yetkili için personel kaydı tirigger ile oluşturuluyor.
                #region Trigger'e Alındı
                //PersonelGorevleri grv = dbContext.PersonelGorevleris.FirstOrDefault(x => x.Gorev == "Şube Yetkilisi" && x.FirmaId == FirmaId);
                //PersonelGorevleri gorev = new PersonelGorevleri();
                //long GorevId;
                //if (grv == null)
                //{
                //    gorev.Gorev = "Şube Yetkilisi";
                //    gorev.FirmaId = FirmaId;
                //    gorev.KilitBit = true;
                //    gorev.OlusturanKullaniciId = KullaniciId;
                //    gorev.OlusturmaTarih = DateTime.Now;
                //    gorev.DegistirenKullaniciId = KullaniciId;
                //    gorev.DegistirmeTarih = DateTime.Now;
                //    gorev.Aktif = true;
                //    gorev.Sil = false;
                //    dbContext.PersonelGorevleris.Add(gorev);
                //    try
                //    {
                //        dbContext.SaveChanges();
                //        GorevId = gorev.Id;
                //    }
                //    catch (Exception e)
                //    {
                //        HataLoglari hata = new HataLoglari();
                //        hata.FirmaId = FirmaId;
                //        hata.SubeId = SubeId;
                //        hata.Islem = "Yeni açılan şubeye tanımlanan yetkili için kullanıcı oluşturuluyor.Personel görevler itablosuna ŞUBE YETKİLİSİ görevi eklenirken oluşan hata.";
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
                //else
                //{
                //    GorevId = grv.Id;
                //}
                //Personel personel = new Personel();
                //personel.FirmaId = FirmaId;
                //personel.SubeId = sb.Id;
                //personel.AdiSoyadi = Yetkili;
                //personel.TCKimlikNo = TCKimlikNo;
                //personel.BaslamaTarihi = DateTime.Now;
                //personel.BitisTarihi = Convert.ToDateTime("01.01.2500");
                //personel.SabitTel = SabitTel;
                //personel.CepTel = CepTel;
                //personel.Email = Email;
                //personel.Adres = Adres;
                //personel.GorevId = GorevId;
                //personel.YillikIzinHakki = 10;
                //personel.ToplamIzin = 10;
                //personel.CalismaSekli = "Tam Zamanlı (Full Time)";
                //personel.Ucret = 0;
                //personel.OlusturanKullaniciId = KullaniciId;
                //personel.OlusturmaTarih = DateTime.Now;
                //personel.DegistirenKullaniciId = KullaniciId;
                //personel.DegistirmeTarih = DateTime.Now;
                //personel.Aktif = true;
                //personel.Sil = false;
                //personel.KilitBit = true;
                //try
                //{
                //    dbContext.Personels.Add(personel);
                //    dbContext.SaveChanges();
                //}
                //catch (Exception e)
                //{
                //    HataLoglari hata = new HataLoglari();
                //    hata.FirmaId = FirmaId;
                //    hata.SubeId = SubeId;
                //    hata.Islem = "Yeni açılan şubeye tanımlanan yetkili persoenl olarak tanımlanıyor. Personel Eklenirken oluşan hata.";
                //    hata.HataMesajı = e.Message;
                //    hata.OlusturanKullaniciId = 1;
                //    hata.OlusturmaTarih = DateTime.Now;
                //    hata.DegistirenKullaniciId = 1;
                //    hata.DegistirmeTarih = DateTime.Now;
                //    hata.Aktif = true;
                //    hata.Sil = false;
                //    dbContext.HataLoglaris.Add(hata);
                //    dbContext.SaveChanges();
                //    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                //}
                #endregion
                #region Kullanıcı Oluşturma
                // Kayıt yapılan mail adresi kullanıcı tablosunda varsa yeniden kayıt oluşturulmasın.

                Kullanici mailvarmi = dbContext.Kullanicis.FirstOrDefault(x => x.Email == Email && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                if (mailvarmi != null)
                {
                    mailvarmi.YetkiliSubeler = mailvarmi.YetkiliSubeler + "," + sb.Id;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Yeni açılan şubeye Firma Sahibi Yetkilendirilirken oluşan hata.";
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
                    kullaniciSubeGuncelle(KullaniciId, FirmaId);
                    return Json(new { Sonuc = true, Mesaj = "Şube başarıyla oluşturuldu.<br />Şube yetkilisi için bir kullanıcı oluşturuldu.<br />Şifre oluşturma maili gönderildi" }, JsonRequestBehavior.AllowGet);
                }
                // Firmaya ait personel Görevleri tanımı olarak "Şube Yetklisi" Varmı kotrolüi Yoksa Ekler
                PersonelGorevleri grv = dbContext.PersonelGorevleris.FirstOrDefault(x => x.Gorev == "Şube Yetkilisi" && x.FirmaId == FirmaId);
                PersonelGorevleri gorev = new PersonelGorevleri();
                long GorevId;
                if (grv == null)
                {
                    gorev.Gorev = "Şube Yetkilisi";
                    gorev.FirmaId = FirmaId;
                    gorev.KilitBit = true;
                    gorev.OlusturanKullaniciId = KullaniciId;
                    gorev.OlusturmaTarih = DateTime.Now;
                    gorev.DegistirenKullaniciId = KullaniciId;
                    gorev.DegistirmeTarih = DateTime.Now;
                    gorev.Aktif = true;
                    gorev.Sil = false;
                    dbContext.PersonelGorevleris.Add(gorev);
                    try
                    {
                        dbContext.SaveChanges();
                        GorevId = gorev.Id;
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Yeni açılan şubeye tanımlanan yetkili için kullanıcı oluşturuluyor.Personel görevler itablosuna ŞUBE YETKİLİSİ görevi eklenirken oluşan hata.";
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
                else
                    GorevId = grv.Id;
                Kullanici kullanici = new Kullanici();
                kullanici.FirmaId = FirmaId;
                kullanici.AdSoyad = Yetkili;
                kullanici.CepTel = CepTel;
                kullanici.Email = Email;
                kullanici.GorevId = GorevId;
                kullanici.Notlar = SubeAdi + " Şubesi Yetkilisi";
                kullanici.RolId = 3; // Şube Yetkilisi
                kullanici.GeciciSifre = "7nyGUP";
                kullanici.SifreHash = sifre.sifreolustur(6);
                kullanici.ResimId = 2;
                kullanici.YetkiliSubeler = sb.Id.ToString();
                kullanici.OlusturanKullaniciId = KullaniciId;
                kullanici.OlusturmaTarih = DateTime.Now;
                kullanici.DegistirenKullaniciId = KullaniciId;
                kullanici.DegistirmeTarih = DateTime.Now;
                kullanici.Aktif = true;
                kullanici.Sil = false;
                kullanici.KilitBit = true;
                try
                {
                    dbContext.Kullanicis.Add(kullanici);
                    dbContext.SaveChanges();
                    SubeToKullanici subekullanici = new SubeToKullanici();
                    subekullanici.KullaniciId = kullanici.Id;
                    subekullanici.SubeId = sb.Id;
                    dbContext.SubeToKullanicis.Add(subekullanici);
                    sb.YetkiliKullanicilar = kullanici.Id.ToString();
                    dbContext.SaveChanges();
                    #region Şifre Oluşturma Linkini Mail Gönder
                    string token = kullanici.Id.ToString() + "-" + kullanici.SifreHash + "-" + FirmaId.ToString();
                    string domainname = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    string SifreOlusturmaLinki = domainname + "/Otomasyon/KullaniciIslemleri/SifreOlustur/" + token;
                    string konu = "Fotoğrafçı Takip - Kullanıcı Şifre Oluşturma";
                    string body = string.Empty;
                    string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
                    string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");

                    using (StreamReader reader = new StreamReader(Server.MapPath("/Areas/Otomasyon/Dosyalar/EmailSablonlari/Kullanici_Sifre_Olustur2.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{FirmaLogo}", "src=cid:firmalogo");
                    body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
                    body = body.Replace("{FirmaAdi}", frm.FirmaAdi);
                    body = body.Replace("{KullaniciAdi}", kullanici.AdSoyad);
                    body = body.Replace("{SifreOlusturmaLinki}", SifreOlusturmaLinki);
                    body = body.Replace("{FirmaWeb}", frm.WebSitesi);
                    body = body.Replace("{FirmaFacebook}", frm.Facebook);
                    body = body.Replace("{FirmaInstagram}", frm.Instagram);
                    body = body.Replace("{FirmaTwitter}", frm.Twitter);

                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, new System.Net.Mime.ContentType("text/html"));
                    LinkedResource firmalogo = new LinkedResource(FirmaLogo);
                    firmalogo.ContentId = "firmalogo";
                    LinkedResource fotografcitakiplogo = new LinkedResource(FotografciTakipLogo);
                    fotografcitakiplogo.ContentId = "fotografcitakiplogo";
                    htmlView.LinkedResources.Add(firmalogo);
                    htmlView.LinkedResources.Add(fotografcitakiplogo);
                    mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, kullanici.Email, konu, body, htmlView);
                    if (mailsonuc == false)
                    {
                        return Json(new { Sonuc = true, Mail = false, Mesaj = "Kullanıcıya şifre oluşturma maili gönderilemedi!<br/>Kullanıcı mail adresini veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz.", JsonRequestBehavior.AllowGet });
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Yeni açılan şubeye tanımlanan yetkili için kullanıcı oluşturuluyor.Kullanıcı Eklenirken veya şubeye yetkilendirilirken oluşan hata.";
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
                return Json(new { Sonuc = true, Mesaj = "Şube başarıyla oluşturuldu.<br />Şube yetkilisi için bir kullanıcı oluşturuldu.<br />Şifre oluşturma maili gönderildi" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Şube Ekleme esnasında oluşan hata. ŞUBE tablosu";
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
        public ActionResult SubeGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.OlusturanKullaniciId == 1).Id;
            }
            string SubeAdi = Request["SubeAdi"];
            string Yetkili = Request["Yetkili"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string CepTel = Request["CepTel"];
            string SabitTel = Request["SabitTel"];
            string Email = Request["Email"];
            string Fax = Request["Fax"];
            int illerSube = Convert.ToInt32(Request["illerSube"]);
            int ilcelerSube = Convert.ToInt32(Request["ilcelerSube"]);
            string Adres = Request["Adres"];
            string SubeHakkinda = Request["SubeHakkinda"];
            string WebSitesi = Request["WebSitesi"];
            string Facebook = Request["Facebook"];
            string Twitter = Request["Twitter"];
            string Instagram = Request["Instagram"];
            string[] PersonelListesi = Request["PersonelListesi"].Split(','); // SubetoPersonel tablosuna tek tek eklemek için
            string PersonelList = Request["PersonelListesi"];
            TelefonDüzelt td = new TelefonDüzelt();

            if (CepTel != null) { CepTel = td.düzelt(CepTel); }
            if (SabitTel != null) { SabitTel = td.düzelt(SabitTel); }
            if (Fax != null) { Fax = td.düzelt(Fax); }
            if (string.IsNullOrEmpty(Adres))
            {
                Adres = Adres.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
                Adres = Adres.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            }
            if (string.IsNullOrEmpty(SubeHakkinda))
            {
                SubeHakkinda = SubeHakkinda.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
                SubeHakkinda = SubeHakkinda.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            }
            Sube s = dbContext.Subes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (s == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            s.SubeAdi = SubeAdi;
            s.Yetkili = Yetkili;
            s.TCKimlikNo = TCKimlikNo;
            s.Email = Email;
            s.CepTel = CepTel;
            s.SabitTel = SabitTel;
            s.Fax = Fax;
            s.IlId = illerSube;
            s.IlceId = ilcelerSube;
            s.Adres = Adres;
            s.WebSitesi = WebSitesi;
            s.Facebook = Facebook;
            s.Instagram = Instagram;
            s.Twitter = Twitter;
            s.GorevliPersoneller = PersonelList;
            s.SubeHakkinda = SubeHakkinda;
            s.DegistirenKullaniciId = KullaniciId;
            s.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                // Şubede çalışan personeller SubetoPersonel tablosuna ekleniyor.
                #region Personel Şube Yetkilendirme
                // şubenin tüm çalışanları personeltosube tablosundan silincek.
                SubeToPersonel subepersonel = new SubeToPersonel();
                dbContext.SubeToPersonels.RemoveRange(dbContext.SubeToPersonels.Where(x => x.SubeId == id));
                SubeToPersonel subeToPersonel = new SubeToPersonel();
                dbContext.SaveChanges();
                foreach (var pers in PersonelListesi)
                {
                    subeToPersonel.PersonelId = Convert.ToInt64(pers);
                    subeToPersonel.SubeId = s.Id;
                    try
                    {
                        dbContext.SubeToPersonels.Add(subeToPersonel);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Yeni açılan şubede çalışan personel kaydı yapılırken oluşan hata.";
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
                #endregion
                personelSubeGuncelle(KullaniciId, FirmaId);
                return Json(new { Sonuc = true, Mesaj = "Güncelleme Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Şube Güncelle, Kayıt Id: " + id.ToString();
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
        public ActionResult SubeSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            }

            Sube sube = dbContext.Subes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (sube == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            // Şubeye ait kayıt var mı? Kontrolü
            List<Sozlesme> sz = dbContext.Sozlesmes.Where(x => x.SubeId == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
            List<GunlukIsler> gis = dbContext.GunlukIslers.Where(x => x.SubeId == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
            List<GelirGider> gg = dbContext.GelirGiders.Where(x => x.SubeId == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
            List<Models.Musteri> musteri = dbContext.Musteris.Where(x => x.SubeId == id && x.FirmaId == FirmaId && x.AdiSoyadi != "-- Müşterisiz İşlem --" && x.Sil == false && x.Aktif == true).ToList();
            PersonelGorevleri gorev = dbContext.PersonelGorevleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Gorev == "Şube Yetkilisi" && x.Sil == false && x.Aktif == true);
            List<Personel> per = dbContext.Personels.Where(x => x.SubeId == id && x.FirmaId == FirmaId && x.GorevId == gorev.Id && x.Sil == false && x.Aktif == true).ToList();
            List<Kullanici> kl = dbContext.Kullanicis.Where(x => x.FirmaId == FirmaId && x.GorevId == gorev.Id && x.Sil == false && x.Aktif == true).ToList();
            // Personele ait İzin ve Odeme Bilgileri kontrol ediliyor.
            List<PersonelIzin> izin = new List<PersonelIzin>();
            List<PersonelOdeme> perodeme = new List<PersonelOdeme>();
            if (per.Count > 0)
            {
                foreach (var personel in per)
                {
                    List<PersonelIzin> pizin = dbContext.PersonelIzins.Where(x => x.PersonelId == personel.Id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
                    if (pizin.Count > 0)
                        izin.AddRange(pizin);
                    List<PersonelOdeme> podeme = dbContext.PersonelOdemes.Where(x => x.PersonelId == personel.Id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
                    if (podeme.Count > 0)
                        perodeme.AddRange(podeme);
                }
            }

            //List<PersonelIzin> izin = dbContext.PersonelIzins.Where(x => x.PersonelId == per.Id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
            //List<PersonelOdeme> perodeme = dbContext.PersonelOdemes.Where(x => x.PersonelId == per.Id && x.Sil == false && x.Aktif == true).ToList();
            //Şube ile birlikte yetkilisi de silinecek
            // ***** ÖNEMLİ *****  bu şubeye ait diğer tablolarda kayıt olup olmadığı kontrol edilecek. kayıt vasa silinmeyecek. 
            if (sz.Count > 0 || gis.Count > 0 || gg.Count > 0 || musteri.Count > 0) // Subeye ait bir kayıt varsa silme işlemi olmayacak
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Şube Sil, Şubeye ait detay kayıtları var, Silinemez! Kayıt Id: " + id.ToString();
                hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Şubeye ait Kayıtlar var, Silinemez!", JsonRequestBehavior.AllowGet });
            }
            else  // Subeye ait bir kayıt yoksa şube silinecek, şubeye ait kullanıcı ve personeller de silinmeyecek...
            {
                // Şube ile birlikte eklenene Personel ve Kullanıcı Kayıları silinmeyecek. Kullanıcı ve Personele ait Şube İlişkileri kaldırılıcak.
                List<string> yenisubelist = new List<string>();
                long AnaSubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.Aktif == true && x.Sil == false).Id;
                if (kl.Count > 0)
                {
                    foreach (var kullanici in kl)
                    {
                        SubeToKullanici stk = new SubeToKullanici();
                        string[] kullanicisube = kullanici.YetkiliSubeler.Split(',');
                        foreach (var s in kullanicisube)
                        {
                            long sid = Convert.ToInt64(s);
                            if (sid != id)
                            {
                                yenisubelist.Add(s);
                                stk.SubeId = sid;
                                stk.KullaniciId = kullanici.Id;
                                dbContext.SubeToKullanicis.Add(stk);
                            }
                        }
                        if (yenisubelist.Count == 0)
                        {
                            kullanici.YetkiliSubeler = AnaSubeId.ToString();
                            kullanici.KilitBit = false;
                            stk.SubeId = AnaSubeId;
                            stk.KullaniciId = kullanici.Id;
                            dbContext.SubeToKullanicis.Add(stk);
                        }
                        else
                        {
                            kullanici.YetkiliSubeler = yenisubelist.ToString();
                            kullanici.KilitBit = false;
                        }

                        dbContext.SaveChanges();
                        yenisubelist.Clear();
                    }
                }
                if (per.Count > 0)
                {
                    foreach (var personel in per)
                    {
                        SubeToPersonel stk = new SubeToPersonel();
                        string[] personelsube = personel.GorevliSubeler.Split(',');
                        foreach (var s in personelsube)
                        {
                            long sid = Convert.ToInt64(s);
                            if (sid != id)
                            {
                                yenisubelist.Add(s);
                                stk.SubeId = sid;
                                stk.PersonelId = personel.Id;
                                dbContext.SubeToPersonels.Add(stk);
                            }
                        }
                        if (yenisubelist.Count == 0)
                        {
                            personel.GorevliSubeler = AnaSubeId.ToString();
                            personel.KilitBit = false;
                            stk.SubeId = AnaSubeId;
                            stk.PersonelId = personel.Id;
                            dbContext.SubeToPersonels.Add(stk);
                        }
                        else
                        {
                            personel.GorevliSubeler = yenisubelist.ToString();
                            personel.KilitBit = false;
                        }

                        dbContext.SaveChanges();
                        yenisubelist.Clear();
                    }
                }
                Models.Musteri m = dbContext.Musteris.FirstOrDefault(x => x.SubeId == id && x.FirmaId == FirmaId && x.AdiSoyadi == "-- Müşterisiz İşlem --" && x.Sil == false && x.Aktif == true);
                if (m != null)
                {
                    m.Aktif = false;
                    m.Sil = true;
                    m.DegistirenKullaniciId = KullaniciId;
                    m.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }
                //Personelin şube ilişikileri siliniyor.
                List<SubeToPersonel> subepersonel = dbContext.SubeToPersonels.Where(x => x.SubeId == id).ToList();
                if (subepersonel.Count > 0)
                {
                    foreach (var item in subepersonel)
                    {
                        dbContext.SubeToPersonels.Remove(item);
                        dbContext.SaveChanges();
                    }
                }
                //Kullanıcının şube ilişikileri siliniyor.
                List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.SubeId == id).ToList();
                if (subekullanici.Count > 0)
                {
                    foreach (var sbkullanici in subekullanici)
                    {
                        dbContext.SubeToKullanicis.Remove(sbkullanici);
                        dbContext.SaveChanges();
                    }
                }
                personelSubeGuncelle(KullaniciId, FirmaId); // personel tablosunda ilgili personelin GorevliSube alanında güncelleme
                kullaniciSubeGuncelle(KullaniciId, FirmaId); // kullanıcı tablosunda ilgili kullanıcının YetkiliSube alanında güncelleme
                sube.Sil = true;
                sube.Aktif = false;
                sube.DegistirenKullaniciId = KullaniciId;
                sube.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Şube Silme, Kayıt Id: " + id.ToString();
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
        [HttpPost]
        public JsonResult IlceGetir(int id)
        {
            List<Ilce> ilceler = dbContext.Ilces.Where(x => x.IlId == id).ToList();
            var ilce = ilceler.Select(m => new { Id = m.Id, IlceAd = m.Ilce1 }).OrderBy(x => x.IlceAd); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(ilce, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public string SubeDurumDegistir(long? id)
        {
            Sube sube = dbContext.Subes.FirstOrDefault(x => x.Id == id && x.KilitBit == false && x.Sil == false);
            if (sube.Aktif == true)
                sube.Aktif = false;
            else
                sube.Aktif = true;
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
        void personelSubeGuncelle(long kullaniciId, long firmaId)
        {
            List<Personel> personels = dbContext.Personels.Where(x => x.FirmaId == firmaId && x.Aktif == true && x.Sil == false).ToList();
            string GorevliSubeler = "";
            foreach (var pers in personels)
            {
                GorevliSubeler = "";
                Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == pers.Id);
                List<SubeToPersonel> subeToPersonels = dbContext.SubeToPersonels.Where(x => x.PersonelId == pers.Id).ToList();
                foreach (var item in subeToPersonels)
                {
                    if (GorevliSubeler == "")
                        GorevliSubeler = item.SubeId.ToString();
                    else
                        GorevliSubeler = GorevliSubeler + "," + item.SubeId.ToString();
                }
                personel.GorevliSubeler = GorevliSubeler;
                personel.DegistirmeTarih = DateTime.Now;
                personel.DegistirenKullaniciId = kullaniciId;
                dbContext.SaveChanges();
            }
        }
        void kullaniciSubeGuncelle(long kullaniciId, long firmaId)
        {
            List<Kullanici> kullanici = dbContext.Kullanicis.Where(x => x.FirmaId == firmaId && x.Aktif == true && x.Sil == false).ToList();
            string YetkiliSubeler = "";
            foreach (var kl in kullanici)
            {
                YetkiliSubeler = "";
                Kullanici k = dbContext.Kullanicis.FirstOrDefault(x => x.Id == kl.Id);
                List<SubeToKullanici> subeToKullanicis = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == kl.Id).ToList();
                foreach (var item in subeToKullanicis)
                {
                    if (YetkiliSubeler == "")
                        YetkiliSubeler = item.SubeId.ToString();
                    else
                        YetkiliSubeler = YetkiliSubeler + "," + item.SubeId.ToString();
                }
                k.YetkiliSubeler = YetkiliSubeler;
                k.DegistirmeTarih = DateTime.Now;
                k.DegistirenKullaniciId = kullaniciId;
                dbContext.SaveChanges();
            }
        }
    }
}