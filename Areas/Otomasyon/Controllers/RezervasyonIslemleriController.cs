using Rotativa;
using FotografciTakipWeb.App_Settings;
using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Collections;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class RezervasyonIslemleriController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/RezervasyonIslem
        public ActionResult RezervasyonTakvimi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Rezervasyon İşlemleri";
            ViewBag.AltMenu = "Rezervasyon Takvimi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 21 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 21 && x.Aktif == true && x.Sil == false);

            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            ViewBag.SubeKullanici = subekullanici;
            List<Sube> subeler = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Subeler = subeler;
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
            List<Surecler> surecler = dbContext.Sureclers.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).OrderBy(x => x.Sira).ToList();
            ViewBag.Surecler = surecler;
            long sozlesmeno = 0;
            List<Sozlesme> ss = dbContext.Sozlesmes.Select(x => x).ToList();
            if (ss.Count > 0)
                sozlesmeno = dbContext.Sozlesmes.Max(x => x.Id);
            sozlesmeno = sozlesmeno + 1;
            ViewBag.SozlesmeNo = FirmaId.ToString() + "00" + sozlesmeno.ToString();
            // ---- Sözleşme Numarası
            // ---- Müşreti Kodu
            long musterikodu = 0;
            List<Models.Musteri> mm = dbContext.Musteris.Select(x => x).ToList();
            if (mm.Count > 0)
                musterikodu = dbContext.Musteris.Max(x => x.Id);
            musterikodu = musterikodu + 1;
            ViewBag.Musterikodu = FirmaId.ToString() + "0" + musterikodu.ToString();
            return View();

        }
        // Takvim için veri gönderme
        public ActionResult TakvimRandevular()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            List<Randevu> randevular;

            Sube akitifSube = dbContext.Subes.FirstOrDefault(x => x.Id == AktifSubeId);

            //if (RolId == 2) // Giriş Yapan Kullanıcı Firma Yetkilis ise
            //    randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            //else
            //    randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false).ToList();
            if (AktifSubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            else
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false).ToList();

            var rndv = randevular.Select(m => new
            {
                Id = m.Id,
                Baslik = m.Baslik, // Rezervasyon türü
                Aciklama = m.Aciklama, // Çekim yapılacak kişiler + Çekim Yeri
                Cekim = m.CekimRandevu, // Çekim randevusu
                //Personel = m.Personel.AdiSoyadi, // Görevli Personel
                //Musteri = m.Sozlesme.Musteri.AdiSoyadi,
                SozlesmeNo = m.Sozlesme.SozlesmeNo,
                BaslangicTarih = m.Baslangic,
                BaslangicSaat = m.Baslangic.ToShortTimeString(),
                BitisTarih = m.Bitis,
                BitisSaat = m.Bitis.ToShortTimeString(),
                Iptal = m.Iptal,
                Sinif = m.RandevuGorunum.Gorunum
            });
            return Json(new { data = rndv }, JsonRequestBehavior.AllowGet);
        }
        // Takvim için veri gönderme
        #region Yeni Rezervasyon İşlemleri
        public ActionResult YeniRezervasyonEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            ViewBag.UstMenu = "Rezervasyon İşlemleri";
            ViewBag.AltMenu = "Yeni Rezervasyon Ekle";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
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
            AyarlarSozlesmeCikti sozlesmeayar = dbContext.AyarlarSozlesmeCiktis.FirstOrDefault(x => x.FirmaId == FirmaId);
            ViewBag.SozlesmeAyar = sozlesmeayar;
            string Il = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId).Il.Il1;
            string Ilce = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId).Ilce.Ilce1;
            string Adres = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId).Adres;

            ViewBag.FirmaAdres = Adres + " " + Ilce + " / " + Il;
            // ---- Sözleşme Numarası
            long sozlesmeId = 0;
            string sno;
            List<Sozlesme> ss = dbContext.Sozlesmes.Select(x => x).ToList();
            if (ss.Count > 0)
            {
                sozlesmeId = dbContext.Sozlesmes.Max(x => x.Id);
            }
            sozlesmeId = sozlesmeId + 1;
            sno = FirmaId.ToString() + "00" + sozlesmeId.ToString();
            ViewBag.SozlesmeNo = sno;

            // ---- Müşreti Kodu
            long musteriId = 0;
            string mkod;
            List<Models.Musteri> mm = dbContext.Musteris.Select(x => x).ToList();
            if (mm.Count > 0)
            {
                musteriId = dbContext.Musteris.Max(x => x.Id);
            }
            musteriId = musteriId + 1;
            mkod = FirmaId.ToString() + "0" + musteriId.ToString();
            ViewBag.Musterikodu = mkod;
            return View();

        }
        [ValidateInput(false)]
        public ActionResult YeniRezervasyonEkleTakvim(string id)
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                string[] tarihsaat = id.Split('T');
                string Tarih = tarihsaat[0];
                if (tarihsaat.Length > 1)
                {
                    string Baslangic = tarihsaat[1].Replace("-", ":");
                    Baslangic = Baslangic.Remove(5, 3);
                    ViewBag.TakvimBaslangic = Baslangic;
                }

                DateTime TakvimTarih = Convert.ToDateTime(Tarih);
                ViewBag.TakvimTarih = TakvimTarih;
                ViewBag.UstMenu = "Rezervasyon İşlemleri";
                ViewBag.AltMenu = "Yeni Rezervasyon Ekle";
                long FirmaId = Convert.ToInt64(Session["FirmaId"]);
                long KullaniciId = Convert.ToInt64(Session["Id"]);
                long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
                if (SubeId == 0)
                    SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
                Sube subeyetkili = dbContext.Subes.FirstOrDefault(x => x.Id == SubeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                ViewBag.SubeYetkilisi = subeyetkili;
                List<Personel> personeller = dbContext.Personels.Where(x => x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();
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
                List<KullaniciYetki> ky = dbContext.KullaniciYetkis.Where(x => x.FirmaId == FirmaId && x.KullaniciId == KullaniciId).ToList();
                ViewBag.KullaniciYetkileri = ky;
                AyarlarSozlesmeCikti sozlesmeayar = dbContext.AyarlarSozlesmeCiktis.FirstOrDefault(x => x.FirmaId == FirmaId);
                ViewBag.SozlesmeAyar = sozlesmeayar;
                string Il = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId).Il.Il1;
                string Ilce = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId).Ilce.Ilce1;
                string Adres = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId).Adres;

                ViewBag.FirmaAdres = Adres + " " + Ilce + " / " + Il;
                // ---- Sözleşme Numarası
                long sozlesmeId = 0;
                string sno;
                List<Sozlesme> ss = dbContext.Sozlesmes.Select(x => x).ToList();
                if (ss.Count > 0)
                {
                    sozlesmeId = dbContext.Sozlesmes.Max(x => x.Id);
                }
                sozlesmeId = sozlesmeId + 1;
                sno = FirmaId.ToString() + "00" + sozlesmeId.ToString();
                ViewBag.SozlesmeNo = sno;

                // ---- Müşreti Kodu
                long musteriId = 0;
                string mkod;
                List<Models.Musteri> mm = dbContext.Musteris.Select(x => x).ToList();
                if (mm.Count > 0)
                {
                    musteriId = dbContext.Musteris.Max(x => x.Id);
                }
                musteriId = musteriId + 1;
                mkod = FirmaId.ToString() + "0" + musteriId.ToString();
                ViewBag.Musterikodu = mkod;
                return View();
            }
            else
            {
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult SozlesmeEkle()
        {
            if (Session != null && Session["AdSoyad"] != null && Session["AdSoyad"].ToString() != null)
            {
                long FirmaId = Convert.ToInt64(Session["FirmaId"]);
                long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
                if (SubeId == 0)
                    SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
                long KullaniciId = Convert.ToInt64(Session["Id"]);
                DateTime SozlesmeTarihi = Convert.ToDateTime(Request["SozlesmeTarihi"]);
                long SozlesmeNo = Convert.ToInt64(Request["SozlesmeNo"]);
                string Yetkili = Request["Yetkili"];
                string PersonelIdler = Request["PersonelIdArray"];
                string[] PersonelIdArray = Request["PersonelIdArray"].Split(',');
                long RezervasyonTurId = Convert.ToInt64(Request["RezervasyonTurId"]);
                string OrganizasyonYeri = Request["OrganizasyonYeri"];
                long RezervasyonSekliId = Convert.ToInt64(Request["RezervasyonSekliId"]);
                DateTime OpsiyonTarihi = Convert.ToDateTime(Request["OpsiyonTarihi"]);
                DateTime RezervasyonTarihi = Convert.ToDateTime(Request["RezervasyonTarihi"]);
                long ZamanDilimiId = Convert.ToInt64(Request["ZamanDilimiId"]);
                string BaslangicZaman = Request["BaslangicZaman"];
                string BitisZaman = Request["BitisZaman"];
                TimeSpan baslangicsaat = TimeSpan.Parse(BaslangicZaman);
                TimeSpan bitissaat = TimeSpan.Parse(BitisZaman);
                DateTime randevubaslangic; // Randevu tablosu için
                DateTime randevubitis; // Randevu tablosu için
                                       //DateTime randevuopsiyon; // Randevu tablosu için
                Sozlesme sozlesme = new Sozlesme();
                Randevu randevu = new Randevu();
                RandevuToPersonel rtop = new RandevuToPersonel();
                SozlesmeTarihi = SozlesmeTarihi.AddHours(DateTime.Now.Hour);
                SozlesmeTarihi = SozlesmeTarihi.AddMinutes(DateTime.Now.Minute);

                sozlesme.FirmaId = FirmaId;
                sozlesme.SubeId = SubeId;
                sozlesme.SozlesmeNo = SozlesmeNo;
                sozlesme.SozlesmeTarihi = SozlesmeTarihi;
                sozlesme.YetkiliPersonel = Yetkili;
                sozlesme.GorevliPersonellerId = PersonelIdler;
                sozlesme.GorevliPersonelId = null;
                sozlesme.RezervasyonTarihi = RezervasyonTarihi;
                sozlesme.RezervasyonTurId = RezervasyonTurId;
                sozlesme.FotgrafHatirlatma = false;
                sozlesme.PaketlerFiyat = 0;
                sozlesme.KDVDahil = true;
                sozlesme.KilitBit = false;
                sozlesme.Bitti = false;
                sozlesme.Iptal = false;
                if (ZamanDilimiId == 0)
                {
                    sozlesme.BaslangicSaat = baslangicsaat;
                    sozlesme.BitisSaat = bitissaat;
                    randevubaslangic = RezervasyonTarihi.Add(baslangicsaat);
                    randevubitis = RezervasyonTarihi.Add(bitissaat);
                }
                else
                {
                    ZamanDilimleri zmn = dbContext.ZamanDilimleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == ZamanDilimiId);
                    sozlesme.BaslangicSaat = zmn.BaslangicZaman;
                    sozlesme.BitisSaat = zmn.BitisZaman;
                    randevubaslangic = RezervasyonTarihi.Add(zmn.BaslangicZaman);
                    randevubitis = RezervasyonTarihi.Add(zmn.BitisZaman);
                }
                if (RezervasyonSekliId == 1) // Rezervasyon Şekli = "Kesin Rezervasyon"
                {
                    sozlesme.Durum = "Kesin Rezervasyon";
                    sozlesme.KesinRezervasyonBit = true;
                    sozlesme.OpsiyonBit = false;
                    sozlesme.TeklifBit = false;
                }
                else if (RezervasyonSekliId == 2) // Rezervasyon Şekli = "Opsiyonlu Rezervasyon"
                {
                    sozlesme.Durum = "Opsiyonlu Rezervasyon";
                    sozlesme.KesinRezervasyonBit = false;
                    sozlesme.OpsiyonBit = true;
                    sozlesme.TeklifBit = false;
                    sozlesme.OpsiyonTarihi = OpsiyonTarihi;
                    randevu.Opsiyon = OpsiyonTarihi;
                }
                else if (RezervasyonSekliId == 3) // Rezervasyon Şekli = "Teklif"
                {
                    sozlesme.Durum = "Teklif";
                    sozlesme.KesinRezervasyonBit = false;
                    sozlesme.OpsiyonBit = false;
                    sozlesme.TeklifBit = true;
                    sozlesme.OpsiyonTarihi = OpsiyonTarihi;
                    randevu.Opsiyon = OpsiyonTarihi;
                }
                sozlesme.FotografSecimDurum = "0";
                sozlesme.FotografSecimDurumTarihi = DateTime.Now;
                sozlesme.OrganizasyonYeri = OrganizasyonYeri;
                sozlesme.Aktif = true;
                sozlesme.Sil = false;
                sozlesme.OlusturanKullaniciId = KullaniciId;
                sozlesme.OlusturmaTarih = DateTime.Now;
                sozlesme.DegistirenKullaniciId = KullaniciId;
                sozlesme.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.Sozlesmes.Add(sozlesme);
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Yeni Rezervasyon Ekle, Rezervasyon Bilgileri Kaydet";
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

                RezervasyonTurleri tur = dbContext.RezervasyonTurleris.FirstOrDefault(x => x.Id == RezervasyonTurId);

                try
                {
                    randevu.FirmaId = FirmaId;
                    randevu.SubeId = SubeId;
                    randevu.SozlesmeId = sozlesme.Id;
                    randevu.Baslangic = randevubaslangic;
                    randevu.Bitis = randevubitis;
                    randevu.Aciklama = "Çekim Yeri: "+OrganizasyonYeri;
                    randevu.Baslik = tur.RezervasyonTuru;
                    randevu.RandevuGorunumId = tur.RandevuGorunumId;
                    randevu.RezervasyonTurId = RezervasyonTurId;
                    randevu.GorevliPersonellerId = PersonelIdler;
                    randevu.CekimRandevu = false;
                    randevu.Aktif = true;
                    randevu.Sil = false;
                    randevu.OlusturanKullaniciId = KullaniciId;
                    randevu.OlusturmaTarih = DateTime.Now;
                    randevu.DegistirenKullaniciId = KullaniciId;
                    randevu.DegistirmeTarih = DateTime.Now;
                    dbContext.Randevus.Add(randevu);
                    dbContext.SaveChanges();

                    // RandevutoPersonel tablosunaseçilen personel eklenecek
                    foreach (var pId in PersonelIdArray)
                    {
                        rtop.PersonelId = Convert.ToInt64(pId);
                        rtop.RandevuId = randevu.Id;
                        rtop.Aktif = true;
                        rtop.Sil = false;
                        rtop.Iptal = false;
                        rtop.OlusturanKullaniciId = KullaniciId;
                        rtop.OlusturmaTarih = DateTime.Now;
                        rtop.DegistirenKullaniciId = KullaniciId;
                        rtop.DegistirmeTarih = DateTime.Now;
                        dbContext.RandevuToPersonels.Add(rtop);
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Yeni Rezervasyon Ekle, Personel Ekle";
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

                    return Json(new { Sonuc = true, sozlesmeId = sozlesme.Id, sozlesmeNo = sozlesme.SozlesmeNo, randevuId = randevu.Id, Mesaj = "Kayıt Başarılı", JsonRequestBehavior.AllowGet });
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Yeni Rezervasyon Ekle, Randevu Ekle Kaydet";
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
                return RedirectToAction("GirisYap", "Giris");
            }
        }
        [HttpPost]
        public ActionResult SozlesmeGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            DateTime SozlesmeTarihi = Convert.ToDateTime(Request["SozlesmeTarihi"]);
            long SozlesmeNo = Convert.ToInt64(Request["SozlesmeNo"]);
            //string Yetkili = Request["Yetkili"];
            //long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            string PersonelIdler = Request["PersonelIdArray"];
            string[] PersonelIdArray = Request["PersonelIdArray"].Split(',');
            long RezervasyonTurId = Convert.ToInt64(Request["RezervasyonTurId"]);
            string OrganizasyonYeri = Request["OrganizasyonYeri"];
            long RezervasyonSekliId = Convert.ToInt64(Request["RezervasyonSekliId"]);

            DateTime OpsiyonTarihi = new DateTime();
            if (!string.IsNullOrEmpty(Request["OpsiyonTarihi"]))
                OpsiyonTarihi = Convert.ToDateTime(Request["OpsiyonTarihi"]);
            
            DateTime RezervasyonTarihi = Convert.ToDateTime(Request["RezervasyonTarihi"]);
            long ZamanDilimiId = Convert.ToInt64(Request["ZamanDilimiId"]);
            string BaslangicZaman = Request["BaslangicZaman"];
            string BitisZaman = Request["BitisZaman"];
            TimeSpan baslangicsaat = TimeSpan.Parse(BaslangicZaman);
            TimeSpan bitissaat = TimeSpan.Parse(BitisZaman);
            DateTime randevubaslangic; // Randevu tablosu için
            DateTime randevubitis; // Randevu tablosu için
            Sozlesme sozlesme = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (sozlesme == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            Randevu randevu = dbContext.Randevus.FirstOrDefault(x => x.SozlesmeId == id && x.CekimRandevu == false);

            sozlesme.SozlesmeTarihi = SozlesmeTarihi;
            //sozlesme.YetkiliPersonel = Yetkili;
            sozlesme.GorevliPersonellerId = PersonelIdler;
            sozlesme.RezervasyonTarihi = RezervasyonTarihi;
            sozlesme.RezervasyonTurId = RezervasyonTurId;
            if (ZamanDilimiId == 0)
            {
                sozlesme.BaslangicSaat = baslangicsaat;
                sozlesme.BitisSaat = bitissaat;
                randevubaslangic = RezervasyonTarihi.Add(baslangicsaat);
                randevubitis = RezervasyonTarihi.Add(bitissaat);
            }
            else
            {
                ZamanDilimleri zmn = dbContext.ZamanDilimleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == ZamanDilimiId);
                sozlesme.BaslangicSaat = zmn.BaslangicZaman;
                sozlesme.BitisSaat = zmn.BitisZaman;
                randevubaslangic = RezervasyonTarihi.Add(zmn.BaslangicZaman);
                randevubitis = RezervasyonTarihi.Add(zmn.BitisZaman);
            }
            if (RezervasyonSekliId == 1) // Rezervasyon Şekli = "Kesin Rezervasyon"
            {
                sozlesme.Durum = "Kesin Rezervasyon";
                sozlesme.KesinRezervasyonBit = true;
                sozlesme.OpsiyonBit = false;
                sozlesme.TeklifBit = false;
            }
            else if (RezervasyonSekliId == 2) // Rezervasyon Şekli = "Opsiyonlu Rezervasyon"
            {
                sozlesme.Durum = "Opsiyonlu Rezervasyon";
                sozlesme.KesinRezervasyonBit = false;
                sozlesme.OpsiyonBit = true;
                sozlesme.TeklifBit = false;
                sozlesme.OpsiyonTarihi = OpsiyonTarihi;
                randevu.Opsiyon = OpsiyonTarihi;
            }
            else if (RezervasyonSekliId == 3) // Rezervasyon Şekli = "Teklif"
            {
                sozlesme.Durum = "Teklif";
                sozlesme.KesinRezervasyonBit = false;
                sozlesme.OpsiyonBit = false;
                sozlesme.TeklifBit = true;
                sozlesme.OpsiyonTarihi = OpsiyonTarihi;
                randevu.Opsiyon = OpsiyonTarihi;
            }
            sozlesme.OrganizasyonYeri = OrganizasyonYeri;
            sozlesme.DegistirenKullaniciId = KullaniciId;
            sozlesme.DegistirmeTarih = DateTime.Now;

            RezervasyonTurleri tur = dbContext.RezervasyonTurleris.FirstOrDefault(x => x.Id == RezervasyonTurId);
            RandevuToPersonel rtop = new RandevuToPersonel();
            // RandevutoPersonel tablosunaseçilen personel eklenecek

            randevu.Baslangic = randevubaslangic;
            randevu.Bitis = randevubitis;
            randevu.Baslik = tur.RezervasyonTuru;
            randevu.RandevuGorunumId = tur.RandevuGorunumId;
            //randevu.PersonelId = PersonelId;
            randevu.DegistirenKullaniciId = KullaniciId;
            randevu.DegistirmeTarih = DateTime.Now;

            //Eski Randevudaki personeller silinip yeniden ekleniyor.

            List<RandevuToPersonel> randevudakipersoneller = dbContext.RandevuToPersonels.Where(x => x.RandevuId == randevu.Id).ToList();
            dbContext.RandevuToPersonels.RemoveRange(randevudakipersoneller);

            try
            {
                dbContext.SaveChanges();
                foreach (var pId in PersonelIdArray)
                {
                    rtop.PersonelId = Convert.ToInt64(pId);
                    rtop.RandevuId = randevu.Id;
                    rtop.Aktif = true;
                    rtop.Sil = false;
                    rtop.OlusturanKullaniciId = KullaniciId;
                    rtop.OlusturmaTarih = DateTime.Now;
                    rtop.DegistirenKullaniciId = KullaniciId;
                    rtop.DegistirmeTarih = DateTime.Now;
                    dbContext.RandevuToPersonels.Add(rtop);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Yeni Rezervasyon Ekle, Personel Ekle";
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
                return Json(new { Sonuc = true, Bilgi = "Güncelleme Başarılı", sozlesmeId = sozlesme.Id, sozlesmeNo = sozlesme.SozlesmeNo, randevuId = randevu.Id, JsonRequestBehavior.AllowGet });
            }
            catch (Exception)
            {
                return Json(new { Sonuc = false, Bilgi = "Kayıt esnasında bit hata oluştu", JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public ActionResult SozlesmeMusteriGuncelle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeNo = Convert.ToInt64(Request["SozlesmeNo"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            Sozlesme szlsm = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.SozlesmeNo == SozlesmeNo && x.FirmaId == FirmaId);
            if (szlsm == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            szlsm.MusteriId = MusteriId;
            szlsm.DegistirenKullaniciId = KullaniciId;
            szlsm.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Yeni Rezervasyon Ekle, Müşteri Bilgileri Güncelle, Müşteri Id: " + MusteriId + " Sözleşme Id: " + SozlesmeId;
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz." }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SozlesmeDigerBilgilerGuncelle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            TelefonDüzelt td = new TelefonDüzelt();
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeNo = Convert.ToInt64(Request["SozlesmeNo"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long RandevuId = Convert.ToInt64(Request["RandevuId"]);
            string GelinAdSoyad = Request["GelinAdSoyad"];
            string GelinCep = Request["GelinCep"];
            if (!string.IsNullOrEmpty(GelinCep)) { GelinCep = td.düzelt(GelinCep); }
            string GelinEmail = Request["GelinEmail"];
            string DamatAdSoyad = Request["DamatAdSoyad"];
            string DamatCep = Request["DamatCep"];
            if (!string.IsNullOrEmpty(DamatCep)) { DamatCep = td.düzelt(DamatCep); }
            string DamatEmail = Request["DamatEmail"];
            string BebekAdSoyad = Request["BebekAdSoyad"];
            string BabaAdSoyad = Request["BabaAdSoyad"];
            string AnneAdSoyad = Request["AnneAdSoyad"];
            string BabaCep = Request["BabaCep"];
            if (!string.IsNullOrEmpty(BabaCep)) { BabaCep = td.düzelt(BabaCep); }
            string AnneCep = Request["AnneCep"];
            if (!string.IsNullOrEmpty(AnneCep)) { AnneCep = td.düzelt(AnneCep); }
            string BabaEmail = Request["BabaEmail"];
            string Urunler = Request["Urunler"];
            Urunler = Urunler.Replace("\r\n", "<br />");
            Urunler = Urunler.Replace("\n", "<br />");
            string UrunYetkiliAdSoyad = Request["UrunYetkiliAdSoyad"];
            string UrunYetkiliCep = Request["UrunYetkiliCep"];
            if (!string.IsNullOrEmpty(UrunYetkiliCep)) { UrunYetkiliCep = td.düzelt(UrunYetkiliCep); }
            string UrunYetkiliEmail = Request["UrunYetkiliEmail"];
            string Modeller = Request["Modeller"];
            Modeller = Modeller.Replace("\r\n", "<br />");
            Modeller = Modeller.Replace("\n", "<br />");
            string ModelYetkiliAdSoyad = Request["ModelYetkiliAdSoyad"];
            string ModelYetkiliCep = Request["ModelYetkiliCep"];
            if (!string.IsNullOrEmpty(ModelYetkiliCep)) { ModelYetkiliCep = td.düzelt(ModelYetkiliCep); }
            string ModelYetkiliEmail = Request["ModelYetkiliEmail"];
            string YetkiliAdSoyad = Request["YetkiliAdSoyad"];
            string YetkiliCep = Request["YetkiliCep"];
            if (!string.IsNullOrEmpty(YetkiliCep)) { YetkiliCep = td.düzelt(YetkiliCep); }
            string YetkiliEmail = Request["YetkiliEmail"];
            string randevuaciklama = "";
            //randevuaciklama = randevuaciklama + "Sözleşme No: " + SozlesmeNo;
            Sozlesme szlsm = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.SozlesmeNo == SozlesmeNo && x.FirmaId == FirmaId);
            if (szlsm == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            randevuaciklama = szlsm.OrganizasyonYeri;
            randevuaciklama = randevuaciklama + "Sözleşme No: " + SozlesmeNo;
            Randevu randevu = dbContext.Randevus.FirstOrDefault(x => x.Id == RandevuId);
            RezervasyonTurleri tur = dbContext.RezervasyonTurleris.FirstOrDefault(x => x.Id == szlsm.RezervasyonTurId);
            AyarlarGenel genelayar = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId);

            if (tur.FormAlan == "Gelin/Damat")
            {
                szlsm.GelinAd = GelinAdSoyad;
                szlsm.GelinTel = GelinCep;
                szlsm.GelinEmail = GelinEmail;
                szlsm.DamatAd = DamatAdSoyad;
                szlsm.DamatTel = DamatCep;
                szlsm.DamatEmail = DamatEmail;
                if (!string.IsNullOrEmpty(GelinAdSoyad) || !string.IsNullOrEmpty(DamatAdSoyad))
                    randevuaciklama = randevuaciklama + "<br /> Çekim Yapılacak: " + GelinAdSoyad + " & " + DamatAdSoyad;
                #region Rehbere Ekle
                RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Gelin-Damat");
                TelefonRehberi rehbergelin = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.AdSoyad == szlsm.GelinAd);
                if (rehbergelin != null)
                {
                    rehbergelin.AdSoyad = GelinAdSoyad;
                    rehbergelin.CepTel1 = GelinCep;
                    rehbergelin.Email = GelinEmail;
                    rehbergelin.DegistirenKullaniciId = KullaniciId;
                    rehbergelin.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }  // Gelin Damat Rehberde var ise güncellenecek
                else
                {
                    // Cep telefonu boşsa veya cep telefonu dolu mail adresi boşsa rehbere kaydetme
                    if (!string.IsNullOrEmpty(GelinCep) || (!string.IsNullOrEmpty(GelinEmail) && string.IsNullOrEmpty(GelinCep)))
                    {
                        TelefonRehberi rehber = new TelefonRehberi();
                        rehber.FirmaId = FirmaId;
                        rehber.RehberGrupId = rehbergrup.Id;
                        rehber.SozlesmeId = SozlesmeId;
                        rehber.FirmaAdi = "";
                        rehber.AdSoyad = GelinAdSoyad;
                        rehber.SabitTel1 = "";
                        rehber.SabitTel2 = "";
                        rehber.CepTel1 = GelinCep;
                        rehber.CepTel2 = "";
                        rehber.Fax = "";
                        rehber.Email = GelinEmail;
                        rehber.SmsKabul = true;
                        rehber.EmailKabul = true;
                        rehber.Notlar = "Yeni Gelin Kaydı";
                        rehber.OlusturanKullaniciId = KullaniciId;
                        rehber.OlusturmaTarih = DateTime.Now;
                        rehber.DegistirenKullaniciId = KullaniciId;
                        rehber.DegistirmeTarih = DateTime.Now;
                        if (genelayar.GelinDamatRehberKayit == true)
                            rehber.Aktif = true;
                        else
                            rehber.Aktif = false;
                        rehber.Sil = false;
                        rehber.KilitBit = false;
                        dbContext.TelefonRehberis.Add(rehber);
                        dbContext.SaveChanges();
                    }
                }
                TelefonRehberi rehberdamat = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.AdSoyad == szlsm.DamatAd);
                if (rehbergelin != null)
                {
                    rehbergelin.AdSoyad = DamatAdSoyad;
                    rehbergelin.CepTel1 = DamatCep;
                    rehbergelin.Email = DamatEmail;
                    rehbergelin.DegistirenKullaniciId = KullaniciId;
                    rehbergelin.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }
                else
                {
                    // Cep telefonu boşsa veya cep telefonu dolu mail adresi boşsa rehbere kaydetme
                    if (!string.IsNullOrEmpty(DamatCep) || (!string.IsNullOrEmpty(DamatEmail) && string.IsNullOrEmpty(DamatCep)))
                    {
                        TelefonRehberi rehber2 = new TelefonRehberi();
                        rehber2.FirmaId = FirmaId;
                        rehber2.RehberGrupId = rehbergrup.Id;
                        rehber2.SozlesmeId = SozlesmeId;
                        rehber2.FirmaAdi = "";
                        rehber2.AdSoyad = DamatAdSoyad;
                        rehber2.SabitTel1 = "";
                        rehber2.SabitTel2 = "";
                        rehber2.CepTel1 = DamatCep;
                        rehber2.CepTel2 = "";
                        rehber2.Fax = "";
                        rehber2.Email = DamatEmail;
                        rehber2.SmsKabul = true;
                        rehber2.EmailKabul = true;
                        rehber2.Notlar = "Yeni Damat Kaydı";
                        rehber2.OlusturanKullaniciId = KullaniciId;
                        rehber2.OlusturmaTarih = DateTime.Now;
                        rehber2.DegistirenKullaniciId = KullaniciId;
                        rehber2.DegistirmeTarih = DateTime.Now;
                        if (genelayar.GelinDamatRehberKayit == true)
                            rehber2.Aktif = true;
                        else
                            rehber2.Aktif = false;
                        rehber2.Sil = false;
                        rehber2.KilitBit = false;
                        dbContext.TelefonRehberis.Add(rehber2);
                        dbContext.SaveChanges();
                    }
                }
                #endregion
            }
            else if (tur.FormAlan == "Bebek/Çocuk")
            {
                szlsm.CocukAdSoyad = BebekAdSoyad;
                szlsm.AnneAd = AnneAdSoyad;
                szlsm.BabaAd = BabaAdSoyad;
                szlsm.AnneTel = AnneCep;
                szlsm.BabaTel = BabaCep;
                szlsm.BabaEmail = BabaEmail;
                szlsm.AnneEmail = "";
                if (!string.IsNullOrEmpty(BebekAdSoyad))
                    randevuaciklama = randevuaciklama + "<br /> Çekim Yapılacak: " + BebekAdSoyad;
                #region Rehbere Ekle
                RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Anne-Baba");
                TelefonRehberi rehberbaba = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.AdSoyad == szlsm.BabaAd);
                if (rehberbaba != null)
                {
                    rehberbaba.AdSoyad = BabaAdSoyad;
                    rehberbaba.CepTel1 = BabaCep;
                    rehberbaba.Email = BabaEmail;
                    rehberbaba.DegistirenKullaniciId = KullaniciId;
                    rehberbaba.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }
                else
                {
                    // Cep telefonu boşsa veya cep telefonu dolu mail adresi boşsa rehbere kaydetme
                    if (!string.IsNullOrEmpty(BabaCep) || (!string.IsNullOrEmpty(BabaEmail) && string.IsNullOrEmpty(BabaCep)))
                    {
                        TelefonRehberi rehber = new TelefonRehberi();
                        rehber.FirmaId = FirmaId;
                        rehber.RehberGrupId = rehbergrup.Id;
                        rehber.SozlesmeId = SozlesmeId;
                        rehber.FirmaAdi = "";
                        rehber.AdSoyad = BabaAdSoyad;
                        rehber.SabitTel1 = "";
                        rehber.SabitTel2 = "";
                        rehber.CepTel1 = BabaCep;
                        rehber.CepTel2 = "";
                        rehber.Fax = "";
                        rehber.Email = BabaEmail;
                        rehber.SmsKabul = true;
                        rehber.EmailKabul = true;
                        rehber.Notlar = "Yeni Baba Kaydı";
                        rehber.OlusturanKullaniciId = KullaniciId;
                        rehber.OlusturmaTarih = DateTime.Now;
                        rehber.DegistirenKullaniciId = KullaniciId;
                        rehber.DegistirmeTarih = DateTime.Now;
                        if (genelayar.AnneBabaRehberKayit == true)
                            rehber.Aktif = true;
                        else
                            rehber.Aktif = false;
                        rehber.Sil = false;
                        rehber.KilitBit = false;
                        dbContext.TelefonRehberis.Add(rehber);
                        dbContext.SaveChanges();
                    }
                }
                TelefonRehberi rehberanne = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.AdSoyad == szlsm.AnneAd);
                if (rehberanne != null)
                {
                    rehberanne.AdSoyad = AnneAdSoyad;
                    rehberanne.CepTel1 = AnneCep;
                    rehberanne.Email = "";
                    rehberanne.DegistirenKullaniciId = KullaniciId;
                    rehberanne.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }
                else
                {
                    // Cep telefonu boşsa veya cep telefonu dolu mail adresi boşsa rehbere kaydetme
                    if (!string.IsNullOrEmpty(AnneCep))
                    {
                        TelefonRehberi rehber2 = new TelefonRehberi();
                        rehber2.FirmaId = FirmaId;
                        rehber2.RehberGrupId = rehbergrup.Id;
                        rehber2.SozlesmeId = SozlesmeId;
                        rehber2.FirmaAdi = "";
                        rehber2.AdSoyad = AnneAdSoyad;
                        rehber2.SabitTel1 = "";
                        rehber2.SabitTel2 = "";
                        rehber2.CepTel1 = AnneCep;
                        rehber2.CepTel2 = "";
                        rehber2.Fax = "";
                        rehber2.Email = "";
                        rehber2.SmsKabul = true;
                        rehber2.EmailKabul = true;
                        rehber2.Notlar = "Yeni Anne Kaydı";
                        rehber2.OlusturanKullaniciId = KullaniciId;
                        rehber2.OlusturmaTarih = DateTime.Now;
                        rehber2.DegistirenKullaniciId = KullaniciId;
                        rehber2.DegistirmeTarih = DateTime.Now;
                        if (genelayar.AnneBabaRehberKayit == true)
                            rehber2.Aktif = true;
                        else
                            rehber2.Aktif = false;
                        rehber2.Sil = false;
                        rehber2.KilitBit = false;
                        dbContext.TelefonRehberis.Add(rehber2);
                        dbContext.SaveChanges();
                    }
                }
                #endregion
            }
            else if (tur.FormAlan == "Ürün/Katalog")
            {
                szlsm.Urunler = Urunler;
                szlsm.YetkiliAdSoyad = UrunYetkiliAdSoyad;
                szlsm.YetkiliEmail = UrunYetkiliEmail;
                szlsm.YetkiliTel = UrunYetkiliCep;
                if (!string.IsNullOrEmpty(Urunler))
                    randevuaciklama = randevuaciklama + "<br /> Çekim Yapılacak: " + Urunler;
                #region Rehbere Ekle
                RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Rezervasyon Yetkilileri");
                TelefonRehberi rehberyetkili = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.AdSoyad == szlsm.YetkiliAdSoyad);
                if (rehberyetkili != null)
                {
                    rehberyetkili.AdSoyad = UrunYetkiliAdSoyad;
                    rehberyetkili.CepTel1 = UrunYetkiliCep;
                    rehberyetkili.Email = UrunYetkiliEmail;
                    rehberyetkili.DegistirenKullaniciId = KullaniciId;
                    rehberyetkili.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }
                else
                {
                    // Cep telefonu boşsa veya cep telefonu dolu mail adresi boşsa rehbere kaydetme
                    if (!string.IsNullOrEmpty(UrunYetkiliCep) || (!string.IsNullOrEmpty(UrunYetkiliEmail) && string.IsNullOrEmpty(UrunYetkiliCep)))
                    {
                        TelefonRehberi rehber = new TelefonRehberi();
                        rehber.FirmaId = FirmaId;
                        rehber.RehberGrupId = rehbergrup.Id;
                        rehber.SozlesmeId = SozlesmeId;
                        rehber.FirmaAdi = "";
                        rehber.AdSoyad = UrunYetkiliAdSoyad;
                        rehber.SabitTel1 = "";
                        rehber.SabitTel2 = "";
                        rehber.CepTel1 = UrunYetkiliCep;
                        rehber.CepTel2 = "";
                        rehber.Fax = "";
                        rehber.Email = UrunYetkiliEmail;
                        rehber.SmsKabul = true;
                        rehber.EmailKabul = true;
                        rehber.Notlar = "Yeni Rezervasyon Yetkilisi Kaydı";
                        rehber.OlusturanKullaniciId = KullaniciId;
                        rehber.OlusturmaTarih = DateTime.Now;
                        rehber.DegistirenKullaniciId = KullaniciId;
                        rehber.DegistirmeTarih = DateTime.Now;
                        if (genelayar.RezervasyonYetkiliRehberKayit == true)
                            rehber.Aktif = true;
                        else
                            rehber.Aktif = false;
                        rehber.Sil = false;
                        rehber.KilitBit = false;
                        dbContext.TelefonRehberis.Add(rehber);
                        dbContext.SaveChanges();
                    }
                }
                #endregion
            }
            else if (tur.FormAlan == "Model/Manken")
            {
                szlsm.Modeller = Modeller;
                szlsm.YetkiliAdSoyad = ModelYetkiliAdSoyad;
                szlsm.YetkiliEmail = ModelYetkiliEmail;
                szlsm.YetkiliTel = ModelYetkiliCep;
                if (string.IsNullOrEmpty(Modeller))
                    randevuaciklama = randevuaciklama + "<br /> Çekim Yapılacak: " + Modeller;
                #region Rehbere Ekle
                RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Rezervasyon Yetkilileri");
                TelefonRehberi rehberyetkili = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.AdSoyad == szlsm.YetkiliAdSoyad);
                if (rehberyetkili != null)
                {
                    rehberyetkili.AdSoyad = ModelYetkiliAdSoyad;
                    rehberyetkili.CepTel1 = ModelYetkiliCep;
                    rehberyetkili.Email = ModelYetkiliEmail;
                    rehberyetkili.DegistirenKullaniciId = KullaniciId;
                    rehberyetkili.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }
                else
                {
                    // Cep telefonu boşsa veya cep telefonu dolu mail adresi boşsa rehbere kaydetme
                    if (!string.IsNullOrEmpty(ModelYetkiliCep) || (!string.IsNullOrEmpty(ModelYetkiliEmail) && string.IsNullOrEmpty(ModelYetkiliCep)))
                    {
                        TelefonRehberi rehber = new TelefonRehberi();
                        rehber.FirmaId = FirmaId;
                        rehber.RehberGrupId = rehbergrup.Id;
                        rehber.SozlesmeId = SozlesmeId;
                        rehber.FirmaAdi = "";
                        rehber.AdSoyad = ModelYetkiliAdSoyad;
                        rehber.SabitTel1 = "";
                        rehber.SabitTel2 = "";
                        rehber.CepTel1 = ModelYetkiliCep;
                        rehber.CepTel2 = "";
                        rehber.Fax = "";
                        rehber.Email = ModelYetkiliEmail;
                        rehber.SmsKabul = true;
                        rehber.EmailKabul = true;
                        rehber.Notlar = "Yeni Rezervasyon Yetkilisi Kaydı";
                        rehber.OlusturanKullaniciId = KullaniciId;
                        rehber.OlusturmaTarih = DateTime.Now;
                        rehber.DegistirenKullaniciId = KullaniciId;
                        rehber.DegistirmeTarih = DateTime.Now;
                        if (genelayar.RezervasyonYetkiliRehberKayit == true)
                            rehber.Aktif = true;
                        else
                            rehber.Aktif = false;
                        rehber.Sil = false;
                        rehber.KilitBit = false;
                        dbContext.TelefonRehberis.Add(rehber);
                        dbContext.SaveChanges();
                    }
                }
                #endregion
            }
            else if (tur.FormAlan == "Yetkili")
            {
                szlsm.YetkiliAdSoyad = YetkiliAdSoyad;
                szlsm.YetkiliEmail = YetkiliEmail;
                szlsm.YetkiliTel = YetkiliCep;
                if (string.IsNullOrEmpty(YetkiliAdSoyad))   
                    randevuaciklama = randevuaciklama + "<br /> Yapılacak: " + YetkiliAdSoyad;
                #region Rehbere Ekle
                RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Rezervasyon Yetkilileri");
                TelefonRehberi rehberyetkili = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.AdSoyad == szlsm.YetkiliAdSoyad);
                if (rehberyetkili != null)
                {
                    rehberyetkili.AdSoyad = YetkiliAdSoyad;
                    rehberyetkili.CepTel1 = YetkiliCep;
                    rehberyetkili.Email = YetkiliEmail;
                    rehberyetkili.DegistirenKullaniciId = KullaniciId;
                    rehberyetkili.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }
                else
                {
                    // Cep telefonu boşsa veya cep telefonu dolu mail adresi boşsa rehbere kaydetme
                    if (!string.IsNullOrEmpty(YetkiliCep) || (!string.IsNullOrEmpty(YetkiliEmail) && string.IsNullOrEmpty(YetkiliCep)))
                    {
                        TelefonRehberi rehber = new TelefonRehberi();
                        rehber.FirmaId = FirmaId;
                        rehber.RehberGrupId = rehbergrup.Id;
                        rehber.SozlesmeId = SozlesmeId;
                        rehber.FirmaAdi = "";
                        rehber.AdSoyad = YetkiliAdSoyad;
                        rehber.SabitTel1 = "";
                        rehber.SabitTel2 = "";
                        rehber.CepTel1 = YetkiliCep;
                        rehber.CepTel2 = "";
                        rehber.Fax = "";
                        rehber.Email = YetkiliEmail;
                        rehber.SmsKabul = true;
                        rehber.EmailKabul = true;
                        rehber.Notlar = "Yeni Rezervasyon Yetkilisi Kaydı";
                        rehber.OlusturanKullaniciId = KullaniciId;
                        rehber.OlusturmaTarih = DateTime.Now;
                        rehber.DegistirenKullaniciId = KullaniciId;
                        rehber.DegistirmeTarih = DateTime.Now;
                        if (genelayar.RezervasyonYetkiliRehberKayit == true)
                            rehber.Aktif = true;
                        else
                            rehber.Aktif = false;
                        rehber.Sil = false;
                        rehber.KilitBit = false;
                        dbContext.TelefonRehberis.Add(rehber);
                        dbContext.SaveChanges();
                    }
                }
                #endregion
            }
            szlsm.DegistirenKullaniciId = KullaniciId;
            szlsm.DegistirmeTarih = DateTime.Now;

            randevu.Aciklama = randevuaciklama;
            randevu.DegistirmeTarih = DateTime.Now;
            randevu.DegistirenKullaniciId = KullaniciId;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Diğer Bilgileri Güncelle, Sözleşme Id: " + SozlesmeId;
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
        public ActionResult SozlesmePaketlerEkhizmetlerGuncelle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeNo = Convert.ToInt64(Request["SozlesmeNo"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            string secilenpaketID = Request["secilenpaketID"];
            string secilenekhizmetID = Request["secilenekhizmetID"];
            decimal PaketlerFiyat = Convert.ToDecimal(Request["PaketlerFiyat"]);
            decimal EkHizmetlerFiyat = Convert.ToDecimal(Request["EkHizmetlerFiyat"]);
            decimal Iskonto = Convert.ToDecimal(Request["Iskonto"]);
            decimal ToplamTutar = Convert.ToDecimal(Request["ToplamTutar"]);

            string Paketler = "";
            string EkHizmetler = "";
            string[] paketIDler = secilenpaketID.Split(',');
            string[] ekhizmetIDler = secilenekhizmetID.Split(',');
            if (paketIDler[0] != "")
            {
                foreach (var paket in paketIDler)
                {
                    long pkid = Convert.ToInt64(paket);
                    CekimPaketleri pk = dbContext.CekimPaketleris.FirstOrDefault(x => x.Id == pkid);
                    Paketler = Paketler + "<div><h5><span class='fw-700'>" + pk.PaketAdi + "</span></h5>" + pk.PaketDetayi + "</div>";
                }
            }
            if (ekhizmetIDler[0] != "")
            {
                foreach (var ekhizmet in ekhizmetIDler)
                {
                    long hzid = Convert.ToInt64(ekhizmet);
                    RezervasyonEkHizmet hz = dbContext.RezervasyonEkHizmets.FirstOrDefault(x => x.Id == hzid);
                    EkHizmetler = EkHizmetler + "<div><h5><span class='fw-700'>" + hz.EkHizmetAdi + "</span></h5>" + hz.Aciklama + "</div>";
                }
            }
            Sozlesme szlsm = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.SozlesmeNo == SozlesmeNo && x.FirmaId == FirmaId);
            if (szlsm == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            szlsm.PaketlerId = secilenpaketID;
            szlsm.EkHizmetlerId = secilenekhizmetID;
            szlsm.Paketler = Paketler;
            szlsm.EkHizmetler = EkHizmetler;
            szlsm.PaketlerFiyat = PaketlerFiyat;
            szlsm.EkHizmetlerFiyat = EkHizmetlerFiyat;
            szlsm.Iskonto = Iskonto;
            szlsm.ToplamFiyat = ToplamTutar;
            szlsm.DegistirenKullaniciId = KullaniciId;
            szlsm.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Paket ve Ekhizmetler Güncelle, Sözleşme Id: " + SozlesmeId;
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
        public ActionResult SureclerGuncelle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeNo = Convert.ToInt64(Request["SozlesmeNo"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            string secilensurecID = Request["secilensurecID"];
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            string sonuc_musteri_randevu_sms = "";


            string Surecler = "";

            List<string> srcad = new List<string>();
            string[] surecIDler = secilensurecID.Split(',');
            if (surecIDler[0] != "")
            {
                foreach (var surec in surecIDler)
                {
                    long sid = Convert.ToInt64(surec);
                    Surecler sr = dbContext.Sureclers.FirstOrDefault(x => x.Id == sid);
                    Surecler = Surecler + sr.SurecAdi + "<br />";
                    srcad.Add(sr.SurecAdi);
                }
            }

            Sozlesme szlsm = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.SozlesmeNo == SozlesmeNo && x.FirmaId == FirmaId);
            if (szlsm == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            szlsm.SureclerId = secilensurecID;
            szlsm.Surecler = Surecler;
            szlsm.DegistirenKullaniciId = KullaniciId;
            szlsm.DegistirmeTarih = DateTime.Now;

            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == szlsm.MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            bool smskabul = true;
            if (musteri == null)
                smskabul = true;
            else
                smskabul = musteri.SMSKabul;

            try
            {
                dbContext.SaveChanges();
                #region Süreç Güncelleme Bilgi Smsi Gönder
                if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                {
                    AyarlarSmsGonderim smsayar = null;
                    string smsmetin = "";
                    smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.SurecDegisiklikBilgiGonderimSuresi != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (smsayar != null)
                        smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.SurecDegisiklikBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                    else
                        smsmetin = null;

                    if (smsayar != null && smsmetin != null)
                    {
                        if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                            smsmetin = smsmetin.Replace("{FirmaAdi}", szlsm.Firma.FirmaAdi);
                        if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{GelinAdSoyad}", szlsm.GelinAd);
                        if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{DamatAdSoyad}", szlsm.DamatAd);
                        if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (smsmetin.IndexOf("{RezervasyonTuru}") != -1)
                            smsmetin = smsmetin.Replace("{RezervasyonTuru}", szlsm.RezervasyonTurleri.RezervasyonTuru);
                        if (smsmetin.IndexOf("{Tarih}") != -1)
                            smsmetin = smsmetin.Replace("{Tarih}", szlsm.RezervasyonTarihi.ToShortDateString());
                        if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{RandevuTarihi}", szlsm.RezervasyonTarihi.ToShortDateString());
                        if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                            smsmetin = smsmetin.Replace("{SozlesmeNo}", szlsm.SozlesmeNo.ToString());
                        if (smsmetin.IndexOf("{Surec}") != -1)
                            smsmetin = smsmetin.Replace("{Surec}", srcad.Last());
                        sonuc_musteri_randevu_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                    }
                }
                #endregion
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Süreçler Güncelle, Sözleşme Id: " + SozlesmeId;
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
        public ActionResult AlinanOdemeEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeNo = Convert.ToInt64(Request["SozlesmeNo"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            string OdemeAdSoyad = Request["OdemeAdSoyad"];

            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            DateTime OdemeTarihi = Convert.ToDateTime(Request["OdemeTarihi"]);
            decimal OdemeTutar = Convert.ToDecimal(Request["OdemeTutar"]);
            string OdemeSekli = Request["OdemeSekli"];
            string OdemeAciklama = Request["OdemeAciklama"];
            bool sonuc_musteri_odeme_mail = true;
            string sonuc_musteri_odeme_sms = "";
            // OdemeAlacak tablosunda odeme alınıp alınmama durumuna göre güncelleme
            bool OdemeAlDurum = Convert.ToBoolean(Request["OdemeAlDurum"]);
            long OdemeAlacakId;
            string Mesaj = "";
            string OAId = Request["OdemeAlacakId"];
            if (OAId == null || OAId == "" || OAId == "undefined")
                OdemeAlacakId = 1;
            else
                OdemeAlacakId = Convert.ToInt64(Request["OdemeAlacakId"]);
            // Makbuz Numarası Oluşturuluyor.
            long makbuzno = 0;
            string mn;
            List<GelirGider> alinanodemevarmi = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId).ToList();
            if (alinanodemevarmi.Count > 0)
            {
                makbuzno = Convert.ToInt64(alinanodemevarmi.Max(x => x.MakbuzNo));
                makbuzno = makbuzno + 1;
                mn = makbuzno.ToString();
            }
            else
                mn = FirmaId.ToString() + "101" + makbuzno.ToString();

            //mn = makbuzno.ToString();
            // Makbuz Numarası Oluşturuluyor.

            OdemeTarihi = OdemeTarihi.AddHours(DateTime.Now.Hour);
            OdemeTarihi = OdemeTarihi.AddMinutes(DateTime.Now.Minute);

            Odemeler odemeal = new Odemeler();
            odemeal.FirmaId = FirmaId;
            odemeal.SubeId = SubeId;
            odemeal.SozlesmeId = SozlesmeId;
            odemeal.GisId = 1;
            odemeal.CariHareketId = 1;
            odemeal.MusteriId = MusteriId;
            odemeal.OdemeTuru = "AlinanOdeme";
            if (OdemeAlacakId != 1)
            {
                odemeal.GelecekOdemeID = OdemeAlacakId;
            }
            odemeal.OdemeYapanAdSoyad = OdemeAdSoyad;
            odemeal.Tarih = OdemeTarihi;
            odemeal.OdemeTarihi = OdemeTarihi;
            odemeal.Tutar = OdemeTutar;
            odemeal.OdemeSekli = OdemeSekli;
            odemeal.OdemeAl = false; // Ödeme Türü: GelecekÖdeme den bir ödeme alınıyor ise true olacak.
            odemeal.AlinanOdemeMakbuzNo = Convert.ToInt64(mn); // GelirGider tablosundaki MakbuzNo = Odemler tablosundaki MakbuzNo
            odemeal.Notlar = OdemeAciklama;
            odemeal.Kapora = false;
            odemeal.Iptal = false;
            odemeal.KilitBit = false;
            odemeal.Aktif = true;
            odemeal.Sil = false;
            odemeal.OlusturanKullaniciId = KullaniciId;
            odemeal.OlusturmaTarih = DateTime.Now;
            odemeal.DegistirenKullaniciId = KullaniciId;
            odemeal.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.Odemelers.Add(odemeal);
                dbContext.SaveChanges();
                if (OdemeAlDurum == true && OdemeAlacakId != 0)
                {
                    // Yapılan ödeme, daha önce gelecek ödeme olarak kaydedilmiş ise yeni bir alınan ödeme kaydı giriliyor ve eski kayıt güncelleniyor.
                    Odemeler odemealguncelle = dbContext.Odemelers.FirstOrDefault(x => x.Id == OdemeAlacakId);
                    //odemealguncelle.GelecekOdemeID = OdemeAlacakId;
                    odemealguncelle.OdemeTarihi = OdemeTarihi;
                    odemealguncelle.OdemeAl = true;
                    odemealguncelle.KilitBit = true;
                    dbContext.SaveChanges();
                }// OdemeAlacak tablosunda odeme alınıp alınmama durumuna göre güncelleme
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, OdemelerAlinan Kaydı, Sözleşme Id: " + SozlesmeId;
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

            GelirGiderTurleri gtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.GelirGiderTur == "Sözleşme Ödemesi");
            GelirGider gg = new GelirGider();
            if (gtur != null)
            {
                gg.GelirGiderTurId = gtur.Id;
            }
            else
            {
                GelirGiderTurleri ggtur = new GelirGiderTurleri();
                ggtur.FirmaId = FirmaId; // sessiondaki FirmaID
                ggtur.GelirGiderTur = "Sözleşme Ödemesi";
                ggtur.OlusturanKullaniciId = KullaniciId;
                ggtur.OlusturmaTarih = DateTime.Now;
                ggtur.DegistirenKullaniciId = KullaniciId;
                ggtur.DegistirmeTarih = DateTime.Now;
                ggtur.Aktif = true;
                ggtur.Sil = false;
                ggtur.KilitBit = true;
                dbContext.GelirGiderTurleris.Add(ggtur);
                dbContext.SaveChanges();
                gg.GelirGiderTurId = ggtur.Id;
            }
            gg.FirmaId = FirmaId;
            gg.SubeId = SubeId;
            gg.Tarih = OdemeTarihi;
            gg.SozlesmeId = SozlesmeId;
            gg.GisId = 1;
            gg.CariHareketId = 1;
            gg.PersonelOdemeId = 1;
            gg.OdemeId = odemeal.Id;
            gg.OdemeTuru = OdemeSekli;
            gg.Tip = "Gelir";
            gg.Tutar = OdemeTutar;
            gg.MakbuzNo = Convert.ToInt64(mn); // GelirGider tablosundaki MakbuzNo = Odemler tablosundaki MakbuzNo
            gg.Notlar = SozlesmeNo + " Numaralı sözleşmeye ait ödeme";
            gg.OlusturanKullaniciId = KullaniciId;
            gg.Aktif = true;
            gg.Sil = false;
            gg.OlusturmaTarih = DateTime.Now;
            gg.DegistirenKullaniciId = KullaniciId;
            gg.DegistirmeTarih = DateTime.Now;

            try
            {
                dbContext.GelirGiders.Add(gg);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Gelir-Gider Kaydı, Sözleşme Id: " + SozlesmeId;
                hata.HataMesajı = e.Message.ToString();
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz." }, JsonRequestBehavior.AllowGet);
            }
            List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.OdemeTuru == "AlinanOdeme" && x.Aktif == true && x.Sil == false).ToList();
            //List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.SozlesmeId == SozlesmeId && x.OdemeTuru == "AlinanOdeme" && x.Aktif == true && x.Sil == false).ToList();
            var odemelistesi = odemeler.Select(m => new
            {
                Id = m.Id,
                OdemelerGelecekId = m.GelecekOdemeID,
                Tarih = m.Tarih.ToShortDateString(),
                Tutar = m.Tutar,
                Iptal = m.Iptal,
                Aciklama = m.Notlar
            }).ToList();
            Mesaj = "Kayıt Başarılı";
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            bool smskabul = true, emailkabul = true;
            if (musteri == null)
            {
                smskabul = true; emailkabul = true;
            }
            else
            {
                smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
            }
            #region Sözleşme Bilgi Emaili
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.Email != "" && emailkabul)
            {
                AyarlarMailGonderim mailayar = null;
                MailMetinleri mmetin = null;
                string mailmetin = "";

                mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriOdemeBilgiGonderimSuresi == 6 && x.CariTahsilatBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                if (mailayar == null)
                    Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırılmaış. Bilgi Maili gönderilemedi.";
                //return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırılmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                else
                    mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.MusteriOdemeBilgiMaili && x.Aktif == true && x.Sil == false);

                Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);

                if (mailayar != null && mmetin != null)
                {
                    mailmetin = mmetin.MailMetni;
                    string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
                    string IcerikResim = Server.MapPath(mmetin.IcerikResim);
                    string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");
                    string body = string.Empty;

                    if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        mailmetin = mailmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (mailmetin.IndexOf("{FirmaAdi}") != -1)
                        mailmetin = mailmetin.Replace("{FirmaAdi}", "");
                    if (mailmetin.IndexOf("{RezervasyonTuru}") != -1)
                        mailmetin = mailmetin.Replace("{RezervasyonTuru}", gg.GelirGiderTurleri.GelirGiderTur);
                    if (mailmetin.IndexOf("{Tarih}") != -1)
                        mailmetin = mailmetin.Replace("{Tarih}", OdemeTarihi.ToShortDateString());
                    if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                        mailmetin = mailmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                    if (mailmetin.IndexOf("{Tutar}") != -1)
                        mailmetin = mailmetin.Replace("{Tutar}", OdemeTutar.ToString());
                    if (mailmetin.IndexOf("{SozlesmeNo}") != -1)
                        mailmetin = mailmetin.Replace("{SozlesmeNo}", mn.ToString());
                    if (mailmetin.IndexOf("{MakbuzNo}") != -1)
                        mailmetin = mailmetin.Replace("{MakbuzNo}", mn.ToString());

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

                        sonuc_musteri_odeme_mail = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, body, htmlView);
                    }
                    else
                        sonuc_musteri_odeme_mail = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, mailmetin);
                }
            }
            #endregion
            #region Sms Bilgi Mesajı

            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
            {
                AyarlarSmsGonderim smsayar = null;
                string smsmetin = "";
                smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriOdemeBilgiGonderimSuresi == 6);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                if (smsayar != null)
                    smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.MusteriOdemeBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                if (smsayar != null && smsmetin != null)
                {
                    if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                        smsmetin = smsmetin.Replace("{FirmaAdi}", "");
                    if (smsmetin.IndexOf("{RezervasyonTuru}") != -1)
                        smsmetin = smsmetin.Replace("{RezervasyonTuru}", gg.GelirGiderTurleri.GelirGiderTur);
                    if (smsmetin.IndexOf("{Tarih}") != -1)
                        smsmetin = smsmetin.Replace("{Tarih}", OdemeTarihi.ToShortDateString());
                    if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                    if (smsmetin.IndexOf("{Tutar}") != -1)
                        smsmetin = smsmetin.Replace("{Tutar}", OdemeTutar.ToString());
                    if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                        smsmetin = smsmetin.Replace("{SozlesmeNo}", mn.ToString());
                    if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                        smsmetin = smsmetin.Replace("{MakbuzNo}", mn.ToString());
                    sonuc_musteri_odeme_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                }
            }
            #endregion


            return Json(new { Sonuc = true, Mesaj, data = odemelistesi }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AlinanOdemeIptal(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string OdemelerGelecekId = Request["OdemelerGelecekId"];
            Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (odeme == null)
                return Json(new { Sonuc = false, Bilgi = "İptal edilmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            long GelirGiderId = dbContext.GelirGiders.FirstOrDefault(x => x.SozlesmeId == odeme.SozlesmeId && x.OdemeId == id && x.FirmaId == FirmaId).Id;
            long gelecekOdemeId;
            if (OdemelerGelecekId == null || OdemelerGelecekId == "0")
                gelecekOdemeId = 0;
            else
                gelecekOdemeId = Convert.ToInt64(Request["OdemelerGelecekId"]);

            long SozlesmeId = Convert.ToInt64(odeme.SozlesmeId);
            odeme.Iptal = true;
            odeme.DegistirenKullaniciId = KullaniciId;
            odeme.DegistirmeTarih = DateTime.Now;
            if (GelirGiderId != 0)
            {
                GelirGider gg = dbContext.GelirGiders.FirstOrDefault(x => x.Id == GelirGiderId);
                dbContext.GelirGiders.Remove(gg);
            }
            if (gelecekOdemeId != 0)
            {
                Odemeler go = dbContext.Odemelers.FirstOrDefault(x => x.Id == gelecekOdemeId);
                go.OdemeAl = false;
                go.KilitBit = false;
                go.DegistirenKullaniciId = KullaniciId;
                go.DegistirmeTarih = DateTime.Now;
            }
            try
            {
                dbContext.SaveChanges();
                List<Odemeler> gelirgiderodeme = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.OdemeTuru == "AlinanOdeme" && x.Aktif == true && x.Sil == false).ToList();
                //List<Odemeler> gelirgiderodeme = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.SozlesmeId == SozlesmeId && x.OdemeTuru == "AlinanOdeme" && x.Aktif == true && x.Sil == false).ToList();
                var gelirgider = gelirgiderodeme.Select(m => new
                {
                    Id = m.Id,
                    OdemelerGelecekId = m.GelecekOdemeID,
                    Tarih = m.Tarih.ToShortDateString(),
                    Tutar = m.Tutar,
                    Iptal = m.Iptal,
                    Aciklama = m.Notlar
                }).ToList(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
                return Json(new { Sonuc = true, Mesaj = "Kayıt İptal Edildi", data = gelirgider }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Alınan Ödeme İptal, Kayıt Id: " + id + " Sözleşme Id: " + SozlesmeId;
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
        public ActionResult AlinanOdemeSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string OdemelerGelecekId = Request["OdemelerGelecekId"];
            Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (odeme == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            long GelirGiderId = dbContext.GelirGiders.FirstOrDefault(x => x.SozlesmeId == odeme.SozlesmeId && x.OdemeId == id && x.FirmaId == FirmaId).Id;
            long gelecekOdemeId;
            if (OdemelerGelecekId == null || OdemelerGelecekId == "0")
                gelecekOdemeId = 0;
            else
                gelecekOdemeId = Convert.ToInt64(Request["OdemelerGelecekId"]);

            long SozlesmeId = Convert.ToInt64(odeme.SozlesmeId);
            dbContext.Odemelers.Remove(odeme);
            if (GelirGiderId != 0)
            {
                GelirGider gg = dbContext.GelirGiders.FirstOrDefault(x => x.Id == GelirGiderId);
                dbContext.GelirGiders.Remove(gg);
            }
            if (gelecekOdemeId != 0)
            {
                Odemeler go = dbContext.Odemelers.FirstOrDefault(x => x.Id == gelecekOdemeId);
                go.OdemeAl = false;
                go.KilitBit = false;
                go.DegistirenKullaniciId = KullaniciId;
                go.DegistirmeTarih = DateTime.Now;
            }
            try
            {
                dbContext.SaveChanges();
                List<Odemeler> gelirgiderodeme = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.OdemeTuru == "AlinanOdeme" && x.Aktif == true && x.Sil == false).ToList();
                //List<Odemeler> gelirgiderodeme = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.SozlesmeId == SozlesmeId && x.OdemeTuru == "AlinanOdeme" && x.Aktif == true && x.Sil == false).ToList();
                var gelirgider = gelirgiderodeme.Select(m => new
                {
                    Id = m.Id,
                    OdemelerGelecekId = m.GelecekOdemeID,
                    Tarih = m.Tarih.ToShortDateString(),
                    Tutar = m.Tutar,
                    Iptal = m.Iptal,
                    Aciklama = m.Notlar
                }).ToList(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
                return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi", data = gelirgider }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Alınan Ödeme Sil, Kayıt Id: " + id + " Sözleşme Id: " + SozlesmeId;
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
        public ActionResult AlinanOdemeSMSGonder()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long OdemeId = Convert.ToInt64(Request["OdemeId"]);
            string sonuc_musteri_randevu_sms = "";
            string randevubilgi = "";
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.Sil == false && x.Aktif == true);
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            bool smskabul = true;
            if (musteri == null)
                smskabul = true;
            else
                smskabul = musteri.SMSKabul;
            Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.Id == OdemeId && x.Iptal == false && x.Aktif == true && x.Sil == false);
            #region Alınan Ödeme Bilgi Smsi
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
            {
                AyarlarSmsGonderim smsayar = null;
                string smsmetin = "";
                smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriOdemeBilgiGonderimSuresi != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                if (smsayar != null)
                    smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.MusteriOdemeBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                else
                    smsmetin = null;

                if (smsayar != null && smsmetin != null)
                {
                    if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                        smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                    if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                        smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                    if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                        smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                    if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{Tarih}") != -1)
                        smsmetin = smsmetin.Replace("{Tarih}", odeme.Tarih.ToShortDateString());
                    if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                    if (smsmetin.IndexOf("{RezervasyonTuru}") != -1)
                        smsmetin = smsmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                    if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.Tarih.ToShortDateString());
                    if (smsmetin.IndexOf("{Tutar}") != -1)
                        smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                    if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                    if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                        smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                    if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                        smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo.ToString());
                    if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                        smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevubilgi);
                    sonuc_musteri_randevu_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                    return Json(new { Sonuc = true, Mesaj = sonuc_musteri_randevu_sms }, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion
            return Json(new { Sonuc = false, Mesaj = "SMS Gönderilemedi." }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AlinanOdemeSMSAyarKotrol()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            AyarlarSmsGonderim smsayar = null;
            string smsmetin = "";
            smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriOdemeBilgiGonderimSuresi != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
            if (smsayar != null)
                smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.MusteriOdemeBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
            else
                smsmetin = null;

            if (smsayar != null && smsmetin != null)
            {
                return Json(new { Sonuc = true, Mesaj = "SMS Gönderilebilir." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Sonuc = false, Mesaj = "SMS Gönderim Ayarları Yapılmamış. Lütfen ayarları kontrol ediniz.." }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult AlacakEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeNo = Convert.ToInt64(Request["SozlesmeNo"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            DateTime KalanOdemeTarihi = Convert.ToDateTime(Request["KalanOdemeTarihi"]);
            decimal KalanOdemeTutar = Convert.ToDecimal(Request["KalanOdemeTutar"]);
            string KalanOdemeAciklama = Request["KalanOdemeAciklama"];

            Odemeler odemgelecek = new Odemeler();
            odemgelecek.FirmaId = FirmaId;
            odemgelecek.SubeId = SubeId;
            odemgelecek.SozlesmeId = SozlesmeId;
            odemgelecek.GisId =1;
            odemgelecek.CariHareketId = 1;
            odemgelecek.MusteriId = MusteriId;
            odemgelecek.OdemeTuru = "GelecekOdeme";
            odemgelecek.Tarih = DateTime.Now;
            odemgelecek.OdemeTarihi = KalanOdemeTarihi;
            odemgelecek.Tutar = KalanOdemeTutar;
            odemgelecek.OdemeAl = false; // Ödeme Türü: GelecekÖdeme den bir ödeme alınıyor ise true olacak.
            odemgelecek.Notlar = KalanOdemeAciklama;
            odemgelecek.Kapora = false;
            odemgelecek.Iptal = false;
            odemgelecek.KilitBit = false;
            odemgelecek.Aktif = true;
            odemgelecek.Sil = false;
            odemgelecek.OlusturanKullaniciId = KullaniciId;
            odemgelecek.OlusturmaTarih = DateTime.Now;
            odemgelecek.DegistirenKullaniciId = KullaniciId;
            odemgelecek.DegistirmeTarih = DateTime.Now;
            dbContext.Odemelers.Add(odemgelecek);
            try
            {
                dbContext.SaveChanges();

                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.OdemeTuru == "GelecekOdeme" && x.Aktif == true && x.Sil == false).ToList();
                //List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.SozlesmeId == SozlesmeId && x.OdemeTuru == "GelecekOdeme" && x.Aktif == true && x.Sil == false).ToList();
                var odemelistesi = odemeler.Select(m => new
                {
                    Id = m.Id,
                    //OdemelerGelecekId = m.GelecekOdemeID,
                    Tarih = m.OdemeTarihi.ToShortDateString(),
                    Tutar = m.Tutar,
                    Iptal = m.Iptal,
                    Aciklama = m.Notlar
                }).ToList();
                // Müşteri ödeme hatırlatma mesajı Kayıt yapıldığı anda seçilmişse burada gönderilecek.
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı", data = odemelistesi }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Alacak Ekle, Sözleşme Id: " + SozlesmeId;
                hata.HataMesajı = e.Message.ToString();
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
        public ActionResult AlacakSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (odeme == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            long SozlesmeId = Convert.ToInt64(odeme.SozlesmeId);
            try
            {
                dbContext.Odemelers.Remove(odeme);
                dbContext.SaveChanges();
                List<Odemeler> alinanodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.OdemeTuru == "GelecekOdeme" && x.Aktif == true && x.Sil == false).ToList();
                //List<Odemeler> alinanodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.SozlesmeId == SozlesmeId && x.OdemeTuru == "GelecekOdeme" && x.Aktif == true && x.Sil == false).ToList();
                var odelelist = alinanodemeler.Select(m => new
                {
                    Id = m.Id,
                    Tarih = m.OdemeTarihi.ToShortDateString(),
                    Tutar = m.Tutar,
                    OdemeAl = m.OdemeAl,
                    Aciklama = m.Notlar
                }).ToList(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı", data = odelelist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Alacak Sil, Kayıt Id: " + id + " Sözleşme Id: " + SozlesmeId;
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
        public ActionResult AlacakOdemeSMSGonder()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long AlacakId = Convert.ToInt64(Request["AlacakId"]);
            string sonuc_musteri_randevu_sms = "";
            string randevubilgi = "";
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.Sil == false && x.Aktif == true);
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            bool smskabul = true;
            if (musteri == null)
                smskabul = true;
            else
                smskabul = musteri.SMSKabul;
            Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.Id == AlacakId && x.Iptal == false && x.Aktif == true && x.Sil == false && x.OdemeTuru == "GelecekOdeme");
            #region Alınan Ödeme Bilgi Smsi
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
            {
                AyarlarSmsGonderim smsayar = null;
                string smsmetin = "";
                smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriOdemeBilgiGonderimSuresi != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                if (smsayar != null)
                    smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.MusteriOdemeBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                else
                    smsmetin = null;

                if (smsayar != null && smsmetin != null)
                {
                    if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                        smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                    if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                        smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                    if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                        smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                    if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{RezervasyonTuru}") != -1)
                        smsmetin = smsmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                    if (smsmetin.IndexOf("{Tarih}") != -1)
                        smsmetin = smsmetin.Replace("{Tarih}", odeme.Tarih.ToShortDateString());
                    if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                    if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OdemeTarihi}", odeme.Tarih.ToShortDateString());
                    if (smsmetin.IndexOf("{Tutar}") != -1)
                        smsmetin = smsmetin.Replace("{Tutar}", odeme.Tutar.ToString());
                    if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                    if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                        smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                    if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                        smsmetin = smsmetin.Replace("{MakbuzNo}", odeme.AlinanOdemeMakbuzNo.ToString());
                    if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                        smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevubilgi);
                    sonuc_musteri_randevu_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                    return Json(new { Sonuc = true, Mesaj = sonuc_musteri_randevu_sms }, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion
            return Json(new { Sonuc = false, Mesaj = "SMS Gönderilemedi." }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AlacakOdemeSMSAyarKotrol()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            AyarlarSmsGonderim smsayar = null;
            string smsmetin = "";
            smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriOdemeHatirlatmaGonderimSuresi != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
            if (smsayar != null)
                smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.MusteriOdemeHatirlatmaMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
            else
                smsmetin = null;

            if (smsayar != null && smsmetin != null)
            {
                return Json(new { Sonuc = true, Mesaj = "SMS Gönderilebilir." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Sonuc = false, Mesaj = "SMS Gönderim Ayarları Yapılmamış. Lütfen ayarları kontrol ediniz.." }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult CekimRandevuEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeNo = Convert.ToInt64(Request["SozlesmeNo"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long RezervasyonTurId = Convert.ToInt64(Request["RezervasyonTurId"]);
            //long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            string PersonelIdler = Request["PersonelIdArray"];
            string[] PersonelIdArray = Request["PersonelIdArray"].Split(',');
            DateTime CekimTarihi = Convert.ToDateTime(Request["CekimTarihi"]);
            string baslangicZamani = Request["CekimBaslangic"];
            string bitisZamani = Request["CekimBitis"];
            TimeSpan baslangic = TimeSpan.Parse(baslangicZamani);
            TimeSpan bitis = TimeSpan.Parse(bitisZamani);
            string CekimYeri = Request["CekimYeri"];
            long rezid = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.SozlesmeNo == SozlesmeNo && x.FirmaId == FirmaId).RezervasyonTurId;
            long randevugorunumId = Convert.ToInt64(dbContext.RezervasyonTurleris.FirstOrDefault(x => x.Id == rezid && x.FirmaId == FirmaId).RandevuGorunumId);
            string randevuaciklama = "";
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.FirmaId == FirmaId);
            randevuaciklama = randevuaciklama + "Sözleşme No: " + SozlesmeNo;
            if (!string.IsNullOrEmpty(sz.GelinAd) && !string.IsNullOrEmpty(sz.DamatAd))
            {
                randevuaciklama = randevuaciklama + "<br />Çekim Yapılacak: " + sz.GelinAd + " & " + sz.DamatAd;
            }
            else if (!string.IsNullOrEmpty(sz.CocukAdSoyad))
            {
                randevuaciklama = randevuaciklama + "<br />Çekim Yapılacak: " + sz.CocukAdSoyad;
            }
            else if (!string.IsNullOrEmpty(sz.Urunler))
            {
                randevuaciklama = randevuaciklama + "<br />Çekim Yapılacak: " + sz.Urunler;
            }
            else if (!string.IsNullOrEmpty(sz.Modeller))
            {
                randevuaciklama = randevuaciklama + "<br />Çekim Yapılacak: " + sz.Modeller;
            }
            else if (!string.IsNullOrEmpty(sz.YetkiliAdSoyad))
            {
                randevuaciklama = randevuaciklama + "<br />Çekim Yapılacak: " + sz.YetkiliAdSoyad;
            }
            RandevuToPersonel rtop = new RandevuToPersonel();
            Randevu randevu = new Randevu();
            randevu.FirmaId = FirmaId;
            randevu.SubeId = SubeId;
            randevu.SozlesmeId = SozlesmeId;
            randevu.GorevliPersonellerId = PersonelIdler;
            //randevu.PersonelId = null;
            randevu.RezervasyonTurId = RezervasyonTurId;
            randevu.CekimRandevu = true;
            randevu.Baslik = sz.RezervasyonTurleri.RezervasyonTuru + " - Fotoğraf Çekimi";
            if (randevuaciklama == "")
                randevu.Aciklama = "Çekim Yeri: " + CekimYeri;
            else
                randevu.Aciklama = "Çekim Yeri: " + CekimYeri + "<br />" + randevuaciklama;
            randevu.Baslangic = CekimTarihi.Add(baslangic);
            randevu.Bitis = CekimTarihi.Add(bitis);
            randevu.RandevuGorunumId = randevugorunumId;
            randevu.Iptal = false;
            randevu.OlusturanKullaniciId = KullaniciId;
            randevu.OlusturmaTarih = DateTime.Now;
            randevu.DegistirenKullaniciId = KullaniciId;
            randevu.DegistirmeTarih = DateTime.Now;
            randevu.Aktif = true;
            randevu.Sil = false;
            dbContext.Randevus.Add(randevu);

            // Randevu kaydedilmeden önce randevu tarih ve saatinin boş olup olmadığı kontrol edilecek.
            try
            {
                dbContext.SaveChanges();
                // RandevutoPersonel tablosunaseçilen personel eklenecek
                foreach (var pId in PersonelIdArray)
                {
                    rtop.PersonelId = Convert.ToInt64(pId);
                    rtop.RandevuId = randevu.Id;
                    rtop.Aktif = true;
                    rtop.Sil = false;
                    rtop.Iptal = false;
                    rtop.OlusturanKullaniciId = KullaniciId;
                    rtop.OlusturmaTarih = DateTime.Now;
                    rtop.DegistirenKullaniciId = KullaniciId;
                    rtop.DegistirmeTarih = DateTime.Now;
                    dbContext.RandevuToPersonels.Add(rtop);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Yeni Rezervasyon Ekle, Personel Ekle";
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
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Çekim Randevusu, Sözleşme Id: " + SozlesmeId;
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
            var rand = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.CekimRandevu == true && x.Aktif == true && x.Sil == false).Select(m => new
            //var rand = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.SozlesmeId == SozlesmeId && x.CekimRandevu == true && x.Aktif == true && x.Sil == false).Select(m => new
            {
                Id = m.Id,
                Tarih = m.Baslangic,
                Baslangic = m.Baslangic,
                Bitis = m.Bitis,
                Aciklama = m.Aciklama,
                Personeller = m.GorevliPersonellerId,
                Iptal = m.Iptal
            }).OrderBy(x => x.Baslangic).ToList();
            var pers = dbContext.RandevuToPersonels.Where(x => x.RandevuId == randevu.Id).Select(m => new
            {
                PersonelId = m.PersonelId,
                PersonelAdiSoyadi = m.Personel.AdiSoyadi,
                PersonelGorevi = m.Personel.PersonelGorevleri.Gorev
            }).ToList();
            return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı", data = rand, PersonelList = pers }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CekimRandevuIptal(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            Randevu randevu = dbContext.Randevus.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (randevu == null)
                return Json(new { Sonuc = false, Bilgi = "İptal edilmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            long SozlesmeId = Convert.ToInt64(randevu.SozlesmeId);
            randevu.Iptal = true;
            randevu.DegistirenKullaniciId = KullaniciId;
            randevu.DegistirmeTarih = DateTime.Now;
            string[] PersonelIdler = randevu.GorevliPersonellerId.Split(',');
            foreach (var pers in PersonelIdler)
            {
                long pId = Convert.ToInt64(pers);
                RandevuToPersonel rtp = dbContext.RandevuToPersonels.FirstOrDefault(x => x.RandevuId == randevu.Id && x.PersonelId == pId);
                rtp.Aktif = false;
                rtp.DegistirenKullaniciId = KullaniciId;
                rtp.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();
            }
            try
            {
                dbContext.SaveChanges();
                //var rand = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.SozlesmeId == SozlesmeId && x.CekimRandevu == true && x.Aktif == true && x.Sil == false).Select(m => new
                var rand = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.CekimRandevu == true && x.Aktif == true && x.Sil == false).Select(m => new
                {
                    Id = m.Id,
                    Tarih = m.Baslangic,
                    Baslangic = m.Baslangic,
                    Bitis = m.Bitis,
                    Aciklama = m.Aciklama,
                    Personeller = m.GorevliPersonellerId,
                    Iptal = m.Iptal
                }).OrderBy(x => x.Baslangic).ToList();
                var pers = dbContext.RandevuToPersonels.Where(x => x.RandevuId == randevu.Id).Select(m => new
                {
                    PersonelId = m.PersonelId,
                    PersonelAdiSoyadi = m.Personel.AdiSoyadi,
                    PersonelGorevi = m.Personel.PersonelGorevleri.Gorev
                }).ToList();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı", data = rand, PersonelList = pers }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Çekim Randevusu İptal et, Randevu Id: " + id + " Sözleşme Id: " + SozlesmeId;
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
        public ActionResult CekimRandevuSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            Randevu randevu = dbContext.Randevus.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (randevu == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            long SozlesmeId = Convert.ToInt64(randevu.SozlesmeId);
            
            // RandevutoPersonel tablosundaki kayıtlarda silinecek
            string[] PersonelIdler = randevu.GorevliPersonellerId.Split(',');
            foreach (var pers in PersonelIdler)
            {
                long pId = Convert.ToInt64(pers);
                RandevuToPersonel rtp = dbContext.RandevuToPersonels.FirstOrDefault(x => x.RandevuId == randevu.Id && x.PersonelId == pId);
                dbContext.RandevuToPersonels.Remove(rtp);
            }
            dbContext.Randevus.Remove(randevu);
            try
            {
                dbContext.SaveChanges();
                //var rand = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.SozlesmeId == SozlesmeId && x.CekimRandevu == true && x.Aktif == true && x.Sil == false).Select(m => new
                var rand = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.CekimRandevu == true && x.Aktif == true && x.Sil == false).Select(m => new
                {
                    Id = m.Id,
                    Tarih = m.Baslangic,
                    Baslangic = m.Baslangic,
                    Bitis = m.Bitis,
                    Aciklama = m.Aciklama,
                    Personeller = m.GorevliPersonellerId,
                    Iptal = m.Iptal
                }).OrderBy(x => x.Baslangic).ToList();
                var pers = dbContext.RandevuToPersonels.Where(x => x.RandevuId == randevu.Id).Select(m => new
                {
                    PersonelId = m.PersonelId,
                    PersonelAdiSoyadi = m.Personel.AdiSoyadi,
                    PersonelGorevi = m.Personel.PersonelGorevleri.Gorev
                }).ToList();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı", data = rand, PersonelList = pers }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Çekim Randevusu Sil, Randevu Id: " + id + " Sözleşme Id: " + SozlesmeId;
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
        public ActionResult CekimRandevuSMSGonder()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            long RandevuId = Convert.ToInt64(Request["RandevuId"]);
            string sonuc_musteri_randevu_sms = "";
            string randevubilgi = "";
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.Sil == false && x.Aktif == true);
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            bool smskabul = true;
            if (musteri == null)
                smskabul = true;
            else
                smskabul = musteri.SMSKabul;

            //Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == PersonelId && x.Aktif == true && x.Sil == false && x.SMSKabul == true);
            Randevu randevu = dbContext.Randevus.FirstOrDefault(x => x.Id == RandevuId && x.Aktif == true && x.Sil == false);
            randevubilgi = randevu.Baslangic.ToShortDateString() + " saat " + randevu.Baslangic.ToString(@"HH\:mm") + "-" + randevu.Bitis.ToString(@"HH\:mm") + " ";
            randevubilgi = randevubilgi + randevu.Aciklama.Replace("<br />", " ") + " ";
            #region Çekim Randevusu Müşteri Sms Bilgi Mesajı
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
            {
                AyarlarSmsGonderim smsayar = null;
                string smsmetin = "";
                smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriCekimRandevusuBilgiGonderimSuresi != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                if (smsayar != null)
                    smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.MusteriCekimRandevusuBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                else
                    smsmetin = null;

                if (smsmetin != null && smsmetin != "")
                {
                    if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                        smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                    if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                        smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                    if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                        smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                    if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{Tarih}") != -1)
                        smsmetin = smsmetin.Replace("{Tarih}", randevu.Baslangic.ToShortDateString());
                    //smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                    if (smsmetin.IndexOf("{RezervasyonTuru}") != -1)
                        smsmetin = smsmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                    if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{RandevuTarihi}", randevu.Baslangic.ToShortDateString());
                    //smsmetin = smsmetin.Replace("{RandevuTarihi}", rnd.Baslangic.ToShortDateString());
                    if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                    if (smsmetin.IndexOf("{Tutar}") != -1)
                        smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                    if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                    if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                        smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                    if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                        smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevubilgi);
                    sonuc_musteri_randevu_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                    return Json(new { Sonuc = true, Mesaj = sonuc_musteri_randevu_sms }, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion
            return Json(new { Sonuc = false, Mesaj = "SMS Gönderilemedi." }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CekimRandevuSMSAyarKotrol()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            AyarlarSmsGonderim smsayar = null;
            string smsmetin = "";
            smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriCekimRandevusuBilgiGonderimSuresi != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
            if (smsayar != null)
                smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.MusteriCekimRandevusuBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
            else
                smsmetin = null;

            if (smsayar != null && smsmetin != null)
            {
                return Json(new { Sonuc = true, Mesaj = "SMS Gönderilebilir." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Sonuc = false, Mesaj = "SMS Gönderim Ayarları Yapılmamış. Lütfen ayarları kontrol ediniz.." }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult SozlesmeSil()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long RandevuId = Convert.ToInt64(Request["RandevuId"]);
            if (SozlesmeId != 0)
            {
                Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                if (sz == null)
                    return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

                List<Randevu> rnd = dbContext.Randevus.Where(x => x.SozlesmeId == SozlesmeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
                if (rnd.Count > 0)
                {
                    foreach (var item in rnd)
                    {
                        List<RandevuToPersonel> randevupersonel = dbContext.RandevuToPersonels.Where(x => x.RandevuId == item.Id).ToList();
                        foreach (var pers in randevupersonel)
                        {
                            //pers.Aktif = false;
                            //pers.Sil = true;
                            //pers.DegistirenKullaniciId = KullaniciId;
                            //pers.DegistirmeTarih = DateTime.Now;
                            dbContext.RandevuToPersonels.Remove(pers);
                        }
                        //item.Aktif = false;
                        //item.Sil = true;
                        //item.DegistirenKullaniciId = KullaniciId;
                        //item.DegistirmeTarih = DateTime.Now;
                        dbContext.Randevus.Remove(item);
                        dbContext.SaveChanges();
                    }
                }
                List<Odemeler> odeme = dbContext.Odemelers.Where(x => x.SozlesmeId == SozlesmeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
                if (odeme.Count > 0)
                {
                    foreach (var item in odeme)
                    {
                        //item.Aktif = false;
                        //item.Sil = true;
                        //item.DegistirenKullaniciId = KullaniciId;
                        //item.DegistirmeTarih = DateTime.Now;
                        dbContext.Odemelers.Remove(item);
                        dbContext.SaveChanges();
                    }
                }
                List<TelefonRehberi> rehber = dbContext.TelefonRehberis.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == SozlesmeId && x.Aktif == true && x.Sil == false).ToList();
                if (rehber.Count > 0)
                {
                    foreach (var item in rehber)
                    {
                        //item.Aktif = false;
                        //item.Sil = true;
                        //item.DegistirenKullaniciId = KullaniciId;
                        //item.DegistirmeTarih = DateTime.Now;
                        dbContext.TelefonRehberis.Remove(item);
                        dbContext.SaveChanges();
                    }
                }

                //sz.Aktif = false;
                //sz.Sil = true;
                //sz.DegistirenKullaniciId = KullaniciId;
                //sz.DegistirmeTarih = DateTime.Now;
                dbContext.Sozlesmes.Remove(sz);
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Bilgi = "Kayıt Silindi", JsonRequestBehavior.AllowGet });
                }
                catch (Exception)
                {
                    return Json(new { Sonuc = false, Bilgi = "Kayıt silinirken bir hata oluştu", JsonRequestBehavior.AllowGet });
                }
            }
            else
                return Json(new { Sonuc = true, Bilgi = "Kayıt Silindi", JsonRequestBehavior.AllowGet });

        }
        [HttpPost]
        public ActionResult SozlesmeBilgileriGetir(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Sozlesme> sz = dbContext.Sozlesmes.Where(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            if (sz == null)
                return Json(new { Sonuc = false, Bilgi = "İstenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            List<AyarlarSozlesmeCikti> ayr = dbContext.AyarlarSozlesmeCiktis.Where(x => x.FirmaId == FirmaId).ToList();
            List<Randevu> rnd = dbContext.Randevus.Where(x => x.SozlesmeId == id && x.CekimRandevu == true && x.FirmaId == FirmaId).ToList();
            List<Odemeler> alinan = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "AlinanOdeme").ToList();
            List<Odemeler> gelecek = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "GelecekOdeme").ToList();
            List<SozlesmeSartlari> sozlesmemetni = dbContext.SozlesmeSartlaris.Where(x => x.FirmaId == FirmaId).ToList();
            var sozlesme = sz.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                Yetkili = m.YetkiliPersonel,
                FirmaAdres = m.Firma.Adres + " " + m.Firma.Ilce.Ilce1 + "/" + m.Firma.Il.Il1,
                FirmaCepTel = m.Firma.CepTel,
                FirmaSabitTel = m.Firma.SabitTel,
                MusteriAdSoyad = m.Musteri.AdiSoyadi,
                MusteriKodu = m.Musteri.MusteriKodu,
                MusteriAdres = m.Musteri.Adres,
                MusteriCepTel = m.Musteri.CepTel,
                MusteriSabitTel = m.Musteri.SabitTel,
                RezervasyonTuru = m.RezervasyonTurleri.RezervasyonTuru,
                RezervasyonTarihi = m.RezervasyonTarihi.ToShortDateString(),
                RezervasyonSaati = m.BaslangicSaat + " - " + m.BitisSaat,
                OrganizasyonYeri = m.OrganizasyonYeri,
                //GorevliPersonel = m.Personel.AdiSoyadi,
                SozlesmeTutari = m.ToplamFiyat,
                Paketler = m.Paketler,
                EkHizmetler = m.EkHizmetler
            });
            var ayar = ayr.Select(m => new
            {
                Id = m.Id,
                FirmaId = m.FirmaId,
                LogoGoster = m.LogoGoster,
                PaketlerGoster = m.PaketlerGoster,
                EkHizmetlerGoster = m.EkHizmetlerGoster,
                CekimRandevulariGoster = m.CekimRandevulariGoster,
                YapilanOdemelerGoster = m.YapilanOdemelerGoster,
                KalanOdemelerGoster = m.KalanOdemelerGoster,
                CepTelefonuGoster = m.CepTelefonuGoster,
                SabitTelefonGoster = m.SabitTelefonGoster,
                FaxGoster = m.FaxGoster
            });
            var randevular = rnd.Select(m => new
            {
                Id = m.Id,
                FirmaId = m.FirmaId,
                //Personel = m.Personel.AdiSoyadi,
                Personeller=m.GorevliPersonellerId,
                Tarih = m.Baslangic.ToShortDateString(),
                Saat = m.Baslangic.ToShortTimeString() + " - " + m.Bitis.ToShortTimeString(),
                CekimYeri = m.Aciklama.Split('>')
            });
          
            var alinanodeme = alinan.Select(m => new
            {
                Id = m.Id,
                FirmaId = m.FirmaId,
                Tarih = m.Tarih.ToShortDateString(),
                Tutar = m.Tutar,
                Aciklama = m.Notlar
            });
            var gelecekodeme = gelecek.Select(m => new
            {
                Id = m.Id,
                FirmaId = m.FirmaId,
                Tarih = m.Tarih.ToShortDateString(),
                Tutar = m.Tutar,
                Aciklama = m.Notlar
            });
            var sozelesme = sozlesmemetni.Select(m => new
            {
                Id = m.Id,
                FirmaId = m.FirmaId,
                SozlesmeMetni = m.SozlesmeSartlari1
            });

            return Json(new { Sozlesme = sozlesme, Ayarlar = ayar, Randevular = randevular, AlinanOdemeler = alinanodeme, GelecekOdemeler = gelecekodeme, SozlesmeMetni = sozelesme }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult PersonelIzinKontrol()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            //long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            string[] PersonelIdArray = Request["PersonelIdArray"].Split(',');
            List<Personel> CalisamazPersonel = new List<Personel>();
            DateTime RezervasyonTarihi = Convert.ToDateTime(Request["RezervasyonTarihi"]);
            List<PersonelIzin> izinler = new List<PersonelIzin>();

            foreach (var personeId in PersonelIdArray)
            {
                long pId = Convert.ToInt64(personeId);
                izinler = dbContext.PersonelIzins.Where(x => x.PersonelId == pId && x.IzinBaslama <= RezervasyonTarihi && x.IzinBitis >= RezervasyonTarihi && x.Aktif == true && x.Sil == false).ToList();
                if (izinler.Count > 0)
                    CalisamazPersonel.Add(dbContext.Personels.FirstOrDefault(x => x.Id == pId && x.Aktif == true && x.Sil == false));
            }
            //List<PersonelIzin> izinler = dbContext.PersonelIzins.Where(x => x.PersonelId == PersonelId && x.IzinBaslama <= RezervasyonTarihi && x.IzinBitis >= RezervasyonTarihi && x.Aktif == true && x.Sil == false).ToList();
            var calisamaz = CalisamazPersonel.Select(m => new
            {
                Id = m.Id,
                AdiSoyadi = m.AdiSoyadi, // Rezervasyon türü

            });
            AyarlarRezervasyon rezervasyonayar = dbContext.AyarlarRezervasyons.FirstOrDefault(x => x.FirmaId == FirmaId);
            if (!rezervasyonayar.PersonelIzinTakibi) // --- Rezervasyon ayarlarında personel izin takibi kapalı ise personel çalışabilir. Değilse
                return Json(new { Sonuc = true, Mesaj = "Personel Çalışabilir" }, JsonRequestBehavior.AllowGet);

            if (CalisamazPersonel.Count > 0)
                return Json(new { Sonuc = false, Mesaj = "İzinli Personel(ler) var", CalisamazPersonel = calisamaz }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Sonuc = true, Mesaj = "Personel Çalışabilir" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult TatilKotrol()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            DateTime RezervasyonTarihi = Convert.ToDateTime(Request["RezervasyonTarihi"]);
            List<TatilGunleri> tatil = dbContext.TatilGunleris.Where(x => x.FirmaId == FirmaId && x.Calisilacak == false && x.Baslangic <= RezervasyonTarihi && x.Bitis >= RezervasyonTarihi && x.Aktif == true && x.Sil == false).ToList();
            AyarlarRezervasyon rezervasyonayar = dbContext.AyarlarRezervasyons.FirstOrDefault(x => x.FirmaId == FirmaId);
            if (!rezervasyonayar.TatilGunuTakibi) // --- Rezervasyon ayarlarında tatil günü takibi kapalı ise personel çalışabilir. Değilse
                return Json(new { Sonuc = true, Mesaj = "Personel Çalışabilir" }, JsonRequestBehavior.AllowGet);
            if (tatil.Count > 0)
                return Json(new { Sonuc = false, Mesaj = "Seçtiğiniz Rezervasyon (Randevu) Tarihi Tatil olarak belirlenmiş. Bu tarihte Rezervasyon (Randevu) oluşturamazsınız.", JsonRequestBehavior.AllowGet });
            else
                return Json(new { Sonuc = true, Mesaj = "Rezervasyon (Randevu) Yapılabilir", JsonRequestBehavior.AllowGet });
        }
        [HttpPost]
        public ActionResult GorevlendirmeKontrol()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            DateTime RezervasyonTarihi = Convert.ToDateTime(Request["RezervasyonTarihi"]);
            string[] PersonelIdArray = Request["PersonelIdArray"].Split(',');
            long ZamanDilimiId = Convert.ToInt64(Request["ZamanDilimiId"]);
            string BaslangicSaati = Request["BaslangicSaati"];
            string BitisSaati = Request["BitisSaati"];
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long RandevuId = Convert.ToInt64(Request["RandevuId"]);
            TimeSpan baslangicsaat = TimeSpan.Parse(BaslangicSaati);
            TimeSpan bitissaat = TimeSpan.Parse(BitisSaati);
            DateTime randevubaslangic; // Randevu tablosu için
            DateTime randevubitis; // Randevu tablosu için

            List<Randevu> randevulist = new List<Randevu>();
            string[] RandevuPersonelIdArray;
            List<string> CalisamazPersonelIdArray = new List<string>();
            List<Personel> CalisamazPersonelList = new List<Personel>();
            // Sorgulana saat aralıkları 
            if (ZamanDilimiId == 0)
            {
                randevubaslangic = RezervasyonTarihi.Add(baslangicsaat);
                randevubitis = RezervasyonTarihi.Add(bitissaat);
            }
            else
            {
                ZamanDilimleri zmn = dbContext.ZamanDilimleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == ZamanDilimiId);
                randevubaslangic = RezervasyonTarihi.Add(zmn.BaslangicZaman);
                randevubitis = RezervasyonTarihi.Add(zmn.BitisZaman);
            }
            if (SozlesmeId!=0 && RandevuId == 0)
            {
                RandevuId = dbContext.Randevus.FirstOrDefault(x => x.SozlesmeId == SozlesmeId && x.Aktif == true && x.Sil == false && x.Iptal == false && x.CekimRandevu == false).Id;
            }
  
            // gelen randevu tarihinde başka randevu var mı bakılıyor. // Randevu düzenleme işlemi yapılacaksa mevcut sözleşmenin randvularına bakılmayacak
            randevulist = dbContext.Randevus.Where(x => x.Iptal == false && x.Aktif == true && x.Sil == false && ((x.Baslangic >= randevubaslangic && x.Bitis <= randevubitis) || (x.Baslangic <= randevubaslangic && x.Bitis >= randevubitis))).ToList();
            // Aranan tarihler arasında randevu varsa
            if (randevulist != null)
            {
                foreach (var randevu in randevulist)
                {
                    // bulunan randevulardan birisi gelen RandevuId ile eşleşiyorsa bu randevu görmezden gelinecek.
                    if (randevu.Id != RandevuId)
                    {
                        //RandevuPersonelIdArray.AddRange(randevu.GorevliPersonellerId.Split(','));
                        RandevuPersonelIdArray = randevu.GorevliPersonellerId.Split(',');
                        //CalisamazPersonelIdArray.AddRange(RandevuPersonelIdArray.Except(PersonelIdArray));
                        CalisamazPersonelIdArray.AddRange(RandevuPersonelIdArray.Intersect(PersonelIdArray));
                    }
                }
            }
            if (CalisamazPersonelIdArray.Count > 0)
            {
                foreach (var persId in CalisamazPersonelIdArray)
                {
                    double personeleid = Convert.ToInt64(persId);
                    CalisamazPersonelList.Add(dbContext.Personels.FirstOrDefault(x => x.Id == personeleid));
                }
            }
            var calisamazlar = CalisamazPersonelList.Select(p => new
            {
                PersonelId = p.Id,
                AdiSoyadi = p.AdiSoyadi
            });

            AyarlarRezervasyon rezervasyonayar = dbContext.AyarlarRezervasyons.FirstOrDefault(x => x.FirmaId == FirmaId);
            if (!rezervasyonayar.PersonelGorevliTakibi) // --- Rezervasyon ayarlarında personelin başka bir işte göreli olduğu takibi kapalı ise personel çalışabilir. Değilse
                return Json(new { Sonuc = true, Mesaj = "Personel Çalışabilir" }, JsonRequestBehavior.AllowGet);
            if (CalisamazPersonelList.Count > 0)
                return Json(new { Sonuc = false, Mesaj = "Personel(ler), Rezervasyon Tarihinde ve saatinde başka bir randevu için görevlendirilmiş. <br/> Lütfen başka bir personel seçiniz.", CalisamazPersonel = calisamazlar }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Sonuc = true, Mesaj = "Personel Çalışabilir" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult BilgiSmsMailGonder(long Id) // Sözleşme Tamamlandığında gönderiliyor. 
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string retunmesaj = "";
            string sonuc_sozlesme_sms = "";
            string sonuc_musteri_randevu_sms = "";
            string sonuc_personel_randevu_sms = "";
            bool sonuc_sozlesme_mail = true;
            bool sonuc_musteri_randevu_mail = true;
            bool sonuc_personel_randevu_mail = true;
            #region Metne eklenecek randevular
            string randevular = "";
            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == Id && x.Aktif == true && x.Sil == false && x.Iptal == false).ToList();
            List<RandevuToPersonel> randeupersonel = new List<RandevuToPersonel>();
            foreach (var item in randevu.OrderBy(x => x.Baslangic.ToShortDateString()))
            {
                randevular = randevular + item.Baslangic.ToShortDateString() + " saat " + item.Baslangic.ToString(@"HH\:mm") + "-" + item.Bitis.ToString(@"HH\:mm") + " ";
                if (!string.IsNullOrEmpty(item.Aciklama))
                {
                    randevular = randevular + item.Aciklama.Replace("<br />", " ") + " ";
                }

                // randunun personelleri burada belirlenecek.
                List<RandevuToPersonel> r2p = dbContext.RandevuToPersonels.Where(x => x.RandevuId == item.Id).ToList();
                randeupersonel.AddRange(r2p);
            }
            #endregion
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == Id && x.Sil == false && x.Aktif == true && x.Iptal == false);
            // Randevu rnd = dbContext.Randevus.FirstOrDefault(y => y.SozlesmeId == sz.Id && y.CekimRandevu == false && y.Aktif == true && y.Sil == false); tek bir randevu değil bu sözleşmenin tüm randevuları için SMS gönderilecek.
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);


            bool smskabul = true;
            bool emailkabul = true;
            if (musteri == null)
            {
                smskabul = true; emailkabul = true;
            }
            else
            {
                smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul;
            }
            #region Sözleşme Sms Bilgi Mesajı
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
            {
                AyarlarSmsGonderim smsayar = null;
                string smsmetin = "";
                if (sz.OpsiyonBit == true || sz.TeklifBit == true)
                {
                    smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.OpsiyonTarihiBilgiGonderimSuresi == 6);  // Sms gönderim ayarı Opsiyon Tarihi Bilgi Gonderim Suresi için "Kayıt Yapıldığında" Seçilmişse
                    if (smsayar != null)
                        smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.OpsiyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                    else
                        smsmetin = null;
                }
                else
                {
                    smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.RezervasyonTarihiBilgiGonderimSuresi == 6);  // Sms gönderim ayarı Rezervasyon Tarihi Bilgi Gonderim Suresi için "Kayıt Yapıldığında" Seçilmişse
                    if (smsayar != null)
                        smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.RezervasyonTarihiBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                    else
                        smsmetin = null;
                }
                if (smsayar != null && smsmetin != null)
                {
                    if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                        smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                    if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                        smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                    if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                        smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                    if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{RezervasyonTuru}") != -1)
                        smsmetin = smsmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                    if (smsmetin.IndexOf("{Tarih}") != -1)
                        smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                    if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                    if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                    if (smsmetin.IndexOf("{Tutar}") != -1)
                        smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                    if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                    if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                        smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                    if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                        smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);

                    sonuc_sozlesme_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                }
            }
            #endregion

            #region Sözleşme Bilgi Emaili
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.Email != "" && emailkabul)
            {
                AyarlarMailGonderim mailayar = null;
                MailMetinleri mmetin = null;
                string mailmetin = "";
                if (sz.OpsiyonBit == true || sz.TeklifBit == true)
                {
                    mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.OpsiyonTarihiBilgiGonderimSuresi == 6 && x.CariTahsilatBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (mailayar == null)
                        return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                    else
                        mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.OpsiyonTarihiBilgiMaili && x.Aktif == true && x.Sil == false);
                }
                else
                {
                    mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.RezervasyonTarihiBilgiGonderimSuresi == 6 && x.CariTahsilatBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (mailayar == null)
                        return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                    else
                        mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.RezervasyonTarihiBilgiMaili && x.Aktif == true && x.Sil == false);
                }
                if (mailayar != null && mmetin != null)
                {
                    mailmetin = mmetin.MailMetni;
                    if (mailmetin.IndexOf("{FirmaAdi}") != -1)
                        mailmetin = mailmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                    if (mailmetin.IndexOf("{GelinAdSoyad}") != -1)
                        mailmetin = mailmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                    if (mailmetin.IndexOf("{DamatAdSoyad}") != -1)
                        mailmetin = mailmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                    if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        mailmetin = mailmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (mailmetin.IndexOf("{RezervasyonTuru}") != -1)
                        mailmetin = mailmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                    if (mailmetin.IndexOf("{Tarih}") != -1)
                        mailmetin = mailmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                    if (mailmetin.IndexOf("{RandevuTarihi}") != -1)
                        mailmetin = mailmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                    if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                        mailmetin = mailmetin.Replace("{OdemeTarihi}", "");
                    if (mailmetin.IndexOf("{Tutar}") != -1)
                        mailmetin = mailmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                    if (mailmetin.IndexOf("{OpsiyonTarihi}") != -1)
                        mailmetin = mailmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                    if (mailmetin.IndexOf("{SozlesmeNo}") != -1)
                        mailmetin = mailmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                    if (mailmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                        mailmetin = mailmetin.Replace("{CekimRandevuBilgileri}", randevular);

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

                        sonuc_sozlesme_mail = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, body, htmlView);
                    }
                    else
                        sonuc_sozlesme_mail = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, mailmetin);
                }
            }
            #endregion

            #region Çekim Randevusu Müşteri Sms Bilgi Mesajı
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
            {
                AyarlarSmsGonderim smsayar = null;
                string smsmetin = "";

                if (sz.OpsiyonBit == false || sz.TeklifBit == false)
                {
                    smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriCekimRandevusuBilgiGonderimSuresi == 6);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (smsayar != null)
                        smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.MusteriCekimRandevusuBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                    else
                        smsmetin = null;
                }
                else
                    smsmetin = null; // Yapılan rezervasyon teklifse veya opsiyonlu ise randevular sms gönderilmeyecek.
                if (smsayar != null && smsmetin != null)
                {
                    if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                        smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                    if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                        smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                    if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                        smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                    if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{Tarih}") != -1)
                        smsmetin = smsmetin.Replace("{Tarih}", randevular);
                    //smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                    if (smsmetin.IndexOf("{RezervasyonTuru}") != -1)
                        smsmetin = smsmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                    if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{RandevuTarihi}", randevular);
                    //smsmetin = smsmetin.Replace("{RandevuTarihi}", rnd.Baslangic.ToShortDateString());
                    if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                    if (smsmetin.IndexOf("{Tutar}") != -1)
                        smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                    if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                    if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                        smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                    if (smsmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                        smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevular);
                    sonuc_musteri_randevu_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                }
            }
            #endregion

            #region Çekim Randevuları Müşteri Bilgi Emaili
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.Email != "" && emailkabul)
            {
                AyarlarMailGonderim mailayar = null;
                MailMetinleri mmetin = null;
                string mailmetin = "";
                if (sz.OpsiyonBit == true || sz.TeklifBit == true)
                {
                    mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.OpsiyonTarihiBilgiGonderimSuresi == 6 && x.CariTahsilatBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (mailayar == null)
                        return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                    else
                        mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.OpsiyonTarihiBilgiMaili && x.Aktif == true && x.Sil == false);
                }
                else
                {
                    mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.RezervasyonTarihiBilgiGonderimSuresi == 6 && x.CariTahsilatBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (mailayar == null)
                        return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                    else
                        mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.RezervasyonTarihiBilgiMaili && x.Aktif == true && x.Sil == false);
                }

                if (mailayar != null && mmetin != null)
                {
                    mailmetin = mmetin.MailMetni;
                    if (mailmetin.IndexOf("{FirmaAdi}") != -1)
                        mailmetin = mailmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                    if (mailmetin.IndexOf("{GelinAdSoyad}") != -1)
                        mailmetin = mailmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                    if (mailmetin.IndexOf("{DamatAdSoyad}") != -1)
                        mailmetin = mailmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                    if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        mailmetin = mailmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (mailmetin.IndexOf("{Tarih}") != -1)
                        mailmetin = mailmetin.Replace("{Tarih}", randevular);
                    //smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                    if (mailmetin.IndexOf("{RezervasyonTuru}") != -1)
                        mailmetin = mailmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                    if (mailmetin.IndexOf("{RandevuTarihi}") != -1)
                        mailmetin = mailmetin.Replace("{RandevuTarihi}", randevular);
                    //smsmetin = smsmetin.Replace("{RandevuTarihi}", rnd.Baslangic.ToShortDateString());
                    if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                        mailmetin = mailmetin.Replace("{OdemeTarihi}", "");
                    if (mailmetin.IndexOf("{Tutar}") != -1)
                        mailmetin = mailmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                    if (mailmetin.IndexOf("{OpsiyonTarihi}") != -1)
                        mailmetin = mailmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                    if (mailmetin.IndexOf("{SozlesmeNo}") != -1)
                        mailmetin = mailmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                    if (mailmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                        mailmetin = mailmetin.Replace("{CekimRandevuBilgileri}", randevular);

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

                        sonuc_musteri_randevu_mail = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, body, htmlView);
                    }
                    else
                        sonuc_musteri_randevu_mail = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, mailmetin);
                }

            }
            #endregion

            foreach (var rndpers in randeupersonel)
            {
                Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == rndpers.PersonelId && x.Aktif == true && x.Sil == false);
                #region Çekim Randevusu Personel Sms Bilgi Mesajı
                if (personel != null && personel.CepTel != "") // Cariye bir cep telefonu girilmiş ise SMS gönder
                {
                    AyarlarSmsGonderim smsayar = null;
                    string smsmetin = "";
                    if (sz.OpsiyonBit == false || sz.TeklifBit == false)
                    {
                        smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.PersonelRandevuBilgiGonderimSuresi == 6);  // Sms gönderim ayarı PersonelRandevuBilgiMesaji için "Kayıt Yapıldığında" Seçilmişse
                        if (smsayar != null)
                            smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.PersonelRandevuBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                        else
                            smsmetin = null;
                    }
                    if (smsayar != null && smsmetin != null)
                    {
                        if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                            smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                        if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                        if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                        if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (smsmetin.IndexOf("{Personel}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            smsmetin = smsmetin.Replace("{Personel}", personel.AdiSoyadi);  //Personel AdSoyad bilgisini Parametre olarak ekle
                        if (smsmetin.IndexOf("{Tarih}") != -1)
                            smsmetin = smsmetin.Replace("{Tarih}", randevular);
                        //smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                        if (smsmetin.IndexOf("{RezervasyonTuru}") != -1)
                            smsmetin = smsmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                        if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{RandevuTarihi}", randevular);
                        //smsmetin = smsmetin.Replace("{RandevuTarihi}", rnd.Baslangic.ToShortDateString());
                        if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{OdemeTarihi}", "");
                        if (smsmetin.IndexOf("{Tutar}") != -1)
                            smsmetin = smsmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                        if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                        if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                            smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                        if (smsmetin.IndexOf("{CekimYeri}") != -1)
                            smsmetin = smsmetin.Replace("{CekimYeri}", "");
                        if (smsmetin.IndexOf("{CekimSaati}") != -1)
                            smsmetin = smsmetin.Replace("{CekimSaati}", "");

                        sonuc_personel_randevu_sms = SMSGonder.Gonder_AtakSms(smsmetin, personel.CepTel, FirmaId, SubeId, KullaniciId);
                    }
                }
                #endregion
                #region Çekim Randevuları Personel Bilgi Emaili
                if (personel != null && personel.CepTel != "")
                {
                    AyarlarMailGonderim mailayar = null;
                    MailMetinleri mmetin = null;
                    string mailmetin = "";
                    if (sz.OpsiyonBit == true || sz.TeklifBit == true)
                    {
                        mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.OpsiyonTarihiBilgiGonderimSuresi == 6 && x.CariTahsilatBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                        if (mailayar == null)
                            return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                        else
                            mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.OpsiyonTarihiBilgiMaili && x.Aktif == true && x.Sil == false);
                    }
                    else
                    {
                        mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.RezervasyonTarihiBilgiGonderimSuresi == 6 && x.CariTahsilatBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                        if (mailayar == null)
                            return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                        else
                            mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.RezervasyonTarihiBilgiMaili && x.Aktif == true && x.Sil == false);
                    }

                    if (mailayar != null && mailmetin != null)
                    {
                        mailmetin = mmetin.MailMetni;
                        if (mailmetin.IndexOf("{FirmaAdi}") != -1)
                            mailmetin = mailmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                        if (mailmetin.IndexOf("{GelinAdSoyad}") != -1)
                            mailmetin = mailmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                        if (mailmetin.IndexOf("{DamatAdSoyad}") != -1)
                            mailmetin = mailmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                        if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            mailmetin = mailmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (mailmetin.IndexOf("{Tarih}") != -1)
                            mailmetin = mailmetin.Replace("{Tarih}", randevular);
                        //smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                        if (mailmetin.IndexOf("{RezervasyonTuru}") != -1)
                            mailmetin = mailmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                        if (mailmetin.IndexOf("{RandevuTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{RandevuTarihi}", randevular);
                        //smsmetin = smsmetin.Replace("{RandevuTarihi}", rnd.Baslangic.ToShortDateString());
                        if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OdemeTarihi}", "");
                        if (mailmetin.IndexOf("{Tutar}") != -1)
                            mailmetin = mailmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                        if (mailmetin.IndexOf("{OpsiyonTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                        if (mailmetin.IndexOf("{SozlesmeNo}") != -1)
                            mailmetin = mailmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
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

                            sonuc_personel_randevu_mail = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, body, htmlView);
                        }
                        else
                            sonuc_personel_randevu_mail = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, mailmetin);
                    }

                }
                #endregion
            }

            if (sonuc_sozlesme_sms == "Mesaj Başarıyla Göderildi")
                retunmesaj = retunmesaj + "Sözleşe Bilgilendirme SMS'i müşteriye gönderildi. <br />";
            else
                retunmesaj = retunmesaj + "Sözleşe Bilgilendirme SMS'i müşteriye gönderimi BAŞARISIZ! <br />";
            if (sonuc_musteri_randevu_sms == "Mesaj Başarıyla Göderildi")
                retunmesaj = retunmesaj + "Randevu Bilgilendirme SMS'i müşteriye gönderildi. <br />";
            else
                retunmesaj = retunmesaj + "Randevu Bilgilendirme SMS'i müşteriye gönderimi BAŞARISIZ! <br />";
            if (sonuc_personel_randevu_sms == "Mesaj Başarıyla Göderildi")
                retunmesaj = retunmesaj + "Randevu Bilgilendirme SMS'i personele gönderildi. <br />";
            else
                retunmesaj = retunmesaj + "Randevu Bilgilendirme SMS'i personele gönderimi BAŞARISIZ! <br />";

            if (sonuc_sozlesme_mail)
                retunmesaj = retunmesaj + "Sözleşe Bilgilendirme Email'i müşteriye gönderildi. <br />";
            else
                retunmesaj = retunmesaj + "Sözleşe Bilgilendirme Email'i müşteriye gönderimi BAŞARISIZ!<br/>Müşteri mail adresini veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz. <br />";
            if (sonuc_musteri_randevu_mail)
                retunmesaj = retunmesaj + "Randevu Bilgilendirme Email'i müşteriye gönderildi. <br />";
            else
                retunmesaj = retunmesaj + "Randevu Bilgilendirme Email'i müşteriye gönderimi BAŞARISIZ!<br/>Müşteri mail adresini veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz. <br />";
            if (sonuc_personel_randevu_mail)
                retunmesaj = retunmesaj + "Randevu Bilgilendirme Email'i personele gönderildi. <br />";
            else
                retunmesaj = retunmesaj + "Randevu Bilgilendirme Email'i personele gönderimi BAŞARISIZ!<br/>Müşteri mail adresini veya <br/>Mail Gönderim Ayarlarınızı kontrol ediniz. <br />";

            return Json(new { Sonuc = true, Mesaj = retunmesaj }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region Rezervasyon(Sözleşme) Düzenleme(Güncelle) İşlemleri
        public ActionResult RezervasyonDuzenle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            ViewBag.UstMenu = "Rezervasyon İşlemleri";
            ViewBag.AltMenu = "Rezervasyon Listesi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == id && x.Sil == false && x.Aktif == true && x.FirmaId == FirmaId);
            if (sz == null || id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
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
            List<Surecler> surecler = dbContext.Sureclers.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).OrderBy(x => x.Sira).ToList();
            ViewBag.Surecler = surecler;
            List<Randevu> cekimrandevulari = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.SozlesmeId == id && x.CekimRandevu == true && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.CekimRandevulari = cekimrandevulari;
            List<Odemeler> alinan = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "AlinanOdeme" && x.GelecekOdemeID == null).ToList();
            List<Odemeler> gelecek = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "GelecekOdeme").ToList();
            ViewBag.AlinanOdemeler = alinan;
            ViewBag.GelecekOdemeler = gelecek;
            Randevu szRandevu = dbContext.Randevus.FirstOrDefault(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.CekimRandevu == false && x.Aktif == true && x.Sil == false);
            //List<RandevuToPersonel> szPersonelList = dbContext.RandevuToPersonels.Where(x => x.RandevuId == szRandevu.Id).ToList();
            //ViewBag.szPersonelList = szPersonelList;

            if (szRandevu != null)
            {
                List<RandevuToPersonel> szPersonelList = dbContext.RandevuToPersonels.Where(x => x.RandevuId == szRandevu.Id).ToList();
                ViewBag.szPersonelList = szPersonelList;
            }
            else
                ViewBag.szPersonelList = null;

            List<KullaniciYetki> ky = dbContext.KullaniciYetkis.Where(x => x.FirmaId == FirmaId && x.KullaniciId == KullaniciId).ToList();
            ViewBag.KullaniciYetkileri = ky;
            return View(sz);

        }
        #endregion
        #region Rezervasyon Alınan Ödeme Makbuz Yazdırma İşemleri
        public ActionResult AlinanOdemeMakbuztoPDF(long? id)
        {
            //if (Session.Count == 0)
            //    return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.Id == id);
            if (odeme == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == odeme.FirmaId);

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
        public ActionResult AlinanOdemeMakbuzYazdir(long id)
        {
            // id = alinanodemeid
            var p = new ActionAsPdf("AlinanOdemeMakbuztoPDF", new { id = id })
            {
                CustomSwitches = "--page-offset 0 --footer-center [page]/[toPage] --footer-font-size 10",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 }// ayarlar sayfasından alınabilir
            };
            return p;
        }
        #endregion
        #region Rezervasyon Listesi İşlemleri
        public ActionResult RezervasyonListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            ViewBag.UstMenu = "Rezervasyon İşlemleri";
            ViewBag.AltMenu = "Rezervasyon Listesi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 21 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).RezervasyonListesi;
            ViewBag.FiltreAyar = filtreayar;
            ViewBag.kullaniciId = KullaniciId;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 21 && x.Aktif == true && x.Sil == false);
            ViewBag.FotografYukleSayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 36 && x.Aktif == true && x.Sil == false).SayfaYetki;

            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            ViewBag.SubeKullanici = subekullanici;
            List<Sube> subeler = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Subeler = subeler;
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
            List<Surecler> surecler = dbContext.Sureclers.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).OrderBy(x => x.Sira).ToList();
            ViewBag.Surecler = surecler;

            //string Il = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId).Il.Il1;
            //string Ilce = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId).Ilce.Ilce1;
            //string Adres = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId).Adres;
            //ViewBag.FirmaAdres = Adres + " " + Ilce + " / " + Il;
            // ---- Sözleşme Numarası
            long sozlesmeno = 0;
            List<Sozlesme> ss = dbContext.Sozlesmes.Select(x => x).ToList();
            if (ss.Count > 0)
                sozlesmeno = dbContext.Sozlesmes.Max(x => x.Id);
            sozlesmeno = sozlesmeno + 1;
            ViewBag.SozlesmeNo = FirmaId.ToString() + "00" + sozlesmeno.ToString();
            // ---- Sözleşme Numarası
            // ---- Müşreti Kodu
            long musterikodu = 0;
            List<Models.Musteri> mm = dbContext.Musteris.Select(x => x).ToList();
            if (mm.Count > 0)
                musterikodu = dbContext.Musteris.Max(x => x.Id);
            musterikodu = musterikodu + 1;
            ViewBag.Musterikodu = FirmaId.ToString() + "0" + musterikodu.ToString();
            // ---- Müşreti Kodu
            return View();
        }
        public ActionResult RezervasyonListesiFiltre(string TarihSecim, string IlkTarih, string SonTarih, int Sube, int Goruntule)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<Sozlesme> sozlesmeler = null;
            //string RezervasyonDurum = "";
            //if (Goruntule == 1)
            //    RezervasyonDurum = "Deval Eden Sözleşme";
            //else if (Goruntule == 2)
            //    RezervasyonDurum = "İptal Sözleşme";
            //else if (Goruntule == 3)
            //    RezervasyonDurum = "Biten Sözleşme";
            if (TarihSecim == "on") // Filter tarihleri Sözleşme tarihi
            {
                if (Sube == 0 && Goruntule == 0)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 1)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Iptal == false && x.Bitti == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 2)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Iptal == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 3)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Bitti == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 0)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 1)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Iptal == false && x.Bitti == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 2)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Iptal == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 3)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Bitti == true).OrderBy(x => x.SozlesmeNo).ToList();
            }
            else // Filter tarihleri Rezervasyon tarihi
            {
                if (Sube == 0 && Goruntule == 0)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 1)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Iptal == false && x.Bitti == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 2)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Iptal == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 3)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Bitti == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 0)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 1)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Iptal == false && x.Bitti == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 2)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Iptal == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 3)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && x.Bitti == true).OrderBy(x => x.SozlesmeNo).ToList();
            }

            //RandevuToPersonel rtop = new RandevuToPersonel();
            //Randevu randevu = new Randevu();

            //var pers = dbContext.RandevuToPersonels.Where(x => x.RandevuId == randevu.Id).Select(m => new
            //{
            //    PersonelId = m.PersonelId,
            //    PersonelAdiSoyadi = m.Personel.AdiSoyadi,
            //    PersonelGorevi = m.Personel.PersonelGorevleri.Gorev
            //}).ToList();

            var rezlist = sozlesmeler.Select(m => new
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
                Sil = m.Sil,
                FotografSecimDurum = m.FotografSecimDurum,
                FotografSecimDurumTarihi = m.FotografSecimDurumTarihi
            });

            return Json(new { data = rezlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RezervasyonlarListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            List<Sozlesme> sozlesme;
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).RezervasyonListesi;
            DateTime IlkTarih = DateTime.Today;
            DateTime SonTarih = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            switch (filtreayar)
            {
                case "0": // 1 Gün
                    IlkTarih = DateTime.Today;
                    SonTarih = DateTime.Today;
                    break;
                case "1": // 1 Hafta Öncesi - 1 Hafta Sonrası
                    IlkTarih = DateTime.Today.AddDays(-7);
                    SonTarih = DateTime.Today.AddDays(7);
                    break;
                case "2": // 1 Ay Öncesi - 1 Ay Sonrası
                    IlkTarih = DateTime.Today.AddMonths(-1);
                    SonTarih = DateTime.Today.AddMonths(1);
                    break;
                case "3": // 1 Yıl Öncesi - 1 Yıl Sonrası
                    IlkTarih = DateTime.Today.AddYears(-1);
                    SonTarih = DateTime.Today.AddYears(1);
                    break;
                case "4": // Ayın başından bu güne
                    IlkTarih = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    SonTarih = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case "5": // Yılın başından bu güne
                    IlkTarih = new DateTime(DateTime.Now.Year, 1, 1);
                    SonTarih = DateTime.Now;
                    break;
            }
            //Müşteri Id si olmayan Sözleşmeler silinecek
            #region SözleşmeSil
            sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && (x.MusteriId == null || x.Paketler == null)).ToList();
            foreach (var item in sozlesme)
            {
                Randevu rnd = dbContext.Randevus.FirstOrDefault(x => x.SozlesmeId == item.Id);
                List<RandevuToPersonel> rndpers = dbContext.RandevuToPersonels.Where(x => x.RandevuId == rnd.Id).ToList();
                foreach (var pers in rndpers)
                {
                    dbContext.RandevuToPersonels.Remove(pers);
                }
                dbContext.Randevus.Remove(rnd);
                dbContext.Sozlesmes.Remove(item);
                dbContext.SaveChanges();
            }
            #endregion
            //sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.TeklifBit == false && x.KesinRezervasyonBit == true && ((x.SozlesmeTarihi.Day >= IlkTarih.Day && x.SozlesmeTarihi.Month >= IlkTarih.Month && x.SozlesmeTarihi.Year >= IlkTarih.Year) && (x.SozlesmeTarihi.Day <= SonTarih.Day && x.SozlesmeTarihi.Month <= SonTarih.Month && x.SozlesmeTarihi.Year <= SonTarih.Year))).OrderBy(x => x.SozlesmeNo).ToList();
            if (AktifSubeId == 0)
                sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && ((x.SozlesmeTarihi >= IlkTarih) && (x.SozlesmeTarihi <= SonTarih))).OrderBy(x => x.SozlesmeNo).ToList();
            else
                sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && x.Aktif == true && x.Sil == false && (x.TeklifBit == false || x.OpsiyonBit == false) && x.KesinRezervasyonBit == true && ((x.SozlesmeTarihi >= IlkTarih) && (x.SozlesmeTarihi <= SonTarih))).OrderBy(x => x.SozlesmeNo).ToList();

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
                Sil = m.Sil,
                FotografSecimDurum = m.FotografSecimDurum,
                FotografSecimDurumTarihi = m.FotografSecimDurumTarihi
            });

            return Json(new { data = sz }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SozlesmeIptalEt(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (sz == null)
                return Json(new { Sonuc = false, Bilgi = "İptal edilmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            sz.Iptal = true;
            sz.DegistirenKullaniciId = KullaniciId;
            sz.DegistirmeTarih = DateTime.Now;
            //Sözleşme iptal edildikten sonra sözleşmeye ait randevu kayıtları silinecek
            List<Randevu> rnd = dbContext.Randevus.Where(x => x.SozlesmeId == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
            if (rnd.Count > 0)
            {
                foreach (var item in rnd)
                {
                    List<RandevuToPersonel> randevupersonel = dbContext.RandevuToPersonels.Where(x => x.RandevuId == item.Id).ToList();
                    foreach (var pers in randevupersonel)
                    {
                        pers.Iptal = true;
                        pers.DegistirenKullaniciId = KullaniciId;
                        pers.DegistirmeTarih = DateTime.Now;
                    }
                    item.Iptal = true;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            //Sözleşme iptal edildikten sonra sözleşmeye ait Ödeme Kayıtları silinecek
            List<Odemeler> alinanodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "AlinanOdeme" && x.Sil == false && x.Aktif == true).ToList();
            List<Odemeler> gelecekodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "GelecekOdeme" && x.Sil == false && x.Aktif == true).ToList();
            if (alinanodemeler.Count > 0)
            {
                foreach (var item in alinanodemeler)
                {
                    item.Iptal = true;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            if (gelecekodemeler.Count > 0)
            {
                foreach (var item in gelecekodemeler)
                {
                    item.Sil = true;
                    item.Aktif = false;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            List<GelirGider> gg = dbContext.GelirGiders.Where(x => x.SozlesmeId == id && x.Sil == false && x.Aktif == true).ToList();
            GelirGider gider = new GelirGider();
            if (gg.Count > 0)
            {
                foreach (var item in gg)
                {
                    gider.SubeId = SubeId;
                    gider.FirmaId = FirmaId;
                    gider.Tarih = DateTime.Now; ;
                    gider.SozlesmeId = sz.Id;
                    gider.GisId = 1;
                    gider.CariHareketId = 1;
                    gider.PersonelOdemeId = 1;
                    gider.Tip = "Gider";
                    gider.GelirGiderTurId = item.GelirGiderTurId;
                    gider.Tutar = item.Tutar;
                    gider.Notlar = sz.SozlesmeNo + " Numaralı sözleşmenin İptaliden oluşan geri ödeme";
                    gider.OlusturanKullaniciId = KullaniciId;
                    gider.OlusturmaTarih = DateTime.Now;
                    gider.DegistirenKullaniciId = KullaniciId;
                    gider.DegistirmeTarih = DateTime.Now;
                    dbContext.GelirGiders.Add(gider);
                }
            }
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt İptal Edildi" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Sözleşme İptel et, Kayıt Id: " + id;
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
        public ActionResult SozlesmeListeSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (sz == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            sz.Sil = true;
            sz.Aktif = false;
            sz.DegistirenKullaniciId = KullaniciId;
            sz.DegistirmeTarih = DateTime.Now;
            // Sözleşme silindikten sonra Alinan Odemeler, Gelecek Ödemeler, Gelirler, Randevular da silinecek
            List<Randevu> rnd = dbContext.Randevus.Where(x => x.SozlesmeId == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            if (rnd.Count > 0)
            {
                foreach (var item in rnd)
                {
                    List<RandevuToPersonel> randevupersonel = dbContext.RandevuToPersonels.Where(x => x.RandevuId == item.Id).ToList();
                    foreach (var pers in randevupersonel)
                    {
                        pers.Aktif = false;
                        pers.Sil = true;
                        pers.DegistirenKullaniciId = KullaniciId;
                        pers.DegistirmeTarih = DateTime.Now;
                    }
                    item.Sil = true;
                    item.Aktif = false;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            List<Odemeler> alinanodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "AlinanOdeme" && x.Sil == false && x.Aktif == true).ToList();
            List<Odemeler> gelecekodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "GelecekOdeme" && x.Sil == false && x.Aktif == true).ToList();
            if (alinanodemeler.Count > 0)
            {
                foreach (var item in alinanodemeler)
                {
                    item.Sil = true;
                    item.Aktif = false;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            if (gelecekodemeler.Count > 0)
            {
                foreach (var item in gelecekodemeler)
                {
                    item.Sil = true;
                    item.Aktif = false;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            List<GelirGider> gg = dbContext.GelirGiders.Where(x => x.SozlesmeId == id).ToList();
            if (gg.Count > 0)
            {
                foreach (var item in gg)
                {
                    item.Sil = true;
                    item.Aktif = false;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
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
                hata.Islem = "Sözleşme İptel et, Kayıt Id: " + id;
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
        public ActionResult SozlesmeBitir(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (sz == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            sz.Bitti = true;
            sz.KilitBit = true;
            sz.DegistirenKullaniciId = KullaniciId;
            sz.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                //Bu sözleşmeye ait ödeme kayıtları kilitlenecek.
                List<Odemeler> odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.Aktif == true && x.Sil == false && x.OdemeTuru == "AlinanOdeme").ToList();
                foreach (var odeme in odemeler)
                {
                    odeme.KilitBit = true;
                    odeme.DegistirenKullaniciId = KullaniciId;
                    odeme.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }
                List<GelirGider> gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.Aktif == true && x.Sil == false).ToList();
                foreach (var gelirgider in gg)
                {
                    gelirgider.KilitBit = true;
                    gelirgider.DegistirenKullaniciId = KullaniciId;
                    gelirgider.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }
                return Json(new { Sonuc = true, Mesaj = "Sözleşme başarıyla tamamlandı", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Rezervasyon Listesi, Sözleşme Bitir";
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
        public string AlacakVarMi(long id)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Odemeler> gelecekodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "GelecekOdeme" && x.Sil == false && x.Aktif == true).ToList();
            //if (gelecekodemeler.Count > 0)
            //    return "var";
            //else
            //    return "yok";
            return gelecekodemeler.Count > 0 ? "var" : "yok";
        }
        [HttpPost]
        public ActionResult SozlesmeSurecGuncelle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            string SurecIdler = Request["surecGuncelleSecilenID"];
            string[] surecler = Request["surecGuncelleSecilenID"].Split(',');
            long SozlesmeId = Convert.ToInt64(Request["Sozlesmeid"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            string surecadlari = "";
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            string sonuc_musteri_randevu_sms = "";
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == sz.MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            bool smskabul = true;
            if (musteri == null)
                smskabul = true;
            else
                smskabul = musteri.SMSKabul;

            List<string> srcad = new List<string>();

            for (int i = 0; i < surecler.Length; i++)
            {
                long surecid = Convert.ToInt64(surecler[i]);
                Surecler surec = dbContext.Sureclers.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == surecid && x.Aktif == true && x.Sil == false);
                surecadlari = surecadlari + surec.SurecAdi + "<br />";
                srcad.Add(surec.SurecAdi);
             
            }
            sz.Surecler = surecadlari;
            sz.SureclerId = SurecIdler;
            sz.DegistirenKullaniciId = KullaniciId;
            sz.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                #region Süreç Güncelleme Bilgi Smsi Gönder
                if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                {
                    AyarlarSmsGonderim smsayar = null;
                    string smsmetin = "";
                    smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.SurecDegisiklikBilgiGonderimSuresi != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (smsayar != null)
                        smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.SurecDegisiklikBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                    else
                        smsmetin = null;

                    if (smsayar != null && smsmetin != null)
                    {
                        if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                            smsmetin = smsmetin.Replace("{FirmaAdi}", sz.Firma.FirmaAdi);
                        if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{GelinAdSoyad}", sz.GelinAd);
                        if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{DamatAdSoyad}", sz.DamatAd);
                        if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                        if (smsmetin.IndexOf("{Tarih}") != -1)
                            smsmetin = smsmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                        if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                        if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                            smsmetin = smsmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                        if (smsmetin.IndexOf("{Surec}") != -1)
                            smsmetin = smsmetin.Replace("{Surec}", srcad.Last());
                        sonuc_musteri_randevu_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Sözleşme - Süreç Güncelle";
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
            return Json(new { Sonuc = true, Mesaj = "Süreçler Güncellendi", Sms_Mesaj = sonuc_musteri_randevu_sms }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SozlesmeRandevulari(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            List<Randevu> randevu = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.Aktif == true && x.Sil == false).OrderBy(x => x.Baslangic).ToList();
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
            return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi", data = randevular }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Rezervasyon Önizleme ve Yazdır/İndirme İşlemleri
        public ActionResult RezervasyonOnizleme(long? id)
        {
            //if (Session.Count == 0)
            //    return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.SozlesmeNo == id);
            if (sz == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            AyarlarSozlesmeCikti ciktiayar = dbContext.AyarlarSozlesmeCiktis.FirstOrDefault(x => x.FirmaId == sz.FirmaId);
            ViewBag.SozlesmeAyar = ciktiayar;

            List<Randevu> rnd = dbContext.Randevus.Where(x => x.SozlesmeId == sz.Id && x.Sil == false && x.Aktif == true).OrderBy(x => x.Baslangic).ToList();
            ViewBag.CekimRandevulari = rnd;

            ViewBag.SozlesmeMetni = dbContext.SozlesmeSartlaris.FirstOrDefault(x => x.FirmaId == sz.FirmaId).SozlesmeSartlari1;
            List<Odemeler> alinan = dbContext.Odemelers.Where(x => x.SozlesmeId == sz.Id && x.OdemeTuru == "AlinanOdeme" && x.GelecekOdemeID == null).ToList();
            List<Odemeler> gelecek = dbContext.Odemelers.Where(x => x.SozlesmeId == sz.Id && x.OdemeTuru == "GelecekOdeme").ToList();
            ViewBag.alinanodemeler = alinan;
            ViewBag.kalanodemeler = gelecek;
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == sz.FirmaId);
            ViewBag.Firma = frm;
            return View(sz);
        }
        public ActionResult SozlesmeYazdir(long id)
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
        public ActionResult SozlesmeIndir(long id)
        {
            // id sözleşme numarası yada sözleşme id
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == id);
            var p = new ActionAsPdf("RezervasyonOnizleme", new { id = id })
            {
                CustomSwitches = "--page-offset 0 --footer-center [page]/[toPage] --footer-font-size 10",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },  // ayarlar sayfasından alınabilir
                FileName = sz.SozlesmeTarihi.ToShortDateString().Replace(".", "-") + "-" + sz.SozlesmeNo + ".pdf"
            };
            return p;
        }
        #endregion
        public ActionResult YillikRezervasyonTakvimi()
        {
            ViewBag.UstMenu = "Rezervasyon İşlemleri";
            ViewBag.AltMenu = "Yıllık Rezervasyon Takvimi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            return View();
        }
        #region Rezervasyon Teklifleri İşlemleri
        public ActionResult RezervasyonTeklifleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Rezervasyon İşlemleri";
            ViewBag.AltMenu = "Rezervasyon Teklifleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 22 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            //List<VW_Kullanici_Gorevler_Resim> kullanicilar = dbContext.VW_Kullanici_Gorevler_Resim.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            //ViewBag.kullanicilar = kullanicilar;
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).RezervasyonTeklifleri;
            ViewBag.FiltreAyar = filtreayar;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 22 && x.Aktif == true && x.Sil == false);
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            ViewBag.SubeKullanici = subekullanici;
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
            return View();
        }
        [HttpPost]
        public ActionResult RezervasyonTeklifleriListesiFiltre(string TarihSecim, string IlkTarih, string SonTarih, int Sube, int Goruntule)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<Sozlesme> sozlesmeler = null;
            //string RezervasyonDurum = "";
            //if (Goruntule == 1)
            //    RezervasyonDurum = "Rezervasyon Teklifleri";
            //else if (Goruntule == 2)
            //    RezervasyonDurum = "Opsiyonlu Rezervasyonlar";
            //else if (Goruntule == 3)
            //    RezervasyonDurum = "İptal Rezervasyonlar";
            if (TarihSecim == "on") // Filter tarihleri Sözleşme tarihi
            {
                if (Sube == 0 && Goruntule == 0)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == true || x.OpsiyonBit == true) && x.KesinRezervasyonBit == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 1)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.TeklifBit == true && x.OpsiyonBit == false && x.KesinRezervasyonBit == false && x.Iptal == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 2)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.TeklifBit == false && x.OpsiyonBit == true && x.KesinRezervasyonBit == false && x.Iptal == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 3)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == true || x.OpsiyonBit == true) && x.KesinRezervasyonBit == false && x.Iptal == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 0)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == true || x.OpsiyonBit == true) && x.KesinRezervasyonBit == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 1)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.TeklifBit == true && x.OpsiyonBit == false && x.KesinRezervasyonBit == false && x.Iptal == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 2)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.TeklifBit == false && x.OpsiyonBit == true && x.KesinRezervasyonBit == false && x.Iptal == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 3)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.SozlesmeTarihi >= ilktarih) && (x.SozlesmeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == true || x.OpsiyonBit == true) && x.KesinRezervasyonBit == false && x.Iptal == true).OrderBy(x => x.SozlesmeNo).ToList();
            }
            else // Filter tarihleri Rezervasyon tarihi
            {
                if (Sube == 0 && Goruntule == 0)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == true || x.OpsiyonBit == true) && x.KesinRezervasyonBit == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 1)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.TeklifBit == true && x.OpsiyonBit == false && x.KesinRezervasyonBit == false && x.Iptal == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 2)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.TeklifBit == false && x.OpsiyonBit == true && x.KesinRezervasyonBit == false && x.Iptal == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube == 0 && Goruntule == 3)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == true || x.OpsiyonBit == true) && x.KesinRezervasyonBit == false && x.Iptal == true).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 0)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == true || x.OpsiyonBit == true) && x.KesinRezervasyonBit == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 1)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.TeklifBit == true && x.OpsiyonBit == false && x.KesinRezervasyonBit == false && x.Iptal == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 2)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.TeklifBit == false && x.OpsiyonBit == true && x.KesinRezervasyonBit == false && x.Iptal == false).OrderBy(x => x.SozlesmeNo).ToList();
                else if (Sube != 0 && Goruntule == 3)
                    sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.RezervasyonTarihi >= ilktarih) && (x.RezervasyonTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && (x.TeklifBit == true || x.OpsiyonBit == true) && x.KesinRezervasyonBit == false && x.Iptal == true).OrderBy(x => x.SozlesmeNo).ToList();
            }


            var sz = sozlesmeler.Select(m => new
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

                BaslangicSaat = m.BaslangicSaat.ToString(),
                BitisSaat = m.BitisSaat.ToString(),
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
                OpsiyonBit = m.OpsiyonBit,
                Paketler = m.Paketler,
                EkHizmetler = m.EkHizmetler,
                PeketlerFiyat = m.PaketlerFiyat,
                EkHizmetlerFiyat = m.EkHizmetlerFiyat,
                SozlesmeTutar = m.ToplamFiyat,
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
                Aciklama = m.Notlar,
                Durum = m.Durum,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil,
                FotografSecimDurum = m.FotografSecimDurum,
                FotografSecimDurumTarihi = m.FotografSecimDurumTarihi
            });
            return Json(new { data = sz }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RezervasyonTeklifListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).RezervasyonTeklifleri;
            DateTime IlkTarih = DateTime.Today;
            DateTime SonTarih = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            switch (filtreayar)
            {
                case "0": // 1 Gün
                    IlkTarih = DateTime.Today;
                    SonTarih = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case "1": // 1 Hafta Öncesi - 1 Hafta Sonrası
                    IlkTarih = DateTime.Today.AddDays(-7);
                    SonTarih = DateTime.Today.AddDays(7);
                    break;
                case "2": // 1 Ay Öncesi - 1 Ay Sonrası
                    IlkTarih = DateTime.Today.AddMonths(-1);
                    SonTarih = DateTime.Today.AddMonths(1);
                    break;
                case "3": // 1 Yıl Öncesi - 1 Yıl Sonrası
                    IlkTarih = DateTime.Today.AddYears(-1);
                    SonTarih = DateTime.Today.AddYears(1);
                    break;
                case "4": // Ayın başından bu güne
                    IlkTarih = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    SonTarih = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case "5": // Yılın başından bu güne
                    IlkTarih = new DateTime(DateTime.Now.Year, 1, 1);
                    SonTarih = DateTime.Now;
                    break;
            }
            List<Sozlesme> sozlesme = new List<Sozlesme>();
            if (SubeId == 0)
                sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && (x.TeklifBit == true || x.OpsiyonBit == true) && ((x.SozlesmeTarihi >= IlkTarih) && (x.SozlesmeTarihi <= SonTarih))).OrderBy(x => x.SozlesmeNo).ToList();
            else
                sozlesme = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Aktif == true && x.Sil == false && (x.TeklifBit == true || x.OpsiyonBit == true) && ((x.SozlesmeTarihi >= IlkTarih) && (x.SozlesmeTarihi <= SonTarih))).OrderBy(x => x.SozlesmeNo).ToList();

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
                BaslangicSaat = m.BaslangicSaat.ToString(),
                BitisSaat = m.BitisSaat.ToString(),
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
                OpsiyonBit = m.OpsiyonBit,
                Paketler = m.Paketler,
                EkHizmetler = m.EkHizmetler,
                PeketlerFiyat = m.PaketlerFiyat,
                EkHizmetlerFiyat = m.EkHizmetlerFiyat,
                SozlesmeTutar = m.ToplamFiyat,
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
                Aciklama = m.Notlar,
                Durum = m.Durum,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil,
                FotografSecimDurum = m.FotografSecimDurum,
                FotografSecimDurumTarihi = m.FotografSecimDurumTarihi
            });
            return Json(new { data = sz }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TeklifIptalEt(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (sz == null)
                return Json(new { Sonuc = false, Bilgi = "İptal edilmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            sz.Iptal = true;
            sz.DegistirenKullaniciId = KullaniciId;
            sz.DegistirmeTarih = DateTime.Now;

            //Sözleşme iptal edildikten sonra sözleşmeye ait randevu kayıtları silinecek
            List<Randevu> rnd = dbContext.Randevus.Where(x => x.SozlesmeId == id && x.Sil == false && x.Aktif == true).ToList();
            if (rnd.Count > 0)
            {
                foreach (var item in rnd)
                {
                    item.Iptal = true;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            //Sözleşme iptal edildikten sonra sözleşmeye ait Ödeme Kayıtları silinecek
            List<Odemeler> alinanodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "AlinanOdeme" && x.Sil == false && x.Aktif == true).ToList();
            List<Odemeler> gelecekodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "GelecekOdeme" && x.Sil == false && x.Aktif == true).ToList();

            if (alinanodemeler.Count > 0)
            {
                foreach (var item in alinanodemeler)
                {
                    item.Iptal = true;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            if (gelecekodemeler.Count > 0)
            {
                foreach (var item in gelecekodemeler)
                {
                    item.Sil = true;
                    item.Aktif = false;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            List<GelirGider> gg = dbContext.GelirGiders.Where(x => x.SozlesmeId == id && x.Tip == "Gelir" && x.Sil == false && x.Aktif == true).ToList();
            GelirGider gdr = new GelirGider();
            if (gg.Count > 0)
            {
                foreach (var item in gg)
                {
                    gdr.SubeId = SubeId;
                    gdr.FirmaId = FirmaId;
                    gdr.Tarih = DateTime.Now;
                    gdr.SozlesmeId = sz.Id;
                    gdr.GisId = 1;
                    gdr.CariHareketId = 1;
                    gdr.PersonelOdemeId = 1;
                    gdr.Tip = "Gider";
                    gdr.GelirGiderTurId = item.GelirGiderTurId;
                    gdr.Tutar = item.Tutar;
                    gdr.Notlar = sz.SozlesmeNo + " Numaralı sözleşmenin İptaliden oluşan geri ödeme";
                    gdr.Aktif = true;
                    gdr.Sil = false;
                    gdr.OlusturanKullaniciId = KullaniciId;
                    gdr.OlusturmaTarih = DateTime.Now;
                    gdr.DegistirenKullaniciId = KullaniciId;
                    gdr.DegistirmeTarih = DateTime.Now;
                    dbContext.GelirGiders.Add(gdr);
                }
            }

            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Teklif İptal Edildi", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Rezervasyon Teklifleri, Teklif İptal Et";
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
        public ActionResult TeklifSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (sz == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            sz.Sil = true;
            sz.Aktif = false;
            sz.DegistirenKullaniciId = KullaniciId;
            sz.DegistirmeTarih = DateTime.Now;

            // Sözleşme silindikten sonra Alinan Odemeler, Gelecek Ödemeler, Gelirler, Randevular da silinecek
            List<Randevu> rnd = dbContext.Randevus.Where(x => x.SozlesmeId == id).ToList();
            if (rnd.Count > 0)
            {
                foreach (var item in rnd)
                {
                    item.Sil = true;
                    item.Aktif = false;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            List<Odemeler> alinanodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "AlinanOdeme" && x.Sil == false && x.Aktif == true).ToList();
            List<Odemeler> gelecekodemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == id && x.OdemeTuru == "GelecekOdeme" && x.Sil == false && x.Aktif == true).ToList();

            if (alinanodemeler.Count > 0)
            {
                foreach (var item in alinanodemeler)
                {
                    item.Sil = true;
                    item.Aktif = false;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            if (gelecekodemeler.Count > 0)
            {
                foreach (var item in gelecekodemeler)
                {
                    item.Sil = true;
                    item.Aktif = false;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }
            List<GelirGider> gg = dbContext.GelirGiders.Where(x => x.SozlesmeId == id).ToList();
            if (gg.Count > 0)
            {
                foreach (var item in gg)
                {
                    item.Sil = true;
                    item.Aktif = false;
                    item.DegistirenKullaniciId = KullaniciId;
                    item.DegistirmeTarih = DateTime.Now;
                }
            }

            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Rezervasyon Teklifleri, Teklif Sil";
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
        public ActionResult TeklifDonustur(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (sz == null)
                return Json(new { Sonuc = false, Bilgi = "Değiştirilmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            sz.KesinRezervasyonBit = true;
            sz.TeklifBit = false;
            sz.OpsiyonBit = false;
            sz.Durum = "Kesin Rezervasyon";
            sz.DegistirenKullaniciId = KullaniciId;
            sz.DegistirmeTarih = DateTime.Now;

            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Teklif Kesin Rezervasyona Dönüştürüldü", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Rezervasyon Teklifleri, Teklifi Sözleşmeye Dönüştür";
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

        #region Randevular Sayfası
        public ActionResult Randevular()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Rezervasyon İşlemleri";
            ViewBag.AltMenu = "Randevular";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 23 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            //List<VW_Kullanici_Gorevler_Resim> kullanicilar = dbContext.VW_Kullanici_Gorevler_Resim.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            //ViewBag.kullanicilar = kullanicilar;
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).Randevular;
            ViewBag.FiltreAyar = filtreayar;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 23 && x.Aktif == true && x.Sil == false);
            ViewBag.Personeller = dbContext.Personels.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Musteriler = dbContext.Musteris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.SubeKullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            //ViewBag.SubePersonel = dbContext.SubeToPersonels.Where(x => x ).ToList();
            return View();
        }

        public ActionResult RandevularFiltre(string IlkTarih, string SonTarih, int Sube, int Goruntule, int Personel)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<Randevu> randevular;
            List<RandevuToPersonel> randevupersonel = new List<RandevuToPersonel>();

            string PersId = Personel.ToString();

            randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();

            if (Sube == 0 && Goruntule == 0 && Personel == 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && Goruntule == 0 && Personel != 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.GorevliPersonellerId.Contains(PersId) && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && Goruntule == 1 && Personel == 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.Iptal == false && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && Goruntule == 2 && Personel == 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.Iptal == true && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && Goruntule == 1 && Personel != 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.Iptal == false && x.GorevliPersonellerId.Contains(PersId) && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && Goruntule == 2 && Personel != 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.Iptal == true && x.GorevliPersonellerId.Contains(PersId) && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && Goruntule == 0 && Personel == 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && Goruntule == 0 && Personel != 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.GorevliPersonellerId.Contains(PersId) && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && Goruntule == 1 && Personel == 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.Iptal == false && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && Goruntule == 2 && Personel == 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.Iptal == true && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && Goruntule == 1 && Personel != 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.Iptal == false && x.GorevliPersonellerId.Contains(PersId) && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && Goruntule == 2 && Personel != 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.Iptal == true && x.GorevliPersonellerId.Contains(PersId) && ((x.Baslangic >= ilktarih) && (x.Baslangic <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            foreach (var item in randevular)
            {
                List<RandevuToPersonel> pers = dbContext.RandevuToPersonels.Where(x => x.RandevuId == item.Id).ToList();

                foreach (var randevu in pers)
                {
                    randevupersonel.Add(randevu);
                }
            }
            var rndv = randevular.Select(m => new
            {
                Id = m.Id,
                SubeId = m.SubeId,
                Baslik = m.Baslik, // Rezervasyon türü
                Aciklama = m.Aciklama, // Çekim yapılacak kişiler + Çekim Yeri
                Cekim = m.CekimRandevu, // Çekim randevusu
                //Personel = m.Personel.AdiSoyadi, // Görevli Personel
                //PersonelId = m.PersonelId,
                Musteri = m.Sozlesme.Musteri.AdiSoyadi,
                SozlesmeNo = m.Sozlesme.SozlesmeNo,
                BaslangicTarih = m.Baslangic.ToString("dd/MM/yyyy"),
                BaslangicSaat = m.Baslangic.ToShortTimeString(),
                BitisTarih = m.Bitis.ToString("dd/MM/yyyy"),
                BitisSaat = m.Bitis.ToShortTimeString(),
                Iptal = m.Iptal,
                Sinif = m.RandevuGorunum.Gorunum,
                Personeller = randevupersonel.Where(x => x.RandevuId == m.Id).Select(p => new
                {
                    RandevuId = p.RandevuId,
                    PersonelId = p.PersonelId,
                    PersonelAdSoyad = p.Personel.AdiSoyadi

                })
                //Personeller =  randevupersonel.Select(p => new
                //{
                //    RandevuId = p.RandevuId,
                //    PersonelId = p.PersonelId,
                //    PersonelAdSoyad = p.Personel.AdiSoyadi

                //})
            });
            var randpers = randevupersonel.Select(p => new
            {
                RandevuId = p.RandevuId,
                PersonelId = p.PersonelId,
                PersonelAdSoyad = p.Personel.AdiSoyadi

            }).ToList();

            return Json(new { data = rndv, PersonelList = randpers }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RandevularListe()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            //int RolId = Convert.ToInt32(Session["RolId"]);
            List<Randevu> randevular;
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).Randevular;
            DateTime IlkTarih = DateTime.Today;
            DateTime SonTarih = DateTime.Today;
            switch (filtreayar)
            {
                case "0": // 1 Gün
                    IlkTarih = DateTime.Today;
                    SonTarih = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case "1": // 1 Hafta Öncesi - 1 Hafta Sonrası
                    IlkTarih = DateTime.Today.AddDays(-7);
                    SonTarih = DateTime.Today.AddDays(7);
                    break;
                case "2": // 1 Ay Öncesi - 1 Ay Sonrası
                    IlkTarih = DateTime.Today.AddMonths(-1);
                    SonTarih = DateTime.Today.AddMonths(1);
                    break;
                case "3": // 1 Yıl Öncesi - 1 Yıl Sonrası
                    IlkTarih = DateTime.Today.AddYears(-1);
                    SonTarih = DateTime.Today.AddYears(1);
                    break;
                case "4": // Ayın başından bu güne
                    IlkTarih = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    SonTarih = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case "5": // Yılın başından bu güne
                    IlkTarih = new DateTime(DateTime.Now.Year, 1, 1);
                    SonTarih = DateTime.Now;
                    break;
            }
            List<RandevuToPersonel> randevupersonel = new List<RandevuToPersonel>();
            //randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && ((x.Baslangic >= IlkTarih) && (x.Baslangic <= SonTarih)) && x.Aktif == true && x.Sil == false).ToList();

            if (AktifSubeId == 0)
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && ((x.Baslangic >= IlkTarih) && (x.Baslangic <= SonTarih)) && x.Aktif == true && x.Sil == false).ToList();
            else
                randevular = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == AktifSubeId && ((x.Baslangic >= IlkTarih) && (x.Baslangic <= SonTarih)) && x.Aktif == true && x.Sil == false).ToList();

            foreach (var item in randevular)
            {
                List<RandevuToPersonel> pers = dbContext.RandevuToPersonels.Where(x => x.RandevuId == item.Id).ToList();

                foreach (var randevu in pers)
                {
                    randevupersonel.Add(randevu);
                }
            }
            var rand = randevular.Select(m => new
            {
                Id = m.Id,
                SubeId = m.SubeId,
                Baslik = m.Baslik, // Rezervasyon türü
                Aciklama = m.Aciklama, // Çekim yapılacak kişiler + Çekim Yeri
                Cekim = m.CekimRandevu, // Çekim randevusu
                //Personel = m.Personel.AdiSoyadi, // Görevli Personel
                //PersonelId = m.PersonelId,
                Musteri = m.Sozlesme.Musteri.AdiSoyadi,
                SozlesmeNo = m.Sozlesme.SozlesmeNo,
                BaslangicTarih = m.Baslangic.ToString("dd/MM/yyyy"),
                BaslangicSaat = m.Baslangic.ToShortTimeString(),
                BitisTarih = m.Bitis.ToString("dd/MM/yyyy"),
                BitisSaat = m.Bitis.ToShortTimeString(),
                Iptal = m.Iptal,
                Sinif = m.RandevuGorunum.Gorunum,
                Personeller = randevupersonel.Where(x => x.RandevuId == m.Id).Select(p => new
                {
                    RandevuId = p.RandevuId,
                    PersonelId = p.PersonelId,
                    PersonelAdSoyad = p.Personel.AdiSoyadi

                })
                //Personeller = randevupersonel.Select(p => new
                //{
                //    RandevuId = p.RandevuId,
                //    PersonelId = p.PersonelId,
                //    PersonelAdSoyad = p.Personel.AdiSoyadi

                //})
            });
            var randpers = randevupersonel.Select(p => new
            {
                RandevuId = p.RandevuId,
                PersonelId = p.PersonelId,
                PersonelAdSoyad = p.Personel.AdiSoyadi

            }).ToList();

            return Json(new { data = rand, PersonelList = randpers }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RandevuGuncelle(long? id)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            //long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            string PersonelIdler = Request["PersonelIdArray"];
            string[] PersonelIdArray = Request["PersonelIdArray"].Split(',');
            DateTime RandevuTarihi = Convert.ToDateTime(Request["RandevuTarihi"]);
            string baslangicZamani = Request["BaslangicSaat"];
            string bitisZamani = Request["BitisSaat"];
            TimeSpan baslangic = TimeSpan.Parse(baslangicZamani);
            TimeSpan bitis = TimeSpan.Parse(bitisZamani);
            string Aciklama = Request["Aciklama"];
            Aciklama = Aciklama.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            Aciklama = Aciklama.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            Randevu randevu = dbContext.Randevus.FirstOrDefault(x => x.Id == id && x.Sil == false && x.Iptal == false && x.Aktif == true);
            randevu.Aciklama = Aciklama;
            randevu.GorevliPersonellerId = PersonelIdler;
            randevu.Baslangic = RandevuTarihi.Add(baslangic);
            randevu.Bitis = RandevuTarihi.Add(bitis);
            randevu.DegistirenKullaniciId = KullaniciId;
            randevu.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();

                // Önce Eski personel ilişkiler siliniyor. sonra yeni seçilen personeller ekleniyor.
                List<RandevuToPersonel> rntopers = dbContext.RandevuToPersonels.Where(x => x.RandevuId == id).ToList();
                foreach (var item in rntopers)
                {
                    dbContext.RandevuToPersonels.Remove(item);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Randevu Güncelle, Eski Personelleri Sil, Randevu Id: " + id;
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
                RandevuToPersonel rtop = new RandevuToPersonel();
                foreach (var pId in PersonelIdArray)
                {
                    rtop.PersonelId = Convert.ToInt64(pId);
                    rtop.RandevuId = randevu.Id;
                    rtop.Aktif = true;
                    rtop.Sil = false;
                    rtop.Iptal = false;
                    rtop.OlusturanKullaniciId = KullaniciId;
                    rtop.OlusturmaTarih = DateTime.Now;
                    rtop.DegistirenKullaniciId = KullaniciId;
                    rtop.DegistirmeTarih = DateTime.Now;
                    dbContext.RandevuToPersonels.Add(rtop);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Randevu Güncelle, Personel Ekle PersonelId:" + pId + ", Randevu Id: " + id;
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
                return Json(new { Sonuc = true, Mesaj = "Güncelleme Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Randevular, Randevu Güncelle, Randevu Id: " + id;
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
        public ActionResult RandevuEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SozlesmeNo = Convert.ToInt64(Request["SozlesmeNo"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            //long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            string PersonelIdler = Request["PersonelIdArray"];
            string[] PersonelIdArray = Request["PersonelIdArray"].Split(',');
            DateTime CekimTarihi = Convert.ToDateTime(Request["RandevuTarihi"]);
            string baslangicZamani = Request["BaslangicZaman"];
            string bitisZamani = Request["BitisZaman"];
            TimeSpan baslangic = TimeSpan.Parse(baslangicZamani);
            TimeSpan bitis = TimeSpan.Parse(bitisZamani);
            string Aciklama = Request["Aciklama"];
            string RandevuBaslik = Request["RandevuBaslik"];
            string CekimRandevu = Request["Aciklama"];
            string Clas = Request["Clas"];
            Aciklama = Aciklama.Replace("\r\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.
            Aciklama = Aciklama.Replace("\n", "<br />"); // Textarea dan gelen string veri satırlar halinde olacak şekilde veri tabanına kaydediliyor.

            long rezid = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.SozlesmeNo == SozlesmeNo && x.FirmaId == FirmaId).RezervasyonTurId;
            long randevugorunumId = Convert.ToInt64(dbContext.RezervasyonTurleris.FirstOrDefault(x => x.Id == rezid && x.FirmaId == FirmaId).RandevuGorunumId);
            RandevuToPersonel rtop = new RandevuToPersonel();
            Randevu randevu = new Randevu();
            randevu.FirmaId = FirmaId;
            randevu.SubeId = SubeId;
            randevu.SozlesmeId = SozlesmeId;
            randevu.GorevliPersonellerId = PersonelIdler;
            //randevu.PersonelId = null;
            if (CekimRandevu == "Aktif")
                randevu.CekimRandevu = true;
            else
                randevu.CekimRandevu = false;

            randevu.Baslik = RandevuBaslik;
            randevu.Aciklama = Aciklama;
            randevu.Baslangic = CekimTarihi.Add(baslangic);
            randevu.Bitis = CekimTarihi.Add(bitis);
            randevu.RandevuGorunumId = randevugorunumId;
            randevu.Iptal = false;
            randevu.OlusturanKullaniciId = KullaniciId;
            randevu.OlusturmaTarih = DateTime.Now;
            randevu.DegistirenKullaniciId = KullaniciId;
            randevu.DegistirmeTarih = DateTime.Now;
            randevu.Aktif = true;
            randevu.Sil = false;
            dbContext.Randevus.Add(randevu);
            // Randevu kaydedilmeden önce randevu tarih ve saatinin boş olup olmadığı kontrol edilecek.
            try
            {
                dbContext.SaveChanges();
                // RandevutoPersonel tablosunaseçilen personel eklenecek
                foreach (var pId in PersonelIdArray)
                {
                    rtop.PersonelId = Convert.ToInt64(pId);
                    rtop.RandevuId = randevu.Id;
                    rtop.Aktif = true;
                    rtop.Sil = false;
                    rtop.Iptal = false;
                    rtop.OlusturanKullaniciId = KullaniciId;
                    rtop.OlusturmaTarih = DateTime.Now;
                    rtop.DegistirenKullaniciId = KullaniciId;
                    rtop.DegistirmeTarih = DateTime.Now;
                    dbContext.RandevuToPersonels.Add(rtop);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Yeni Rezervasyon Ekle, Personel Ekle";
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
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Yeni Rezervasyon Ekle, Çekim Randevusu, Sözleşme Id: " + SozlesmeId;
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
        public JsonResult SozlesmeGetir(long id)
        {
            List<Sozlesme> sozlesmeler = dbContext.Sozlesmes.Where(x => x.MusteriId == id && x.Aktif == true && x.Sil == false).ToList();
            var sozlesme = sozlesmeler.Select(m => new {
                Id = m.Id,
                SozlesmeNo = m.SozlesmeNo,
                SozlesmeTarihi = m.SozlesmeTarihi,
                RezervasyonTuru = m.RezervasyonTurleri.RezervasyonTuru,
                GelinDamat = (string.IsNullOrEmpty(m.GelinAd) || string.IsNullOrEmpty(m.DamatAd)) ? "": m.GelinAd + "&" + m.DamatAd,
                Yetkili= (string.IsNullOrEmpty(m.YetkiliAdSoyad))?"":m.YetkiliAdSoyad,
                Urunler= (string.IsNullOrEmpty(m.Urunler)) ? "" : m.Urunler,
                Modeller= (string.IsNullOrEmpty(m.Modeller)) ? "" : m.Modeller,
                Cocuk= (string.IsNullOrEmpty(m.CocukAdSoyad)) ? "" : m.CocukAdSoyad,
                CekimYeri= (string.IsNullOrEmpty(m.OrganizasyonYeri)) ? "" : m.OrganizasyonYeri,
            }).OrderByDescending(x => x.SozlesmeNo); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(sozlesme, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}