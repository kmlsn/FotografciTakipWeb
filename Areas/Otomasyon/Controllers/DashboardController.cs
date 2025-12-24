using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FotografciTakipWeb.Models;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using FotografciTakipWeb.App_Settings;


namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class DashboardController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/Default
        public ActionResult Index()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            //ViewBag.UstMenu = "Dashboard";
            ViewBag.UstMenu = "Dashboard";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            int kalankontor = 0;
            int konturuyarimiktari = 0;
            string kalankonturuyarimesaji = "Kontor çok";
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 1 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");

            ViewBag.GelirGiderYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 90 && x.Aktif == true && x.Sil == false).KayitDuzenle;
            ViewBag.KullaniciYetkileri = dbContext.KullaniciYetkis.Where(x => x.KullaniciId == KullaniciId && x.Aktif == true && x.Sil == false).ToList();
            #region KasaPanelGelirGiderEkle
            ViewBag.GGTur = dbContext.GelirGiderTurleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            #endregion
            #region KasaPanel
            DateTime IlkTarih = DateTime.Today;
            DateTime SonTarih = DateTime.Today;
            List<GelirGider> gelir;
            List<GelirGider> gider;
            if (SubeId == 0)
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId & x.Tip == "Gelir" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId & x.Tip == "Gider" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
            }
            else
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gelir" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gider" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
            }

            //if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
            //{
            //    gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId & x.Tip == "Gelir" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
            //    gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId & x.Tip == "Gider" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();

            //}

            decimal toplamgelir = gelir.Sum(x => x.Tutar);
            decimal toplamgider = gider.Sum(x => x.Tutar);
            decimal kasa = toplamgelir - toplamgider;
            ViewBag.ToplamGelir = toplamgelir;
            ViewBag.ToplamGider = toplamgider;
            ViewBag.Kasa = kasa;
            #endregion
            #region GünlükİşlerPanel
            ViewBag.GunlukIsKategoriler = dbContext.GunlukIsKategoris.Where(x => x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId).ToList();
            ViewBag.Musteriler = dbContext.Musteris.Where(x => x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId).ToList();
            //if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
            //    ViewBag.GunlukIsler = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && (x.Tarih.Day == DateTime.Today.Day && x.Tarih.Month == DateTime.Today.Month && x.Tarih.Year == DateTime.Today.Year) && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
            //else
            //    ViewBag.GunlukIsler = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && (x.Tarih.Day == DateTime.Today.Day && x.Tarih.Month == DateTime.Today.Month && x.Tarih.Year == DateTime.Today.Year) && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
            if (SubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                ViewBag.GunlukIsler = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && (x.Tarih.Day == DateTime.Today.Day && x.Tarih.Month == DateTime.Today.Month && x.Tarih.Year == DateTime.Today.Year) && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
            else
                ViewBag.GunlukIsler = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && (x.Tarih.Day == DateTime.Today.Day && x.Tarih.Month == DateTime.Today.Month && x.Tarih.Year == DateTime.Today.Year) && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();

            #endregion
            #region RezervasyonTeklifleriPanel
            //if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
            //    ViewBag.BugunkuTeklifler = dbContext.Sozlesmes.Where(x => x.OpsiyonTarihi == DateTime.Today && (x.OpsiyonBit == true || x.TeklifBit == true) && x.Iptal == false && x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId).ToList();
            //else
            //    ViewBag.BugunkuTeklifler = dbContext.Sozlesmes.Where(x => x.OpsiyonTarihi == DateTime.Today && (x.OpsiyonBit == true || x.TeklifBit == true) && x.Iptal == false && x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId && x.SubeId == SubeId).ToList();
            if (SubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                ViewBag.BugunkuTeklifler = dbContext.Sozlesmes.Where(x => x.OpsiyonTarihi == DateTime.Today && (x.OpsiyonBit == true || x.TeklifBit == true) && x.Iptal == false && x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId).ToList();
            else
                ViewBag.BugunkuTeklifler = dbContext.Sozlesmes.Where(x => x.OpsiyonTarihi == DateTime.Today && (x.OpsiyonBit == true || x.TeklifBit == true) && x.Iptal == false && x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId && x.SubeId == SubeId).ToList();

            #endregion
            #region Kalan Kontor Uyarı
            kalankontor = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).Bakiye;
            konturuyarimiktari = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).KonturUyariMiktari;
            if (kalankontor <= konturuyarimiktari)
            {
                kalankonturuyarimesaji = "Kontor az";
            }
            ViewBag.KalanKontorUyariMesaji = kalankonturuyarimesaji;
            ViewBag.KalanKontorUyariMesajiVer = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).KonturUyariVer;
            #endregion
            #region YeniRezervasyonEkle
            Sube subeyetkili;
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
                subeyetkili = dbContext.Subes.FirstOrDefault(x => x.Id == SubeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            }
            else
                subeyetkili = dbContext.Subes.FirstOrDefault(x => x.Id == SubeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

            ViewBag.SubeYetkilisi = subeyetkili;
            List<Personel> personeller = dbContext.Personels.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Personeller = personeller;
            List<Models.Musteri> musteriler;
            if (SubeId == 0)
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
                musteriler = dbContext.Musteris.Where(x => x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();
            }
            else
                musteriler = dbContext.Musteris.Where(x => x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();

            ViewBag.Musteriler = musteriler;
            List<CekimPaketleri> paketler = dbContext.CekimPaketleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Paketler = paketler;
            List<RezervasyonTurleri> rezervasyonturleri = dbContext.RezervasyonTurleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.RezervasyonTurleri = rezervasyonturleri;
            List<RezervasyonEkHizmet> ekhizmetler = dbContext.RezervasyonEkHizmets.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.EkHizmetler = ekhizmetler;
            List<ZamanDilimleri> zamanlar = dbContext.ZamanDilimleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Zamanlar = zamanlar;
            List<Surecler> surecler = dbContext.Sureclers.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Surecler = surecler;
            // ---- Sözleşme Numarası
            long sozlesmeno = 0;
            List<Sozlesme> ss = dbContext.Sozlesmes.Select(x => x).ToList();
            if (ss.Count > 0)
            {
                sozlesmeno = dbContext.Sozlesmes.Max(x => x.Id);
            }
            sozlesmeno = sozlesmeno + 1;
            ViewBag.SozlesmeNo = FirmaId.ToString() + "00" + sozlesmeno.ToString();

            // ---- Müşreti Kodu
            long musterikodu = 0;
            List<Models.Musteri> mm = dbContext.Musteris.Select(x => x).ToList();
            if (mm.Count > 0)
            {
                musterikodu = dbContext.Musteris.Max(x => x.Id);
            }
            musterikodu = musterikodu + 1;
            ViewBag.Musterikodu = FirmaId.ToString() + "0" + musterikodu.ToString();
            #endregion
            #region BugünküRandevularPanel

            #endregion
            #region BugünGelecekOdemeler
            //if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
            //    ViewBag.BugunkuOdemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == "GelecekOdeme" && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false && x.Iptal == false).OrderByDescending(x => x.Id).Take(10).ToList();
            //else
            //    ViewBag.BugunkuOdemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.OdemeTuru == "GelecekOdeme" && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false && x.Iptal == false).OrderByDescending(x => x.Id).Take(10).ToList();
            if (SubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                ViewBag.BugunkuOdemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == "GelecekOdeme" && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false && x.Iptal == false).OrderByDescending(x => x.Id).Take(10).ToList();
            else
                ViewBag.BugunkuOdemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.OdemeTuru == "GelecekOdeme" && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false && x.Iptal == false).OrderByDescending(x => x.Id).Take(10).ToList();

            #endregion
            #region Günü Geçen Teklifleri / Opsiyonları İptal Et
            AyarlarRezervasyon teklifopsiyonayar = dbContext.AyarlarRezervasyons.FirstOrDefault(x => x.FirmaId == FirmaId);
            if (teklifopsiyonayar.GunuGecenTeklifOpsiyonIptal) // günü geçen teklifler silinsin şeklinde ayarlandı ise
            {
                List<Sozlesme> sz = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && (x.TeklifBit == true || x.OpsiyonBit == true) && x.Iptal == false && x.Sil == false && x.Aktif == true).ToList();
                if (sz.Count > 0)
                {
                    foreach (var item in sz)
                    {
                        if (item.OpsiyonTarihi < DateTime.Today)
                        {
                            item.Iptal = true;
                            item.Notlar = "Opsiyon tarihi dolan Rezervasyon sistem tarafından iptal edildi.";
                            item.DegistirenKullaniciId = 1;
                            item.DegistirmeTarih = DateTime.Now;

                            List<Odemeler> alinanodeme = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == "AlinanOdeme" && x.SozlesmeId == item.Id && x.Iptal == false && x.Sil == false && x.Aktif == true).ToList();
                            List<Odemeler> gelecekodeme = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == "GelecekOdeme" && x.SozlesmeId == item.Id && x.Iptal == false && x.Sil == false && x.Aktif == true).ToList();

                            if (alinanodeme.Count > 0)
                            {
                                // Müşteriye Teklifin idae edildiğine ve Ödemesini almasına ilişkin bilgilendirme SMS i gönderilecek.
                            }
                            if (gelecekodeme.Count > 0) // sözleşmeye ait gelecek ödemeler iptal edilecek.
                            {
                                foreach (var o in gelecekodeme)
                                {
                                    o.Sil = true;
                                    o.Aktif = false;
                                    o.Iptal = true;
                                    o.Notlar = "Opsiyon tarihi dolan Rezervasyon ve bu rezervasyona ait Gelecek Ödeme sistem tarafından iptal edildi/silindi.";
                                    o.DegistirenKullaniciId = 1;
                                    o.DegistirmeTarih = DateTime.Now;
                                }
                            }

                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == item.Id && x.Iptal == false && x.Sil == false && x.Aktif == true).ToList();
                            if (randevu.Count > 0)
                            {
                                foreach (var r in randevu)
                                {
                                    r.Sil = true;
                                    r.Aktif = false;
                                    r.Iptal = true;
                                    r.Aciklama = "Opsiyon tarihi dolan Rezervasyon ve bu rezervasyona ait randevular sistem tarafından iptal edildi/silindi.";
                                    r.DegistirenKullaniciId = 1;
                                    r.DegistirmeTarih = DateTime.Now;
                                }
                            }
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            #region CariHareket
            //if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
            //    ViewBag.CariHareketListesi = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
            //else
            //    ViewBag.CariHareketListesi = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
            if (SubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                ViewBag.CariHareketListesi = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
            else
                ViewBag.CariHareketListesi = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
            #endregion
            return View();
        }
        public ActionResult Index2()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            //ViewBag.UstMenu = "Dashboard";
            ViewBag.UstMenu = "Dashboard";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            int kalankontor = 0;
            int konturuyarimiktari = 0;
            string kalankonturuyarimesaji = "Kontor çok";
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 90 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.FirmaLogo = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true).Resim.ResimAdres;
            ViewBag.Firma = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true).FirmaAdi;
            ViewBag.AdSoyad = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KullaniciId && x.Sil == false && x.Aktif == true).AdSoyad;

            kalankontor = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).Bakiye;
            konturuyarimiktari = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).KonturUyariMiktari;
            if (kalankontor <= konturuyarimiktari)
            {
                kalankonturuyarimesaji = "Kontor az";
            }
            ViewBag.KalanKontorUyariMesaji = kalankonturuyarimesaji;
            ViewBag.KalanKontorUyariMesajiVer = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).KonturUyariVer;

            return View();
        }
        public ActionResult Index3()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            //ViewBag.UstMenu = "Dashboard";
            ViewBag.UstMenu = "Dashboard";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            int kalankontor = 0;
            int konturuyarimiktari = 0;
            string kalankonturuyarimesaji = "Kontor çok";
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 1 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");

            ViewBag.GelirGiderYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 90 && x.Aktif == true && x.Sil == false).KayitDuzenle;
            ViewBag.KullaniciYetkileri = dbContext.KullaniciYetkis.Where(x => x.KullaniciId == KullaniciId && x.Aktif == true && x.Sil == false).ToList();
            #region KasaPanelGelirGiderEkle
            ViewBag.GGTur = dbContext.GelirGiderTurleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();

            #endregion
            #region KasaPanel
            DateTime IlkTarih = DateTime.Today;
            DateTime SonTarih = DateTime.Today;
            List<GelirGider> gelir;
            List<GelirGider> gider;
            if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId & x.Tip == "Gelir" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId & x.Tip == "Gider" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();

            }
            else
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gelir" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gider" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
            }

            decimal toplamgelir = gelir.Sum(x => x.Tutar);
            decimal toplamgider = gider.Sum(x => x.Tutar);
            decimal kasa = toplamgelir - toplamgider;
            ViewBag.ToplamGelir = toplamgelir;
            ViewBag.ToplamGider = toplamgider;
            ViewBag.Kasa = kasa;
            #endregion
            #region GünlükİşlerPanel
            ViewBag.GunlukIsKategoriler = dbContext.GunlukIsKategoris.Where(x => x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId).ToList();
            ViewBag.Musteriler = dbContext.Musteris.Where(x => x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId).ToList();
            if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                ViewBag.GunlukIsler = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && (x.Tarih.Day == DateTime.Today.Day && x.Tarih.Month == DateTime.Today.Month && x.Tarih.Year == DateTime.Today.Year) && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
            else
                ViewBag.GunlukIsler = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && (x.Tarih.Day == DateTime.Today.Day && x.Tarih.Month == DateTime.Today.Month && x.Tarih.Year == DateTime.Today.Year) && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();

            #endregion
            #region RezervasyonTeklifleriPanel
            if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                ViewBag.BugunkuTeklifler = dbContext.Sozlesmes.Where(x => x.OpsiyonTarihi == DateTime.Today && (x.OpsiyonBit == true || x.TeklifBit == true) && x.Iptal == false && x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId).ToList();
            else
                ViewBag.BugunkuTeklifler = dbContext.Sozlesmes.Where(x => x.OpsiyonTarihi == DateTime.Today && (x.OpsiyonBit == true || x.TeklifBit == true) && x.Iptal == false && x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId && x.SubeId == SubeId).ToList();

            #endregion
            #region Kalan Kontor Uyarı
            kalankontor = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).Bakiye;
            konturuyarimiktari = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).KonturUyariMiktari;
            if (kalankontor <= konturuyarimiktari)
            {
                kalankonturuyarimesaji = "Kontor az";
            }
            ViewBag.KalanKontorUyariMesaji = kalankonturuyarimesaji;
            ViewBag.KalanKontorUyariMesajiVer = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).KonturUyariVer;
            #endregion
            #region YeniRezervasyonEkle
            Sube subeyetkili = dbContext.Subes.FirstOrDefault(x => x.Id == SubeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            ViewBag.SubeYetkilisi = subeyetkili;
            List<Personel> personeller = dbContext.Personels.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Personeller = personeller;
            List<Models.Musteri> musteriler = dbContext.Musteris.Where(x => x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Musteriler = musteriler;
            List<CekimPaketleri> paketler = dbContext.CekimPaketleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Paketler = paketler;
            List<RezervasyonTurleri> rezervasyonturleri = dbContext.RezervasyonTurleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.RezervasyonTurleri = rezervasyonturleri;
            List<RezervasyonEkHizmet> ekhizmetler = dbContext.RezervasyonEkHizmets.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.EkHizmetler = ekhizmetler;
            List<ZamanDilimleri> zamanlar = dbContext.ZamanDilimleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Zamanlar = zamanlar;
            List<Surecler> surecler = dbContext.Sureclers.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Surecler = surecler;
            // ---- Sözleşme Numarası
            long sozlesmeno = 0;
            List<Sozlesme> ss = dbContext.Sozlesmes.Select(x => x).ToList();
            if (ss.Count > 0)
            {
                sozlesmeno = dbContext.Sozlesmes.Max(x => x.Id);
            }
            sozlesmeno = sozlesmeno + 1;
            ViewBag.SozlesmeNo = FirmaId.ToString() + "00" + sozlesmeno.ToString();

            // ---- Müşreti Kodu
            long musterikodu = 0;
            List<Models.Musteri> mm = dbContext.Musteris.Select(x => x).ToList();
            if (mm.Count > 0)
            {
                musterikodu = dbContext.Musteris.Max(x => x.Id);
            }
            musterikodu = musterikodu + 1;
            ViewBag.Musterikodu = FirmaId.ToString() + "0" + musterikodu.ToString();
            #endregion
            #region BugünküRandevularPanel

            #endregion
            #region BugünGelecekOdemeler
            if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                ViewBag.BugunkuOdemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == "GelecekOdeme" && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false && x.Iptal == false).OrderByDescending(x => x.Id).Take(10).ToList();
            else
                ViewBag.BugunkuOdemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.OdemeTuru == "GelecekOdeme" && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false && x.Iptal == false).OrderByDescending(x => x.Id).Take(10).ToList();

            #endregion
            #region Günü Geçen Teklifleri / Opsiyonları İptal Et
            AyarlarRezervasyon teklifopsiyonayar = dbContext.AyarlarRezervasyons.FirstOrDefault(x => x.FirmaId == FirmaId);
            if (teklifopsiyonayar.GunuGecenTeklifOpsiyonIptal) // günü geçen teklifler silinsin şeklinde ayarlandı ise
            {
                List<Sozlesme> sz = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && (x.TeklifBit == true || x.OpsiyonBit == true) && x.Iptal == false && x.Sil == false && x.Aktif == true).ToList();
                if (sz.Count > 0)
                {
                    foreach (var item in sz)
                    {
                        if (item.OpsiyonTarihi < DateTime.Today)
                        {
                            item.Iptal = true;
                            item.Notlar = "Opsiyon tarihi dolan Rezervasyon sistem tarafından iptal edildi.";
                            item.DegistirenKullaniciId = 1;
                            item.DegistirmeTarih = DateTime.Now;

                            List<Odemeler> alinanodeme = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == "AlinanOdeme" && x.SozlesmeId == item.Id && x.Iptal == false && x.Sil == false && x.Aktif == true).ToList();
                            List<Odemeler> gelecekodeme = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == "GelecekOdeme" && x.SozlesmeId == item.Id && x.Iptal == false && x.Sil == false && x.Aktif == true).ToList();

                            if (alinanodeme.Count > 0)
                            {
                                // Müşteriye Teklifin idae edildiğine ve Ödemesini almasına ilişkin bilgilendirme SMS i gönderilecek.
                            }
                            if (gelecekodeme.Count > 0) // sözleşmeye ait gelecek ödemeler iptal edilecek.
                            {
                                foreach (var o in gelecekodeme)
                                {
                                    o.Sil = true;
                                    o.Aktif = false;
                                    o.Iptal = true;
                                    o.Notlar = "Opsiyon tarihi dolan Rezervasyon ve bu rezervasyona ait Gelecek Ödeme sistem tarafından iptal edildi/silindi.";
                                    o.DegistirenKullaniciId = 1;
                                    o.DegistirmeTarih = DateTime.Now;
                                }
                            }

                            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == item.Id && x.Iptal == false && x.Sil == false && x.Aktif == true).ToList();
                            if (randevu.Count > 0)
                            {
                                foreach (var r in randevu)
                                {
                                    r.Sil = true;
                                    r.Aktif = false;
                                    r.Iptal = true;
                                    r.Aciklama = "Opsiyon tarihi dolan Rezervasyon ve bu rezervasyona ait randevular sistem tarafından iptal edildi/silindi.";
                                    r.DegistirenKullaniciId = 1;
                                    r.DegistirmeTarih = DateTime.Now;
                                }
                            }
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            #region CariHareket
            if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                ViewBag.CariHareketListesi = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
            else
                ViewBag.CariHareketListesi = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
            #endregion
            return View();
        }
        //[ChildActionOnly]
        public PartialViewResult _SubeListesi() // Tüm Sayfa Actionlarında Kullanılacak
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Sube> subeler = new List<Sube>();
            List<SubeToKullanici> SubeKullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();

            foreach (var suku in SubeKullanici) // Giriş Yapan Kullanıcının yetkili olduğu AKTİF ŞEBELER çekiliyor.
            {
                Sube sb = dbContext.Subes.FirstOrDefault(x => x.Id == suku.SubeId && x.Aktif == true && x.Sil == false);
                if (sb != null)
                    subeler.Add(sb);
            }
            //string a= subeler.Select(x => x).First().SubeAdi;
            //ViewBag.AktifSubeAdi = a;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == KullaniciId && x.SayfaId == 19 && x.Sil == false && x.Aktif == true).SayfaYetki;

            return PartialView(subeler);
        }

        public PartialViewResult _SubeListesi2() // Tüm Sayfa Actionlarında Kullanılacak
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Sube> subeler = new List<Sube>();
            List<SubeToKullanici> SubeKullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();

            foreach (var suku in SubeKullanici) // Giriş Yapan Kullanıcının yetkili olduğu AKTİF ŞEBELER çekiliyor.
            {
                Sube sb = dbContext.Subes.FirstOrDefault(x => x.Id == suku.SubeId && x.Aktif == true && x.Sil == false);
                if (sb != null)
                    subeler.Add(sb);
            }
            //string a= subeler.Select(x => x).First().SubeAdi;
            //ViewBag.AktifSubeAdi = a;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == KullaniciId && x.SayfaId == 19 && x.Sil == false && x.Aktif == true).SayfaYetki;
            return PartialView(subeler);
        }

        public PartialViewResult _SubeListesi3() // Tüm Sayfa Actionlarında Kullanılacak
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Sube> subeler = new List<Sube>();
            List<SubeToKullanici> SubeKullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();

            foreach (var suku in SubeKullanici) // Giriş Yapan Kullanıcının yetkili olduğu AKTİF ŞEBELER çekiliyor.
            {
                Sube sb = dbContext.Subes.FirstOrDefault(x => x.Id == suku.SubeId && x.Aktif == true && x.Sil == false);
                if (sb != null)
                    subeler.Add(sb);
            }
            //string a= subeler.Select(x => x).First().SubeAdi;
            //ViewBag.AktifSubeAdi = a;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.KullaniciId == KullaniciId && x.SayfaId == 19 && x.Sil == false && x.Aktif == true).SayfaYetki;

            return PartialView(subeler);
        }

        public PartialViewResult _LisansSuresi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            Lisanslar lisans = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId);
            //TimeSpan toplamsure = lisans.LisansBitisTarihi - lisans.LisansBaslangicTarihi;
            TimeSpan toplamsure = lisans.LisansBitisTarihi - lisans.LisansYenilemeTarihi; // Başlangıç tarihinden olduğunda toplam gün çok büyük oluyor.
            TimeSpan kalangun = lisans.LisansBitisTarihi - DateTime.Now;
            //TimeSpan gecengun = DateTime.Now - lisans.LisansBaslangicTarihi;
            TimeSpan gecengun = DateTime.Now - lisans.LisansYenilemeTarihi; // Başlangıç tarihinden olduğunda toplam gün çok büyük oluyor.
            int toplam = toplamsure.Days;
            int kalan = kalangun.Days;
            int gecen = gecengun.Days;
            ViewBag.toplam = toplam;
            ViewBag.kalan = kalan;
            ViewBag.gecen = gecen;
            return PartialView();
        }

        public PartialViewResult _LisansSuresi2()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            Lisanslar lisans = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId);
            //TimeSpan toplamsure = lisans.LisansBitisTarihi - lisans.LisansBaslangicTarihi;
            TimeSpan toplamsure = lisans.LisansBitisTarihi - lisans.LisansYenilemeTarihi; // Başlangıç tarihinden olduğunda toplam gün çok büyük oluyor.
            TimeSpan kalangun = lisans.LisansBitisTarihi - DateTime.Now;
            //TimeSpan gecengun = DateTime.Now - lisans.LisansBaslangicTarihi;
            TimeSpan gecengun = DateTime.Now - lisans.LisansYenilemeTarihi; // Başlangıç tarihinden olduğunda toplam gün çok büyük oluyor.
            int toplam = toplamsure.Days;
            int kalan = kalangun.Days;
            int gecen = gecengun.Days;
            ViewBag.toplam = toplam;
            ViewBag.kalan = kalan;
            ViewBag.gecen = gecen;
            return PartialView();
        }

        public PartialViewResult _SMSKontur()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            if (FirmaId == 1)
            {
                string username = "kamilsen";
                string password = "442Bir726";
                string PostAddress;
                try
                {
                    PostAddress = "http://panel4.ekomesaj.com:9587/user/credit";

                    WebClient wUpload = new WebClient();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PostAddress);

                    request.ContentType = "application/json; charset=utf-8";
                    request.Method = "GET";
                    string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

                    request.Headers.Add("Authorization", "Basic " + svcCredentials);

                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    string result;
                    var streamReader = new StreamReader(httpResponse.GetResponseStream());
                    result = streamReader.ReadToEnd();
                    dynamic Sonuc_Kredi = JsonConvert.DeserializeObject(result);
                    int Kredi = Convert.ToInt32(Sonuc_Kredi.data.credit);
                    ViewBag.KalanKontur = Kredi;
                    ViewBag.ToplamKontur = Kredi;
                }
                catch (Exception)
                {
                    ViewBag.KalanKontur = 0;
                    ViewBag.ToplamKontur = 0;
                }
            }
            else
            {
                SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);

                ViewBag.KalanKontur = smsbakiye.Bakiye;
                ViewBag.ToplamKontur = smsbakiye.ToplamKontur;

            }
            return PartialView();
        }

        public PartialViewResult _SMSKontur2()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);

            ViewBag.KalanKontur = smsbakiye.Bakiye;
            ViewBag.ToplamKontur = smsbakiye.ToplamKontur;
            return PartialView();
        }

        public PartialViewResult _OkunmamisMesajlar()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            /* OkunduBit false olan ve durumu "KonuKapandı veya İptal olmayan Mesajlar"*/
            List<MusteriMesaj> okunmamismesajlar = dbContext.MusteriMesajs.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.OkunduBit == false && !(x.Durum == "5" || x.Durum == "7")).ToList();
            return PartialView(okunmamismesajlar);
        }

        public PartialViewResult _Destek()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            /* OkunduBit false olan ve durumu "KonuKapandı veya İptal olmayan Mesajlar"*/
            List<DestekTalepleriDetay> okunmamiscevaplar = dbContext.DestekTalepleriDetays.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.FirmaCevap == true && x.MusteriCevap == false && x.OkunduBit == false).ToList();



            return PartialView(okunmamiscevaplar);
        }

        [HttpPost]
        public string SubeDegistir(long id)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            if (id == 0)
            {
                Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId);
                Session["AktifSubeAdi"] = "Tüm Şubeler";
                Session["AktifSubeId"] = 0;
                bool dashboardYetki1 = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == KullaniciId && x.SayfaId == 1).SayfaYetki;
                if (dashboardYetki1)
                    return "Index";
                else
                    return "Index2";
            }
            Sube sb = dbContext.Subes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            // Giriş Yapan Kullanıcının Yetkili olduğu şubeler
            Session["AktifSubeAdi"] = sb.SubeAdi;
            Session["AktifSubeId"] = sb.Id;
            bool dashboardYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.KullaniciId == KullaniciId && x.SayfaId == 1).SayfaYetki;
            if (dashboardYetki)
                return "Index";
            else
                return "Index2";
        }

        public ActionResult SmsGonder()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            //ViewBag.UstMenu = "Dashboard";
            ViewBag.UstMenu = "SMS Gönder";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 13 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            // Bu firmaya ait tüm TempSmsListe Tablosu Silinecek, Yeni kayıtlar sonra Eklenecek.
            List<TempSmsGonderListe> smsliste = dbContext.TempSmsGonderListes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            if (smsliste != null)
            {
                foreach (var item in smsliste)
                {
                    item.Aktif = false;
                    item.Sil = true;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Hızlı Sms Gönderme için Telefon ekleme esnasında oluşan hata";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = false, Mesaj = "Eski Liste Silinemedi.", JsonRequestBehavior.AllowGet });

                }
            }
            List<RehberGrup> rehberGrup = dbContext.RehberGrups.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.RehberGrup = rehberGrup;
            return View();
        }

        [HttpPost]
        public ActionResult SmsGonderListeEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "SMS Gönder";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            string AdSoyad = Request["AdSoyad"];
            string CepTel = Request["TelefonNo"];
            string TelefonFormatli = CepTel;
            TelefonDüzelt td = new TelefonDüzelt();
            if (!string.IsNullOrEmpty(CepTel)) { CepTel = td.düzelt(CepTel); }
            TempSmsGonderListe SmsListe = new TempSmsGonderListe();
            SmsListe.FirmaId = FirmaId;
            SmsListe.AdSoyad = AdSoyad;
            SmsListe.Telefon = CepTel;
            SmsListe.TelefonFormatli = TelefonFormatli;
            SmsListe.Mesaj = "";
            SmsListe.Durum = "";
            SmsListe.Aktif = true;
            SmsListe.Sil = false;
            SmsListe.OlusturanKullaniciId = KullaniciId;
            SmsListe.OlusturmaTarih = DateTime.Now;
            SmsListe.DegistirenKullaniciId = KullaniciId;
            SmsListe.DegistirmeTarih = DateTime.Now;
            dbContext.TempSmsGonderListes.Add(SmsListe);
            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Hızlı Sms Gönderme için Telefon ekleme esnasında oluşan hata";
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
            List<TempSmsGonderListe> smsliste = dbContext.TempSmsGonderListes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            var smstelefonlistesi = smsliste.Select(m => new
            {
                Id = m.Id,
                AdSoyad = m.AdSoyad,
                Telefon = m.Telefon,
                TelefonFormatli = m.TelefonFormatli
            }).ToList();
            return Json(new { Sonuc = true, Mesaj = "", data = smstelefonlistesi }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult SmsListeTelefonSil(int? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            ViewBag.UstMenu = "SMS Gönder";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            TempSmsGonderListe SmsListeSil = dbContext.TempSmsGonderListes.FirstOrDefault(x => x.Id == id && x.Sil == false && x.Aktif == true);
            if (SmsListeSil != null)
            {
                SmsListeSil.Aktif = false;
                SmsListeSil.Sil = true;
                SmsListeSil.DegistirenKullaniciId = KullaniciId;
                SmsListeSil.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Hızlı Sms Gönderme için Telefon ekleme esnasında oluşan hata";
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

            List<TempSmsGonderListe> smsliste = dbContext.TempSmsGonderListes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            var smstelefonlistesi = smsliste.Select(m => new
            {
                Id = m.Id,
                AdSoyad = m.AdSoyad,
                Telefon = m.Telefon,
                TelefonFormatli = m.TelefonFormatli
            }).ToList();
            return Json(new { Sonuc = true, Mesaj = "", data = smstelefonlistesi }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult HizliSmsGonder()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "SMS Gönder";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string SMSMetin = Request["MesajMetni"];
            SMSMetin = SMSMetin.Replace("\r\n", "");
            SMSMetin = SMSMetin.Replace("\n", "");
            string TelefonListId = Request["TelefonListId"];
            string[] ListeId = TelefonListId.Split(',');
            //List<string> AdSoyadList = new List<string>();
            //List<string> TelefonNoList = new List<string>();
            string Durum = "";
            foreach (var item in ListeId)
            {
                long id = Convert.ToInt64(item);
                TempSmsGonderListe smslist = dbContext.TempSmsGonderListes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                string YeniSMSMetin = SMSMetin.Replace("{AdSoyad}", smslist.AdSoyad);
                string sonuc = SMSGonder.Gonder_AtakSms(YeniSMSMetin, smslist.Telefon, FirmaId, SubeId, KullaniciId);
                if (sonuc == "Mesaj Başarıyla Göderildi")
                    Durum = "Başarılı";
                else
                    Durum = "Başarısız";

                smslist.Mesaj = SMSMetin;
                smslist.Durum = Durum;
                smslist.DegistirenKullaniciId = KullaniciId;
                smslist.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Hızlı Sms Gönderme - SMS gönderimi sonrasında Temp data güncellemede hata";
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
            List<TempSmsGonderListe> smsliste = dbContext.TempSmsGonderListes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            var smstelefonlistesi = smsliste.Select(m => new
            {
                Id = m.Id,
                AdSoyad = m.AdSoyad,
                Telefon = m.Telefon,
                TelefonFormatli = m.TelefonFormatli,
                MesajIcerigi = m.Mesaj,
                Durum = m.Durum
            }).ToList();
            return Json(new { Sonuc = true, Mesaj = "", data = smstelefonlistesi }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RehberGetir(int id)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<TelefonRehberi> rehber = dbContext.TelefonRehberis.Where(x => x.RehberGrupId == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            var Rehber = rehber.Select(m => new
            {
                Id = m.Id,
                //Grup = m.RehberGrup,
                AdSoyad = m.AdSoyad,
                CepTel = m.CepTel1,
                SmsKabul = m.SmsKabul
            }).ToList(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.

            if (rehber.Count > 0)
            {
                return Json(new { Sonuc = true, Mesaj = "", data = Rehber }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Sonuc = false, Mesaj = "", data = Rehber }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult RehberdenHizliSmsGonder()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "SMS Gönder";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string SMSMetin = Request["MesajMetniRehber"];
            SMSMetin = SMSMetin.Replace("\r\n", "");
            SMSMetin = SMSMetin.Replace("\n", "");
            string TelefonListId = Request["RehberIdler"];
            string[] ListeId = TelefonListId.Split(',');
            //List<string> AdSoyadList = new List<string>();
            //List<string> TelefonNoList = new List<string>();
            string Durum = "";
            TempSmsGonderListe liste = new TempSmsGonderListe();
            foreach (var item in ListeId)
            {
                long id = Convert.ToInt64(item);

                TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                liste.FirmaId = FirmaId;
                liste.AdSoyad = rehber.AdSoyad;
                liste.Telefon = rehber.CepTel1;
                liste.TelefonFormatli = String.Format("{0:(###) ### - ####}", rehber.CepTel1);
                liste.Mesaj = "";
                liste.Durum = "";
                liste.Aktif = true;
                liste.Sil = false;
                liste.OlusturanKullaniciId = KullaniciId;
                liste.OlusturmaTarih = DateTime.Now;
                liste.DegistirenKullaniciId = KullaniciId;
                liste.DegistirmeTarih = DateTime.Now;
                dbContext.TempSmsGonderListes.Add(liste);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Rehberden SMS Gönderme - Rehber Kayıydı Temp Listeye Ekleme esnasında oluşan hata";
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

            List<TempSmsGonderListe> TempListe = dbContext.TempSmsGonderListes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();

            foreach (var templiste in TempListe)
            {
                //TempSmsGonderListe smslist = dbContext.TempSmsGonderListes.FirstOrDefault(x => x.Id == templiste.Id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                string YeniSMSMetin = SMSMetin.Replace("{AdSoyad}", templiste.AdSoyad);
                string sonuc = SMSGonder.Gonder_AtakSms(YeniSMSMetin, templiste.Telefon, FirmaId, SubeId, KullaniciId);
                if (sonuc == "Mesaj Başarıyla Göderildi")
                    Durum = "Başarılı";
                else
                    Durum = "Başarısız";

                templiste.Mesaj = SMSMetin;
                templiste.Durum = Durum;
                templiste.DegistirenKullaniciId = KullaniciId;
                templiste.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Hızlı Sms Gönderme - SMS gönderimi sonrasında Temp data güncellemede hata";
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

            List<TempSmsGonderListe> smsliste = dbContext.TempSmsGonderListes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            var smstelefonlistesi = smsliste.Select(m => new
            {
                Id = m.Id,
                AdSoyad = m.AdSoyad,
                Telefon = m.Telefon,
                TelefonFormatli = m.TelefonFormatli,
                MesajIcerigi = m.Mesaj,
                Durum = m.Durum
            }).ToList();
            return Json(new { Sonuc = true, Mesaj = "", data = smstelefonlistesi }, JsonRequestBehavior.AllowGet);
        }

    }
}