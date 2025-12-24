using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FotografciTakipWeb.Models;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class RaporlarController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/Raporlar
        public ActionResult RezervasyonRaporlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Rezervasyon Raporları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 53 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }

        public ActionResult RezervasyonIslemAdetleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Rezervasyon İşlem Adetleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 54 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sozlesme> Rezervasyon = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year).OrderBy(x => x.SozlesmeTarihi).ToList();

            return View(Rezervasyon);
        }
        [HttpPost]
        public ActionResult RezervasyonIslemAdetleriRapor()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Rezervasyon İşlem Adetleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<Sozlesme> Rezervasyon;
            if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                Rezervasyon = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year).OrderBy(x => x.SozlesmeTarihi).ToList();
            else
                Rezervasyon = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year).OrderBy(x => x.SozlesmeTarihi).ToList();

            //if (id == 0) // Yıllık
            //    Rezervasyon = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year).OrderBy(x => x.SozlesmeTarihi).ToList();
            //else if (id != 0) // Yılın ayları
            //    Rezervasyon = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year && x.SozlesmeTarihi.Month == id).OrderBy(x => x.SozlesmeTarihi).ToList();

            var Rezervasyon_Tur = Rezervasyon.GroupBy(c => c.SozlesmeTarihi.Month).Select(g => new
            {
                Ay = g.Key,
                KesinRezervasyon = (g.Where(a => a.KesinRezervasyonBit == true).Count()),
                RezevasyonTeklif = (g.Where(a => a.TeklifBit == true).Count()),
                OpsiyonluRezervasyon = (g.Where(a => a.OpsiyonBit == true).Count())
            }).ToList();

            return Json(new { Rezervasyon_Tur, JsonRequestBehavior.AllowGet });
        }
        public ActionResult GelirGiderRaporlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Gelir-Gider Raporları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 55 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<GelirGider> gelir;
            List<GelirGider> gider;
            gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == "Gelir" && x.Tarih.Year == DateTime.Now.Year && x.Aktif == true && x.Sil == false).ToList();
            gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == "Gider" && x.Tarih.Year == DateTime.Now.Year && x.Aktif == true && x.Sil == false).ToList();

            decimal toplamgelir = gelir.Sum(x => x.Tutar);
            decimal toplamgider = gider.Sum(x => x.Tutar);
            decimal kasa = toplamgelir - toplamgider;

            ViewBag.ToplamGelir = toplamgelir;
            ViewBag.ToplamGider = toplamgider;
            ViewBag.Kazanc = kasa;
            List<GelirGider> Tutar = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.Tarih.Year == DateTime.Now.Year).OrderBy(x => x.Tarih).ToList();
            return View(Tutar);
        }
        [HttpPost]
        public ActionResult GelirGiderRapor()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Gelir-Gider Raporları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<GelirGider> Tutar;
            if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                Tutar = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.Tarih.Year == DateTime.Now.Year).OrderBy(x => x.Tarih).ToList();
            else
                Tutar = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && x.Tarih.Year == DateTime.Now.Year).OrderBy(x => x.Tarih).ToList();

            var GelirGider = Tutar.GroupBy(c => c.Tarih.Month).Select(g => new
            {
                Ay = g.Key,
                Gelir = (g.Where(a => a.Tip == "Gelir").Sum(x => x.Tutar)),
                Gider = (g.Where(a => a.Tip == "Gider").Sum(x => x.Tutar)),
                Kazanç = (g.Where(a => a.Tip == "Gelir").Sum(x => x.Tutar) - g.Where(a => a.Tip == "Gider").Sum(x => x.Tutar))
            }).ToList();

            return Json(new { GelirGider, JsonRequestBehavior.AllowGet });
        }

        public ActionResult RezervasyonTurleriRaporu()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Rezervasyon Türleri Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 56 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }
        [HttpPost]
        public ActionResult RezervasyonTurleriRapor(long id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Rezervasyon Türleri Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<Sozlesme> RezervasyonTurleri = null;
            if (id == 0) // Yıllık
            {
                if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                    RezervasyonTurleri = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year).OrderBy(x => x.SozlesmeTarihi).ToList();
                else
                    RezervasyonTurleri = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year).OrderBy(x => x.SozlesmeTarihi).ToList();
            }
            else if (id != 0) // Yılın ayları
            {
                if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                    RezervasyonTurleri = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year && x.SozlesmeTarihi.Month == id).OrderBy(x => x.SozlesmeTarihi).ToList();
                else
                    RezervasyonTurleri = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year && x.SozlesmeTarihi.Month == id).OrderBy(x => x.SozlesmeTarihi).ToList();
            }
            var Rezervasyon_Tur = RezervasyonTurleri.GroupBy(c => c.RezervasyonTurleri.RezervasyonTuru).Select(g => new
            {
                TurAdi = g.Key,
                Toplam = g.Count()
            }).ToList();

            return Json(new { Rezervasyon_Tur, JsonRequestBehavior.AllowGet });
        }
        public ActionResult GelecekOdemelerRaporu()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Gelecek Ödemeler Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Odemeler> Tutar = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == "GelecekOdeme" && x.Iptal == false && x.OdemeAl == false && x.Aktif == true && x.Sil == false && x.Tarih.Year == DateTime.Now.Year).OrderBy(x => x.Tarih).ToList();
            ViewBag.ToplamGelecekOdeme = Tutar.Sum(x => x.Tutar);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 57 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View(Tutar);
        }
        [HttpPost]
        public ActionResult GelecekOdemelerRapor()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Gelecek Ödemeler Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<Odemeler> Tutar;
            if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                Tutar = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == "GelecekOdeme" && x.Iptal == false && x.OdemeAl == false && x.Aktif == true && x.Sil == false && x.Tarih.Year == DateTime.Now.Year).OrderBy(x => x.Tarih).ToList();
            else
                Tutar = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.OdemeTuru == "GelecekOdeme" && x.Iptal == false && x.OdemeAl == false && x.Aktif == true && x.Sil == false && x.Tarih.Year == DateTime.Now.Year).OrderBy(x => x.Tarih).ToList();

            var GelecekOdemeler = Tutar.GroupBy(c => c.Tarih.Month).Select(g => new
            {
                Ay = g.Key,
                Tutar = (g.Sum(x => x.Tutar)),
            }).ToList();

            return Json(new { GelecekOdemeler, JsonRequestBehavior.AllowGet });
        }
        public ActionResult HaftalikRapor()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Haftalık Rapor";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 58 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }
        public ActionResult EkHizmetlerRaporu()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Rezervasyon Ek Hizmetler Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 59 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }
        [HttpPost]
        public ActionResult EkHizmetlerRapor(long id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Rezervasyon Ek Hizmetler Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<Sozlesme> sozlesmes = null;
            List<RezervasyonEkHizmet> ekhizmetlerlistesi = new List<RezervasyonEkHizmet>();
            //ArrayList ekhizmetIdler = new ArrayList();
            List<string> ekhizmetIdler = new List<string>();
            if (id == 0) // Yıllık
            {
                if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                    sozlesmes = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year).OrderBy(x => x.SozlesmeTarihi).ToList();
                else
                    sozlesmes = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year).OrderBy(x => x.SozlesmeTarihi).ToList();
            }
            else if (id != 0) // Yılın ayları
            {
                if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                    sozlesmes = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year && x.SozlesmeTarihi.Month == id).OrderBy(x => x.SozlesmeTarihi).ToList();
                else
                    sozlesmes = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year && x.SozlesmeTarihi.Month == id).OrderBy(x => x.SozlesmeTarihi).ToList();
            }
            if (sozlesmes != null)
            {
                foreach (var item in sozlesmes)
                {
                    if (!string.IsNullOrEmpty(item.EkHizmetlerId))
                        ekhizmetIdler.Add(item.EkHizmetlerId);
                }
            }
            if (ekhizmetIdler.Count > 0)
            {
                foreach (var item in ekhizmetIdler)
                {
                    long ekhimetId = 0;
                    string[] ayri = item.Split(',');
                    foreach (var a in ayri)
                    {
                        ekhimetId = Convert.ToInt64(a);
                        ekhizmetlerlistesi.Add(dbContext.RezervasyonEkHizmets.FirstOrDefault(x => x.Id == ekhimetId));
                    }
                }
            }
            var Ek_Hizmet = ekhizmetlerlistesi.GroupBy(c => c.EkHizmetAdi).Select(g => new
            {
                TurAdi = g.Key,
                Toplam = g.Count()
            }).ToList();

            return Json(new { Ek_Hizmet, JsonRequestBehavior.AllowGet });
        }

        public ActionResult PaketlerRaporu()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Rezervasyon Paketleri Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 53 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            return View();
        }
        [HttpPost]
        public ActionResult PaketlerRapor(long id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Rezervasyon Paketleri Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<Sozlesme> sozlesmes = null;
            List<CekimPaketleri> paketlerlistesi = new List<CekimPaketleri>();
            //ArrayList ekhizmetIdler = new ArrayList();
            List<string> peketIdler = new List<string>();
            if (id == 0) // Yıllık
            {
                if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                    sozlesmes = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year).OrderBy(x => x.SozlesmeTarihi).ToList();
                else
                    sozlesmes = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year).OrderBy(x => x.SozlesmeTarihi).ToList();
            }
            else if (id != 0) // Yılın ayları
            {
                if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                    sozlesmes = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year && x.SozlesmeTarihi.Month == id).OrderBy(x => x.SozlesmeTarihi).ToList();
                else
                    sozlesmes = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && x.SozlesmeTarihi.Year == DateTime.Now.Year && x.SozlesmeTarihi.Month == id).OrderBy(x => x.SozlesmeTarihi).ToList();
            }
            if (sozlesmes != null)
            {
                foreach (var item in sozlesmes)
                {
                    if (!string.IsNullOrEmpty(item.PaketlerId))
                        peketIdler.Add(item.PaketlerId);
                }
            }
            if (peketIdler.Count > 0)
            {
                foreach (var item in peketIdler)
                {
                    long paketId = 0;
                    string[] ayri = item.Split(',');
                    foreach (var a in ayri)
                    {
                        paketId = Convert.ToInt64(a);
                        paketlerlistesi.Add(dbContext.CekimPaketleris.FirstOrDefault(x => x.Id == paketId));
                    }
                }
            }
            var Paketler = paketlerlistesi.GroupBy(c => c.PaketAdi).Select(g => new
            {
                TurAdi = g.Key,
                Toplam = g.Count()
            }).ToList();

            return Json(new { Paketler, JsonRequestBehavior.AllowGet });
        }

        public ActionResult PersonelPerformansRaporu()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Personel Performans Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 55 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            else
                return View();
        }

        [HttpPost]
        public ActionResult PersonelPerformansRapor()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Personel Performans Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<Randevu> Rezervasyon;
            List<RandevuToPersonel> randevuToPersonels = new List<RandevuToPersonel>();
            List<Personel> PersonelListesi = dbContext.Personels.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.PersonelListesi = PersonelListesi;

            if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                Rezervasyon = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.Baslangic.Year == DateTime.Now.Year).OrderBy(x => x.Baslangic).ToList();
            else
                Rezervasyon = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && x.Baslangic.Year == DateTime.Now.Year).OrderBy(x => x.Baslangic).ToList();

            foreach (var randevu in Rezervasyon)
            {
                randevuToPersonels.AddRange(dbContext.RandevuToPersonels.Where(x => x.RandevuId == randevu.Id));
            }

            var personel = randevuToPersonels.GroupBy(c => c.Personel.AdiSoyadi).Select(g => new
            {
                PersonelAdSoyad = g.Key,
                RandevuSayisi = (g.Where(a => a.Iptal == false && a.Aktif == true).Count()),
            }).ToList();

            return Json(new { personel, JsonRequestBehavior.AllowGet });
        }

        public ActionResult PersonelPerformansRaporTablo()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Raporlar";
            ViewBag.AltMenu = "Personel Performans Raporu";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<Randevu> Rezervasyon;
            List<RandevuToPersonel> randevuToPersonels = new List<RandevuToPersonel>();
            List<Personel> PersonelListesi = dbContext.Personels.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.PersonelListesi = PersonelListesi;

            if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                Rezervasyon = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.Baslangic.Year == DateTime.Now.Year).OrderBy(x => x.Baslangic).ToList();
            else
                Rezervasyon = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && x.Baslangic.Year == DateTime.Now.Year).OrderBy(x => x.Baslangic).ToList();

            foreach (var randevu in Rezervasyon)
            {
                randevuToPersonels.AddRange(dbContext.RandevuToPersonels.Where(x => x.RandevuId == randevu.Id));
            }

            var personel = randevuToPersonels.GroupBy(c => c.Personel.AdiSoyadi).Select(g => new
            {
                PersonelAdSoyad = g.Key,
                RandevuSayisi = (g.Where(a => a.Iptal == false && a.Aktif == true).Count())
            }).ToList();
            return Json(new { data = personel }, JsonRequestBehavior.AllowGet);
        }
    }
}