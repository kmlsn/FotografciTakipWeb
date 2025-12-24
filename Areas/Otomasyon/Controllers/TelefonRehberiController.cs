using FotografciTakipWeb.App_Settings;
using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class TelefonRehberiController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/TelefonRehberi
        public ActionResult Rehber()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Telefon Rehberi";
            ViewBag.AltMenu = "Rehber";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 65 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            List<GelirGiderTurleri> ggtur = dbContext.GelirGiderTurleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<RehberGrup> regbergrup = dbContext.RehberGrups.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.gruplar = regbergrup;
            ViewBag.SubeListesi = sube;
            ViewBag.SubeKullanici = subekullanici;
            ViewBag.GGTur = ggtur;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 65 && x.Aktif == true && x.Sil == false);

            return View();
        }
        public ActionResult RehberListele()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            List<TelefonRehberi> tl = dbContext.TelefonRehberis.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            var tellist = tl.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                RehberGrupId = m.RehberGrupId,
                GrupAdi = m.RehberGrup.GrupAdi,
                AdSoyad = m.AdSoyad,
                FirmaAdi = m.FirmaAdi,
                SabitTel1 = m.SabitTel1,
                SabitTel2 = m.SabitTel2,
                CepTel1 = m.CepTel1,
                CepTel2 = m.CepTel2,
                Fax = m.Fax,
                Email = m.Email,
                EmailKabul = m.EmailKabul,
                SmsKabul = m.SmsKabul,
                Notlar = m.Notlar,
                Aktif = m.Aktif,
                Sil = m.Sil,
                KilitBit = m.KilitBit
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = tellist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RehberEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            TelefonRehberi rehber = new TelefonRehberi();
            string notlar = Request["Notlar"];
            notlar = notlar.Replace("\r\n", "<br />");
            notlar = notlar.Replace("\n", "<br />");
            long RehberGrupId = Convert.ToInt64(Request["RehberGrupId"]);
            string AdSoyad = Request["AdSoyad"];
            string cep1 = Request["CepTel1"];
            string cep2 = Request["CepTel2"];
            string sabit1 = Request["SabitTel1"];
            string sabit2 = Request["SabitTel2"];
            string Email = Request["Email"];
            TelefonDüzelt td = new TelefonDüzelt();
            if (cep1 != null) { cep1 = td.düzelt(cep1); }
            if (cep2 != null) { cep2 = td.düzelt(cep2); }
            if (sabit1 != null) { sabit1 = td.düzelt(sabit1); }
            if (sabit2 != null) { sabit2 = td.düzelt(sabit2); }
            TelefonRehberi kayitvarmi = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.RehberGrupId == RehberGrupId && x.AdSoyad == AdSoyad && x.CepTel1 == cep1 && x.Aktif == false && x.Sil == true); // eklenmek istenilen kayıt daha önceden silinmişse aktif yapılıyor.
            if (kayitvarmi != null)
            {
                kayitvarmi.Sil = false;
                kayitvarmi.Aktif = true;
                kayitvarmi.DegistirenKullaniciId = KullaniciId;
                kayitvarmi.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Rehber Kaydı Ekle, Silinmiş kaydı aktif yap, Kayıt Id: " + kayitvarmi.Id.ToString();
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
            {
                rehber.RehberGrupId = RehberGrupId;
                rehber.AdSoyad = AdSoyad;
                rehber.CepTel1 = cep1;
                rehber.CepTel2 = cep2;
                rehber.SabitTel1 = sabit1;
                rehber.SabitTel2 = sabit2;
                rehber.Email = Email;
                rehber.Notlar = notlar;
                rehber.FirmaId = FirmaId;
                rehber.OlusturanKullaniciId = KullaniciId;
                rehber.OlusturmaTarih = DateTime.Now;
                rehber.DegistirenKullaniciId = KullaniciId;
                rehber.DegistirmeTarih = DateTime.Now;
                rehber.Aktif = true;
                rehber.Sil = false;
                rehber.KilitBit = false;
                try
                {
                    dbContext.TelefonRehberis.Add(rehber);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Rehber Kaydı Ekle";
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
        public ActionResult RehberGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            string notlar = Request["Notlar"];
            long RehberGrupId = Convert.ToInt64(Request["RehberGrupId"]);
            string AdSoyad = Request["AdSoyad"];
            string cep1 = Request["CepTel1"];
            string cep2 = Request["CepTel2"];
            string sabit1 = Request["SabitTel1"];
            string sabit2 = Request["SabitTel2"];
            string Email = Request["Email"];
            TelefonDüzelt td = new TelefonDüzelt();
            if (notlar != null) { notlar = notlar.Replace("\r\n", "<br />"); }
            if (notlar != null) { notlar = notlar.Replace("\n", "<br />"); }
            if (cep1 != null) { cep1 = td.düzelt(cep1); }
            if (cep2 != null) { cep2 = td.düzelt(cep2); }
            if (sabit1 != null) { sabit1 = td.düzelt(sabit1); }
            if (sabit2 != null) { sabit2 = td.düzelt(sabit2); }

            TelefonRehberi r = dbContext.TelefonRehberis.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (r == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            r.RehberGrupId = RehberGrupId;
            r.AdSoyad = AdSoyad;
            r.CepTel1 = cep1;
            r.CepTel2 = cep2;
            r.SabitTel1 = sabit1;
            r.SabitTel2 = sabit2;
            r.Email = Email;
            r.Notlar = notlar;
            r.DegistirenKullaniciId = KullaniciId;
            r.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Rehber Kaydı gündelle, Kayıt Id: " + id.ToString();
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
        public string KayıtVarmi()
        {
            long RehberGrupId = Convert.ToInt64(Request["RehberGrupId"]);
            string AdSoyad = Request["AdSoyad"].ToLower();
            string cep1 = Request["CepTel1"];
            TelefonDüzelt td = new TelefonDüzelt();
            if (cep1 != null) { cep1 = td.düzelt(cep1); }
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.RehberGrupId == RehberGrupId && x.FirmaId == FirmaId && x.AdSoyad.ToLower() == AdSoyad && x.CepTel1 == cep1 && x.Aktif == true && x.Sil == false);

            if (rehber != null)
            {
                return rehber.Id.ToString();
            }
            else
            {
                return "Yok";
            }

        }
        [HttpPost]
        public ActionResult RehberSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            TelefonRehberi r = dbContext.TelefonRehberis.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (r == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            r.Aktif = false;
            r.Sil = true;
            r.DegistirenKullaniciId = KullaniciId;
            r.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Gelir - Gider Sil, Sözleşmeden alınan ödeme, Kayıt Id: " + id.ToString();
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
        public string EmailDurumDegistir(long? id)
        {
            TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (rehber.EmailKabul == true)
                rehber.EmailKabul = false;
            else
                rehber.EmailKabul = true;
            try
            {
                dbContext.SaveChanges();
                if (rehber.RehberGrup.GrupAdi == "Müşteriler")
                {
                    Models.Musteri m = dbContext.Musteris.FirstOrDefault(x => x.Id == rehber.MusteriId && x.Sil == false);
                    m.EmailKabul = rehber.EmailKabul;
                    dbContext.SaveChanges();
                }
                if (rehber.RehberGrup.GrupAdi == "Personeller")
                {
                    Personel p = dbContext.Personels.FirstOrDefault(x => x.Id == rehber.PersonelId && x.Sil == false);
                    p.EmailKabul = rehber.EmailKabul;
                    dbContext.SaveChanges();
                }
                if (rehber.RehberGrup.GrupAdi == "Cariler")
                {
                    Cari c = dbContext.Caris.FirstOrDefault(x => x.Id == rehber.CariId && x.Sil == false);
                    c.EmailKabul = rehber.EmailKabul;
                    dbContext.SaveChanges();
                }
                return "degisti";
            }
            catch (Exception)
            {
                return "hata";
            }
        }
        [HttpPost]
        public string SmsDurumDegistir(long? id)
        {
            TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.Id == id && x.Sil == false);
           
            if (rehber.SmsKabul == true)
                rehber.SmsKabul = false;
            else
                rehber.SmsKabul = true;
            try
            {
                dbContext.SaveChanges();
                if (rehber.RehberGrup.GrupAdi == "Müşteriler")
                {
                    Models.Musteri m = dbContext.Musteris.FirstOrDefault(x => x.Id == rehber.MusteriId && x.Sil == false);
                    m.SMSKabul = rehber.SmsKabul;
                    dbContext.SaveChanges();
                }
                if (rehber.RehberGrup.GrupAdi == "Personeller")
                {
                    Personel p = dbContext.Personels.FirstOrDefault(x => x.Id == rehber.PersonelId && x.Sil == false);
                    p.SMSKabul = rehber.SmsKabul;
                    dbContext.SaveChanges();
                }
                if (rehber.RehberGrup.GrupAdi == "Cariler")
                {
                    Cari c = dbContext.Caris.FirstOrDefault(x => x.Id == rehber.CariId && x.Sil == false);
                    c.SMSKabul = rehber.SmsKabul;
                    dbContext.SaveChanges();
                }
                return "degisti";
            }
            catch (Exception)
            {
                return "hata";
            }
        }

        public ActionResult ExceldenEkle()
        {
            ViewBag.UstMenu = "Telefon Rehberi";
            ViewBag.AltMenu = "Excel'den Kayıt Ekle";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 66 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<RehberGrup> regbergrup = dbContext.RehberGrups.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.gruplar = regbergrup;

            return View();
        }

        [HttpPost]
        public ActionResult ExcelGoruntule()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            //Dosya kontrolü
            long RehberGrupId = Convert.ToInt64(Request["GrupId"]);
            HttpFileCollectionBase file = Request.Files;
            HttpPostedFileBase excel = file[0];

            if (excel == null || excel.ContentLength == 0)
            {
                ViewBag.Error = "Lütfen dosya seçimi yapınız.";
                return View();
            }
            else
            {
                //Dosyanın uzantısı xls ya da xlsx ise;
                if (excel.FileName.EndsWith("xls") || excel.FileName.EndsWith("xlsx") || excel.FileName.EndsWith("csv"))
                {
                    //Seçilen dosyanın nereye kaydedileceği belirtiliyor.
                    string path = Server.MapPath("~/Temp/" + excel.FileName);

                    //Dosya kontrol edilir, varsa silinir.
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    //Excel path altına kaydedilir.
                    excel.SaveAs(path);

                    //+Exceli açıyoruz
                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(path,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, true);
                    Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;
                    //-

                    //Verileri listeye atıp listele viewında göstereceğim, o yüzden modelimin tipinde liste değişkeni tanımlıyorum.
                    List<TelefonRehberi> rehber = new List<TelefonRehberi>();
                    //tüm verilerde dönüp ilgili veriyi ilgili modele atıyorum. sonrasında modeli listeye atıyorum.
                    for (int i = 2; i <= range.Rows.Count; i++)
                    {
                        TelefonRehberi r = new TelefonRehberi();
                        r.RehberGrupId = RehberGrupId;
                        r.FirmaAdi = ((Excel.Range)range.Cells[i, 1]).Text;
                        r.AdSoyad = ((Excel.Range)range.Cells[i, 2]).Text;
                        r.SabitTel1 = ((Excel.Range)range.Cells[i, 3]).Text;
                        r.SabitTel2 = ((Excel.Range)range.Cells[i, 4]).Text;
                        r.CepTel1 = ((Excel.Range)range.Cells[i, 5]).Text;
                        r.CepTel2 = ((Excel.Range)range.Cells[i, 6]).Text;
                        r.Email = ((Excel.Range)range.Cells[i, 7]).Text;
                        r.Notlar = ((Excel.Range)range.Cells[i, 8]).Text;
                        rehber.Add(r);
                        // Burada excel dosyası okunuyor ve sayfada görüntüleniyor. Veri tabanına herhangi bir kayıt yapılmıyor.
                    }
                    application.Quit(); // Excel kapatılıyor.
                    foreach (var process in Process.GetProcesses()) // Arka planda çalışan Excel uygulaması kapatılıyor.
                    {
                        string program = process.ProcessName.ToLower();
                        if (program == "excel")
                            process.Kill();
                    }
                    //listeyi bu sayfaya taşımak için bu viewBag içine alıyorum.
                    //ViewBag.MailGrupId = MailGrupId;
                    //ViewBag.MailGrup = dbcontext.MailGrups.FirstOrDefault(x => x.Id == MailGrupId).GrupAdi;
                    //ViewBag.DosyaYol = path;
                    //listele viewına döndürüyorum.
                    return Json(rehber, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ViewBag.Error = "Dosya tipiniz yanlış, lütfen '.xls' yada '.xlsx' uzantılı dosya yükleyiniz.";
                    return RedirectToAction("ExceldenEkle");
                }
            }
        }
    }
}