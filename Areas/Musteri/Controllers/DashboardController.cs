using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FotografciTakipWeb.Models;
using Rotativa;
using FotografciTakipWeb.App_Settings;
using System.IO;
using System.Net.Mail;

namespace FotografciTakipWeb.Areas.Musteri.Controllers
{
    public class DashboardController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Musteri/Dashboard
        public ActionResult Index()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Dashboard";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                //Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                ViewBag.MusteriYetkileri = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);
                return View();
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        #region Rezervasyonlarım Sayfası
        public ActionResult Rezervasyonlarim()
        {
                if (Session.Count == 0)
                    return RedirectToAction("GirisYap", "Giris");

                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Rezervasyonlarım";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                //Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                ViewBag.MusteriYetkileri = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);

                List<Sozlesme> sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.MusteriId == MusteriId && x.TeklifBit == false && x.OpsiyonBit == false && x.Aktif == true && x.Sil == false).ToList();

                return View(sozlesmeler);
        }
        [HttpPost]
        public ActionResult RezervasyonlarListesi()
        {
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int MusteriId = Convert.ToInt32(Session["MusteriId"]);
            List<Sozlesme> sozlesme;
            //Müşteri Id si olmayan Sözleşmeler silinecek

            sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.MusteriId == MusteriId && x.TeklifBit == false && x.OpsiyonBit == false && x.Aktif == true && x.Sil == false).ToList();

            var sz = sozlesme.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                SozlesmeNo = m.SozlesmeNo,
                SozlesmeTarihi = m.SozlesmeTarihi.ToShortDateString(),
                RezervasyonTarihi = m.RezervasyonTarihi.ToShortDateString(),
                OpsiyonTarihi = Convert.ToDateTime(m.OpsiyonTarihi).ToShortDateString(),
                RezervasyonTur = m.RezervasyonTurleri.RezervasyonTuru,
                YetkiliPersonel = m.YetkiliPersonel,
                //GorevliPersonel = m.Personel.AdiSoyadi,
                //GorevliPersoneller=m.Randevus.Where(x=>x.SozlesmeId==m.Id)
                //.Select(r=> new {
                //    Rnd=r.RandevuToPersonels.Where(rp => rp.RandevuId == r.Id).ToList().Select(y => new
                //    {
                //        pers = y.Personel.AdiSoyadi,
                //        gorev = y.Personel.PersonelGorevleri.Gorev
                //    }).ToList(),
                //}),
                BaslangicSaat = m.BaslangicSaat.ToString(@"hh\:mm"),
                BitisSaat = m.BitisSaat.ToString(@"hh\:mm"),
                YetkiliAdSoyad = m.YetkiliAdSoyad,
                YetkiliTel = m.YetkiliTel,
                YetkiliEmail = m.YetkiliEmail,
                AnneAd = m.AnneAd,
                AnneTel = m.AnneTel,
                AnneEmail = m.AnneEmail,
                BabaAd = m.BabaAd,
                BabaTel = m.BabaTel,
                BabaEmail = m.BabaEmail,
                CocukAdSoyad = m.CocukAdSoyad,
                Modeller = m.Modeller,
                Urunler = m.Urunler,
                GelinAd = m.GelinAd,
                GelinTel = m.GelinTel,
                GelinEmail = m.GelinEmail,
                DamatAd = m.DamatAd,
                DamatTel = m.DamatTel,
                DamatEmail = m.DamatEmail,
                TeklifBit = m.TeklifBit,
                PaketlerId = m.PaketlerId,
                EkHizmetlerId = m.EkHizmetlerId,
                Paketler = m.Paketler,
                EkHizmetler = m.EkHizmetler,
                PeketlerFiyat = m.PaketlerFiyat,
                EkHizmetlerFiyat = m.EkHizmetlerFiyat,
                SureclerId = m.SureclerId,
                Surecler = m.Surecler,
                SozlesmeTutar = m.ToplamFiyat,
                Iskonto = m.Iskonto,
                OrganizasyonYeri = m.OrganizasyonYeri,
                Notlar = m.Notlar,
                MusteriId = m.MusteriId,
                MusteriKodu = m.Musteri.MusteriKodu,
                MusteriAdiSoyadi = m.Musteri.AdiSoyadi,
                MusteriTC = m.Musteri.TCKimlikNo,
                MusteriSabitTel = m.Musteri.SabitTel,
                MusteriCepTel = m.Musteri.CepTel,
                MusteriEmail = m.Musteri.Email,
                MusteriAdres = m.Musteri.Adres,
                MusteriNotlar = m.Musteri.Notlar,
                KilitBit = m.KilitBit,
                Bitti = m.Bitti,
                Iptal = m.Iptal,
                OpsiyonBit = m.OpsiyonBit,
                Aciklama = m.Notlar,
                Durum = m.Durum,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            }).OrderByDescending(x => x.SozlesmeNo).ToList();

            return Json(new { data = sz }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SozlesmeRandevulari(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            int MusteriId = Convert.ToInt32(Session["MusteriId"]);

            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.Aktif == true && x.Sil == false).ToList();
            var randevular = randevu.Select(m => new
            {
                Id = m.Id,
                Tarih = m.Baslangic.ToShortDateString(),
                Baslangic = m.Baslangic.ToShortTimeString(),
                Bitis = m.Bitis.ToShortTimeString(),
                Aciklama = m.Aciklama,
                CekimRandevu = m.CekimRandevu,
                Personel = m.RandevuToPersonels.Where(x => x.RandevuId == m.Id).ToList().Select(y => new
                {
                    pers = y.Personel.AdiSoyadi,
                    gorev = y.Personel.PersonelGorevleri.Gorev
                }).ToList(),
                Iptal = m.Iptal
            }).ToList(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { Sonuc = true, data = randevular }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Rezervasyon Teklifleri Sayfası
        public ActionResult RezervasyonTekliflerim()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Rezervasyon Tekliflerim";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                //Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                ViewBag.MusteriYetkileri = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);
                return View();
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult RezervasyonTeklifleriListesi()
        {
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int MusteriId = Convert.ToInt32(Session["MusteriId"]);
            List<Sozlesme> sozlesme;
            //Müşteri Id si olmayan Sözleşmeler silinecek

            sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.MusteriId == MusteriId && (x.TeklifBit == true || x.OpsiyonBit == true) && x.Aktif == true && x.Sil == false).ToList();
            var sz = sozlesme.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                SozlesmeNo = m.SozlesmeNo,
                SozlesmeTarihi = m.SozlesmeTarihi.ToShortDateString(),
                RezervasyonTarihi = m.RezervasyonTarihi.ToShortDateString(),
                OpsiyonTarihi = Convert.ToDateTime(m.OpsiyonTarihi).ToShortDateString(),
                RezervasyonTur = m.RezervasyonTurleri.RezervasyonTuru,
                YetkiliPersonel = m.YetkiliPersonel,
                //GorevliPersonel = m.Personel.AdiSoyadi,
                BaslangicSaat = m.BaslangicSaat.ToString(@"hh\:mm"),
                BitisSaat = m.BitisSaat.ToString(@"hh\:mm"),
                YetkiliAdSoyad = m.YetkiliAdSoyad,
                YetkiliTel = m.YetkiliTel,
                YetkiliEmail = m.YetkiliEmail,
                AnneAd = m.AnneAd,
                AnneTel = m.AnneTel,
                AnneEmail = m.AnneEmail,
                BabaAd = m.BabaAd,
                BabaTel = m.BabaTel,
                BabaEmail = m.BabaEmail,
                CocukAdSoyad = m.CocukAdSoyad,
                Modeller = m.Modeller,
                Urunler = m.Urunler,
                GelinAd = m.GelinAd,
                GelinTel = m.GelinTel,
                GelinEmail = m.GelinEmail,
                DamatAd = m.DamatAd,
                DamatTel = m.DamatTel,
                DamatEmail = m.DamatEmail,
                TeklifBit = m.TeklifBit,
                PaketlerId = m.PaketlerId,
                EkHizmetlerId = m.EkHizmetlerId,
                Paketler = m.Paketler,
                EkHizmetler = m.EkHizmetler,
                PeketlerFiyat = m.PaketlerFiyat,
                EkHizmetlerFiyat = m.EkHizmetlerFiyat,
                SureclerId = m.SureclerId,
                Surecler = m.Surecler,
                SozlesmeTutar = m.ToplamFiyat,
                Iskonto = m.Iskonto,
                OrganizasyonYeri = m.OrganizasyonYeri,
                Notlar = m.Notlar,
                MusteriId = m.MusteriId,
                MusteriKodu = m.Musteri.MusteriKodu,
                MusteriAdiSoyadi = m.Musteri.AdiSoyadi,
                MusteriTC = m.Musteri.TCKimlikNo,
                MusteriSabitTel = m.Musteri.SabitTel,
                MusteriCepTel = m.Musteri.CepTel,
                MusteriEmail = m.Musteri.Email,
                MusteriAdres = m.Musteri.Adres,
                MusteriNotlar = m.Musteri.Notlar,
                KilitBit = m.KilitBit,
                Bitti = m.Bitti,
                Iptal = m.Iptal,
                OpsiyonBit = m.OpsiyonBit,
                Aciklama = m.Notlar,
                Durum = m.Durum,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            }).OrderByDescending(x => x.SozlesmeNo).ToList();
            return Json(new { data = sz }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Sözleşme Yazdırma İşlemleri
        public ActionResult RezervasyonOnizleme(int id)
        {
            //int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            //int KullaniciId = Convert.ToInt32(Session["Id"]);
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.SozlesmeNo == id);
            ViewBag.SozlesmeAyar = dbContext.AyarlarSozlesmeCiktis.FirstOrDefault(x => x.FirmaId == sz.FirmaId);

            List<Randevu> rnd = dbContext.Randevus.Where(x => x.SozlesmeId == sz.Id && x.Aktif==true && x.Sil==false).ToList();
            ViewBag.CekimRandevulari = rnd;

            ViewBag.SozlesmeMetni = dbContext.SozlesmeSartlaris.FirstOrDefault(x => x.FirmaId == sz.FirmaId).SozlesmeSartlari1;
            //List<OdemelerAlinan> alinan = dbContext.OdemelerAlinans.Where(x => x.SozlesmeId == sz.Id).ToList();
            List<Odemeler> alinan = dbContext.Odemelers.Where(x => x.SozlesmeId == sz.Id && x.OdemeTuru == "AlinanOdeme" && x.GelecekOdemeID == null).ToList();
            List<Odemeler> gelecek = dbContext.Odemelers.Where(x => x.SozlesmeId == sz.Id && x.OdemeTuru == "GelecekOdeme").ToList();
            ViewBag.alinanodemeler = alinan;
            ViewBag.kalanodemeler = gelecek;
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == sz.FirmaId);
            ViewBag.Firma = frm;
            return View(sz);
        }
        public ActionResult SozlesmeYazdir(int id)
        {
            // id sözleşme numarası yada sözleşme id
            var p = new ActionAsPdf("RezervasyonOnizleme", new { id = id })
            {
                CustomSwitches = "--page-offset 0 --footer-center [page]/[toPage] --footer-font-size 10",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 }// ayarlar sayfasından alınabilir
            };
            return p;
        }
        #endregion
        #region Ödemelerim Sayfası
        public ActionResult Odemeler()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Ödemelerim";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                //Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                ViewBag.MusteriYetkileri = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);

                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.MusteriId == MusteriId && x.Iptal == false && x.Aktif == true && x.Sil == false).ToList();

                return View(odemeler);
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult OdemelerListesi()
        {
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int MusteriId = Convert.ToInt32(Session["MusteriId"]);
            List<Odemeler> odeme = new List<Odemeler>();
            //Müşteri Id si olmayan Sözleşmeler silinecek
            //var o = dbContext.Odemelers.Where(x => x.MusteriId == MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false)
            //   .Join(dbContext.Musteris, odeme => odeme.MusteriId, musteri => musteri.Id, (odeme, musteri) => new { odeme, musteri })
            //   .Select(m => new
            //   {
            //       Id = m.odeme.Id,
            //       FirmaAdi = m.odeme.Firma.FirmaAdi,
            //       GisNo = m.odeme.GunlukIsler.TakipNo,
            //       OdemeTuru = m.odeme.OdemeTuru,
            //       IslemTarihi = m.odeme.Tarih.ToShortDateString(),
            //       OdemeTarihi = m.odeme.OdemeTarihi.ToShortDateString(),
            //       OdemeYapan = m.odeme.OdemeYapanAdSoyad,
            //       Tutar = m.odeme.Tutar,
            //       OdemeSekli = m.odeme.OdemeSekli,
            //       OdemeAl = m.odeme.OdemeAl,
            //       AlinanOdemeMakbuzNo = m.odeme.AlinanOdemeMakbuzNo,
            //       Notlar = m.odeme.Notlar,
            //       // Sözleşme Detayı İçin
            //       SozlesmeId = m.odeme.SozlesmeId,
            //       SozlesmeNo = m.odeme.Sozlesme.SozlesmeNo,
            //       SozlesmeFirmaAdi = m.odeme.Sozlesme.Firma.FirmaAdi,
            //       SozlesmeTarihi = m.odeme.Sozlesme.SozlesmeTarihi.ToShortDateString(),
            //       OpsiyonTarihi = Convert.ToDateTime(m.odeme.Sozlesme.OpsiyonTarihi).ToShortDateString(),
            //       RezervasyonTur = m.odeme.Sozlesme.RezervasyonTurleri.RezervasyonTuru,
            //       YetkiliPersonel = m.odeme.Sozlesme.YetkiliPersonel,
            //       GorevliPersonel = m.odeme.Sozlesme.Personel.AdiSoyadi,
            //       BaslangicSaat = m.odeme.Sozlesme.BaslangicSaat.ToString(),
            //       BitisSaat = m.odeme.Sozlesme.BitisSaat.ToString(),
            //       YetkiliAdSoyad = m.odeme.Sozlesme.YetkiliAdSoyad,
            //       YetkiliTel = m.odeme.Sozlesme.YetkiliTel,
            //       YetkiliEmail = m.odeme.Sozlesme.YetkiliEmail,
            //       AnneAd = m.odeme.Sozlesme.AnneAd,
            //       AnneTel = m.odeme.Sozlesme.AnneTel,
            //       AnneEmail = m.odeme.Sozlesme.AnneEmail,
            //       BabaAd = m.odeme.Sozlesme.BabaAd,
            //       BabaTel = m.odeme.Sozlesme.BabaTel,
            //       BabaEmail = m.odeme.Sozlesme.BabaEmail,
            //       CocukAdSoyad = m.odeme.Sozlesme.CocukAdSoyad,
            //       Modeller = m.odeme.Sozlesme.Modeller,
            //       Urunler = m.odeme.Sozlesme.Urunler,
            //       GelinAd = m.odeme.Sozlesme.GelinAd,
            //       GelinTel = m.odeme.Sozlesme.GelinTel,
            //       GelinEmail = m.odeme.Sozlesme.GelinEmail,
            //       DamatAd = m.odeme.Sozlesme.DamatAd,
            //       DamatTel = m.odeme.Sozlesme.DamatTel,
            //       DamatEmail = m.odeme.Sozlesme.DamatEmail,
            //       TeklifBit = m.odeme.Sozlesme.TeklifBit,
            //       Paketler = m.odeme.Sozlesme.Paketler,
            //       EkHizmetler = m.odeme.Sozlesme.EkHizmetler,
            //       PeketlerFiyat = m.odeme.Sozlesme.PaketlerFiyat,
            //       EkHizmetlerFiyat = m.odeme.Sozlesme.EkHizmetlerFiyat,
            //       Surecler = m.odeme.Sozlesme.Surecler,
            //       SozlesmeTutar = m.odeme.Sozlesme.ToplamFiyat,
            //       OrganizasyonYeri = m.odeme.Sozlesme.OrganizasyonYeri,
            //       SozlesmeNotlar = m.odeme.Sozlesme.Notlar,
            //       Bitti = m.odeme.Sozlesme.Bitti,
            //       SozlesmeIptal = m.odeme.Sozlesme.Iptal,
            //       Durum = m.odeme.Sozlesme.Durum,
            //       // Sözleşme Detayı İçin

            //       MusteriId = m.odeme.MusteriId,
            //       MusteriKodu = m.musteri.MusteriKodu,
            //       MusteriAdiSoyadi = m.musteri.AdiSoyadi,
            //       MusteriTC = m.musteri.TCKimlikNo,
            //       MusteriSabitTel = m.musteri.SabitTel,
            //       MusteriCepTel = m.musteri.CepTel,
            //       MusteriEmail = m.musteri.Email,
            //       MusteriAdres = m.musteri.Adres,
            //       MusteriNotlar = m.musteri.Notlar,
            //       KilitBit = m.odeme.KilitBit,
            //       OdemeIptal = m.odeme.Iptal,
            //       Aciklama = m.odeme.Notlar,
            //       OlusturanKullanici = m.odeme.OlusturanKullaniciId,
            //       OlusturmaTarih = m.odeme.OlusturmaTarih,
            //       DegistirenKullanici = m.odeme.DegistirenKullaniciId,
            //       DegistirmeTarih = m.odeme.DegistirmeTarih,
            //       Aktif = m.odeme.Aktif,
            //       Sil = m.odeme.Sil
            //   }).OrderByDescending(x => x.SozlesmeNo).ToList();

            odeme = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.MusteriId == MusteriId && x.Iptal == false && x.Aktif == true && x.Sil == false).ToList();
            var sz = odeme.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                GisNo = m.GunlukIsler.TakipNo,
                OdemeTuru = m.OdemeTuru,
                IslemTarihi = m.Tarih.ToShortDateString(),
                OdemeTarihi = m.OdemeTarihi.ToShortDateString(),
                OdemeYapan = m.OdemeYapanAdSoyad,
                Tutar = m.Tutar,
                OdemeSekli = m.OdemeSekli,
                OdemeAl = m.OdemeAl,
                AlinanOdemeMakbuzNo = m.AlinanOdemeMakbuzNo,
                Notlar = m.Notlar,
                // Sözleşme Detayı İçin
                SozlesmeId = m.SozlesmeId,
                SozlesmeNo = m.Sozlesme.SozlesmeNo,
                SozlesmeFirmaAdi = m.Sozlesme.Firma.FirmaAdi,
                SozlesmeTarihi = m.Sozlesme.SozlesmeTarihi.ToShortDateString(),
                OpsiyonTarihi = Convert.ToDateTime(m.Sozlesme.OpsiyonTarihi).ToShortDateString(),
                RezervasyonTur = m.Sozlesme.RezervasyonTurleri.RezervasyonTuru,
                YetkiliPersonel = m.Sozlesme.YetkiliPersonel,
                //GorevliPersonel = m.Sozlesme.Personel.AdiSoyadi,
                BaslangicSaat = m.Sozlesme.BaslangicSaat.ToString(),
                BitisSaat = m.Sozlesme.BitisSaat.ToString(),
                YetkiliAdSoyad = m.Sozlesme.YetkiliAdSoyad,
                YetkiliTel = m.Sozlesme.YetkiliTel,
                YetkiliEmail = m.Sozlesme.YetkiliEmail,
                AnneAd = m.Sozlesme.AnneAd,
                AnneTel = m.Sozlesme.AnneTel,
                AnneEmail = m.Sozlesme.AnneEmail,
                BabaAd = m.Sozlesme.BabaAd,
                BabaTel = m.Sozlesme.BabaTel,
                BabaEmail = m.Sozlesme.BabaEmail,
                CocukAdSoyad = m.Sozlesme.CocukAdSoyad,
                Modeller = m.Sozlesme.Modeller,
                Urunler = m.Sozlesme.Urunler,
                GelinAd = m.Sozlesme.GelinAd,
                GelinTel = m.Sozlesme.GelinTel,
                GelinEmail = m.Sozlesme.GelinEmail,
                DamatAd = m.Sozlesme.DamatAd,
                DamatTel = m.Sozlesme.DamatTel,
                DamatEmail = m.Sozlesme.DamatEmail,
                TeklifBit = m.Sozlesme.TeklifBit,
                Paketler = m.Sozlesme.Paketler,
                EkHizmetler = m.Sozlesme.EkHizmetler,
                PeketlerFiyat = m.Sozlesme.PaketlerFiyat,
                EkHizmetlerFiyat = m.Sozlesme.EkHizmetlerFiyat,
                Surecler = m.Sozlesme.Surecler,
                SozlesmeTutar = m.Sozlesme.ToplamFiyat,
                OrganizasyonYeri = m.Sozlesme.OrganizasyonYeri,
                SozlesmeNotlar = m.Sozlesme.Notlar,
                Bitti = m.Sozlesme.Bitti,
                SozlesmeIptal = m.Sozlesme.Iptal,
                Durum = m.Sozlesme.Durum,
                // Sözleşme Detayı İçin

                MusteriId = m.MusteriId,
                MusteriKodu = m.Musteri.MusteriKodu,
                MusteriAdiSoyadi = m.Musteri.AdiSoyadi,
                MusteriTC = m.Musteri.TCKimlikNo,
                MusteriSabitTel = m.Musteri.SabitTel,
                MusteriCepTel = m.Musteri.CepTel,
                MusteriEmail = m.Musteri.Email,
                MusteriAdres = m.Musteri.Adres,
                MusteriNotlar = m.Musteri.Notlar,
                KilitBit = m.KilitBit,
                OdemeIptal = m.Iptal,
                Aciklama = m.Notlar,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            }).OrderByDescending(x => x.SozlesmeNo).ToList();

            return Json(new { data = sz }, JsonRequestBehavior.AllowGet);

            //List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.MusteriId == MusteriId && x.Iptal == false && x.Aktif == true && x.Sil == false).ToList();
            //var odemelist = odemeler.Select(m => new
            //{
            //    Id = m.Id,
            //    FirmaAdi = m.Firma.FirmaAdi,
            //    SubeAdi = m.Sube.SubeAdi,
            //    Tarih = m.Tarih.ToShortDateString(),
            //    OdemeTarihi = m.OdemeTarihi.ToShortDateString(),
            //    OdemeTuru = m.OdemeTuru,
            //    GelecekOdemeId = m.GelecekOdemeID,
            //    // Sözleşme Detayı İçin
            //    SozlesmeId = m.SozlesmeId,
            //    SozlesmeNo = m.Sozlesme.SozlesmeNo,
            //    SozlesmeFirmaAdi = m.Sozlesme.Firma.FirmaAdi,
            //    SozlesmeTarihi = m.Sozlesme.SozlesmeTarihi.ToShortDateString(),
            //    OpsiyonTarihi = Convert.ToDateTime(m.Sozlesme.OpsiyonTarihi).ToShortDateString(),
            //    RezervasyonTur = m.Sozlesme.RezervasyonTurleri.RezervasyonTuru,
            //    YetkiliPersonel = m.Sozlesme.YetkiliPersonel,
            //    GorevliPersonel = m.Sozlesme.Personel.AdiSoyadi,
            //    BaslangicSaat = m.Sozlesme.BaslangicSaat.ToString(),
            //    BitisSaat = m.Sozlesme.BitisSaat.ToString(),
            //    YetkiliAdSoyad = m.Sozlesme.YetkiliAdSoyad,
            //    YetkiliTel = m.Sozlesme.YetkiliTel,
            //    YetkiliEmail = m.Sozlesme.YetkiliEmail,
            //    AnneAd = m.Sozlesme.AnneAd,
            //    AnneTel = m.Sozlesme.AnneTel,
            //    AnneEmail = m.Sozlesme.AnneEmail,
            //    BabaAd = m.Sozlesme.BabaAd,
            //    BabaTel = m.Sozlesme.BabaTel,
            //    BabaEmail = m.Sozlesme.BabaEmail,
            //    CocukAdSoyad = m.Sozlesme.CocukAdSoyad,
            //    Modeller = m.Sozlesme.Modeller,
            //    Urunler = m.Sozlesme.Urunler,
            //    GelinAd = m.Sozlesme.GelinAd,
            //    GelinTel = m.Sozlesme.GelinTel,
            //    GelinEmail = m.Sozlesme.GelinEmail,
            //    DamatAd = m.Sozlesme.DamatAd,
            //    DamatTel = m.Sozlesme.DamatTel,
            //    DamatEmail = m.Sozlesme.DamatEmail,
            //    TeklifBit = m.Sozlesme.TeklifBit,
            //    Paketler = m.Sozlesme.Paketler,
            //    EkHizmetler = m.Sozlesme.EkHizmetler,
            //    PeketlerFiyat = m.Sozlesme.PaketlerFiyat,
            //    EkHizmetlerFiyat = m.Sozlesme.EkHizmetlerFiyat,
            //    Surecler = m.Sozlesme.Surecler,
            //    SozlesmeTutar = m.Sozlesme.ToplamFiyat,
            //    OrganizasyonYeri = m.Sozlesme.OrganizasyonYeri,
            //    SozlesmeNotlar = m.Sozlesme.Notlar,
            //    Bitti = m.Sozlesme.Bitti,
            //    Iptal = m.Sozlesme.Iptal,
            //    Durum = m.Sozlesme.Durum,
            //    // Sözleşme Detayı İçin
            //    // Müşteri Detayı İçin
            //    MusteriId = m.MusteriId,
            //    MusteriKodu = m.Musteri.MusteriKodu,
            //    MusteriAdiSoyadi = m.Musteri.AdiSoyadi,
            //    MusteriTC = m.Musteri.TCKimlikNo,
            //    MusteriSabitTel = m.Musteri.SabitTel,
            //    MusteriCepTel = m.Musteri.CepTel,
            //    MusteriEmail = m.Musteri.Email,
            //    MusteriAdres = m.Musteri.Adres,
            //    MusteriNotlar = m.Musteri.Notlar,
            //    // Müşteri Detayı İçin
            //    GisId = m.GisId,
            //    GisTakipNo = m.GunlukIsler.TakipNo,
            //    AlinanOdemeMakbuzNo = m.AlinanOdemeMakbuzNo,
            //    Tutar = m.Tutar,
            //    Aciklama = m.Notlar,
            //    KilitBit = m.KilitBit,
            //    OdemeAl = m.OdemeAl,
            //    OlusturanKullanici = m.OlusturanKullaniciId,
            //    OlusturmaTarih = m.OlusturmaTarih,
            //    DegistirenKullanici = m.DegistirenKullaniciId,
            //    DegistirmeTarih = m.DegistirmeTarih,
            //    Aktif = m.Aktif,
            //    Sil = m.Sil
            //}).OrderByDescending(x => x.SozlesmeNo).ToList();
            //return Json(new { data = odemelist }, JsonRequestBehavior.AllowGet);



        }
        #endregion
        #region Fotoğraf Seçim Sayfası
        public ActionResult FotografSec()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Fotoğraf Seç";
                long FirmaId = Convert.ToInt64(Session["FirmaId"]);
                long MusteriId = Convert.ToInt64(Session["MusteriId"]);
                ViewBag.MusteriYetkileri = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);

                List<MusteriFotograf> musterifotolari = dbContext.MusteriFotografs.Where(x => x.MusteriId == MusteriId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).OrderBy(y => y.SecildiDurum).ToList();
                List<Sozlesme> aktifsozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.MusteriId == MusteriId && x.Bitti == false && x.KesinRezervasyonBit == true && x.Aktif == true && x.Sil == false).ToList();
                ViewBag.AktifSozlesmeler = aktifsozlesmeler;
                return View(musterifotolari);
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult SecimleriKaydet()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long MusteriId = Convert.ToInt64(Session["MusteriId"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            string[] Secilmeyenler = Request["Secilmedi"].Split(',');
            string[] Duzenlenecekler = Request["Duzenlenecek"].Split(',');
            string[] Basilacaklar = Request["Basilacak"].Split(',');
            int Kapak = Convert.ToInt32(Request["Kapak"]);
            int Poster = Convert.ToInt32(Request["Poster"]);
            List<Sube> SubeId = dbContext.Subes.Where(x => x.FirmaId == FirmaId).OrderByDescending(x => x.Id).Take(1).ToList();
            int secilemeyen_id = 0, duzenlenecek_id = 0, basilacak_id = 0;
            string secilemeyen_not = "", duzenlenecek_not = "", basilacak_not = "";
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif==true);
            MusteriFotograf mf = new MusteriFotograf();
            if (Secilmeyenler[0] != "")
            {
                foreach (string secilmeyen in Secilmeyenler)
                {

                    if (secilmeyen.Contains("Foto_Id"))
                    {
                        int index = secilmeyen.IndexOf("Foto_Id:");
                        secilemeyen_id = (secilmeyen.Remove(index, 8) != null) ? Convert.ToInt32(secilmeyen.Remove(index, 8)) : 0;
                    }
                    if (secilmeyen.Contains("Foto_Not"))
                    {
                        int index = secilmeyen.IndexOf("Foto_Not:");
                        secilemeyen_not = secilmeyen.Remove(index, 9).Replace('&', ',');
                    }
                    //int fotoid = Convert.ToInt32(secilmeyen);
                    mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.Id == secilemeyen_id && x.FirmaId == FirmaId);
                    if (mf != null)
                    {
                        mf.Notlar = secilemeyen_not;
                        mf.SecildiDurum = "0"; // 0: Seçilmedi  1: Düzenleme yapılmalı  2: Baskıya Uygun
                        mf.DegistirenKullaniciId = 1;
                        mf.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId[0].Id;
                            hata.Islem = "Müşteri Fotoğraf Seçimi - Seçilenlerin kaydedilmesi";
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
            if (Duzenlenecekler[0] != "")
            {
                foreach (string duzenlenecek in Duzenlenecekler)
                {
                    if (duzenlenecek.Contains("Foto_Id"))
                    {
                        int index = duzenlenecek.IndexOf("Foto_Id:");
                        duzenlenecek_id = (duzenlenecek.Remove(index, 8) != null) ? Convert.ToInt32(duzenlenecek.Remove(index, 8)) : 0;
                    }
                    if (duzenlenecek.Contains("Foto_Not"))
                    {
                        int index = duzenlenecek.IndexOf("Foto_Not:");
                        duzenlenecek_not = duzenlenecek.Remove(index, 9).Replace('&', ',');
                    }
                    //int fotoid = Convert.ToInt32(duzenlenecek);
                    mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.Id == duzenlenecek_id && x.FirmaId == FirmaId);
                    if (mf != null)
                    {
                        mf.Notlar = duzenlenecek_not;
                        mf.SecildiDurum = "1"; // 0: Seçilmedi  1: Düzenleme yapılmalı  2: Baskıya Uygun
                        mf.DegistirenKullaniciId = 1;
                        mf.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId[0].Id;
                            hata.Islem = "Müşteri Fotoğraf Seçimi - Düzenleneceklerin kaydedilmesi";
                            hata.HataMesajı = e.Message;
                            hata.OlusturanKullaniciId = 1;
                            hata.OlusturmaTarih = DateTime.Now;
                            hata.DegistirenKullaniciId = 1;
                            hata.DegistirmeTarih = DateTime.Now;
                            hata.Aktif = true;
                            hata.Sil = false;
                            dbContext.HataLoglaris.Add(hata);
                            dbContext.SaveChanges();
                            return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
                        }
                    }
                }
            }
            if (Basilacaklar[0] != "")
            {
                foreach (string basilacak in Basilacaklar)
                {
                    if (basilacak.Contains("Foto_Id"))
                    {
                        int index = basilacak.IndexOf("Foto_Id:");
                        basilacak_id = (basilacak.Remove(index, 8) != null) ? Convert.ToInt32(basilacak.Remove(index, 8)) : 0;
                    }
                    if (basilacak.Contains("Foto_Not"))
                    {
                        int index = basilacak.IndexOf("Foto_Not:");
                        basilacak_not = basilacak.Remove(index, 9).Replace('&', ',');
                    }
                    //int fotoid = Convert.ToInt32(secilmeyen);
                    mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.Id == basilacak_id && x.FirmaId == FirmaId);
                    if (mf != null)
                    {
                        mf.Notlar = basilacak_not;
                        mf.SecildiDurum = "2"; // 0: Seçilmedi  1: Düzenleme yapılmalı  2: Baskıya Uygun
                        mf.DegistirenKullaniciId = 1;
                        mf.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId[0].Id;
                            hata.Islem = "Müşteri Fotoğraf Seçimi - Basılacaklar kaydedilmesi";
                            hata.HataMesajı = e.Message;
                            hata.OlusturanKullaniciId = 1;
                            hata.OlusturmaTarih = DateTime.Now;
                            hata.DegistirenKullaniciId = 1;
                            hata.DegistirmeTarih = DateTime.Now;
                            hata.Aktif = true;
                            hata.Sil = false;
                            dbContext.HataLoglaris.Add(hata);
                            dbContext.SaveChanges();
                            return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
                        }
                    }
                }
            }

            if (Kapak != 0)
            {
                mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.Id == Kapak && x.FirmaId == FirmaId);
                mf.KapakFotograf = true;
                mf.DegistirenKullaniciId = 1;
                mf.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId[0].Id;
                    hata.Islem = "Müşteri Fotoğraf Seçimi - Kapak Fotoğrafı kaydedilmesi";
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
            if (Poster != 0)
            {
                mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.Id == Poster && x.FirmaId == FirmaId);
                mf.PosterFotograf = true;
                mf.DegistirenKullaniciId = 1;
                mf.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId[0].Id;
                    hata.Islem = "Müşteri Fotoğraf Seçimi - Poster Fotoğrafı kaydedilmesi";
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
            // "SEÇİMLERİ BASKI İÇİN ONAYLA" butonuna basılırsa farklı bir senayo çalışacak
            Models.Musteri m = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId && x.Sil == false && x.Aktif == true);
            //m.FotografSecimDurum = "2"; //0: İşlem Yok  1: Müşteriye Seçime Gönderildi  2: Müşteri Seçimi Yaptı  3: Müşteriye Tekrar Göderildi  4: Müşteri Seçimleri Baskı için Onayladı
            //m.DegistirenKullaniciId = 1;
            //m.DegistirmeTarih = DateTime.Now;
            sz.FotografSecimDurum ="2";
            sz.FotografSecimDurumTarihi = DateTime.Now;
            sz.DegistirenKullaniciId = 1;
            sz.DegistirmeTarih = DateTime.Now;
            dbContext.SaveChanges();
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
            try
            {
                dbContext.SaveChanges();

                string smsmetinfirma = "", sonuc_fotograf_secim_firma_sms = "";
                bool sonuc_fotograf_secim_firma_mail = true;
                #region Sms Bilgi Mesajı Müşteri-Firma
                AyarlarSmsGonderim smsayarfirma = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.FotografSecimiBilgiMesajiFirmaGonderimSuresi == 6 && x.FotografSecimiBilgiMesajiFirma != 0);  // Sms gönderim ayarı FotografSecimiBilgiMesajiFirma için "Kayıt Yapıldığında" Seçilmişse Firmaya Bilgi mesajı gönder.
                if (!string.IsNullOrEmpty(smsmetinfirma))
                    smsmetinfirma = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayarfirma.FotografSecimiBilgiMesajiFirma && x.Aktif == true && x.Sil == false).SMSMetni;

                //if (smsayarfirma != null || smsayarfirma.FotografSecimiBilgiMesajiFirma != 0)
                //    smsmetinfirma = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayarfirma.FotografSecimiBilgiMesajiFirma && x.Aktif == true && x.Sil == false).SMSMetni;
                //else
                //    return Json(new { Sonuc = false, Mesaj = "", JsonRequestBehavior.AllowGet });
                //if (string.IsNullOrEmpty(smsmetinfirma))
                //    return Json(new { Sonuc = false, Mesaj = "", JsonRequestBehavior.AllowGet });

                string sozlesmeNo = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriId == MusteriId && x.Bitti == false && x.KesinRezervasyonBit == true && x.Sil == false && x.Aktif == true).SozlesmeNo.ToString();
                if (frm.CepTel != "") // Firma Yetkilisinin bir cep telefonu girilmiş ise SMS gönder
                {
                    if (smsayarfirma != null && smsmetinfirma != null)
                    {
                        if (smsmetinfirma.IndexOf("{FirmaAdi}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{FirmaAdi}", frm.FirmaAdi);
                        if (smsmetinfirma.IndexOf("{GelinAdSoyad}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{GelinAdSoyad}", "");
                        if (smsmetinfirma.IndexOf("{DamatAdSoyad}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{DamatAdSoyad}", "");
                        if (smsmetinfirma.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            smsmetinfirma = smsmetinfirma.Replace("{YetkiliAdSoyad}", frm.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (smsmetinfirma.IndexOf("{MusteriKodu}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{MusteriKodu}", m.MusteriKodu.ToString());
                        if (smsmetinfirma.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            smsmetinfirma = smsmetinfirma.Replace("{AdSoyad}", m.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (smsmetinfirma.IndexOf("{RezervasyonTuru}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                        if (smsmetinfirma.IndexOf("{Tarih}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{Tarih}", DateTime.Now.ToShortDateString());
                        if (smsmetinfirma.IndexOf("{RandevuTarihi}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{RandevuTarihi}", "");
                        if (smsmetinfirma.IndexOf("{OdemeTarihi}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{OdemeTarihi}", "");
                        if (smsmetinfirma.IndexOf("{Tutar}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{Tutar}", "");
                        if (smsmetinfirma.IndexOf("{OpsiyonTarihi}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{OpsiyonTarihi}", "");
                        if (smsmetinfirma.IndexOf("{SozlesmeNo}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{SozlesmeNo}", sozlesmeNo);
                        if (smsmetinfirma.IndexOf("{CekimYeri}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{CekimYeri}", "");
                        if (smsmetinfirma.IndexOf("{CekimSaati}") != -1)
                            smsmetinfirma = smsmetinfirma.Replace("{CekimSaati}", "");

                        sonuc_fotograf_secim_firma_sms = SMSGonder.Gonder_AtakSms(smsmetinfirma, frm.CepTel, FirmaId, SubeId[0].Id, 1);
                    }
                }
                #endregion

                #region Çekim Randevuları Personel Bilgi Emaili
                if (frm.Email != "")
                {
                    AyarlarMailGonderim mailayar = null;
                    MailMetinleri mmetin = null;
                    string mailmetin = "";

                    mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.FotografSecimiBilgiMailiFirmaGonderimSuresi == 6);
                    if (mailayar != null)
                    {
                        mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.FotografSecimiBilgiMailiFirma && x.Aktif == true && x.Sil == false);
                        mailmetin = mmetin.MailMetni;
                    }
                    else
                        mailmetin = null;
                    if (mailayar != null && mailmetin != null)
                    {
                        if (mailmetin.IndexOf("{FirmaAdi}") != -1)
                            mailmetin = mailmetin.Replace("{FirmaAdi}", frm.FirmaAdi);
                        if (mailmetin.IndexOf("{GelinAdSoyad}") != -1)
                            mailmetin = mailmetin.Replace("{GelinAdSoyad}", "");
                        if (mailmetin.IndexOf("{DamatAdSoyad}") != -1)
                            mailmetin = mailmetin.Replace("{DamatAdSoyad}", "");
                        if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", frm.Yetkili);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (mailmetin.IndexOf("{MusteriKodu}") != -1)
                            mailmetin = mailmetin.Replace("{MusteriKodu}", m.MusteriKodu.ToString());
                        if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            mailmetin = mailmetin.Replace("{AdSoyad}", m.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (mailmetin.IndexOf("{RezervasyonTuru}") != -1)
                            mailmetin = mailmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                        if (mailmetin.IndexOf("{Tarih}") != -1)
                            mailmetin = mailmetin.Replace("{Tarih}", DateTime.Now.ToShortDateString());
                        if (mailmetin.IndexOf("{RandevuTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{RandevuTarihi}", "");
                        if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OdemeTarihi}", "");
                        if (mailmetin.IndexOf("{Tutar}") != -1)
                            mailmetin = mailmetin.Replace("{Tutar}", "");
                        if (mailmetin.IndexOf("{OpsiyonTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OpsiyonTarihi}", "");
                        if (mailmetin.IndexOf("{SozlesmeNo}") != -1)
                            mailmetin = mailmetin.Replace("{SozlesmeNo}", sozlesmeNo);
                        if (mailmetin.IndexOf("{CekimYeri}") != -1)
                            mailmetin = mailmetin.Replace("{CekimYeri}", "");
                        if (mailmetin.IndexOf("{CekimSaati}") != -1)
                            mailmetin = mailmetin.Replace("{CekimSaati}", "");

                        string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
                        string IcerikResim = Server.MapPath(mmetin.IcerikResim);
                        string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");
                        string body = string.Empty;


                        if (mmetin.TemaYol != "")
                        {
                            using (StreamReader reader = new StreamReader(Server.MapPath(mmetin.TemaYol)))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("{FirmaLogo}", "src=cid:firmalogo");
                            body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
                            body = body.Replace("{IcerikResim}", "src=cid:icerikresim");
                            body = body.Replace("{FirmaAdi}", frm.FirmaAdi);
                            body = body.Replace("{FirmaWeb}", frm.WebSitesi);
                            body = body.Replace("{FirmaFacebook}", frm.Facebook);
                            body = body.Replace("{FirmaInstagram}", frm.Instagram);
                            body = body.Replace("{FirmaTwitter}", frm.Twitter);
                            body = body.Replace("{EmailMetni}", mailmetin);

                            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                            LinkedResource firmalogo = new LinkedResource(FirmaLogo);
                            firmalogo.ContentId = "firmalogo";
                            LinkedResource fotografcitakiplogo = new LinkedResource(FotografciTakipLogo);
                            fotografcitakiplogo.ContentId = "icerikresim";
                            LinkedResource icerikresim = new LinkedResource(IcerikResim);
                            icerikresim.ContentId = "icerikresim";
                            htmlView.LinkedResources.Add(firmalogo);
                            htmlView.LinkedResources.Add(fotografcitakiplogo);
                            htmlView.LinkedResources.Add(icerikresim);

                            sonuc_fotograf_secim_firma_mail = MailGonder.Gonder(FirmaId, SubeId[0].Id, 1, frm.Email, mmetin.MailKonu, body, htmlView);
                        }
                        else
                            sonuc_fotograf_secim_firma_mail = MailGonder.Gonder_Text(FirmaId, SubeId[0].Id, 1, frm.Email, mmetin.MailKonu, mailmetin);

                    }

                }
                #endregion
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = MusteriId;
                hata.Islem = "Müşteri Fotoğraf seç, Seçimleri Kaydet, Müşteri Id: " + MusteriId;
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();

                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public ActionResult AlbumBaskiOnay()
        {
            // Personelin kullanıldığı tablolarda kayıtlar var ise personel silinemeyecek
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int MusteriId = Convert.ToInt32(Session["MusteriId"]);
            long SozlesmeId = Convert.ToInt64(Session["SozlesmeId"]);
            //Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
            //musteri.FotografSecimDurum = "4"; //0: İşlem Yok  1: Müşteriye Seçime Gönderildi  2: Müşteri Seçimi Yaptı  3: Müşteriye Tekrar Göderildi  4: Müşteri Seçimleri Baskı için Onayladı
            //musteri.DegistirenKullaniciId = 1;
            //musteri.DegistirmeTarih = DateTime.Now;
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            sz.FotografSecimDurum = "4";
            sz.FotografSecimDurumTarihi = DateTime.Now;
            sz.DegistirenKullaniciId = 1;
            sz.DegistirmeTarih = DateTime.Now;
            dbContext.SaveChanges();
            try
            {
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
            }


        }
        [HttpPost]
        public ActionResult MusteriFotografListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Müşteriler";
            ViewBag.AltMenu = "Fotoğraf Listesi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long MusteriId = Convert.ToInt64(Session["MusteriId"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);

            List<MusteriFotograf> musterifotolari = dbContext.MusteriFotografs.Where(x => x.MusteriId == MusteriId && x.SozlesmeId == SozlesmeId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
            var musterifotolist = musterifotolari.Select(m => new
            {
                Id = m.Id,
                MusteriId = m.MusteriId,
                MusteriAdi = m.Musteri.AdiSoyadi,
                SozlesmeId = m.SozlesmeId,
                FotografAdi = m.FotografAdi,
                FotografAciklama = m.FotografAciklama,
                FotografYol = m.FotografYol,
                SecildiDurum = m.SecildiDurum,
                Notlar = m.Notlar
            }).OrderBy(y => y.SecildiDurum).ToList();
            return Json(new { Sonuc = true, data = musterifotolist }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Mesajlarım Sayfası
        public ActionResult Mesajlar()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Mesajlar";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                //Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                AyarlarMusteri musteriayar = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);
                ViewBag.MusteriYetkileri = musteriayar;
                if (!musteriayar.MesajGonder)
                {
                    return RedirectToAction("YetkisizGiris");
                }
                List<MusteriMesaj> msj = dbContext.MusteriMesajs.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
                return View(msj);
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult MesajListesi()
        {
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int MusteriId = Convert.ToInt32(Session["MusteriId"]);
            List<MusteriMesaj> msj;
            //Müşteri Id si olmayan Sözleşmeler silinecek

            msj = dbContext.MusteriMesajs.Where(x => x.FirmaId == FirmaId && x.MusteriId == MusteriId && x.MesajId == null && x.Aktif == true && x.Sil == false).ToList();
            var mesajlar = msj.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                MusteriKodu = m.Musteri.MusteriKodu,
                MusteriAdi = m.Musteri.AdiSoyadi,
                Konu = m.Konu,
                Mesaj = m.Mesaj,
                MesajTarihi = m.MesajTarihi,
                CevapTarihi = m.CevapTarihi,
                Durum = m.Durum,
                OkunduBit = m.OkunduBit,
                CevaplaBit = m.CevaplaBit,
                KilitBit = m.KilitBit,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            }).OrderByDescending(x => x.Id).ToList();

            return Json(new { data = mesajlar }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MesajDetay(int? id)
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                ViewBag.AltMenu = "Mesaj Detayı";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);
                Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                AyarlarMusteri musteriayar = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);
                ViewBag.MusteriYetkileri = musteriayar;
                if (!musteriayar.MesajGonder)
                {
                    return RedirectToAction("YetkisizGiris");
                }
                if (id == null)
                {
                    return RedirectToAction("YetkisizGiris");
                }
                MusteriMesaj ilkmesaj = dbContext.MusteriMesajs.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                ViewBag.ilkmesaj = ilkmesaj;
                List<MusteriMesaj> cevapmesajlar = dbContext.MusteriMesajs.Where(x => x.MesajId == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).ToList();
                foreach (var item in cevapmesajlar)
                {
                    if (!item.OkunduBit)
                    {
                        item.OkunduBit = true;
                        item.DegistirenKullaniciId = 1;
                        item.DegistirmeTarih = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                }
                ilkmesaj.OkunduBit = true;
                ilkmesaj.DegistirenKullaniciId = 1;
                ilkmesaj.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();
                return View(cevapmesajlar);
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult MesajKaydet()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Mesajlar";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                //Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                AyarlarMusteri musteriayar = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);
                ViewBag.MusteriYetkileri = musteriayar;
                if (!musteriayar.MesajGonder)
                {
                    return RedirectToAction("YetkisizGiris");
                }
                string Konu = Request["Konu"];
                string Msj = Request["Mesaj"];
                Msj = Msj.Replace("\r\n", "<br />");
                Msj = Msj.Replace("\n", "<br />");

                /*  ****DURUM
                    1: Cevap Bekleniyor
                    2: Okundu
                    3: Cevaplandı
                    4: Değerlendiriliyor
                    5: Konu Kapandı
                    6: Konu Açık
                    7: İptal    */

                MusteriMesaj mesaj = new MusteriMesaj();
                mesaj.FirmaId = FirmaId;
                mesaj.MusteriId = MusteriId;
                mesaj.Konu = Konu;
                mesaj.Mesaj = Msj;
                mesaj.MesajTarihi = DateTime.Now;
                mesaj.Durum = "1";
                mesaj.FirmaCevapBit = false;
                mesaj.CevaplaBit = false;
                mesaj.OkunduBit = false;
                mesaj.KilitBit = false;
                mesaj.OlusturanKullaniciId = 1;
                mesaj.OlusturmaTarih = DateTime.Now;
                mesaj.DegistirenKullaniciId = 1;
                mesaj.DegistirmeTarih = DateTime.Now;
                mesaj.Aktif = true;
                mesaj.Sil = false;
                try
                {
                    dbContext.MusteriMesajs.Add(mesaj);
                    dbContext.SaveChanges();
                    /*      Kayıt Gerçekleştikten Sonra SMS ve Mail Bildirimi Yapılacak.   */
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "Müşteri Yeni mesaj gönder";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Firma Yetkilisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                }
                return Json(new { Sonuc = true, MesajId = mesaj.Id, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult MesajGuncelle()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Mesajlar";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                //Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                AyarlarMusteri musteriayar = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);
                ViewBag.MusteriYetkileri = musteriayar;
                if (!musteriayar.MesajGonder)
                {
                    return RedirectToAction("YetkisizGiris");
                }
                int id = Convert.ToInt32(Request["MesajId"]);
                string Konu = Request["Konu"];
                string Msj = Request["Mesaj"];
                Msj = Msj.Replace("\r\n", "<br />");
                Msj = Msj.Replace("\n", "<br />");

                /*  ****DURUM
                    1: Cevap Bekleniyor
                    2: Okundu
                    3: Cevaplandı
                    4: Değerlendiriliyor
                    5: Konu Kapandı
                    6: Konu Açık
                    7: İptal    */

                MusteriMesaj mesaj = dbContext.MusteriMesajs.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

                mesaj.Konu = Konu;
                mesaj.Mesaj = Msj;
                mesaj.MesajTarihi = DateTime.Now;
                mesaj.Durum = "1";
                mesaj.DegistirenKullaniciId = 1;
                mesaj.DegistirmeTarih = DateTime.Now;

                try
                {
                    dbContext.SaveChanges();
                    /*      Kayıt Gerçekleştikten Sonra SMS ve Mail Bildirimi Yapılacak.   */
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "Müşteri Mesaj Düzenle";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Firma Yetkilisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                }
                return Json(new { Sonuc = true, MesajId = mesaj.Id, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult MesajCevapla()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                ViewBag.AltMenu = "Mesajlar";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                //Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                AyarlarMusteri musteriayar = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);
                ViewBag.MusteriYetkileri = musteriayar;
                if (!musteriayar.MesajGonder)
                {
                    return RedirectToAction("YetkisizGiris");
                }

                int IlkMesajId = Convert.ToInt32(Request["IlkMesajId"]);
                int? MesajId = Convert.ToInt32(Request["MesajId"]);
                string Konu = Request["Konu"];
                string Durum = "3";
                string Mesaj = Request["Mesaj"];
                Mesaj = Mesaj.Replace("\r\n", "<br />");
                Mesaj = Mesaj.Replace("\n", "<br />");
                MusteriMesaj ilkmesaj = dbContext.MusteriMesajs.FirstOrDefault(x => x.Id == IlkMesajId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                /*  ****DURUM
                  1: Cevap Bekleniyor
                  2: Okundu
                  3: Cevaplandı
                  4: Değerlendiriliyor
                  5: Konu Kapandı
                  6: Konu Açık
                  7: İptal    */

                /* İlk mesaja göre Cevap Kaydediliyor. */
                MusteriMesaj mesaj = new MusteriMesaj();
                mesaj.FirmaId = FirmaId;
                mesaj.MesajId = IlkMesajId;
                mesaj.MusteriId = ilkmesaj.MusteriId;
                mesaj.Konu = Konu;
                mesaj.Mesaj = Mesaj;
                mesaj.MesajTarihi = ilkmesaj.MesajTarihi;
                mesaj.CevapTarihi = DateTime.Now;
                mesaj.Durum = Durum;
                mesaj.FirmaCevapBit = false;
                mesaj.CevaplaBit = true; // Cevap müşteri tarafından verildiği için cevapla butonu gösterme
                mesaj.OkunduBit = false;
                mesaj.KilitBit = false;
                mesaj.Aktif = true;
                mesaj.Sil = false;
                mesaj.OlusturanKullaniciId = 1;
                mesaj.OlusturmaTarih = DateTime.Now;
                mesaj.DegistirenKullaniciId = 1;
                mesaj.DegistirmeTarih = DateTime.Now;
                dbContext.MusteriMesajs.Add(mesaj);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = IlkMesajId + " id'li mesaja cevap kaydetme";
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
                /* Cevap Kaydedildikten sonra diğer cevapların düzenlemesi yapılıyor. */
                /* Gelen MesajId boş ise verilen cevap ilkmesaj a dır ve ilk mesajın okundu biti ve cevapbiti true yapılır.  */
                /* Gelen MesajId dolu ise verilen cevap diğer cevaplardan birine verilmiştir ve bu Id ye sahip mesajın okundu biti ve cevapbiti true yapılır.  */
                if (MesajId == 0) // 
                {
                    ilkmesaj.OkunduBit = true;
                    ilkmesaj.CevaplaBit = false;
                    ilkmesaj.CevapTarihi = mesaj.CevapTarihi;
                    ilkmesaj.Durum = mesaj.Durum;
                    ilkmesaj.DegistirenKullaniciId = 1;
                    ilkmesaj.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = 1;
                        hata.Islem = "ilk Mesaj düzenle, IlkMesajId:" + IlkMesajId;
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
                    ilkmesaj.CevapTarihi = mesaj.CevapTarihi;
                    ilkmesaj.Durum = mesaj.Durum;
                    ilkmesaj.DegistirenKullaniciId = 1;
                    ilkmesaj.DegistirmeTarih = DateTime.Now;
                    MusteriMesaj cevaplananmesaj = dbContext.MusteriMesajs.FirstOrDefault(x => x.Id == MesajId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                    cevaplananmesaj.OkunduBit = true;
                    cevaplananmesaj.CevaplaBit = false;
                    cevaplananmesaj.CevapTarihi = mesaj.CevapTarihi;
                    cevaplananmesaj.Durum = mesaj.Durum;
                    cevaplananmesaj.DegistirenKullaniciId = 1;
                    cevaplananmesaj.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = 1;
                        hata.Islem = MesajId + " id'li mesaja cevap kaydetme";
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
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı", JsonRequestBehavior.AllowGet });
            }
            else // Kullanıcı Bu sayfaya yetkili ise işelemlere devam
            {
                return RedirectToAction("YetkisizGiris", "Hata");
            }

        }

        #endregion
        #region Profil İşlemleri
        public ActionResult Profil()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Profil Bilgileri";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                //Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                ViewBag.MusteriYetkileri = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);
                Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId && x.Sil == false && x.Aktif == true);
                return View(musteri);
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult SmsDurumDegistir()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Profil Bilgileri";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId && x.Sil == false && x.Aktif == true);

                if (musteri.SMSKabul)
                    musteri.SMSKabul = false;
                else
                    musteri.SMSKabul = true;
                musteri.DegistirenKullaniciId = 1;
                musteri.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Durum başarıyla değiştirildi", JsonRequestBehavior.AllowGet });
                }
                catch (Exception)
                {
                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult EpostaDurumDegistir()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Profil Bilgileri";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId && x.Sil == false && x.Aktif == true);

                if (musteri.EmailKabul)
                    musteri.EmailKabul = false;
                else
                    musteri.EmailKabul = true;
                musteri.DegistirenKullaniciId = 1;
                musteri.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Durum başarıyla değiştirildi", JsonRequestBehavior.AllowGet });
                }
                catch (Exception)
                {
                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult SifreDegistir()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Profil Bilgileri";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);
                string EskiSifre = Request["EskiSifre"];
                string YeniSifre = Request["YeniSifre"];
                Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId && x.Sil == false && x.Aktif == true);

                if (musteri.Sifre != EskiSifre)
                {
                    return Json(new { Sonuc = false, Mesaj = "Eski şifre yanlış", JsonRequestBehavior.AllowGet });
                }

                musteri.Sifre = YeniSifre;
                musteri.DegistirenKullaniciId = 1;
                musteri.DegistirmeTarih = DateTime.Now;
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
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        #endregion
        #region Rezervasyon Alınan Ödeme Makbuz Yazdırma İşemleri
        public ActionResult AlinanOdemeMakbuztoPDF(int? id)
        {
            //if (Session.Count == 0)
            //    return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Dashboard");

            Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.Id == id);
            if (odeme == null)
                return RedirectToAction("SayfaBulunamadi", "Dashboard");

            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == odeme.FirmaId);
            AyarlarMusteri ayar = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == odeme.FirmaId && x.Sil == false && x.Aktif == true);
            if (ayar == null)
                return RedirectToAction("YetkisizGiris", "Dashboard");
            ViewBag.SozlesmeAyar = dbContext.AyarlarSozlesmeCiktis.FirstOrDefault(x => x.FirmaId == frm.Id);
            ViewBag.Firma = frm;
            if (odeme.MusteriId != 0)
            {
                ViewBag.Musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == odeme.MusteriId);
            }
            if (odeme.SozlesmeId > 1)
            {
                ViewBag.SozlesmeNo = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == odeme.SozlesmeId).SozlesmeNo;
            }
            if (odeme.GisId > 1)
            {
                ViewBag.GisTakipNo = dbContext.GunlukIslers.FirstOrDefault(x => x.Id == odeme.GisId).TakipNo;
            }
            return View(odeme);
        }
        public ActionResult AlinanOdemeMakbuzYazdir(int id)
        {
            // id = makbuzno
            var p = new ActionAsPdf("AlinanOdemeMakbuztoPDF", new { id = id })
            {
                CustomSwitches = "--page-offset 0 --footer-center [page]/[toPage] --footer-font-size 10",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 }// ayarlar sayfasından alınabilir
            };
            return p;
        }
        #endregion
        public ActionResult YetkisizGiris()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                //ViewBag.UstMenu = "Dashboard";
                ViewBag.AltMenu = "Yetkisiz Giriş";
                int FirmaId = Convert.ToInt32(Session["FirmaId"]);
                int MusteriId = Convert.ToInt32(Session["MusteriId"]);

                //Models.Musteri muteri = dbContext.Musteris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == MusteriId);
                ViewBag.MusteriYetkileri = dbContext.AyarlarMusteris.FirstOrDefault(x => x.FirmaId == FirmaId);
                return View();
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        public ActionResult SayfaBulunamadi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Hata";
            ViewBag.AltMenu = "Erişmek İstediğiniz Sayfa Bulunamadı";
            return View();
        }

        public PartialViewResult _OkunmamisMesajlar()
        {
            int FirmaId = Convert.ToInt32(Session["FirmaId"]);
            int KullaniciId = Convert.ToInt32(Session["Id"]);
            /* OkunduBit false olan ve durumu "KonuKapandı veya İptal olmayan Mesajlar"*/
            List<MusteriMesaj> okunmamismesajlar = dbContext.MusteriMesajs.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.OkunduBit == false && !(x.Durum == "5" || x.Durum == "7")).ToList();
            return PartialView(okunmamismesajlar);
        }
    }
}