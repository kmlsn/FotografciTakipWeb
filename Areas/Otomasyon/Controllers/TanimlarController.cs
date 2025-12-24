using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FotografciTakipWeb.Models;
using System.Web;
using System.IO;
using FotografciTakipWeb.App_Settings;
using System.Drawing;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class TanimlarController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        ResimIsim resimisimlendir = new ResimIsim();
        // GET: Otomasyon/Tanimlar
        #region Çekim Paketleri İşlemleri
        public ActionResult CekimPaketleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Çekim Paketleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 74 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.kullaniciId = KullaniciId;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 74 && x.Aktif == true && x.Sil == false);

            return View();
        }
        [HttpPost]
        public ActionResult CekimPaketleriListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<CekimPaketleri> cekimpaketleri = dbContext.CekimPaketleris.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var paketlist = cekimpaketleri.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                PaketAdi = m.PaketAdi,
                PaketDetayi = m.PaketDetayi,
                Fiyat = m.Fiyat,
                Aktif = m.Aktif,
                KilitBit = m.KilitBit,
                Sil = m.Sil
            }).ToList(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = paketlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CekimPaketleriEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            string PaketAdi = Request["paketadi"];
            string PaketDetayi = Request["paketdetayi"];
            decimal Fiyat = Convert.ToDecimal(Request["fiyat"]);
            string detay = PaketDetayi;
            detay = detay.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            detay = detay.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            CekimPaketleri paketvarmi = dbContext.CekimPaketleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == true && x.Aktif == false && x.PaketAdi == PaketAdi && x.PaketDetayi == detay && x.Fiyat == Fiyat);

            if (paketvarmi != null) // Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
            {
                paketvarmi.Sil = false;
                paketvarmi.Aktif = true;
                paketvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                paketvarmi.DegistirmeTarih = DateTime.Now;
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
                    hata.Islem = "Çekim Paketi Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + paketvarmi.Id.ToString();
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
                CekimPaketleri pkt = new CekimPaketleri();
                pkt.FirmaId = FirmaId;  // Login olan kullanıcının firması
                pkt.PaketAdi = PaketAdi;
                pkt.PaketDetayi = detay;
                pkt.Fiyat = Fiyat;
                pkt.KilitBit = false;
                pkt.OlusturanKullaniciId = KullaniciId;
                pkt.OlusturmaTarih = DateTime.Now;
                pkt.DegistirenKullaniciId = KullaniciId;
                pkt.DegistirmeTarih = DateTime.Now;
                pkt.Aktif = true;
                pkt.Sil = false;

                try
                {
                    dbContext.CekimPaketleris.Add(pkt);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Çekim Paketi Ekle";
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
        public ActionResult CekimPaketiGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            string PaketAdi = Request["paketadi"];
            string PaketDetayi = Request["paketdetayi"];
            decimal Fiyat = Convert.ToDecimal(Request["fiyat"]);
            string detay = PaketDetayi;
            detay = detay.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            detay = detay.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.

            CekimPaketleri paket = dbContext.CekimPaketleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (paket == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            paket.PaketAdi = PaketAdi;
            paket.PaketDetayi = detay;
            paket.Fiyat = Fiyat;
            paket.DegistirenKullaniciId = KullaniciId;
            paket.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Çekim Paketi Güncelle, Kayıt Id: " + id.ToString();
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
        public ActionResult CekimPaketiSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            List<Sozlesme> sz = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            foreach (var item in sz)
            {
                long sonuc = item.PaketlerId.IndexOf(id.ToString());
                if (sonuc != -1)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Çekim Paketi Sil, Çekim Paketi Kullanılıyor, Silinemez! Kayıt Id: " + id.ToString();
                    hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Kayıt Kullanılıyor, Silinemez!", JsonRequestBehavior.AllowGet });
                }
            }

            CekimPaketleri pkt = dbContext.CekimPaketleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (pkt == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            pkt.DegistirenKullaniciId = KullaniciId;
            pkt.DegistirmeTarih = DateTime.Now;
            pkt.Aktif = false;
            pkt.Sil = true;
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
                hata.Islem = "Çekim Paketi Sil, Kayıt Id: " + id.ToString();
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
        public string CekimPaketiDurumDegistir(long? id)
        {
            Models.CekimPaketleri paket = dbContext.CekimPaketleris.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (paket.Aktif == true)
                paket.Aktif = false;
            else
                paket.Aktif = true;
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
        public string PaketVarMi()
        {
            string PaketAdi = Request["paketadi"];
            string PaketDetayi = Request["paketdetayi"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            CekimPaketleri pkt = dbContext.CekimPaketleris.FirstOrDefault(x => x.PaketAdi == PaketAdi && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (pkt != null)
                return pkt.Id.ToString();
            else
                return "Yok";
        }
        #endregion

        #region Gelir Gider Türleri İşlemleri
        public ActionResult GelirGiderTurleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Gelir Gider Türleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 75 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 75 && x.Aktif == true && x.Sil == false);

            return View();
        }
        public ActionResult GelirGiderTuruListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<GelirGiderTurleri> ggturlist;
            ggturlist = dbContext.GelirGiderTurleris.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var gglist = ggturlist.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                GelirGiderTur = m.GelirGiderTur,
                KilitBit = m.KilitBit,
                Aktif = m.Aktif,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = gglist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GelirGiderTuruEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            string GelirGiderTuru = Request["GelirGider"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            GelirGiderTurleri ggvarmi = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.GelirGiderTur == GelirGiderTuru && x.FirmaId == FirmaId && x.Sil == true && x.Aktif == false);

            if (ggvarmi != null) // Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
            {
                ggvarmi.Sil = false;
                ggvarmi.Aktif = true;
                ggvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                ggvarmi.DegistirmeTarih = DateTime.Now;

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
                    hata.Islem = "Gelir Gider Türü Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + ggvarmi.Id.ToString();
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
                GelirGiderTurleri ggtur = new GelirGiderTurleri();
                ggtur.FirmaId = FirmaId;  // Sessiondaki FirmaId
                ggtur.GelirGiderTur = GelirGiderTuru;
                ggtur.Aktif = true;
                ggtur.Sil = false;
                ggtur.KilitBit = false;
                ggtur.OlusturanKullaniciId = KullaniciId;
                ggtur.OlusturmaTarih = DateTime.Now;
                ggtur.DegistirenKullaniciId = KullaniciId;
                ggtur.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.GelirGiderTurleris.Add(ggtur);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Gelir Gider Türü Ekle";
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
        public ActionResult GelirGiderTuruGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            string GelirGiderTuru = Request["GelirGider"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            GelirGiderTurleri ggtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);   // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak
            if (ggtur == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            ggtur.GelirGiderTur = GelirGiderTuru;
            ggtur.DegistirenKullaniciId = KullaniciId;
            ggtur.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Gelir Güder Türü Güncelle, Kayıt Id: " + id.ToString();
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
        public ActionResult GelirGiderVarsayilanlariEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            string[] gelirgider = Request["VarsayilanDegerler"].Split(',');
            GelirGiderTurleri ggtur = new GelirGiderTurleri();
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            for (int i = 0; i < gelirgider.Length; i++)
            {
                string gelirgiderturu = gelirgider[i];
                GelirGiderTurleri gelirgidervarmi = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.GelirGiderTur == gelirgiderturu);
                if (gelirgidervarmi != null)// Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise 
                {
                    if (gelirgidervarmi.Sil == true || gelirgidervarmi.Aktif == false) // (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
                    {
                        gelirgidervarmi.Sil = false;
                        gelirgidervarmi.Aktif = true;
                        gelirgidervarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                        gelirgidervarmi.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Gelir Gider Türü Ekle, Varolan Kaydın güncellenmesi, Kayıt Id: " + gelirgidervarmi.Id.ToString();
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
                else // eklenmek istenilen kayıt yoksa yeni kayıt ekleniyor.
                {
                    ggtur.FirmaId = FirmaId; // sessiondaki FirmaID
                    ggtur.GelirGiderTur = gelirgider[i];
                    ggtur.OlusturanKullaniciId = KullaniciId;
                    ggtur.OlusturmaTarih = DateTime.Now;
                    ggtur.DegistirenKullaniciId = KullaniciId;
                    ggtur.DegistirmeTarih = DateTime.Now;
                    ggtur.Aktif = true;
                    ggtur.Sil = false;
                    //if (gelirgider[i] == "Cari İşlem Ödemesi" || gelirgider[i] == "Günlük İşler" || gelirgider[i] == "Sözleşme Ödemesi")
                    //    ggtur.KilitBit = true;
                    //else
                    //    ggtur.KilitBit = false;
                    try
                    {
                        dbContext.GelirGiderTurleris.Add(ggtur);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Gelir Gider Türü Ekle, Varsayılan Değer Ekle";
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
            return Json(new { Sonuc = true, Mesaj = "Kayıtlar Başarıyla Eklendi" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GelirGiderTuruSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            GelirGiderTurleri ggtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (ggtur == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            #region Gelir Gider Türünün kullanılıp kullanılmadığı kontrolü
            List<GelirGider> gg = dbContext.GelirGiders.Where(x => x.GelirGiderTurId == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            #endregion
            if (gg.Count > 0 || ggtur.KilitBit == true) // eğer gelir gider türü kullanılmış -> hata ver ve silme
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Gelir Gider Türü Sil, Gelir Gider Türü Kullanılıyor, Silinemez! Kayıt Id: " + id.ToString();
                hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Kayıt Kullanılıyor, Silinemez!", JsonRequestBehavior.AllowGet });
            }
            else // gelir gider türü kullanılmamış -> sil
            {
                ggtur.DegistirenKullaniciId = KullaniciId;
                ggtur.DegistirmeTarih = DateTime.Now;
                ggtur.Aktif = false;
                ggtur.Sil = true;
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
                    hata.Islem = "Gelir Gider Türü Sil, Kayıt Id: " + id.ToString();
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
        public string GelirGiderTuruDurumDegistir(long? id)
        {
            Models.GelirGiderTurleri gg = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (gg.Aktif == true)
                gg.Aktif = false;
            else
                gg.Aktif = true;
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
        public string GelirGiderTurVarMi()
        {
            string GelirGiderTuru = Request["GelirGider"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            GelirGiderTurleri ggtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.GelirGiderTur == GelirGiderTuru && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

            if (ggtur != null)
                return ggtur.Id.ToString();
            else
                return "Yok";
        }
        #endregion

        #region Günlük İş Kategorileri İşlemleri
        public ActionResult GunlukIsKategorileri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Günlük İş Kategorileri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 76 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 76 && x.Aktif == true && x.Sil == false);
            return View();
        }
        public ActionResult GunlukIsKategorileriListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<GunlukIsKategori> gunlukis = dbContext.GunlukIsKategoris.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var gilist = gunlukis.Select(m => new
            {
                Id = m.Id,
                KategoriAdi = m.KategoriAdi,
                Firma = m.Firma.FirmaAdi,
                KilitBit = m.KilitBit,
                Aktif = m.Aktif,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = gilist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GunlukIsKategorisiEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            string GunlukIsKategorisi = Request["GunlukIs"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            GunlukIsKategori gisvarmi = dbContext.GunlukIsKategoris.FirstOrDefault(x => x.FirmaId == FirmaId && x.KategoriAdi == GunlukIsKategorisi && x.Aktif == false && x.Sil == true);
            if (gisvarmi != null)
            {
                gisvarmi.Sil = false;
                gisvarmi.Aktif = true;
                gisvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                gisvarmi.DegistirmeTarih = DateTime.Now;

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
                    hata.Islem = "Günlük İş Kategorisi Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + gisvarmi.Id.ToString();
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
                GunlukIsKategori gik = new GunlukIsKategori();
                gik.FirmaId = FirmaId;  // Sessiondaki FirmaId
                gik.KategoriAdi = GunlukIsKategorisi;
                gik.Aktif = true;
                gik.Sil = false;
                gik.KilitBit = false;
                gik.OlusturanKullaniciId = KullaniciId;
                gik.OlusturmaTarih = DateTime.Now;
                gik.DegistirenKullaniciId = KullaniciId;
                gik.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.GunlukIsKategoris.Add(gik);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Günlük İş Kategorisi Ekle";
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
        public ActionResult GunlukIsKategorisiGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            string GunlukIs = Request["GunlukIs"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            GunlukIsKategori ggtur = dbContext.GunlukIsKategoris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);   // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak                                                                                             //ggtur.FirmaId = 1;  // Sessiondaki FirmaId
            if (ggtur == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            ggtur.KategoriAdi = GunlukIs;
            ggtur.DegistirenKullaniciId = KullaniciId;
            ggtur.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Günlük İş Kategorisi Güncelle, Kayıt Id: " + id.ToString();
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
        public ActionResult GunlukIsVarsayilanlariEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            string[] kategoriler = Request["VarsayilanDegerler"].Split(',');
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            GunlukIsKategori gik = new GunlukIsKategori();

            for (int i = 0; i < kategoriler.Length; i++)
            {
                string kategoriadi = kategoriler[i];
                GunlukIsKategori gisvarmi = dbContext.GunlukIsKategoris.FirstOrDefault(x => x.FirmaId == FirmaId && x.KategoriAdi == kategoriadi);
                if (gisvarmi != null)
                {
                    if (gisvarmi.Sil == true || gisvarmi.Aktif == false)
                    {
                        gisvarmi.Sil = false;
                        gisvarmi.Aktif = true;
                        gisvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                        gisvarmi.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Günlük İş Kategorisi Ekle, Varolan Kaydın güncellenmesi, Kayıt Id: " + gisvarmi.Id.ToString();
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
                else
                {
                    gik.FirmaId = FirmaId; // sessiondaki FirmaID
                    gik.KategoriAdi = kategoriler[i];
                    gik.OlusturanKullaniciId = KullaniciId;
                    gik.OlusturmaTarih = DateTime.Now;
                    gik.DegistirenKullaniciId = KullaniciId;
                    gik.DegistirmeTarih = DateTime.Now;
                    gik.Aktif = true;
                    gik.Sil = false;
                    gik.KilitBit = false;
                    try
                    {
                        dbContext.GunlukIsKategoris.Add(gik);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Gelir Gider Türü Ekle, Varsayılan Değer Ekle";
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
            return Json(new { Sonuc = true, Mesaj = "Kayıtlar Başarıyla Eklendi" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GunlukIsKategorisiSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<GunlukIsler> gis = dbContext.GunlukIslers.Where(x => x.KategoriId == id).ToList();
            if (gis.Count > 0) // eğer günlük iş katerorisi kullanılmış -> hata ver ve silme
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Günlük İş Kategorisi Sil, Günlük İş Kategorisi Kullanılıyor, Silinemez! Kayıt Id: " + id.ToString();
                hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Kayıt Kullanılıyor, Silinemez!", JsonRequestBehavior.AllowGet });
            }
            else  // günlük iş katerorisi kullanılmamış -> sil
            {
                GunlukIsKategori gg = dbContext.GunlukIsKategoris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
                if (gg == null)
                    return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
                gg.FirmaId = FirmaId; // sesiondaki firmaID
                gg.DegistirenKullaniciId = KullaniciId;
                gg.DegistirmeTarih = DateTime.Now;
                gg.Aktif = false;
                gg.Sil = true;
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public string GunlukIsKategorisiDurumDegistir(long? id)
        {
            Models.GunlukIsKategori kategori = dbContext.GunlukIsKategoris.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (kategori.Aktif == true)
                kategori.Aktif = false;
            else
                kategori.Aktif = true;
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
        public string GunlukIsVarMi()
        {
            string GunlukIs = Request["GunlukIs"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            GunlukIsKategori gis = dbContext.GunlukIsKategoris.FirstOrDefault(x => x.KategoriAdi == GunlukIs && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (gis != null)
                return gis.Id.ToString();
            else
                return "Yok";
        }
        #endregion

        #region Personel Görevleri İşlemleri
        public ActionResult PersonelGorevleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Personel Görevleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 77 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 77 && x.Aktif == true && x.Sil == false);

            return View();
        }
        public ActionResult PersonelGorevleriListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<PersonelGorevleri> pergor = dbContext.PersonelGorevleris.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var pglist = pergor.Select(m => new
            {
                Id = m.Id,
                Gorev = m.Gorev,
                Firma = m.Firma.FirmaAdi,
                KilitBit = m.KilitBit,
                Aktif = m.Aktif,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = pglist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PersonelGoreviEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            string Gorev = Request["Gorev"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            PersonelGorevleri gvarmi = dbContext.PersonelGorevleris.FirstOrDefault(x => x.Gorev == Gorev && x.FirmaId == FirmaId && x.Sil == true && x.Aktif == false);

            if (gvarmi != null) // Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
            {
                gvarmi.Sil = false;
                gvarmi.Aktif = true;
                gvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                gvarmi.DegistirmeTarih = DateTime.Now;

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
                    hata.Islem = "Personel Görevi Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + gvarmi.Id.ToString();
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
                PersonelGorevleri pergor = new PersonelGorevleri();
                pergor.FirmaId = FirmaId;  // Sessiondaki FirmaId
                pergor.Gorev = Gorev;
                pergor.Aktif = true;
                pergor.Sil = false;
                pergor.KilitBit = false;
                pergor.OlusturanKullaniciId = KullaniciId;
                pergor.OlusturmaTarih = DateTime.Now;
                pergor.DegistirenKullaniciId = KullaniciId;
                pergor.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.PersonelGorevleris.Add(pergor);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Personel Görevi Ekle";
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
        public ActionResult PersonelGoreviGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            string Gorev = Request["Gorev"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            PersonelGorevleri pgor = dbContext.PersonelGorevleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (pgor == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            pgor.Gorev = Gorev;
            pgor.DegistirenKullaniciId = KullaniciId;
            pgor.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Güncellendi" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Personel Görevi Güncelle,  Kayıt Id: " + id.ToString();
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
        public ActionResult GorevVarsayilanlariEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            string[] gorevler = Request["VarsayilanDegerler"].Split(',');
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            PersonelGorevleri pgor = new PersonelGorevleri();
            for (int i = 0; i < gorevler.Length; i++)
            {
                string pgorev = gorevler[i];
                PersonelGorevleri gorevvarmi = dbContext.PersonelGorevleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Gorev == pgorev);
                if (gorevvarmi != null)// Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
                {
                    if (gorevvarmi.Sil == true || gorevvarmi.Aktif == false)
                    {
                        gorevvarmi.Sil = false;
                        gorevvarmi.Aktif = true;
                        gorevvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                        gorevvarmi.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Personel Görevi Ekle, Varolan Kaydın güncellenmesi, Kayıt Id: " + gorevvarmi.Id.ToString();
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
                else
                {
                    pgor.FirmaId = FirmaId; // sessiondaki FirmaID
                    pgor.Gorev = gorevler[i];
                    pgor.OlusturanKullaniciId = KullaniciId;
                    pgor.OlusturmaTarih = DateTime.Now;
                    pgor.DegistirenKullaniciId = KullaniciId;
                    pgor.DegistirmeTarih = DateTime.Now;
                    pgor.Aktif = true;
                    pgor.Sil = false;
                    if (gorevler[i] == "Firma Sahibi" || gorevler[i] == "Şube Yetkilisi")
                        pgor.KilitBit = true;
                    else
                        pgor.KilitBit = false;
                    try
                    {
                        dbContext.PersonelGorevleris.Add(pgor);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Personel Görevi Ekle, Varsayılan Değer Ekle";
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
            return Json(new { Sonuc = true, Mesaj = "Kayıtlar Başarıyla Eklendi" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PersonelGoreviSil(long? id)
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                long FirmaId = Convert.ToInt64(Session["FirmaId"]);
                long KullaniciId = Convert.ToInt64(Session["Id"]);
                long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
                PersonelGorevleri pgor = dbContext.PersonelGorevleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
                if (pgor == null)
                    return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
                List<Personel> personel = dbContext.Personels.Where(x => x.GorevId == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == true).ToList();
                if (personel.Count > 0 || pgor.KilitBit == true) // Personel Görevi kullanılmış -> Silinemez
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Personel Görevi Sil, Personel Görevi Kullanılıyor, Silinemez! Kayıt Id: " + id.ToString();
                    hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Kayıt Kullanılıyor, Silinemez!", JsonRequestBehavior.AllowGet });
                }
                else // Personel görevi kullanılmamış -> Sil
                {
                    //pgor.FirmaId = 1; // sesiondaki firmaID
                    pgor.DegistirenKullaniciId = KullaniciId;
                    pgor.DegistirmeTarih = DateTime.Now;
                    pgor.Aktif = false;
                    pgor.Sil = true;
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
                        hata.Islem = "Personel Görevi Sil";
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
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public string PersonelGoreviDurumDegistir(long? id)
        {
            Models.PersonelGorevleri gorev = dbContext.PersonelGorevleris.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (gorev.Aktif == true)
                gorev.Aktif = false;
            else
                gorev.Aktif = true;
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
        public string PersonelGoreviVarMi()
        {
            string Gorev = Request["Gorev"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            PersonelGorevleri pgor = dbContext.PersonelGorevleris.FirstOrDefault(x => x.Gorev == Gorev && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (pgor != null)
                return pgor.Id.ToString();
            else
                return "Yok";
        }
        #endregion

        #region Rehber Grubu İşlemleri
        public ActionResult RehberGruplari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Rehber Grupları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 78 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 78 && x.Aktif == true && x.Sil == false);

            return View();
        }
        public ActionResult RehberGrupList()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<RehberGrup> rg = dbContext.RehberGrups.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var gruplist = rg.Select(m => new
            {
                Id = m.Id,
                GrupAdi = m.GrupAdi,
                Firma = m.Firma.FirmaAdi,
                KilitBit = m.KilitBit,
                Aktif = m.Aktif,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = gruplist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RehberGrubuEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            string GrupAdi = Request["GrupAdi"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            RehberGrup gvarmi = dbContext.RehberGrups.FirstOrDefault(x => x.GrupAdi == GrupAdi && x.FirmaId == FirmaId && x.Sil == true && x.Aktif == false);

            if (gvarmi != null) // Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
            {
                gvarmi.Sil = false;
                gvarmi.Aktif = true;
                gvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                gvarmi.DegistirmeTarih = DateTime.Now;

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
                    hata.Islem = "Rehber Grubu Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + gvarmi.Id.ToString();
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
                RehberGrup rehbergrup = new RehberGrup();
                rehbergrup.FirmaId = FirmaId;  // Sessiondaki FirmaId
                rehbergrup.GrupAdi = GrupAdi;
                rehbergrup.Aktif = true;
                rehbergrup.Sil = false;
                rehbergrup.KilitBit = false;
                rehbergrup.OlusturanKullaniciId = KullaniciId;
                rehbergrup.OlusturmaTarih = DateTime.Now;
                rehbergrup.DegistirenKullaniciId = KullaniciId;
                rehbergrup.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.RehberGrups.Add(rehbergrup);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Rehber Grubu Ekle";
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
        public ActionResult RehberGrubuGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            string GrupAdi = Request["GrupAdi"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            RehberGrup rehber = dbContext.RehberGrups.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (rehber == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            rehber.GrupAdi = GrupAdi;
            rehber.DegistirenKullaniciId = KullaniciId;
            rehber.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Güncellendi" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Rehber Grubu Güncelle, Kayıt Id: " + id.ToString();
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
        public string RehberGrubuVarMi()
        {
            string GrupAdi = Request["GrupAdi"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            RehberGrup rg = dbContext.RehberGrups.FirstOrDefault(x => x.GrupAdi == GrupAdi && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (rg != null)
                return rg.Id.ToString();
            else
                return "Yok";
        }
        [HttpPost]
        public string RehberGrubuDurumDegistir(long? id)
        {
            Models.RehberGrup grup = dbContext.RehberGrups.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (grup.Aktif == true)
                grup.Aktif = false;
            else
                grup.Aktif = true;
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
        public ActionResult RehberGrubuSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            List<TelefonRehberi> rehber = dbContext.TelefonRehberis.Where(x => x.RehberGrupId == id).ToList();
            if (rehber.Count > 0)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Rehber Grubu Sil, Rehber Grubu Kullanılıyor, Silinemez! Kayıt Id: " + id.ToString();
                hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Kayıt Kullanılıyor, Silinemez!", JsonRequestBehavior.AllowGet });
            }
            else
            {
                RehberGrup rhb = dbContext.RehberGrups.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
                if (rhb == null)
                    return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
                rhb.DegistirenKullaniciId = KullaniciId;
                rhb.DegistirmeTarih = DateTime.Now;
                rhb.Aktif = false;
                rhb.Sil = true;
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
                    hata.Islem = "Rehber Grubu Sil, Kayıt Id: " + id.ToString();
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
        
        #region Rezervasyon Ek Hizmetleri İşlemleri
        public ActionResult RezervasyonEkHizmetleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Rezervasyon Ek Hizmetleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 79 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 79 && x.Aktif == true && x.Sil == false);

            return View();
        }
        public ActionResult RezervasyonEkHizmetleriListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<RezervasyonEkHizmet> hizmetler = dbContext.RezervasyonEkHizmets.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var hizmetlist = hizmetler.Select(m => new
            {
                Id = m.Id,
                EkHizmetAdi = m.EkHizmetAdi,
                Aciklama = m.Aciklama,
                Fiyat = m.Fiyat,
                Firma = m.Firma.FirmaAdi,
                KilitBit = m.KilitBit,
                Aktif = m.Aktif,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = hizmetlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RezervasyonEkHizmetiEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            string EkHizmetAdi = Request["EkHizmetAdi"];
            string Aciklama = Request["Aciklama"];
            decimal Fiyat = Convert.ToDecimal(Request["Fiyat"]);
            string aciklama = Aciklama;
            aciklama = aciklama.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            aciklama = aciklama.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            RezervasyonEkHizmet hizmetvarmi = dbContext.RezervasyonEkHizmets.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == true && x.Aktif == false && x.EkHizmetAdi == EkHizmetAdi);

            if (hizmetvarmi != null) // Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
            {
                hizmetvarmi.Sil = false;
                hizmetvarmi.Aktif = true;
                hizmetvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                hizmetvarmi.DegistirmeTarih = DateTime.Now;
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
                    hata.Islem = "Rezervasyon Ek Hizmeti Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + hizmetvarmi.Id.ToString();
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
                RezervasyonEkHizmet ekhiz = new RezervasyonEkHizmet();
                ekhiz.FirmaId = FirmaId;  // Login olan kullanıcının firması
                ekhiz.EkHizmetAdi = EkHizmetAdi;
                ekhiz.Aciklama = aciklama;
                ekhiz.Fiyat = Fiyat;
                ekhiz.OlusturanKullaniciId = KullaniciId;
                ekhiz.OlusturmaTarih = DateTime.Now;
                ekhiz.DegistirenKullaniciId = KullaniciId;
                ekhiz.DegistirmeTarih = DateTime.Now;
                ekhiz.Aktif = true;
                ekhiz.Sil = false;
                try
                {
                    dbContext.RezervasyonEkHizmets.Add(ekhiz);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Rezervasyon Ek Hizmeti Ekle";
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
        public ActionResult RezervasyonEkHizmetiGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string EkHizmetAdi = Request["EkHizmetAdi"];
            string Aciklama = Request["Aciklama"];
            decimal Fiyat = Convert.ToDecimal(Request["Fiyat"]);
            string aciklama = Aciklama;
            aciklama = aciklama.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            aciklama = aciklama.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            RezervasyonEkHizmet ekhiz = dbContext.RezervasyonEkHizmets.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false); // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak
            if (ekhiz == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            ekhiz.EkHizmetAdi = EkHizmetAdi;
            ekhiz.Aciklama = aciklama;
            ekhiz.Fiyat = Fiyat;
            ekhiz.DegistirenKullaniciId = KullaniciId;
            ekhiz.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Rezervasyon Ek Hizmeti Güncelle, Kayıt Id: " + id.ToString();
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
        public ActionResult RezervasyonEkHizmetiSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            List<Sozlesme> sz = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            foreach (var item in sz)
            {
                int sonuc = item.EkHizmetlerId.IndexOf(id.ToString());
                if (sonuc != -1)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Rezervasyon Ek Hizmet Sil, Rezervasyon Ek Hizmeti Kullanılıyor, Silinemez! Kayıt Id: " + id.ToString();
                    hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Kayıt Kullanılıyor, Silinemez!", JsonRequestBehavior.AllowGet });
                }
            }
            RezervasyonEkHizmet ekhiz = dbContext.RezervasyonEkHizmets.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (ekhiz == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            ekhiz.DegistirenKullaniciId = KullaniciId;
            ekhiz.DegistirmeTarih = DateTime.Now;
            ekhiz.Aktif = false;
            ekhiz.Sil = true;
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
                hata.Islem = "Rezervasyon Ek Hizmet Sil, Kayıt Id: " + id.ToString();
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
        public string RezervasyonEkHizmetiDurumDegistir(long? id)
        {
            Models.RezervasyonEkHizmet ekhizmet = dbContext.RezervasyonEkHizmets.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (ekhizmet.Aktif == true)
                ekhizmet.Aktif = false;
            else
                ekhizmet.Aktif = true;
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
        public string RezervasyonEkHizmetiVarMi()
        {
            string EkHizmetAdi = Request["EkHizmetAdi"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            RezervasyonEkHizmet ekhz = dbContext.RezervasyonEkHizmets.FirstOrDefault(x => x.EkHizmetAdi == EkHizmetAdi && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (ekhz != null)
                return ekhz.Id.ToString();
            else
                return "Yok";
        }
        #endregion

        #region Rezervasyon Türleri İşlemleri
        public ActionResult RezervasyonTurleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Rezervasyon Türleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 80 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 80 && x.Aktif == true && x.Sil == false);

            return View();
        }
        public ActionResult RezervasyonTurleriListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<RezervasyonTurleri> reztur = dbContext.RezervasyonTurleris.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var rezlist = reztur.Select(m => new
            {
                Id = m.Id,
                RezervasyonTuru = m.RezervasyonTuru,
                Aciklama = m.Aciklama,
                Clas = m.RandevuGorunum,
                FormAlan = m.FormAlan,
                Firma = m.Firma.FirmaAdi,
                Aktif = m.Aktif,
                KilitBit = m.KilitBit,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = rezlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RezervasyonTurleriEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string RezervasyonTuru = Request["RezervasyonTuru"];
            string Clas = Request["Clas"];
            string FormAlan = Request["FormAlan"];
            string Aciklama = Request["Aciklama"];
            string aciklama = Aciklama;
            aciklama = aciklama.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            aciklama = aciklama.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            RezervasyonTurleri rezvarmi = dbContext.RezervasyonTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.RezervasyonTuru == RezervasyonTuru && x.Sil == true && x.Aktif == false);

            if (rezvarmi != null) // Eklenmek istenilen rezervasyon türü daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
            {
                rezvarmi.Sil = false;
                rezvarmi.Aktif = true;
                rezvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                rezvarmi.DegistirmeTarih = DateTime.Now;

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
                    hata.Islem = "Rezervasyon Türü Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + rezvarmi.Id.ToString();
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
                RandevuGorunum rg = new RandevuGorunum();
                rg.FirmaId = FirmaId;
                rg.Gorunum = Clas;
                rg.OlusturanKullaniciId = KullaniciId;
                rg.OlusturmaTarih = DateTime.Now;
                rg.DegistirenKullaniciId = KullaniciId;
                rg.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.RandevuGorunums.Add(rg);
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Randevu Görünümü Ekle";
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
                RezervasyonTurleri rzt = new RezervasyonTurleri();
                rzt.FirmaId = FirmaId; // Sessiondaki FirmaId
                rzt.RezervasyonTuru = RezervasyonTuru;
                rzt.RandevuGorunumId = rg.Id;
                rzt.RandevuGorunum = Clas;
                rzt.FormAlan = FormAlan;
                rzt.Aciklama = aciklama;
                rzt.KilitBit = false;
                rzt.Aktif = true;
                rzt.Sil = false;
                rzt.OlusturanKullaniciId = KullaniciId;
                rzt.OlusturmaTarih = DateTime.Now;
                rzt.DegistirenKullaniciId = KullaniciId;
                rzt.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.RezervasyonTurleris.Add(rzt);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Rezervasyon Türü Ekle";
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
        public ActionResult RezervasyonTurleriGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string RezervasyonTuru = Request["RezervasyonTuru"];
            string Clas = Request["Clas"];
            string FormAlan = Request["FormAlan"];
            string Aciklama = Request["Aciklama"];
            string aciklama = Aciklama;
            aciklama = aciklama.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            aciklama = aciklama.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            RezervasyonTurleri rzt = dbContext.RezervasyonTurleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId  && x.Sil == false);   // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak
            if (rzt == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });                                                                                                                                                     //RAndevu görünümde değişiklik var mı varsa güncellenecek
            RandevuGorunum rg = dbContext.RandevuGorunums.FirstOrDefault(x => x.Id == rzt.RandevuGorunumId);
            if (rg.Gorunum != Clas)
            {
                rg.Gorunum = Clas;
                rg.DegistirenKullaniciId = KullaniciId;
                rg.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Randevu Görünüm Güncelle";
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

            rzt.RezervasyonTuru = RezervasyonTuru;
            rzt.FormAlan = FormAlan;
            rzt.RandevuGorunum = Clas;
            rzt.Aciklama = aciklama;
            rzt.KilitBit = false;
            rzt.DegistirenKullaniciId = KullaniciId;
            rzt.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Rezervasyon Türü Güncelle";
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
        public ActionResult RezervasyonVarsayilanlariEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            RezervasyonTurleri reztur = new RezervasyonTurleri();
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string[] rzt = Request["VarsayilanDegerler"].Split(',');
            string[] frmalan = Request["VarsayilanAlanlar"].Split(',');
            for (int i = 0; i < rzt.Length; i++)
            {
                RandevuGorunum rg = new RandevuGorunum();
                string rezturu = rzt[i];
                RezervasyonTurleri rzvarmi = dbContext.RezervasyonTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.RezervasyonTuru == rezturu);
                if (rzvarmi != null)// Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
                {
                    if (rzvarmi.Aktif == false || rzvarmi.Sil == true)
                    {
                        rzvarmi.Sil = false;
                        rzvarmi.Aktif = true;
                        rzvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                        rzvarmi.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Rezervasyon Türü Varsayılan Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + rzvarmi.Id.ToString();
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
                else
                {
                    rg.Gorunum = "bg-info border-danger text-white";
                    rg.FirmaId = FirmaId;
                    rg.OlusturanKullaniciId = KullaniciId;
                    rg.OlusturmaTarih = DateTime.Now;
                    rg.DegistirenKullaniciId = KullaniciId;
                    rg.DegistirmeTarih = DateTime.Now;
                    rg.Aktif = true;
                    rg.Sil = false;
                    try
                    {
                        dbContext.RandevuGorunums.Add(rg);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Randevu Görünümü Varsayılan Ekle";
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

                    reztur.FirmaId = FirmaId; // sessiondaki FirmaID
                    reztur.RezervasyonTuru = rzt[i];
                    reztur.FormAlan = frmalan[i];
                    reztur.RandevuGorunumId = rg.Id;
                    reztur.RandevuGorunum = "bg-info border-danger text-white";
                    reztur.Aciklama = "Varsayılan Rezervasyon Türü";
                    reztur.OlusturanKullaniciId = KullaniciId;
                    reztur.OlusturmaTarih = DateTime.Now;
                    reztur.DegistirenKullaniciId = KullaniciId;
                    reztur.DegistirmeTarih = DateTime.Now;
                    reztur.Aktif = true;
                    reztur.Sil = false;
                    try
                    {
                        dbContext.RezervasyonTurleris.Add(reztur);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Rezervasyon Türü Varsayılan Ekle";
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
            return Json(new { Sonuc = true, Mesaj = "Kayıtlar Başarıyla Eklendi" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RezervasyonTurleriSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            List<Sozlesme> sz = dbContext.Sozlesmes.Where(x => x.RezervasyonTurId == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            if (sz.Count > 0)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Rezervasyon Türü Sil, Rezervasyon Türü Kullanılıyor, Silinemez! Kayıt Id: " + id.ToString();
                hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Kayıt Kullanılıyor, Silinemez!", JsonRequestBehavior.AllowGet });
            }
            else
            {
                RezervasyonTurleri reztur = dbContext.RezervasyonTurleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
                if (reztur == null)
                    return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
                reztur.DegistirenKullaniciId = KullaniciId;
                reztur.DegistirmeTarih = DateTime.Now;
                reztur.Aktif = false;
                reztur.Sil = true;
                RandevuGorunum rg = dbContext.RandevuGorunums.FirstOrDefault(x => x.Id == reztur.RandevuGorunumId);
                rg.DegistirenKullaniciId = KullaniciId;
                rg.DegistirmeTarih = DateTime.Now;
                rg.Aktif = false;
                rg.Sil = true;
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
                    hata.Islem = "Rezervasyon Türü Sil, Kayıt Id: " + id.ToString();
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
        public string RezervasyonTurleriDurumDegistir(long? id)
        {
            Models.RezervasyonTurleri tur = dbContext.RezervasyonTurleris.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (tur.Aktif == true)
                tur.Aktif = false;
            else
                tur.Aktif = true;
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
        public string RezervasyonTuruVarMi()
        {
            string RezervasyonTuru = Request["RezervasyonTuru"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            RezervasyonTurleri rzt = dbContext.RezervasyonTurleris.FirstOrDefault(x => x.RezervasyonTuru == RezervasyonTuru && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

            if (rzt != null)
                return rzt.Id.ToString();
            else
                return "Yok";

        }
        #endregion

        #region SMS Metinleri
        public ActionResult SMSMetinleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "SMS Metinleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 82 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 82 && x.Aktif == true && x.Sil == false);

            return View();
        }
        [HttpPost]
        public ActionResult SMSMetinListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<SmsMetinleri> smsmetinleri = dbContext.SmsMetinleris.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var smsmetinlist = smsmetinleri.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                MetinAdi = m.MetinAdi,
                Metin = m.SMSMetni,
                KilitBit = m.KilitBit,
                Aktif = m.Aktif,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = smsmetinlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SMSMetinEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string MetinAdi = Request["MetinAdi"];
            string Metin = Request["Metin"];
            string Durum = Request["Durum"];

            SmsMetinleri smsmetin = new SmsMetinleri();
            smsmetin.FirmaId = FirmaId;  // Sessiondaki FirmaId
            smsmetin.MetinAdi = MetinAdi;
            string metin = Metin;
            metin = metin.Replace("\r\n", ""); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            metin = metin.Replace("\n", ""); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            smsmetin.SMSMetni = metin;
            if (Durum == "Aktif")
                smsmetin.Aktif = true;
            else
                smsmetin.Aktif = false;
            smsmetin.Sil = false;
            smsmetin.KilitBit = false;
            smsmetin.OlusturanKullaniciId = KullaniciId;
            smsmetin.OlusturmaTarih = DateTime.Now;
            smsmetin.DegistirenKullaniciId = KullaniciId;
            smsmetin.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SmsMetinleris.Add(smsmetin);
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "SMS Metni Ekle";
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
        public ActionResult SMSMetinGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string MetinAdi = Request["MetinAdi"];
            string Metin = Request["Metin"];
            string Durum = Request["Durum"];
            Metin = Metin.Replace("\r\n", ""); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            Metin = Metin.Replace("\n", ""); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.

            SmsMetinleri smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id && x.Sil == false);
            if (smsmetin == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            smsmetin.MetinAdi = MetinAdi;
            smsmetin.SMSMetni = Metin;
            smsmetin.KilitBit = false;
            if (Durum == "Aktif")
                smsmetin.Aktif = true;
            else
                smsmetin.Aktif = false;
            smsmetin.DegistirenKullaniciId = KullaniciId;
            smsmetin.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "SMS Metni Güncelle. Kayıt Id:" + id;
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
        public ActionResult SMSMetinSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            List<AyarlarSmsGonderim> ayar = dbContext.AyarlarSmsGonderims.Where(x => x.CariAlacakHatirlatmaMesaji == id || x.CariTahsilatBilgiMesaji == id ||
                                            x.CariyeYapilanOdemeBilgiMesaji == id || x.EvlilikYildonumuTebrikMesaji == id || x.FotografSecimiBilgiMesajiFirma == id ||
                                            x.FotografSecimiBilgiMesajiMusteri == id || x.FotografSecimiHatirlatmaMesaji == id || x.GunlukIsOdemeBilgiMesaji == id ||
                                            x.MusteriCekimRandevusuBilgiMesaji == id || x.MusteriCekimRandevusuHatirlatmaMesaji == id || x.MusteriOdemeBilgiMesaji == id ||
                                            x.MusteriOdemeHatirlatmaMesaji == id || x.OpsiyonTarihiBilgiMesaji == id || x.OpsiyonTarihiHatirlatmaMesaji == id || x.PersonelRandevuBilgiMesaji == id ||
                                            x.PersonelRandevuHatirlatmaMesaji == id || x.RezervasyonTarihiBilgiMesaji == id || x.RezervasyonTarihiHatirlatmaMesaji == id).ToList();
            if (ayar.Count > 0)
                return Json(new { Sonuc = false, Mesaj = "Silmek istediğiniz kayıt kullanılıyor. SİLİNEMEZ", JsonRequestBehavior.AllowGet });

            SmsMetinleri sms = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id && x.Sil == false);
            if (sms == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            sms.DegistirenKullaniciId = KullaniciId;
            sms.DegistirmeTarih = DateTime.Now;
            sms.Aktif = false;
            sms.Sil = true;
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
                hata.Islem = "SMS Metni Sil, Kayıt Id: " + id.ToString();
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
        public string MetinAdiVarMi()
        {
            string MetinAdi = Request["MetinAdi"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            SmsMetinleri smsMetinAdivarmi = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.MetinAdi == MetinAdi && x.Sil == false);
            if (smsMetinAdivarmi != null)
                return smsMetinAdivarmi.Id.ToString();
            else
                return "Yok";
        }
        [HttpPost]
        public ActionResult SMSMetinDurumDegistir(long? id)
        {
            SmsMetinleri sms = dbContext.SmsMetinleris.FirstOrDefault(x => x.Id == id && x.Sil == false);
            List<AyarlarSmsGonderim> ayar = dbContext.AyarlarSmsGonderims.Where(x => x.CariAlacakHatirlatmaMesaji == id || x.CariTahsilatBilgiMesaji == id ||
                                          x.CariyeYapilanOdemeBilgiMesaji == id || x.EvlilikYildonumuTebrikMesaji == id || x.FotografSecimiBilgiMesajiFirma == id ||
                                          x.FotografSecimiBilgiMesajiMusteri == id || x.FotografSecimiHatirlatmaMesaji == id || x.GunlukIsOdemeBilgiMesaji == id ||
                                          x.MusteriCekimRandevusuBilgiMesaji == id || x.MusteriCekimRandevusuHatirlatmaMesaji == id || x.MusteriOdemeBilgiMesaji == id ||
                                          x.MusteriOdemeHatirlatmaMesaji == id || x.OpsiyonTarihiBilgiMesaji == id || x.OpsiyonTarihiHatirlatmaMesaji == id || x.PersonelRandevuBilgiMesaji == id ||
                                          x.PersonelRandevuHatirlatmaMesaji == id || x.RezervasyonTarihiBilgiMesaji == id || x.RezervasyonTarihiHatirlatmaMesaji == id).ToList();
            if (ayar.Count > 0)
                return Json(new { Sonuc = false, Mesaj = "Durumunu değiştirmek istediğiniz kayıt kullanılıyor. DEĞİŞTİRİLEMEZ", JsonRequestBehavior.AllowGet });

            if (sms.Aktif == true)
                sms.Aktif = false;
            else
                sms.Aktif = true;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Durum Değiştirildi", JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                return Json(new { Sonuc = false, Mesaj = "Durum değiştirilemedi.", JsonRequestBehavior.AllowGet });
            }
        }
        #endregion

        #region Email Metinleri
        public ActionResult EmailMetinleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Email Metinleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 81 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 81 && x.Aktif == true && x.Sil == false);

            return View();
        }
        public ActionResult EmailMetinleriListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<MailMetinleri> mailmetinleri = dbContext.MailMetinleris.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var mailmetinlist = mailmetinleri.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                MetinAdi = m.MetinAdi,
                EmailBaslik = m.MailBaslik,
                EmailKonu = m.MailKonu,
                Metin = m.MailMetni,
                IcerikResim = m.IcerikResim,
                Tema = m.TemaYol,
                Aktif = m.Aktif,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = mailmetinlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EmailMetinEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string MetinAdi = Request["MetinAdi"];
            string EmailKonu = Request["EmailKonu"];
            string EmailBaslik = Request["EmailBaslik"];
            string EmailMetni = Request["EmailMetni"];
            string Durum = Request["Durum"];
            string EmailTema = Request["EmailTema"];

            string ResimAdi = "", ResimYol = "", TamYol = "", ResimKonum_Vt = "", ResimAdres = "";
            #region Email İçerik Resmi ile ilgili işlemler
            if (Request.Files.Count > 0) // Kullanıcı Resmi seçilmişse
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    System.IO.FileInfo ff = new System.IO.FileInfo(file.FileName);
                    string uzanti = ff.Extension;

                    ResimAdi = resimisimlendir.resimisimlendir2(file, file.FileName.Replace(uzanti, ""));

                    if (file != null && file.ContentLength > 0)
                    {
                        //veri tabanında saklanacak resim yolu
                        ResimKonum_Vt = "/Areas/Otomasyon/Dosyalar/EmailIcerikResimleri/" + FirmaId + "/";
                        // Resmin kaydedileceği klasörü oluşturma
                        ResimYol = Server.MapPath(ResimKonum_Vt);
                        Directory.CreateDirectory(ResimYol);

                        TamYol = ResimYol + ResimAdi;
                        if (!System.IO.File.Exists(TamYol)) // Bu Resim Yok ise Kaydedilecek, Varsa birşey yapılmayacak
                        {
                            Image img = Image.FromStream(file.InputStream);
                            img.Save(TamYol);
                        }
                        ResimAdres = ResimKonum_Vt + ResimAdi;
                    }
                }
            }
            else
            {
                ResimAdres = "/Areas/Otomasyon/Dosyalar/EmailIcerikResimleri/Default/ResimYok.png";
            }

            #endregion

            MailMetinleri mailmetin = new MailMetinleri();
            mailmetin.FirmaId = FirmaId;  // Sessiondaki FirmaId
            mailmetin.MetinAdi = MetinAdi;
            mailmetin.IcerikResim = ResimAdres;
            mailmetin.MailBaslik = EmailBaslik;
            mailmetin.MailKonu = EmailKonu;
            mailmetin.MailMetni = EmailMetni;
            mailmetin.TemaYol = EmailTema;

            if (Durum == "Aktif")
                mailmetin.Aktif = true;
            else
                mailmetin.Aktif = false;
            mailmetin.Sil = false;
            mailmetin.OlusturanKullaniciId = KullaniciId;
            mailmetin.OlusturmaTarih = DateTime.Now;
            mailmetin.DegistirenKullaniciId = KullaniciId;
            mailmetin.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.MailMetinleris.Add(mailmetin);
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "SMS Metni Ekle";
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
        [ValidateInput(false)]
        public ActionResult EmailMetinGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string MetinAdi = Request["MetinAdi"];
            string EmailKonu = Request["EmailKonu"];
            string EmailBaslik = Request["EmailBaslik"];
            string EmailMetni = Request["EmailMetni"];
            string Durum = Request["Durum"];
            string EmailTema = Request["EmailTema"];

            MailMetinleri mailmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);

            string ResimAdi = "", ResimYol = "", TamYol = "", ResimKonum_Vt = "", ResimAdres = "";
            #region Email İçerik Resmi ile ilgili işlemler
            if (Request.Files.Count > 0) // Kullanıcı Resmi seçilmişse
            {
                //Resim seçilmiş ise eski resmi siliyorum
                if (System.IO.File.Exists(Server.MapPath(mailmetin.IcerikResim))) // bu resimden var mı?
                {
                    System.IO.File.Delete(Server.MapPath(mailmetin.IcerikResim)); // var olan resim siliniyor.
                }
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    System.IO.FileInfo ff = new System.IO.FileInfo(file.FileName);
                    string uzanti = ff.Extension;

                    ResimAdi = resimisimlendir.resimisimlendir2(file, file.FileName.Replace(uzanti, ""));

                    if (file != null && file.ContentLength > 0)
                    {
                        //veri tabanında saklanacak resim yolu
                        ResimKonum_Vt = "/Areas/Otomasyon/Dosyalar/EmailIcerikResimleri/" + FirmaId + "/";
                        // Resmin kaydedileceği klasörü oluşturma
                        ResimYol = Server.MapPath(ResimKonum_Vt);
                        Directory.CreateDirectory(ResimYol);

                        TamYol = ResimYol + ResimAdi;
                        if (!System.IO.File.Exists(TamYol)) // Bu Resim Yok ise Kaydedilecek, Varsa birşey yapılmayacak
                        {
                            Image img = Image.FromStream(file.InputStream);
                            img.Save(TamYol);
                        }
                        ResimAdres = ResimKonum_Vt + ResimAdi;
                    }
                }
            }
            else
            {
                ResimAdres = mailmetin.IcerikResim;
            }

            #endregion

            mailmetin.MetinAdi = MetinAdi;
            mailmetin.IcerikResim = ResimAdres;
            mailmetin.MailBaslik = EmailBaslik;
            mailmetin.MailKonu = EmailKonu;
            mailmetin.MailMetni = EmailMetni;
            mailmetin.TemaYol = EmailTema;

            if (Durum == "Aktif")
                mailmetin.Aktif = true;
            else
                mailmetin.Aktif = false;
            mailmetin.Sil = false;
            mailmetin.DegistirenKullaniciId = KullaniciId;
            mailmetin.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Mail Metni Güncelle. Kayıt Id:" + id;
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
        public ActionResult EmailMetinSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            List<AyarlarMailGonderim> ayar = dbContext.AyarlarMailGonderims.Where(x => x.CariAlacakHatirlatmaMaili == id || x.CariTahsilatBilgiMaili == id ||
                                            x.CariyeYapilanOdemeBilgiMaili == id || x.EvlilikYildonumuTebrikMaili == id || x.FotografSecimiBilgiMailiFirma == id ||
                                            x.FotografSecimiBilgiMailiMusteri == id || x.FotografSecimiHatirlatmaMaili == id || x.GunlukIsOdemeBilgiMaili == id ||
                                            x.MusteriCekimRandevusuBilgiMaili == id || x.MusteriCekimRandevusuHatirlatmaMaili == id || x.MusteriOdemeBilgiMaili == id ||
                                            x.MusteriOdemeHatirlatmaMaili == id || x.OpsiyonTarihiBilgiMaili == id || x.OpsiyonTarihiHatirlatmaMaili == id || x.PersonelRandevuBilgiMaili == id ||
                                            x.PersonelRandevuHatirlatmaMaili == id || x.RezervasyonTarihiBilgiMaili == id || x.RezervasyonTarihiHatirlatmaMaili == id).ToList();
            if (ayar.Count > 0)
                return Json(new { Sonuc = false, Mesaj = "Silmek istediğiniz kayıt kullanılıyor. SİLİNEMEZ", JsonRequestBehavior.AllowGet });

            MailMetinleri sms = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id && x.Sil == false);
            if (sms == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            sms.DegistirenKullaniciId = KullaniciId;
            sms.DegistirmeTarih = DateTime.Now;
            sms.Aktif = false;
            sms.Sil = true;
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
                hata.Islem = "Mail Metni Sil, Kayıt Id: " + id.ToString();
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
        public ActionResult EmailMetinDurumDegistir(long? id)
        {
            MailMetinleri mail = dbContext.MailMetinleris.FirstOrDefault(x => x.Id == id && x.Sil == false);
            List<AyarlarMailGonderim> ayar = dbContext.AyarlarMailGonderims.Where(x => x.CariAlacakHatirlatmaMaili == id || x.CariTahsilatBilgiMaili == id ||
                                            x.CariyeYapilanOdemeBilgiMaili == id || x.EvlilikYildonumuTebrikMaili == id || x.FotografSecimiBilgiMailiFirma == id ||
                                            x.FotografSecimiBilgiMailiMusteri == id || x.FotografSecimiHatirlatmaMaili == id || x.GunlukIsOdemeBilgiMaili == id ||
                                            x.MusteriCekimRandevusuBilgiMaili == id || x.MusteriCekimRandevusuHatirlatmaMaili == id || x.MusteriOdemeBilgiMaili == id ||
                                            x.MusteriOdemeHatirlatmaMaili == id || x.OpsiyonTarihiBilgiMaili == id || x.OpsiyonTarihiHatirlatmaMaili == id || x.PersonelRandevuBilgiMaili == id ||
                                            x.PersonelRandevuHatirlatmaMaili == id || x.RezervasyonTarihiBilgiMaili == id || x.RezervasyonTarihiHatirlatmaMaili == id).ToList();
            if (ayar.Count > 0)
                return Json(new { Sonuc = false, Mesaj = "Durumunu değiştirmek istediğiniz kayıt kullanılıyor. DEĞİŞTİRİLEMEZ", JsonRequestBehavior.AllowGet });

            if (mail.Aktif == true)
                mail.Aktif = false;
            else
                mail.Aktif = true;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Durum Değiştirildi", JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                return Json(new { Sonuc = false, Mesaj = "Durum değiştirilemedi.", JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public string MailMetinAdiVarMi()
        {
            string MetinAdi = Request["MetinAdi"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            MailMetinleri mailMetinAdivarmi = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.MetinAdi == MetinAdi && x.Sil == false);
            if (mailMetinAdivarmi != null)
                return mailMetinAdivarmi.Id.ToString();
            else
                return "Yok";
        }
        #endregion

        #region Sözleşme Şartları İşlemleri
        public ActionResult SozlesmeSartlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Sözleşme Şartları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 83 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<SozlesmeSartlari> ss = dbContext.SozlesmeSartlaris.Where(x => x.FirmaId == FirmaId && x.Aktif == true).ToList();
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 83 && x.Aktif == true && x.Sil == false);

            return View(ss);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SozlesmeSartiGuncelle(SozlesmeSartlari sozs)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            SozlesmeSartlari ss = dbContext.SozlesmeSartlaris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == sozs.Id && x.Aktif == true && x.Sil == false);
            ss.SozlesmeSartlari1 = sozs.SozlesmeSartlari1;
            ss.DegistirenKullaniciId = KullaniciId;
            ss.DegistirmeTarih = DateTime.Now;
           
            try
            {
                dbContext.SaveChanges();
                return RedirectToAction("SozlesmeSartlari");
                //return Json(new { Sonuc = true, Mesaj = "Güncelleme Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Sözleşme şartları güncelle";
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

        #region Süreçler İşlemleri
        public ActionResult Surecler()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Süreçler";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 84 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 84 && x.Aktif == true && x.Sil == false);

            return View();
        }
        public ActionResult SureclerListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<Surecler> surecler = dbContext.Sureclers.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var sureclist = surecler.Select(m => new
            {
                Id = m.Id,
                SurecAdi = m.SurecAdi,
                Firma = m.Firma.FirmaAdi,
                SMSBildirim = m.SMSBildirim,
                Sira = m.Sira,
                KilitBit = m.KilitBit,
                Aktif = m.Aktif,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = sureclist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SurecEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string Surec = Request["Surec"];
            int Sira = Convert.ToInt32(Request["Sira"]);
            string sms = Request["SMSBildir"];
            bool smsbildir;
            if (sms == "aktif")
                smsbildir = true;
            else
                smsbildir = false;

            Surecler surecvarmi = dbContext.Sureclers.FirstOrDefault(x => x.FirmaId == FirmaId && x.SurecAdi == Surec && x.Sil == true && x.Aktif == false);

            if (surecvarmi != null) // Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
            {
                surecvarmi.SMSBildirim = smsbildir;
                surecvarmi.Sil = false;
                surecvarmi.Aktif = true;
                surecvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                surecvarmi.DegistirmeTarih = DateTime.Now;

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
                    hata.Islem = "Süreç Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + surecvarmi.Id.ToString();
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
                Surecler sur = new Models.Surecler();
                sur.FirmaId = FirmaId;  // Sessiondaki FirmaId
                sur.SurecAdi = Surec;
                sur.SMSBildirim = smsbildir;
                sur.Sira = Sira;
                sur.KilitBit = false;
                sur.Aktif = true;
                sur.Sil = false;
                sur.OlusturanKullaniciId = KullaniciId;
                sur.OlusturmaTarih = DateTime.Now;
                sur.DegistirenKullaniciId = KullaniciId;
                sur.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.Sureclers.Add(sur);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Süreç Ekle";
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
        public ActionResult SurecGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string Surec = Request["Surec"];
            int Sira = Convert.ToInt32(Request["Sira"]);
            string sms = Request["SMSBildir"];
            bool smsbildir;
            if (sms == "aktif")
                smsbildir = true;
            else
                smsbildir = false;

            Surecler sr = dbContext.Sureclers.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);   // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak
            if (sr == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            sr.SurecAdi = Surec;
            sr.SMSBildirim = smsbildir;
            sr.Sira = Sira;
            sr.DegistirenKullaniciId = KullaniciId;
            sr.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Süreç Güncelle, Kayıt Id: " + id.ToString();
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
        public string SurecVarMi()
        {
            string Surec = Request["Surec"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            Surecler sr = dbContext.Sureclers.FirstOrDefault(x => x.SurecAdi == Surec && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

            if (sr != null)
                return sr.Id.ToString();
            else
                return "Yok";
        }
        [HttpPost]
        public ActionResult SurecVarsayilanlariEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            string[] surecler = Request["VarsayilanDegerler"].Split(',');
            int sira = SiraGetir();
            Surecler su = new Surecler();
            for (int i = 0; i < surecler.Length; i++)
            {
                string surec = surecler[i];
                Surecler srvarmi = dbContext.Sureclers.FirstOrDefault(x => x.FirmaId == FirmaId && x.SurecAdi == surec);
                if (srvarmi != null)// Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
                {
                    if (srvarmi.Sil == true || srvarmi.Aktif == false)
                    {
                        srvarmi.Sil = false;
                        srvarmi.Aktif = true;
                        srvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                        srvarmi.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Süreç Varsayılan Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + srvarmi.Id.ToString();
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
                else
                {
                    su.FirmaId = FirmaId; // sessiondaki FirmaID
                    su.SurecAdi = surecler[i];
                    su.SMSBildirim = false;
                    su.Sira = sira;
                    su.OlusturanKullaniciId = KullaniciId;
                    su.OlusturmaTarih = DateTime.Now;
                    su.DegistirenKullaniciId = KullaniciId;
                    su.DegistirmeTarih = DateTime.Now;
                    su.Aktif = true;
                    su.Sil = false;
                    try
                    {
                        dbContext.Sureclers.Add(su);
                        dbContext.SaveChanges();
                        sira++;
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Süreç Varsayılan Ekle";
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
            return Json(new { Sonuc = true, Mesaj = "Kayıtlar Başarıyla Eklendi" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SurecSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            List<Sozlesme> sz = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            foreach (var sozlesme in sz)
            {
                string[] sureclerId = sozlesme.SureclerId.Split(',');
                if (sureclerId.Length>0)
                {
                    for (int i = 0; i < sureclerId.Length; i++)
                    {
                        if (id==Convert.ToInt64(sureclerId[i]))
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Rezervasyon Türü Sil, Rezervasyon Türü Kullanılıyor, Silinemez! Kayıt Id: " + id.ToString();
                            hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                            hata.OlusturanKullaniciId = 1;
                            hata.OlusturmaTarih = DateTime.Now;
                            hata.DegistirenKullaniciId = 1;
                            hata.DegistirmeTarih = DateTime.Now;
                            hata.Aktif = true;
                            hata.Sil = false;
                            dbContext.HataLoglaris.Add(hata);
                            dbContext.SaveChanges();
                            return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Kayıt Kullanılıyor, Silinemez!", JsonRequestBehavior.AllowGet });
                        }
                    }
                }
            }
            Surecler sr = dbContext.Sureclers.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (sr == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            sr.Sira = 0;
            sr.DegistirenKullaniciId = KullaniciId;
            sr.DegistirmeTarih = DateTime.Now;
            sr.Aktif = false;
            sr.Sil = true;
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
                hata.Islem = "Süreç Sil, Kayıt Id: " + id.ToString();
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
        public ActionResult SMSBildirimDegistir(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            Surecler kl = dbContext.Sureclers.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (kl == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            if (kl.SMSBildirim == true)
                kl.SMSBildirim = false;
            else
                kl.SMSBildirim = true;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Güncellendi" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Süreç SMSBildirim Değiştir, Kayıt Id: " + id.ToString();
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
        public string SurecDurumDegistir(long? id)
        {
            Models.Surecler surec = dbContext.Sureclers.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (surec.Aktif == true)
                surec.Aktif = false;
            else
                surec.Aktif = true;
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
        public ActionResult SurecTabloSiraGuncelle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string[] Siralar = Request["Siralar"].Split(',');
            string[] Idler = Request["Idler"].Split(',');
            int Sira;
            long Id;
            for (int i = 0; i < Siralar.Length; i++)
            {
                Sira = Convert.ToInt32(Siralar[i]);
                Id = Convert.ToInt64(Idler[i]);
                Surecler sr = dbContext.Sureclers.FirstOrDefault(x => x.Id == Id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);   // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak

                sr.Sira = Sira;
                sr.DegistirenKullaniciId = KullaniciId;
                sr.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Süreç Sıralarını Tablodan Günceller";
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
            return Json(new { Sonuc = true, Mesaj = "Tüm Sıralar Güncellendi" }, JsonRequestBehavior.AllowGet);
        }
        public int SiraGetir()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            int sira;
            List<Surecler> surec = dbContext.Sureclers.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            if (surec.Count > 0)
                sira = Convert.ToInt32(surec.Max(x => x.Sira));
            else
                sira = 0;
            return sira + 1;
        }
        #endregion

        #region Tatil Günleri İşlemleri
        public ActionResult TatilGunleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Tatil Günleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 85 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 85 && x.Aktif == true && x.Sil == false);

            return View();
        }
        public ActionResult TatilGunleriListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<TatilGunleri> tatil = dbContext.TatilGunleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            var tatillist = tatil.Select(m => new
            {
                Id = m.Id,
                Baslangic = m.Baslangic,
                Bitis = m.Bitis,
                Aciklama = m.Aciklama,
                Firma = m.Firma.FirmaAdi,
                Aktif = m.Aktif,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = tatillist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult TatilGunuEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            string Baslangic = Request["Baslangic"];
            string Bitis = Request["Bitis"];
            string Aciklama = Request["Aciklama"];
            string aciklama = Aciklama;
            aciklama = aciklama.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            aciklama = aciklama.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            DateTime bas = Convert.ToDateTime(Baslangic);
            DateTime bit = Convert.ToDateTime(Bitis);
            TatilGunleri tatilvarmi = dbContext.TatilGunleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Baslangic == bas && x.Bitis == bit && x.Sil == true && x.Aktif == false);
            if (tatilvarmi != null) // Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
            {
                tatilvarmi.Sil = false;
                tatilvarmi.Aktif = true;
                tatilvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                tatilvarmi.DegistirmeTarih = DateTime.Now;

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
                    hata.Islem = "Tatil Günü Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + tatilvarmi.Id.ToString();
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
                TatilGunleri tg = new TatilGunleri();
                tg.FirmaId = FirmaId;  // Sessiondaki FirmaId
                tg.Baslangic = bas;
                tg.Bitis = bit;
                tg.Aciklama = aciklama;
                tg.Aktif = true;
                tg.Sil = false;
                tg.OlusturanKullaniciId = KullaniciId;
                tg.OlusturmaTarih = DateTime.Now;
                tg.DegistirenKullaniciId = KullaniciId;
                tg.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.TatilGunleris.Add(tg);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Tatil Günü Ekle";
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
        public ActionResult TatilGunuGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string Baslangic = Request["Baslangic"];
            string Bitis = Request["Bitis"];
            string Aciklama = Request["Aciklama"];
            string aciklama = Aciklama;
            aciklama = aciklama.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            aciklama = aciklama.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            DateTime bas = Convert.ToDateTime(Baslangic);
            DateTime bit = Convert.ToDateTime(Bitis);
            TatilGunleri tatil = dbContext.TatilGunleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);   // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak
            if (tatil == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });                                                                                                                                            //tatil.FirmaId = 1;  // Sessiondaki FirmaId
            tatil.Baslangic = bas;
            tatil.Bitis = bit;
            tatil.Aciklama = aciklama;
            tatil.DegistirenKullaniciId = KullaniciId;
            tatil.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Tatil Günü Güncelle, Kayıt Id: " + id.ToString();
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
        public string TatilGunuVarMi()
        {
            string Baslangic = Request["Baslangic"];
            string Bitis = Request["Bitis"];
            string Aciklama = Request["Aciklama"];
            string aciklama = Aciklama;
            aciklama = aciklama.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            aciklama = aciklama.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            DateTime bas = Convert.ToDateTime(Baslangic);
            DateTime bit = Convert.ToDateTime(Bitis);
            TatilGunleri tt = dbContext.TatilGunleris.FirstOrDefault(x => x.Baslangic == bas && x.Bitis == bit && x.Aciklama == aciklama && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

            if (tt != null)
                return tt.Id.ToString();
            else
                return "Yok";
        }
        [HttpPost]
        public ActionResult TatilGunuSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            TatilGunleri tt = dbContext.TatilGunleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (tt == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            tt.DegistirenKullaniciId = KullaniciId;
            tt.DegistirmeTarih = DateTime.Now;
            tt.Aktif = false;
            tt.Sil = true;
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
                hata.Islem = "Tatil Günü Sil, Kayıt Id: " + id.ToString();
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

        //Unvanlar KULLANILMAYACAK
        #region Unvanlar İşlemleri
        public ActionResult Unvanlar()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                ViewBag.UstMenu = "Tanımlar";
                ViewBag.AltMenu = "Unvanlar";
                long FirmaId = Convert.ToInt64(Session["FirmaId"]);
                long KullaniciId = Convert.ToInt64(Session["Id"]);

                #region Sayfaya Erişim Yetkisi Kotrolü
                KullaniciYetki yetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == KullaniciId && x.SayfaId == 51);
                if (yetki.SayfaYetki == true) // Kullanıcı Bu sayfaya yetkili değilse hata sayfasına yönlendir.
                {
                    List<KullaniciYetki> ky = dbContext.KullaniciYetkis.Where(x => x.FirmaId == FirmaId && x.KullaniciId == KullaniciId).ToList();
                    ViewBag.KullaniciYetkileri = ky;
                    ViewBag.Yetki = yetki;
                    KullaniciYetki yetkilendir = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == KullaniciId && x.SayfaId == 51);
                    ViewBag.Yetkilendir = yetkilendir;
                    ViewBag.kullaniciId = KullaniciId;

                    return View();
                }
                else // Kullanıcı Bu sayfaya yetkili ise işelemlere devam
                {
                    return RedirectToAction("YetkisizGiris", "Hata");
                }
                #endregion
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        public ActionResult UnvanlarListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Unvanlar> unvanlar;
            if (KullaniciId == 1)
                unvanlar = dbContext.Unvanlars.Select(x => x).ToList();
            else
                unvanlar = dbContext.Unvanlars.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            var unvanlist = unvanlar.Select(m => new { Id = m.Id, Unvan = m.Unvan, Firma = m.Firma.FirmaAdi, Aktif = m.Aktif, Sil = m.Sil }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = unvanlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UnvanEkle()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                string Unvan = Request["Unvan"];
                long FirmaId = Convert.ToInt64(Session["FirmaId"]);
                long KullaniciId = Convert.ToInt64(Session["Id"]);
                Unvanlar unvanvarmi;
                if (KullaniciId == 1)
                    unvanvarmi = dbContext.Unvanlars.FirstOrDefault(x => x.Unvan == Unvan && x.Sil == true && x.Aktif == false);
                else
                    unvanvarmi = dbContext.Unvanlars.FirstOrDefault(x => x.FirmaId == FirmaId && x.Unvan == Unvan && x.Sil == true && x.Aktif == false);
                if (unvanvarmi != null) // Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
                {
                    unvanvarmi.Sil = false;
                    unvanvarmi.Aktif = true;
                    unvanvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                    unvanvarmi.DegistirmeTarih = DateTime.Now;

                    try
                    {
                        dbContext.SaveChanges();
                        List<Unvanlar> unvanlar = dbContext.Unvanlars.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
                        var unvanlist = unvanlar.Select(m => new { Id = m.Id, Unvan = m.Unvan }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
                        return Json(new { Sonuc = true, data = unvanlist, Mesaj = "Kayıt başarılı" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu:" + e, JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    Models.Unvanlar unvan = new Models.Unvanlar();
                    unvan.FirmaId = FirmaId;  // Sessiondaki FirmaId
                    unvan.Unvan = Unvan;
                    unvan.Aktif = true;
                    unvan.Sil = false;
                    unvan.OlusturanKullaniciId = KullaniciId;
                    unvan.OlusturmaTarih = DateTime.Now;
                    unvan.DegistirenKullaniciId = KullaniciId;
                    unvan.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.Unvanlars.Add(unvan);
                        dbContext.SaveChanges();
                        List<Models.Unvanlar> unvanlar = dbContext.Unvanlars.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
                        var unvanlist = unvanlar.Select(m => new { Id = m.Id, Unvan = m.Unvan }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
                        return Json(new { Sonuc = true, data = unvanlist, Mesaj = "Kayıt başarılı" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu:" + e, JsonRequestBehavior.AllowGet });
                    }
                }
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult UnvanGuncelle(long id)
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                string Unvan = Request["Unvan"];
                long FirmaId = Convert.ToInt64(Session["FirmaId"]);
                long KullaniciId = Convert.ToInt64(Session["Id"]);
                Unvanlar unv;
                if (KullaniciId == 1)
                    unv = dbContext.Unvanlars.FirstOrDefault(x => x.Id == id);   // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak
                else
                    unv = dbContext.Unvanlars.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId);   // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak

                unv.Unvan = Unvan;
                //unv.Aktif = true;
                //unv.Sil = false;
                unv.DegistirenKullaniciId = KullaniciId;
                unv.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                    List<Unvanlar> unvanlar = dbContext.Unvanlars.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
                    var unvanlist = unvanlar.Select(m => new { Id = m.Id, Unvan = m.Unvan }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
                    return Json(new { Sonuc = true, data = unvanlist, Mesaj = "Kayıt başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu:" + e, JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult UnvanVarsayilanlariEkle()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                string[] unvanlar = { "Firma Sahibi", "Genel Müdür", "Müdür", "Şef" };
                long FirmaId = Convert.ToInt64(Session["FirmaId"]);
                long KullaniciId = Convert.ToInt64(Session["Id"]);
                Models.Unvanlar unvan = new Models.Unvanlar();
                for (int i = 0; i < unvanlar.Length; i++)
                {
                    string unv = unvanlar[i];
                    Models.Unvanlar unvvarmi = dbContext.Unvanlars.FirstOrDefault(x => x.FirmaId == FirmaId && x.Unvan == unv);
                    if (unvvarmi != null)// Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
                    {
                        if (unvvarmi.Sil == true || unvvarmi.Aktif == false)
                        {
                            unvvarmi.Sil = false;
                            unvvarmi.Aktif = true;
                            unvvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                            unvvarmi.DegistirmeTarih = DateTime.Now;
                            try
                            {
                                dbContext.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu:" + e, JsonRequestBehavior.AllowGet });
                            }
                        }
                    }
                    else
                    {
                        unvan.FirmaId = FirmaId; // sessiondaki FirmaID
                        unvan.Unvan = unvanlar[i];
                        unvan.OlusturanKullaniciId = KullaniciId;
                        unvan.OlusturmaTarih = DateTime.Now;
                        unvan.DegistirenKullaniciId = KullaniciId;
                        unvan.DegistirmeTarih = DateTime.Now;
                        unvan.Aktif = true;
                        unvan.Sil = false;
                        try
                        {
                            dbContext.Unvanlars.Add(unvan);
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu:" + e, JsonRequestBehavior.AllowGet });
                        }
                    }
                }
                List<Models.Unvanlar> unvs = dbContext.Unvanlars.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
                var unvanlist = unvs.Select(m => new { Id = m.Id, Unvan = m.Unvan }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
                return Json(new { Sonuc = true, data = unvanlist, Mesaj = "Kayıt başarılı" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult UnvanSil(long id)
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                long FirmaId = Convert.ToInt64(Session["FirmaId"]);
                long KullaniciId = Convert.ToInt64(Session["Id"]);
                Unvanlar unvan;
                if (KullaniciId == 1)
                    unvan = dbContext.Unvanlars.FirstOrDefault(x => x.Id == id);
                else
                    unvan = dbContext.Unvanlars.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId);
                unvan.DegistirenKullaniciId = KullaniciId;
                unvan.DegistirmeTarih = DateTime.Now;
                unvan.Aktif = false;
                unvan.Sil = true;
                dbContext.SaveChanges();
                List<Unvanlar> unvanlar = dbContext.Unvanlars.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
                var unvanlist = unvanlar.Select(m => new { Id = m.Id, Unvan = m.Unvan }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
                return Json(new { Sonuc = true, data = unvanlist, Mesaj = "Kayıt başarılı" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public string UnvanVarMi()
        {
            string Unvan = Request["Unvan"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            Unvanlar un;
            if (KullaniciId == 1)
                un = dbContext.Unvanlars.FirstOrDefault(x => x.Unvan == Unvan && x.Aktif == true && x.Sil == false);
            else
                un = dbContext.Unvanlars.FirstOrDefault(x => x.Unvan == Unvan && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

            if (un != null)
            {
                return un.Id.ToString();
            }
            else
            {
                return "Yok";
            }

        }
        [HttpPost]
        public string UnvanDurumDegistir(long id)
        {
            Unvanlar kl = dbContext.Unvanlars.FirstOrDefault(x => x.Id == id);
            if (kl.Aktif == true)
            {
                kl.Aktif = false;
            }
            else
            {
                kl.Aktif = true;
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
        }
        [HttpPost]
        public string UnvanSilDegistir(long id)
        {
            Unvanlar kl = dbContext.Unvanlars.FirstOrDefault(x => x.Id == id);
            if (kl.Sil == true)
            {
                kl.Sil = false;
            }
            else
            {
                kl.Sil = true;
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
        }
        #endregion
        //Unvanlar KULLANILMAYACAK

        #region Zaman Dilimleri İşlemleri
        public ActionResult ZamanDilimleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Tanımlar";
            ViewBag.AltMenu = "Zaman Dilimleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 86 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 86 && x.Aktif == true && x.Sil == false);

            return View();
        }
        public ActionResult ZamanDilimleriListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<ZamanDilimleri> zmn = dbContext.ZamanDilimleris.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();

            var zmnlist = zmn.Select(m => new
            {
                Id = m.Id,
                BaslangicZaman = m.BaslangicZaman,
                BitisZaman = m.BitisZaman,
                Firma = m.Firma.FirmaAdi,
                Aktif = m.Aktif,
                Sil = m.Sil
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = zmnlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ZamanDilimiEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string baslangicZamani = Request["Baslangic"];
            string bitisZamani = Request["Bitis"];
            TimeSpan baslangic = TimeSpan.Parse(baslangicZamani);
            TimeSpan bitis = TimeSpan.Parse(bitisZamani);
            ZamanDilimleri zmnvarmi = dbContext.ZamanDilimleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.BaslangicZaman == baslangic && x.BitisZaman == bitis && x.Aktif == false && x.Sil == true);

            if (zmnvarmi != null) // Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
            {
                zmnvarmi.Sil = false;
                zmnvarmi.Aktif = true;
                zmnvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                zmnvarmi.DegistirmeTarih = DateTime.Now;
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
                    hata.Islem = "Zaman Dilimi Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + zmnvarmi.Id.ToString();
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
                ZamanDilimleri zmn = new ZamanDilimleri();
                zmn.FirmaId = FirmaId; // Sessiondaki FirmaId
                zmn.BaslangicZaman = baslangic;
                zmn.BitisZaman = bitis;
                zmn.Aktif = true;
                zmn.Sil = false;
                zmn.OlusturanKullaniciId = KullaniciId;
                zmn.OlusturmaTarih = DateTime.Now;
                zmn.DegistirenKullaniciId = KullaniciId;
                zmn.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.ZamanDilimleris.Add(zmn);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Zaman Dilimi Ekle";
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
        public ActionResult ZamanDilimiGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            string baslangicZamani = Request["Baslangic"];
            string bitisZamani = Request["Bitis"];
            TimeSpan baslangic = TimeSpan.Parse(baslangicZamani);
            TimeSpan bitis = TimeSpan.Parse(bitisZamani);
            ZamanDilimleri zmn = dbContext.ZamanDilimleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);   // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak
            if (zmn == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            zmn.BaslangicZaman = baslangic;
            zmn.BitisZaman = bitis;
            zmn.DegistirenKullaniciId = KullaniciId;
            zmn.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Zaman Dilimi Güncelle, Kayıt Id: " + id.ToString();
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
        public ActionResult ZamanVarsayilanlariEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            ZamanDilimleri zmn = new ZamanDilimleri();
            string[] zamanbaslangic = Request["VarsayilanBaslangic"].Split(',');
            string[] zamanbitis = Request["VarsayilanBitis"].Split(',');
            for (int i = 0; i < zamanbaslangic.Length; i++)
            {
                string baslangicZamani = zamanbaslangic[i];
                string bitisZamani = zamanbitis[i];
                TimeSpan baslangic = TimeSpan.Parse(baslangicZamani);
                TimeSpan bitis = TimeSpan.Parse(bitisZamani);
                ZamanDilimleri zmnvarmi = dbContext.ZamanDilimleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.BaslangicZaman == baslangic && x.BitisZaman == bitis);
                if (zmnvarmi != null) // Eklenmek istenilen bu firmaya ait kayıt daha önceden var ise (silinmiş yada pasife alınmış), kayıt eklenmeyip varolan kayıt aktif yapılıyor.
                {
                    if (zmnvarmi.Sil == true || zmnvarmi.Aktif == false)
                    {
                        zmnvarmi.Sil = false;
                        zmnvarmi.Aktif = true;
                        zmnvarmi.DegistirenKullaniciId = KullaniciId; // Sesiondaki kullanıcı
                        zmnvarmi.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Zaman Dilimi Varsayılan Ekle, Silinmiş Kayıt Güncellenirken oluşan hata. Kayıt Id: " + zmnvarmi.Id.ToString();
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
                else
                {
                    zmn.FirmaId = FirmaId; // sessiondaki FirmaID
                    zmn.BaslangicZaman = baslangic;
                    zmn.BitisZaman = bitis;
                    zmn.OlusturanKullaniciId = KullaniciId;
                    zmn.OlusturmaTarih = DateTime.Now;
                    zmn.DegistirenKullaniciId = KullaniciId;
                    zmn.DegistirmeTarih = DateTime.Now;
                    zmn.Aktif = true;
                    zmn.Sil = false;
                    try
                    {
                        dbContext.ZamanDilimleris.Add(zmn);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Zaman Dilimi Varsayılan Ekle";
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
            return Json(new { Sonuc = true, Mesaj = "Kayıtlar Başarıyla Eklendi" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ZamanDilimiSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            ZamanDilimleri zaman = dbContext.ZamanDilimleris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (zaman == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            zaman.DegistirenKullaniciId = KullaniciId;
            zaman.DegistirmeTarih = DateTime.Now;
            zaman.Aktif = false;
            zaman.Sil = true;
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
                hata.Islem = "Zaman Dilimi Sil, Kayıt Id: " + id.ToString();
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
        public string ZamanDilimiDurumDegistir(long? id)
        {
            Models.ZamanDilimleri zaman = dbContext.ZamanDilimleris.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (zaman.Aktif == true)
                zaman.Aktif = false;
            else
                zaman.Aktif = true;
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
        public string ZamanDilimiVarMi()
        {
            string baslangicZamani = Request["Baslangic"];
            string bitisZamani = Request["Bitis"];
            TimeSpan baslangic = TimeSpan.Parse(baslangicZamani);
            TimeSpan bitis = TimeSpan.Parse(bitisZamani);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            ZamanDilimleri zmnvarmi = dbContext.ZamanDilimleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.BaslangicZaman == baslangic && x.BitisZaman == bitis && x.Sil == false && x.Aktif == true);

            if (zmnvarmi != null)
                return zmnvarmi.Id.ToString();
            else
                return "Yok";
        }
        #endregion
    }
}