using FotografciTakipWeb.App_Settings;
using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class MusterilerController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        ResimIsim resimisimlendir = new ResimIsim();
        // GET: Otomasyon/Musteriler
        [HttpPost]
        public string MusteriKoduGetir()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
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
            return mkod;
        }
        public ActionResult MusteriListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Müşteriler";
            ViewBag.AltMenu = "Müşteri Listesi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 34 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> subelistesi = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.SubeListesi = subelistesi;
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            ViewBag.SubeKullanici = subekullanici;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 34 && x.Aktif == true && x.Sil == false);
            ViewBag.FotografYukleSayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 36 && x.Aktif == true && x.Sil == false).SayfaYetki;
            ViewBag.Ayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).MusteriListesiPasifGizle;

            return View();
        }
        public ActionResult MusterilerListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            int RolId = Convert.ToInt32(Session["RolId"]);
            List<Models.Musteri> musteriler;
            if (SubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                musteriler = dbContext.Musteris.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList(); // aktif şubeye ait müşteri listesi
            else
                musteriler = dbContext.Musteris.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Sil == false).ToList(); // aktif şubeye ait müşteri listesi

            var musterilist = musteriler.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                SubeAdi = m.Sube.SubeAdi,
                MusteriKodu = m.MusteriKodu,
                AdiSoyadi = m.AdiSoyadi,
                TCKimlikNo = m.TCKimlikNo,
                SabitTel = m.SabitTel,
                CepTel = m.CepTel,
                Email = m.Email,
                Adres = m.Adres,
                PanelGiris = m.MusteriPanelGirisYetki,
                SmsKabul = m.SMSKabul,
                EmailKabul = m.EmailKabul,
                Notlar = m.Notlar,
                FotografSecimDurum = m.FotografSecimDurum,
                KilitBit = m.KilitBit,
                Aktif=m.Aktif
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = musterilist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult MusteriEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long MusteriKodu = Convert.ToInt64(Request["MusteriKodu"]);
            string AdSoyad = Request["AdSoyad"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string cep = Request["CepTel"];
            string sabit = Request["SabitTel"];
            string PanelGiris = Request["PanelGiris"];
            string Email = Request["Email"];
            if (!string.IsNullOrEmpty(Email))
            {
                Email = Email.Trim().Replace(" ", string.Empty);
                Email = Email.ToLower();
            }
            string Adres = Request["Adres"];
            Adres = Adres.Replace("\r\n", "<br />");
            Adres = Adres.Replace("\n", "<br />");
            string Notlar = Request["Notlar"];
            Notlar = Notlar.Replace("\r\n", "<br />");
            Notlar = Notlar.Replace("\n", "<br />");
            TelefonDüzelt td = new TelefonDüzelt();
            if (!string.IsNullOrEmpty(cep)) { cep = td.düzelt(cep); }
            if (!string.IsNullOrEmpty(sabit)) { sabit = td.düzelt(sabit); }

            // Gelen bilgiler ile ilgili silinmiş bir kayıt varsa aktif yapılıyor.
            Models.Musteri kayitvarmi = dbContext.Musteris.FirstOrDefault(x => x.SubeId == SubeId && x.AdiSoyadi.ToLower() == AdSoyad.ToLower() && x.CepTel == cep && x.Aktif == true && x.Sil == false); // eklenmek istenilen kayıt daha önceden silinmişse aktif yapılıyor.
            if (kayitvarmi != null)
            {
                kayitvarmi.Sil = false;
                kayitvarmi.Aktif = true;
                kayitvarmi.DegistirenKullaniciId = KullaniciId;
                kayitvarmi.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Müşteri Güncelleme Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Müşteri Ekle, Varolan silinmiş kaydın güncellenmesi";
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
                SifreOlustur sf = new SifreOlustur();
                Models.Musteri musteri = new Models.Musteri();
                musteri.FirmaId = FirmaId;
                musteri.SubeId = SubeId;
                musteri.MusteriKodu = MusteriKodu;
                musteri.Sifre = sf.sifreolustur(8);
                musteri.AdiSoyadi = AdSoyad;
                musteri.TCKimlikNo = TCKimlikNo;
                musteri.CepTel = cep;
                musteri.SabitTel = sabit;
                musteri.Email = Email;
                if (PanelGiris == "Var")
                    musteri.MusteriPanelGirisYetki = true;
                else
                    musteri.MusteriPanelGirisYetki = false;
                musteri.Adres = Adres;
                musteri.Notlar = Notlar;
                musteri.SMSKabul = true;
                if (string.IsNullOrEmpty(Email))
                    musteri.EmailKabul = false;
                else
                    musteri.EmailKabul = true;
                musteri.FotografSecimDurum = "0";
                musteri.OlusturanKullaniciId = KullaniciId;
                musteri.OlusturmaTarih = DateTime.Now;
                musteri.DegistirenKullaniciId = KullaniciId;
                musteri.DegistirmeTarih = DateTime.Now;
                musteri.Aktif = true;
                musteri.Sil = false;
                try
                {
                    dbContext.Musteris.Add(musteri);
                    dbContext.SaveChanges();
                    AyarlarGenel genelayar = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId);
                    RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Müşteriler");
                    TelefonRehberi rehber = new TelefonRehberi();
                    rehber.FirmaId = FirmaId;
                    rehber.RehberGrupId = rehbergrup.Id;
                    rehber.MusteriId = musteri.Id;
                    rehber.FirmaAdi = "";
                    rehber.AdSoyad = AdSoyad;
                    rehber.SabitTel1 = sabit;
                    rehber.SabitTel2 = "";
                    rehber.CepTel1 = cep;
                    rehber.CepTel2 = "";
                    rehber.Fax = "";
                    rehber.Email = Email;
                    rehber.SmsKabul = true;
                    rehber.EmailKabul = true;
                    rehber.Notlar = "Yeni Müşteri Kaydı";
                    rehber.OlusturanKullaniciId = KullaniciId;
                    rehber.OlusturmaTarih = DateTime.Now;
                    rehber.DegistirenKullaniciId = KullaniciId;
                    rehber.DegistirmeTarih = DateTime.Now;

                    if (genelayar.PersonelRehberKayit == true)
                        rehber.Aktif = true;
                    else
                        rehber.Aktif = false;
                    rehber.Sil = false;
                    dbContext.TelefonRehberis.Add(rehber);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Müşteri İletişim Bilgilerini Rehbere Ekle";
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
                    return Json(new { Sonuc = true, MusteriId = musteri.Id, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Müşteri Ekle";
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
        public string MusteriVarMi()
        {
            string AdSoyad = Request["AdSoyad"].ToLower();
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            string cep = Request["CepTel"];
            TelefonDüzelt td = new TelefonDüzelt();
            if (!string.IsNullOrEmpty(cep)) { cep = td.düzelt(cep); }
            Models.Musteri cr = dbContext.Musteris.FirstOrDefault(x => x.AdiSoyadi.ToLower() == AdSoyad.ToLower() && x.CepTel == cep && x.Aktif == true && x.Sil == false);
            if (cr != null)
                return cr.Id.ToString();
            else
                return "Yok";
        }
        [HttpPost]
        public ActionResult MusteriGuncelle(long? id)
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
            string AdSoyad = Request["AdSoyad"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string cep = Request["CepTel"];
            string sabit = Request["SabitTel"];
            string PanelGiris = Request["PanelGiris"];
            string Email = Request["Email"];
            string Adres = Request["Adres"];
            string FotografSecimDurum = Request["FotografSecimDurum"];
            Adres = Adres.Replace("\r\n", "<br />");
            Adres = Adres.Replace("\n", "<br />");
            string Notlar = Request["Notlar"];
            Notlar = Notlar.Replace("\r\n", "<br />");
            Notlar = Notlar.Replace("\n", "<br />");
            TelefonDüzelt td = new TelefonDüzelt();
            if (!string.IsNullOrEmpty(cep)) { cep = td.düzelt(cep); }
            if (!string.IsNullOrEmpty(sabit)) { sabit = td.düzelt(sabit); }

            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (musteri == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            musteri.SubeId = SubeId;
            musteri.TCKimlikNo = TCKimlikNo;
            musteri.AdiSoyadi = AdSoyad;
            musteri.SabitTel = sabit;
            musteri.CepTel = cep;
            musteri.Email = Email;
            if (PanelGiris == "Var")
                musteri.MusteriPanelGirisYetki = true;
            else
                musteri.MusteriPanelGirisYetki = false;
            musteri.Adres = Adres;
            musteri.Notlar = Notlar;
            musteri.FotografSecimDurum = FotografSecimDurum;
            musteri.DegistirenKullaniciId = KullaniciId;
            musteri.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();

                TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriId == id);
                if (rehber != null)
                {
                    rehber.SabitTel1 = sabit;
                    rehber.AdSoyad = AdSoyad;
                    rehber.CepTel1 = cep;
                    rehber.Email = Email;
                    rehber.DegistirenKullaniciId = KullaniciId;
                    rehber.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                }
                else
                {
                    // Cep telefonu boşsa veya cep telefonu dolu mail adresi boşsa rehbere kaydetme
                    if (!string.IsNullOrEmpty(cep) || (!string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(cep)))
                    {
                        AyarlarGenel genelayar = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId);
                        RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Müşteriler");
                        TelefonRehberi r = new TelefonRehberi();
                        r.FirmaId = FirmaId;
                        r.RehberGrupId = rehbergrup.Id;
                        r.PersonelId = id;
                        r.FirmaAdi = "";
                        r.AdSoyad = AdSoyad;
                        r.SabitTel1 = sabit;
                        r.SabitTel2 = "";
                        r.CepTel1 = cep;
                        r.CepTel2 = "";
                        r.Fax = "";
                        r.Email = Email;
                        r.SmsKabul = true;
                        r.EmailKabul = true;
                        r.Notlar = "Müşteri Güncelle - Müşteri İletişim Bilgilerini Rehbere Ekle";
                        r.OlusturanKullaniciId = KullaniciId;
                        r.OlusturmaTarih = DateTime.Now;
                        r.DegistirenKullaniciId = KullaniciId;
                        r.DegistirmeTarih = DateTime.Now;
                        if (genelayar.PersonelRehberKayit == true)
                            r.Aktif = true;
                        else
                            r.Aktif = false;
                        r.Sil = false;
                        dbContext.TelefonRehberis.Add(r);
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Personel İletişim Bilgilerini Rehbere Ekle";
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
                return Json(new { Sonuc = true, Mesaj = "Güncelleme Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Müşteri Güncelle, Kayıt Id: " + id;
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
        public ActionResult MusteriSil(long? id)
        {
            // Personelin kullanıldığı tablolarda kayıtlar var ise personel silinemeyecek
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            bool isSavedSuccessfully = true;

            List<Sozlesme> sz = dbContext.Sozlesmes.Where(x => x.MusteriId == id && x.Sil == false).ToList();
            if (sz.Count > 0)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Müşteri Sil, Müşteri Detay Kayıtları Mevcut, Silinemez! Kayıt Id: " + id.ToString();
                hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Müşteriye ait işlem detayları var, Silinemez!", JsonRequestBehavior.AllowGet });
            }
            else
            {
                #region Müşteriye ait fotoğraflar silincek
                List<MusteriFotograf> mf = dbContext.MusteriFotografs.Where(x => x.FirmaId == FirmaId && x.MusteriId == id).ToList();
                if (mf.Count>0)
                {
                    foreach (var item in mf)
                    {
                        string TamYol = Server.MapPath(item.FotografYol);

                        if (System.IO.File.Exists(TamYol)) // bu resimden var mı?
                        {
                            //MusteriFotograf mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.MusteriId == MusteriId && x.Id == FotografId);
                            if (mf != null)
                            {
                                try
                                {
                                    dbContext.MusteriFotografs.Remove(item);
                                    dbContext.SaveChanges();
                                    System.IO.File.Delete(TamYol);  // resim var ise Sil
                                }
                                catch (Exception e)
                                {
                                    HataLoglari hata = new HataLoglari();
                                    hata.FirmaId = FirmaId;
                                    hata.SubeId = SubeId;
                                    hata.Islem = "Müşteri Fotoğraf Sil; Silinen Kayıt ID: " + item.Id;
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
                                isSavedSuccessfully = false;
                        }
                        if (!isSavedSuccessfully)
                        {
                            return Json(new { Sonuc = false, Mesaj = "Müşteriye ait fotoğrağraflar silinemedi. <br />Müşteri Silinemedi" });
                        }
                    }
                    
                }
                
                #endregion

                Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                if (musteri == null)
                    return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
                musteri.Aktif = false;
                musteri.Sil = true;
                musteri.DegistirenKullaniciId = KullaniciId;
                musteri.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                    TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriId == id);
                    if (rehber != null)
                    {
                        rehber.Aktif = false;
                        rehber.Sil = true;
                        rehber.DegistirenKullaniciId = KullaniciId;
                        rehber.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Müşteri Sil, Kayıt Id: " + id + ", Telefon rehberinden müşteri iletişim bilgilerini sil";
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
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Müşteri Sil, Kayıt Id:" + id;
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
        //[HttpPost]
        //public ActionResult MusteriyiSil(long? id)
        //{
        //    // Personelin kullanıldığı tablolarda kayıtlar var ise personel silinemeyecek
        //    if (Session.Count == 0)
        //        return RedirectToAction("GirisYap", "Giris");
        //    if (id == null)
        //        return RedirectToAction("SayfaBulunamadi", "Hata");
        //    long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
        //    long KullaniciId = Convert.ToInt64(Session["Id"]);
        //    long FirmaId = Convert.ToInt64(Session["FirmaId"]);
        //    bool isSavedSuccessfully = true;

        //    List<Sozlesme> sz = dbContext.Sozlesmes.Where(x => x.MusteriId == id && x.Sil == false).ToList();
        //    if (sz.Count > 0)
        //    {
        //        HataLoglari hata = new HataLoglari();
        //        hata.FirmaId = FirmaId;
        //        hata.SubeId = SubeId;
        //        hata.Islem = "Müşteri Sil, Müşteri Detay Kayıtları Mevcut, Silinemez! Kayıt Id: " + id.ToString();
        //        hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
        //        hata.OlusturanKullaniciId = 1;
        //        hata.OlusturmaTarih = DateTime.Now;
        //        hata.DegistirenKullaniciId = 1;
        //        hata.DegistirmeTarih = DateTime.Now;
        //        hata.Aktif = true;
        //        hata.Sil = false;
        //        dbContext.HataLoglaris.Add(hata);
        //        dbContext.SaveChanges();
        //        return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Müşteriye ait işlem detayları var, Silinemez!", JsonRequestBehavior.AllowGet });
        //    }
        //    else
        //    {
        //        #region Müşteriye ait fotoğraflar silincek
        //        MusteriFotograf mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id);

        //        string TamYol = Server.MapPath(mf.FotografYol);

        //        if (System.IO.File.Exists(TamYol)) // bu resimden var mı?
        //        {
        //            //MusteriFotograf mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.MusteriId == MusteriId && x.Id == FotografId);
        //            if (mf != null)
        //            {
        //                try
        //                {
        //                    dbContext.MusteriFotografs.Remove(mf);
        //                    dbContext.SaveChanges();
        //                    System.IO.File.Delete(TamYol);  // resim var ise Sil
        //                }
        //                catch (Exception e)
        //                {
        //                    HataLoglari hata = new HataLoglari();
        //                    hata.FirmaId = FirmaId;
        //                    hata.SubeId = SubeId;
        //                    hata.Islem = "Müşteri Fotoğraf Sil; Silinen Kayıt ID: " + mf.Id;
        //                    hata.HataMesajı = e.Message;
        //                    hata.OlusturanKullaniciId = 1;
        //                    hata.OlusturmaTarih = DateTime.Now;
        //                    hata.DegistirenKullaniciId = 1;
        //                    hata.DegistirmeTarih = DateTime.Now;
        //                    hata.Aktif = true;
        //                    hata.Sil = false;
        //                    dbContext.HataLoglaris.Add(hata);
        //                    dbContext.SaveChanges();
        //                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
        //                }
        //            }
        //            else
        //                isSavedSuccessfully = false;
        //        }

        //        if (!isSavedSuccessfully)
        //        {
        //            return Json(new { Sonuc = false, Mesaj = "Müşteriye ait fotoğrağraflar silinemedi. <br />Müşteri Silinemedi" });
        //        }
        //        #endregion

        //        Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
        //        if (musteri == null)
        //            return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
        //        musteri.Aktif = false;
        //        musteri.Sil = true;
        //        musteri.DegistirenKullaniciId = KullaniciId;
        //        musteri.DegistirmeTarih = DateTime.Now;
        //        try
        //        {
        //            dbContext.SaveChanges();
        //            TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.MusteriId == id);
        //            if (rehber != null)
        //            {
        //                rehber.Aktif = false;
        //                rehber.Sil = true;
        //                rehber.DegistirenKullaniciId = KullaniciId;
        //                rehber.DegistirmeTarih = DateTime.Now;
        //                try
        //                {
        //                    dbContext.SaveChanges();
        //                }
        //                catch (Exception e)
        //                {
        //                    HataLoglari hata = new HataLoglari();
        //                    hata.FirmaId = FirmaId;
        //                    hata.SubeId = SubeId;
        //                    hata.Islem = "Müşteri Sil, Kayıt Id: " + id + ", Telefon rehberinden müşteri iletişim bilgilerini sil";
        //                    hata.HataMesajı = e.Message;
        //                    hata.OlusturanKullaniciId = 1;
        //                    hata.OlusturmaTarih = DateTime.Now;
        //                    hata.DegistirenKullaniciId = 1;
        //                    hata.DegistirmeTarih = DateTime.Now;
        //                    hata.Aktif = true;
        //                    hata.Sil = false;
        //                    dbContext.HataLoglaris.Add(hata);
        //                    dbContext.SaveChanges();
        //                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
        //                }
        //            }
        //            return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi" }, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception e)
        //        {
        //            HataLoglari hata = new HataLoglari();
        //            hata.FirmaId = FirmaId;
        //            hata.SubeId = SubeId;
        //            hata.Islem = "Müşteri Sil, Kayıt Id:" + id;
        //            hata.HataMesajı = e.Message;
        //            hata.OlusturanKullaniciId = 1;
        //            hata.OlusturmaTarih = DateTime.Now;
        //            hata.DegistirenKullaniciId = 1;
        //            hata.DegistirmeTarih = DateTime.Now;
        //            hata.Aktif = true;
        //            hata.Sil = false;
        //            dbContext.HataLoglaris.Add(hata);
        //            dbContext.SaveChanges();
        //            return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
        //        }
        //    }
        //}
        [HttpPost]
        public string PanelGirisYetkiDegistir(long? id)
        {
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == id && x.Sil == false && x.Aktif == true);

            if (musteri.MusteriPanelGirisYetki == true)
                musteri.MusteriPanelGirisYetki = false;
            else
                musteri.MusteriPanelGirisYetki = true;
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
        public string SmsKabulDegistir(long? id)
        {
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == id && x.Sil == false && x.Aktif == true);

            if (musteri.SMSKabul == true)
                musteri.SMSKabul = false;
            else
                musteri.SMSKabul = true;
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
        public string MailKabulDegistir(long? id)  
        {
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == id && x.Sil == false && x.Aktif == true);

            if (musteri.EmailKabul == true)
                musteri.EmailKabul = false;
            else
                musteri.EmailKabul = true;
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
        public string AktifDurumDegistir(long? id)
        {
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (musteri.Aktif == true)
                musteri.Aktif = false;
            else
                musteri.Aktif = true;
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

        public ActionResult MusteriMesajlari()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Müşteriler";
            ViewBag.AltMenu = "Müşteri Mesajları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 35 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> subelistesi = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.SubeListesi = subelistesi;
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            ViewBag.SubeKullanici = subekullanici;
            List<MusteriMesaj> msj = dbContext.MusteriMesajs.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 35 && x.Aktif == true && x.Sil == false);
            return View(msj);
        }
        [HttpPost]
        public ActionResult MesajListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            List<MusteriMesaj> msj = dbContext.MusteriMesajs.Where(x => x.FirmaId == FirmaId && x.MesajId == null && x.Aktif == true && x.Sil == false).ToList();
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
                FirmaCevapBit = m.FirmaCevapBit,
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
        public ActionResult MesajDetay(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            ViewBag.UstMenu = "Müşteriler";
            ViewBag.AltMenu = "Müşteri Mesajları";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Sube> subelistesi = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.SubeListesi = subelistesi;
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            ViewBag.SubeKullanici = subekullanici;
            MusteriMesaj ilkmesaj = dbContext.MusteriMesajs.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (ilkmesaj == null)
                return Json(new { Sonuc = false, Bilgi = "Görüntülenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 35 && x.Aktif == true && x.Sil == false);

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
        [HttpPost]
        public ActionResult MesajCevapla()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long IlkMesajId = Convert.ToInt64(Request["IlkMesajId"]);
            long? MesajId = Convert.ToInt64(Request["MesajId"]);
            string Konu = Request["Konu"];
            string Durum = Request["Durum"];
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
            mesaj.FirmaCevapBit = true;
            if (Durum == "5" || Durum == "7")
                mesaj.CevaplaBit = false; // konu kapandı yada iptal edildi ise Müşteri tarafında Cevapla butonu görünmeyecek, 
            else
                mesaj.CevaplaBit = true; // Cevap firma tarafından verildi ise müşteri cevapla butonunu görebilir.
            mesaj.OkunduBit = false;
            mesaj.KilitBit = false;
            mesaj.Aktif = true;
            mesaj.Sil = false;
            mesaj.OlusturanKullaniciId = KullaniciId;
            mesaj.OlusturmaTarih = DateTime.Now;
            mesaj.DegistirenKullaniciId = KullaniciId;
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
                hata.SubeId = SubeId;
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
            if (MesajId == 0 || MesajId == null) // 
            {
                ilkmesaj.OkunduBit = true;
                ilkmesaj.CevaplaBit = false;
                ilkmesaj.CevapTarihi = mesaj.CevapTarihi;
                ilkmesaj.Durum = mesaj.Durum;
                ilkmesaj.DegistirenKullaniciId = KullaniciId;
                ilkmesaj.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "ilk Mesaj düzenle, İlkMesajId:" + IlkMesajId;
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
                ilkmesaj.Durum = mesaj.Durum;
                ilkmesaj.DegistirenKullaniciId = KullaniciId;
                ilkmesaj.DegistirmeTarih = DateTime.Now;
                MusteriMesaj cevaplananmesaj = dbContext.MusteriMesajs.FirstOrDefault(x => x.Id == MesajId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                cevaplananmesaj.OkunduBit = true;
                cevaplananmesaj.CevaplaBit = false;
                cevaplananmesaj.CevapTarihi = mesaj.CevapTarihi;
                cevaplananmesaj.DegistirenKullaniciId = KullaniciId;
                cevaplananmesaj.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
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
        public ActionResult MusteriFotoYukle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Müşteriler";
            ViewBag.AltMenu = "Fotoğraf Yükle";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 36 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            ViewBag.MusteriId = id;
            List<Sozlesme> aktifsozlesmeler = new List<Sozlesme>();
            if (id != null)
            {
                aktifsozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.MusteriId == id && x.Bitti == false && x.KesinRezervasyonBit == true && x.Aktif == true && x.Sil == false).ToList();
            }
            List<Models.Musteri> musteriler = dbContext.Musteris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Musteriler = musteriler;
            ViewBag.AktifSozlesmeler = aktifsozlesmeler;
            List<MusteriFotograf> musterifotolari = dbContext.MusteriFotografs.Where(x => x.MusteriId == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
            ViewBag.MusteriFotolari = musterifotolari;
            return View();
        }
        public ActionResult SozlesmeFotoYukle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Müşteriler";
            ViewBag.AltMenu = "Fotoğraf Yükle";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 36 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id && x.Bitti == false && x.KesinRezervasyonBit == true && x.Aktif == true && x.Sil == false);
            ViewBag.SozlesmeId = id;
            ViewBag.MusteriId = sz.MusteriId;
            List<Sozlesme> aktifsozlesmeler = new List<Sozlesme>();
            if (id != null)
            {
                aktifsozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.MusteriId == sz.MusteriId && x.Bitti == false && x.KesinRezervasyonBit == true && x.Aktif == true && x.Sil == false).ToList();
            }
            List<MusteriFotograf> Fotograflar = dbContext.MusteriFotografs.Where(x => x.FirmaId == FirmaId && x.MusteriId == sz.MusteriId && x.SozlesmeId == id && x.Sil == false && x.Aktif == true).ToList();
            ViewBag.Fotograflar = Fotograflar;
            List<Models.Musteri> musteriler = dbContext.Musteris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Musteriler = musteriler;
            ViewBag.AktifSozlesmeler = aktifsozlesmeler;
            List<MusteriFotograf> musterifotolari = dbContext.MusteriFotografs.Where(x => x.MusteriId == sz.MusteriId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
            ViewBag.MusteriFotolari = musterifotolari;
            return View();
        }
        [HttpPost]
        public ActionResult MusteriFotolarYukle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            //double MusteriKodu = dbContext.Musteris.FirstOrDefault(x => x.Id == MusteriId).MusteriKodu;
            bool isSavedSuccessfully = true;
            string ResimAdi = "", ResimYol = "", TamYol = "", ResimKonum_Vt = "";
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == SozlesmeId && x.Sil == false && x.Aktif == true && x.Iptal == false);
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                System.IO.FileInfo ff = new System.IO.FileInfo(file.FileName);
                string uzanti = ff.Extension;

                ResimAdi = resimisimlendir.resimisimlendir2(file, file.FileName.Replace(uzanti, ""));

                if (file != null && file.ContentLength > 0)
                {
                    //veri tabanında saklanacak resim yolu
                    ResimKonum_Vt = "/Areas/Otomasyon/Dosyalar/MusteriFoto/" + MusteriId + "/";
                    // Resmin kaydedileceği klasörü oluşturma
                    ResimYol = Server.MapPath(ResimKonum_Vt);
                    Directory.CreateDirectory(ResimYol);
                    MusteriFotograf FotografVarmi = dbContext.MusteriFotografs.FirstOrDefault(x => x.FirmaId == FirmaId && x.FotografYol == ResimKonum_Vt + ResimAdi + "." + uzanti);
                    if (FotografVarmi == null)
                    {
                        MusteriFotograf mf = new MusteriFotograf();
                        mf.FirmaId = FirmaId;
                        mf.MusteriId = MusteriId;
                        mf.FotografAciklama = "";
                        mf.Notlar = "";
                        mf.FotografAdi = ResimAdi.Replace(uzanti, "");
                        mf.FotografYol = ResimKonum_Vt + ResimAdi;
                        mf.SecildiDurum = "0";

                        mf.SozlesmeId = SozlesmeId; // ekrandan seçilerek gelecek

                        mf.OlusturanKullaniciId = KullaniciId;
                        mf.OlusturmaTarih = DateTime.Now;
                        mf.DegistirenKullaniciId = KullaniciId;
                        mf.DegistirmeTarih = DateTime.Now;
                        mf.Aktif = true;
                        mf.Sil = false;
                        dbContext.MusteriFotografs.Add(mf);
                        try
                        {
                            dbContext.SaveChanges();
                            // Dosyayı Tam yola kaydet.
                            TamYol = ResimYol + ResimAdi;
                            if (!System.IO.File.Exists(TamYol)) // Bu Resim Yok ise Kaydedilecek, Varsa birşey yapılmayacak
                            {
                                Image img = Image.FromStream(file.InputStream);
                                img.Save(TamYol);
                            }
                            return Json(new { Sonuc = true, FotografId = mf.Id, Mesaj = "Kayıt başarılı.", JsonRequestBehavior.AllowGet });
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Müşteri Fotoğraf Ekle";
                            hata.HataMesajı = e.Message;
                            hata.OlusturanKullaniciId = 1;
                            hata.OlusturmaTarih = DateTime.Now;
                            hata.DegistirenKullaniciId = 1;
                            hata.DegistirmeTarih = DateTime.Now;
                            hata.Aktif = true;
                            hata.Sil = false;
                            dbContext.HataLoglaris.Add(hata);
                            dbContext.SaveChanges();
                            isSavedSuccessfully = false;
                            return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                        }
                    }
                }
            }

            //  0: İşlem Yok
            //  1: Müşteriye Seçime Gönderildi
            //  2: Müşteri Seçimi Yaptı
            //  3: Müşteriye Tekrar Göderildi
            //  4: Müşteri Seçimleri Baskı için Onayladı

            sz.FotografSecimDurum = sz.FotografSecimDurum == "1"?"3":"1";
            sz.FotografSecimDurumTarihi = DateTime.Now;
            sz.DegistirenKullaniciId = 1;
            sz.DegistirmeTarih = DateTime.Now;
            dbContext.SaveChanges();


            // Fotoğraflar yüklendikten sonra bilgiSMS i Ve Mail i Gönderiliyor.
            if (isSavedSuccessfully)
            {
                //  0: İşlem Yok
                //  1: Müşteriye Seçime Gönderildi
                //  2: Müşteri Seçimi Yaptı
                //  3: Müşteriye Tekrar Göderildi
                //  4: Müşteri Seçimleri Baskı için Onayladı
                Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                if (musteri.FotografSecimDurum == "1")
                    musteri.FotografSecimDurum = "3";
                else
                    musteri.FotografSecimDurum = "1";
                bool smskabul = true;
                bool emailkabul = true;
                if (musteri == null) { smskabul = true; emailkabul = true; }
                else { smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul; }
                string sonuc_fotografsecim_sms = "";
                bool sonuc_fotografsecim_mail = true;

                string randevubilgi = "";
                randevubilgi = sz.RezervasyonTarihi.ToShortDateString() + " saat " + sz.BaslangicSaat.ToString(@"hh\:mm") + "-" + sz.BaslangicSaat.ToString(@"hh\:mm") + " ";
                randevubilgi = randevubilgi + sz.OrganizasyonYeri.Replace("<br />", "") + " ";

                #region Fotoğraf Seçimi Bilgi SMSi
                if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // 
                {
                    AyarlarSmsGonderim smsayar = null;
                    string smsmetin = "";
                    smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.FotografSecimiBilgiMesajiMusteriGonderimSuresi == 6);  // Sms gönderim ayarı Opsiyon Tarihi Bilgi Gonderim Suresi için "Kayıt Yapıldığında" Seçilmişse
                    if (smsayar != null)
                        smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.FotografSecimiBilgiMesajiMusteri && x.Aktif == true && x.Sil == false).SMSMetni;
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
                            smsmetin = smsmetin.Replace("{CekimRandevuBilgileri}", randevubilgi);
                        sonuc_fotografsecim_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                    }
                }
                #endregion

                #region Fotoğraf Seçimi Bilgi Emaili
                if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.Email != "" && emailkabul)
                {
                    AyarlarMailGonderim mailayar = null;
                    MailMetinleri mmetin = null;
                    string mailmetin = "";
                    mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.FotografSecimiBilgiMailiMusteriGonderimSuresi == 6);
                    if (mailayar == null)
                        return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                    else
                        mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.OpsiyonTarihiBilgiMaili && x.Aktif == true && x.Sil == false);

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
                            mailmetin = mailmetin.Replace("{Tarih}", sz.RezervasyonTarihi.ToShortDateString());
                        if (mailmetin.IndexOf("{RandevuTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{RandevuTarihi}", sz.RezervasyonTarihi.ToShortDateString());
                        if (mailmetin.IndexOf("{RezervasyonTuru}") != -1)
                            mailmetin = mailmetin.Replace("{RezervasyonTuru}", sz.RezervasyonTurleri.RezervasyonTuru);
                        if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OdemeTarihi}", "");
                        if (mailmetin.IndexOf("{Tutar}") != -1)
                            mailmetin = mailmetin.Replace("{Tutar}", sz.ToplamFiyat.ToString());
                        if (mailmetin.IndexOf("{OpsiyonTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OpsiyonTarihi}", Convert.ToDateTime(sz.OpsiyonTarihi).ToShortDateString());
                        if (mailmetin.IndexOf("{SozlesmeNo}") != -1)
                            mailmetin = mailmetin.Replace("{SozlesmeNo}", sz.SozlesmeNo.ToString());
                        if (mailmetin.IndexOf("{CekimRandevuBilgileri}") != -1)
                            mailmetin = mailmetin.Replace("{CekimRandevuBilgileri}", randevubilgi);

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

                            sonuc_fotografsecim_mail = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, body, htmlView);
                        }
                        else
                            sonuc_fotografsecim_mail = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, mailmetin);
                    }

                }
                #endregion

                return Json(new { Sonuc = true, ResimAdi = ResimAdi, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Mesaj = "Kayıt Başarısız" });
            }
        }
        [HttpPost]
        public ActionResult MusteriFotografSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            bool isSavedSuccessfully = true;

            MusteriFotograf mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id);

            string TamYol = Server.MapPath(mf.FotografYol);

            if (System.IO.File.Exists(TamYol)) // bu resimden var mı?
            {
                //MusteriFotograf mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.MusteriId == MusteriId && x.Id == FotografId);
                if (mf != null)
                {
                    try
                    {
                        dbContext.MusteriFotografs.Remove(mf);
                        dbContext.SaveChanges();
                        System.IO.File.Delete(TamYol);  // resim var ise Sil
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Müşteri Fotoğraf Sil; Silinen Kayıt ID: " + mf.Id;
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
                    isSavedSuccessfully = false;
            }

            if (isSavedSuccessfully)
            {
                return Json(new { Sonuc = true, Mesaj = "Fotoğraf Silindi" });
            }
            else
            {
                return Json(new { Mesaj = "Kayıt Başarısız" });
            }
        }
        [HttpPost]
        public ActionResult MusteriFotografDegistir()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            long FotografId = Convert.ToInt64(Request["FotografId"]);
            string ResimAdi = "", ResimYol = "", TamYol = "", ResimKonum_Vt = "";
            bool isSavedSuccessfully = true;

            MusteriFotograf mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == FotografId && x.MusteriId == MusteriId);

            TamYol = Server.MapPath(mf.FotografYol);

            if (System.IO.File.Exists(TamYol)) // bu resimden var mı?
            {
                if (mf != null)
                {
                    // Resim klasörde var ise veri tabanında da var ise eski resim dosyasını siliyorum.
                    System.IO.File.Delete(TamYol);  // resim var ise Sil

                    // Resim silindikten sonra yeni yüklene resim klasöre kaydedilecek
                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileName];
                        System.IO.FileInfo ff = new System.IO.FileInfo(file.FileName);
                        string uzanti = ff.Extension;

                        ResimAdi = resimisimlendir.resimisimlendir2(file, file.FileName.Replace(uzanti, ""));
                        if (file != null && file.ContentLength > 0)
                        {
                            //veri tabanında saklanacak resim yolu
                            ResimKonum_Vt = "/Areas/Otomasyon/Dosyalar/MusteriFoto/" + MusteriId + "/";
                            // Resmin kaydedileceği klasörü oluşturma
                            ResimYol = Server.MapPath(ResimKonum_Vt);
                            Directory.CreateDirectory(ResimYol);
                            TamYol = ResimYol + ResimAdi;

                            mf.FotografAciklama = "";
                            mf.Notlar = "";
                            mf.FotografAdi = ResimAdi.Replace(uzanti, "");
                            mf.FotografYol = ResimKonum_Vt + ResimAdi;
                            mf.SecildiDurum = "0";
                            mf.DegistirenKullaniciId = KullaniciId;
                            mf.DegistirmeTarih = DateTime.Now;
                            try
                            {

                                if (!System.IO.File.Exists(TamYol)) // Bu Resim Yok ise Kaydedilecek, Varsa birşey yapılmayacak
                                {
                                    dbContext.SaveChanges();
                                    Image img = Image.FromStream(file.InputStream);
                                    img.Save(TamYol);
                                }
                            }
                            catch (Exception e)
                            {
                                HataLoglari hata = new HataLoglari();
                                hata.FirmaId = FirmaId;
                                hata.SubeId = SubeId;
                                hata.Islem = "Müşteri Fotoğraf Sil; Silinen Kayıt ID: " + mf.Id;
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
                else
                    isSavedSuccessfully = false;
            }
            if (isSavedSuccessfully)
            {
                return Json(new { Sonuc = true, Mesaj = "Fotoğraf Değiştirildi" });
            }
            else
            {
                return Json(new { Mesaj = "Kayıt Başarısız" });
            }
        }
        [HttpPost]
        public ActionResult NotlarKaydet()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            string[] Notlar = Request["Notlar"].Split(',');
            long fotograf_id = 0; string fotograf_not = "";
            List<long> foto_idler = new List<long>();
            List<string> foto_notlar = new List<string>();
            MusteriFotograf mf = new MusteriFotograf();
            if (Notlar[0] != "")
            {
                foreach (string Not in Notlar)
                {
                    if (Not.Contains("Foto_Id"))
                    {
                        int index = Not.IndexOf("Foto_Id:");
                        fotograf_id = (Not.Remove(index, 8) != null) ? Convert.ToInt64(Not.Remove(index, 8)) : 0;
                        foto_idler.Add(fotograf_id);
                    }
                    if (Not.Contains("Foto_Not"))
                    {
                        int index = Not.IndexOf("Foto_Not:");
                        fotograf_not = Not.Remove(index, 9).Replace('&', ',');
                        foto_notlar.Add(fotograf_not);
                    }
                }

                for (int i = 0; i < foto_idler.Count; i++)
                {
                    fotograf_id = foto_idler[i];
                    mf = dbContext.MusteriFotografs.FirstOrDefault(x => x.Id == fotograf_id && x.FirmaId == FirmaId && x.MusteriId == MusteriId);
                    if (mf != null)
                    {
                        mf.FotografAciklama = foto_notlar[i];
                        //mf.SecildiDurum = "0"; // 0: Seçilmedi  1: Düzenleme yapılmalı  2: Baskıya Uygun
                        mf.DegistirenKullaniciId = KullaniciId;
                        mf.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Fotoğrafçı Müşteri fotoğraf açıklamalarını güncelleme";
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
            return Json(new { Sonuc = true, Mesaj = "Açıklamalar başarıyla kaydedildi", JsonRequestBehavior.AllowGet });
        }
        [HttpPost]
        public ActionResult BilgiSMSGonder()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            //Firma firma = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (musteri.FotografSecimDurum == "1")
                musteri.FotografSecimDurum = "3";
            else
                musteri.FotografSecimDurum = "1";
            bool smskabul = true;
            bool emailkabul = true;
            if (musteri == null) { smskabul = true; emailkabul = true; }
            else { smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul; }
            string sonuc_fotografsecim_sms = "";

            string randevubilgi = "";
            randevubilgi = sz.RezervasyonTarihi.ToShortDateString() + " saat " + sz.BaslangicSaat.ToString(@"hh\:mm") + "-" + sz.BitisSaat.ToString(@"hh\:mm") + " ";
            randevubilgi = randevubilgi + sz.OrganizasyonYeri.Replace("<br />", "") + " ";

            #region Fotoğraf Seçimi Bilgi SMSi
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // 
            {
                AyarlarSmsGonderim smsayar = null;
                string smsmetin = "";
                smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.FotografSecimiBilgiMesajiMusteriGonderimSuresi == 6);  // Sms gönderim ayarı Opsiyon Tarihi Bilgi Gonderim Suresi için "Kayıt Yapıldığında" Seçilmişse
                if (smsayar != null)
                    smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.FotografSecimiBilgiMesajiMusteri && x.Aktif == true && x.Sil == false).SMSMetni;
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
                    sonuc_fotografsecim_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                    return Json(new { Sonuc = true, Mesaj = sonuc_fotografsecim_sms }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Sonuc = false, Mesaj = "Bilgi SMS'i gönderilemedi." }, JsonRequestBehavior.AllowGet);
            #endregion
        }
        [HttpPost]
        public ActionResult BilgiEpostaGonder()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);
            bool sonuc_fotografsecim_mail = true;
            Sozlesme sz = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == SozlesmeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.Email != "" && musteri.EmailKabul)
            {
                AyarlarMailGonderim mailayar = null;
                MailMetinleri mmetin = null;
                string mailmetin = "";
                string randevubilgi = "";
                randevubilgi = sz.RezervasyonTarihi.ToShortDateString() + " saat " + sz.BaslangicSaat.ToString(@"hh\:mm") + "-" + sz.BaslangicSaat.ToString(@"hh\:mm") + " ";
                randevubilgi = randevubilgi + sz.OrganizasyonYeri.Replace("<br />", "") + " ";
                mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.FotografSecimiBilgiMailiMusteriGonderimSuresi == 6);
               
                if (mailayar == null)
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                else
                    mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.FotografSecimiBilgiMailiMusteri && x.Aktif == true && x.Sil == false);

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
                        mailmetin = mailmetin.Replace("{CekimRandevuBilgileri}", randevubilgi);

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

                        sonuc_fotografsecim_mail = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, body, htmlView);
                    }
                    else
                        sonuc_fotografsecim_mail = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, musteri.Email, mmetin.MailKonu, mailmetin);
                }
            }

            return Json(new { Sonuc = false, Mesaj = "Bilgi e-postası gönderilemedi.", JsonRequestBehavior.AllowGet });
        }
        [HttpPost]
        public ActionResult MusteriFotografListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Müşteriler";
            ViewBag.AltMenu = "Fotoğraf Listesi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long SozlesmeId = Convert.ToInt64(Request["SozlesmeId"]);

            ViewBag.MusteriId = MusteriId;
            List<Models.Musteri> musteriler = dbContext.Musteris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Musteriler = musteriler;
            List<MusteriFotograf> musterifotolari = dbContext.MusteriFotografs.Where(x => x.MusteriId == MusteriId && x.SozlesmeId == SozlesmeId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true).ToList();
            ViewBag.MusteriFotolari = musterifotolari;
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
        [HttpPost]
        public JsonResult AktifSozlesmeler(long id)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            List<Sozlesme> aktifsozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.MusteriId == id && x.Bitti == false && x.KesinRezervasyonBit == true && x.Aktif == true && x.Sil == false).ToList();

            var AktifSozlesmeler = aktifsozlesmeler.Select(m => new { SozlesmeId = m.Id, SozlesmeNo = m.SozlesmeNo, RezervasyonTarihi = m.RezervasyonTarihi }).OrderBy(x => x.SozlesmeNo); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(AktifSozlesmeler, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MusteriHesapTakibi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Müşteriler";
            ViewBag.AltMenu = "Müşteri Hesap Takibi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            ViewBag.AktifSubeId = SubeId;
            List<Models.Musteri> musteriler;
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 37 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            if (SubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                musteriler = dbContext.Musteris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList(); // aktif şubeye ait müşteri listesi
            else
                musteriler = dbContext.Musteris.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList(); // aktif şubeye ait müşteri listesi
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            List<SmsMetinleri> smsmetinleri = dbContext.SmsMetinleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).MusteriHesapTakibi;
            ViewBag.FiltreAyar = filtreayar;
            ViewBag.SubeListesi = sube;
            ViewBag.SubeKullanici = subekullanici;
            ViewBag.Musteriler = musteriler;
            ViewBag.SmsMetinleri = smsmetinleri;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 37 && x.Aktif == true && x.Sil == false);
            
            return View();
        }
        [HttpPost]
        public ActionResult MusteriHesapBakiyeler(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<Odemeler> o = new List<Odemeler>();
            o = dbContext.Odemelers.Where(x => x.MusteriId == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            var musterilist = o.Select(m => new
            {
                Id = m.Id,
                SubeAdi = m.Sube.SubeAdi,
                FirmaAdi = m.Firma.FirmaAdi,
                MusteriId = m.MusteriId,
                Musteri = m.Musteri.AdiSoyadi,
                MusteriTel=m.Musteri.CepTel,
                MusteriKodu = m.Musteri.MusteriKodu,
                SozlesmeNo = m.Sozlesme.SozlesmeNo,
                SozlesmeId = m.SozlesmeId,
                GunlukIsTakipNo = m.GunlukIsler.TakipNo,
                OdemeTuru = m.OdemeTuru,
                IslemTarihi = m.Tarih.ToShortDateString(),
                GelecekOdemeId = m.GelecekOdemeID,
                OdemeTarihi = m.OdemeTarihi.ToShortDateString(),
                OdemeYapanAdSoyad = m.OdemeYapanAdSoyad,
                OdemeSekli = m.OdemeSekli,
                OdemeAl = m.OdemeAl,
                AlinanOdemeMakbuzNo = m.AlinanOdemeMakbuzNo,
                Iptal = m.Iptal,
                Kilit = m.KilitBit,
                Tutar = m.Tutar,
                Aciklama = m.Notlar
            }).ToList(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.

            //var o = dbContext.Odemelers.Where(x => x.MusteriId == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false)
            //    .Join(dbContext.Musteris, odeme => odeme.MusteriId, musteri => musteri.Id, (odeme, musteri) => new { odeme, musteri })
            //    .Select(m => new
            //    {
            //        Id = m.odeme.Id,
            //        SubeAdi = m.odeme.Sube.SubeAdi,
            //        FirmaAdi = m.odeme.Firma.FirmaAdi,
            //        MusteriId = m.odeme.MusteriId,
            //        Musteri = m.musteri.AdiSoyadi,
            //        MusteriKodu = m.musteri.MusteriKodu,
            //        SozlesmeNo = m.odeme.Sozlesme.SozlesmeNo,
            //        SozlesmeId = m.odeme.SozlesmeId,
            //        GunlukIsTakipNo = m.odeme.GunlukIsler.TakipNo,
            //        OdemeTuru = m.odeme.OdemeTuru,
            //        IslemTarihi = m.odeme.Tarih.ToShortDateString(),
            //        GelecekOdemeId = m.odeme.GelecekOdemeID,
            //        OdemeTarihi = m.odeme.OdemeTarihi.ToShortDateString(),
            //        OdemeYapanAdSoyad = m.odeme.OdemeYapanAdSoyad,
            //        OdemeSekli = m.odeme.OdemeSekli,
            //        OdemeAl = m.odeme.OdemeAl,
            //        AlinanOdemeMakbuzNo = m.odeme.AlinanOdemeMakbuzNo,
            //        Iptal = m.odeme.Iptal,
            //        Kilit = m.odeme.KilitBit,
            //        Tutar = m.odeme.Tutar,
            //        Aciklama = m.odeme.Notlar
            //    }).ToList(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.

            return Json(new { data = musterilist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult MusteriHareketOdemeAl(long? id)
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
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            DateTime IslemTarihi = Convert.ToDateTime(Request["IslemTarihi"]);
            DateTime OdemeTarihi = Convert.ToDateTime(Request["OdemeTarihi"]);
            decimal Tutar = Convert.ToDecimal(Request["Tutar"]);
            string Aciklama = Request["Aciklama"];
            string OdemeSekli = Request["OdemeSekli"];
            string OdemeYapan = Request["OdemeYapan"];

            // Makbuz Numarası Oluşturuluyor.
            long makbuzno = 0;
            string mn;
            List<GelirGider> alinanodemevarmi = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId).ToList();
            if (alinanodemevarmi.Count > 0)
            {
                makbuzno = Convert.ToInt64(alinanodemevarmi.Max(x => x.MakbuzNo));
                makbuzno = makbuzno + 1;
            }
            else
                mn = FirmaId.ToString() + "101" + makbuzno.ToString();
            mn = makbuzno.ToString();
            // Makbuz Numarası Oluşturuluyor.
            IslemTarihi = OdemeTarihi.AddHours(DateTime.Now.Hour);
            IslemTarihi = OdemeTarihi.AddMinutes(DateTime.Now.Minute);
            OdemeTarihi = OdemeTarihi.AddHours(DateTime.Now.Hour);
            OdemeTarihi = OdemeTarihi.AddMinutes(DateTime.Now.Minute);

            Odemeler alinanodeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id && x.Aktif == true && x.Sil == false);
            if (alinanodeme == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            Odemeler odeme = new Odemeler();
            odeme.FirmaId = FirmaId;
            odeme.SubeId = SubeId;
            odeme.SozlesmeId = alinanodeme.SozlesmeId;
            odeme.GisId = alinanodeme.GisId;
            odeme.MusteriId = alinanodeme.MusteriId;
            odeme.GelecekOdemeID = id;
            odeme.OdemeYapanAdSoyad = OdemeYapan;
            odeme.Tarih = IslemTarihi;
            odeme.Tutar = Tutar;
            odeme.OdemeTarihi = OdemeTarihi;
            odeme.OdemeTuru = "AlinanOdeme";
            odeme.OdemeSekli = OdemeSekli;
            odeme.AlinanOdemeMakbuzNo = Convert.ToInt64(mn); // GelirGider tablosundaki MakbuzNo = Odemler tablosundaki MakbuzNo
            odeme.Kapora = false;
            odeme.KilitBit = false;
            odeme.Notlar = Aciklama;
            odeme.Iptal = false;
            odeme.Aktif = true;
            odeme.Sil = false;
            odeme.OlusturanKullaniciId = KullaniciId;
            odeme.OlusturmaTarih = DateTime.Now;
            odeme.DegistirenKullaniciId = KullaniciId;
            odeme.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.Odemelers.Add(odeme);
                dbContext.SaveChanges();
                alinanodeme.OdemeAl = true;
                alinanodeme.KilitBit = true;
                alinanodeme.OdemeTarihi = OdemeTarihi;
                alinanodeme.DegistirenKullaniciId = KullaniciId;
                alinanodeme.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();

                #region Tahsilattan kalan tutarı yeniden ekle
                if (Tutar < alinanodeme.Tutar)
                {
                    Odemeler yenihareket = new Odemeler();
                    yenihareket.FirmaId = FirmaId;
                    yenihareket.SubeId = SubeId;
                    yenihareket.SozlesmeId = alinanodeme.SozlesmeId;
                    yenihareket.GisId = alinanodeme.GisId;
                    yenihareket.MusteriId = alinanodeme.MusteriId;
                    yenihareket.GelecekOdemeID = id;
                    yenihareket.OdemeYapanAdSoyad = OdemeYapan;
                    yenihareket.Tarih = IslemTarihi;
                    yenihareket.Tutar = Tutar;
                    yenihareket.OdemeTarihi = OdemeTarihi;
                    yenihareket.OdemeTuru = "AlinanOdeme";
                    yenihareket.OdemeSekli = OdemeSekli;
                    yenihareket.AlinanOdemeMakbuzNo = Convert.ToInt64(mn);
                    yenihareket.Kapora = false;
                    yenihareket.KilitBit = false;
                    yenihareket.Notlar = alinanodeme.Tarih.ToShortDateString() + " Tarihli Gelecek Ödemenin " + OdemeTarihi.ToShortDateString() + " tarihindeki Ödemesinden kalan";
                    yenihareket.Iptal = false;
                    yenihareket.Aktif = true;
                    yenihareket.Sil = false;
                    yenihareket.OlusturanKullaniciId = KullaniciId;
                    yenihareket.OlusturmaTarih = DateTime.Now;
                    yenihareket.DegistirenKullaniciId = KullaniciId;
                    yenihareket.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.Odemelers.Add(yenihareket);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Cari Hareket Ekle, Ödemeden kalan tutarı Borç olarak yeniden ekle";
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

                #region Gelir Gider olarak ekle
                GelirGider gg = new GelirGider();
                GelirGiderTurleri ggtur = new GelirGiderTurleri();
                if (alinanodeme.GisId > 1)
                {
                    ggtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.GelirGiderTur == "Günlük İşler");

                    if (ggtur == null)
                    {
                        ggtur.FirmaId = FirmaId; // sessiondaki FirmaID
                        ggtur.GelirGiderTur = "Günlük İşler";
                        ggtur.OlusturanKullaniciId = KullaniciId;
                        ggtur.OlusturmaTarih = DateTime.Now;
                        ggtur.DegistirenKullaniciId = KullaniciId;
                        ggtur.DegistirmeTarih = DateTime.Now;
                        ggtur.Aktif = true;
                        ggtur.Sil = false;
                        dbContext.GelirGiderTurleris.Add(ggtur);
                        dbContext.SaveChanges();
                    }
                }
                else if (odeme.SozlesmeId > 1)
                {
                    ggtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.GelirGiderTur == "Sözleşme Ödemesi");

                    if (ggtur == null)
                    {
                        ggtur.FirmaId = FirmaId; // sessiondaki FirmaID
                        ggtur.GelirGiderTur = "Sözleşme Ödemesi";
                        ggtur.OlusturanKullaniciId = KullaniciId;
                        ggtur.OlusturmaTarih = DateTime.Now;
                        ggtur.DegistirenKullaniciId = KullaniciId;
                        ggtur.DegistirmeTarih = DateTime.Now;
                        ggtur.Aktif = true;
                        ggtur.Sil = false;
                        dbContext.GelirGiderTurleris.Add(ggtur);
                        dbContext.SaveChanges();
                    }
                }
                gg.GelirGiderTurId = ggtur.Id;
                gg.FirmaId = FirmaId;
                gg.SubeId = SubeId;
                gg.Tarih = OdemeTarihi;
                gg.SozlesmeId = alinanodeme.SozlesmeId;
                gg.GisId = alinanodeme.GisId;
                gg.OdemeId = odeme.Id;
                gg.Tip = "Gelir";
                gg.Tutar = Tutar;
                gg.MakbuzNo = Convert.ToInt64(mn);// GelirGider tablosundaki MakbuzNo = Odemler tablosundaki MakbuzNo
                if (alinanodeme.SozlesmeId > 1)
                    gg.Notlar = alinanodeme.Sozlesme.SozlesmeNo + " Numaralı sözleşmeye ait ödeme";
                else
                    gg.Notlar = alinanodeme.GunlukIsler.TakipNo + " Numaralı günlük işe ait ödeme";
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
                    hata.Islem = "Odemeler, Ödeme al - GelirGider kaydı, Kayıt Id: " + id;
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
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Odemeler, Ödeme al, Kayıt Id: " + id;
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
            return Json(new { Sonuc = true, Data = odeme.Id, Mesaj = "Ödeme Alındı", JsonRequestBehavior.AllowGet });
        }
        [HttpPost]
        public ActionResult AlacakOdemeSMSGonder()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            long SmsMetinId = Convert.ToInt64(Request["SmsMetinId"]);
            DateTime OdemeTarihi = Convert.ToDateTime(Request["OdemeTarihi"]);
            //string tutar1 = Request["Tutar"];
            //tutar1 = tutar1.Replace("TL", "");
            decimal Tutar = Convert.ToDecimal(Request["Tutar"]);
            // Alacak Metni için SMS metinleri içinden seçim yapılacak.

            string sonuc_musteri_randevu_sms = "";
            Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            bool smskabul = true;
            if (musteri == null)
                smskabul = true;
            else
                smskabul = musteri.SMSKabul;

            #region Alınan Ödeme Bilgi Smsi
            if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
            {

                string smsmetin = "";

                smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == SmsMetinId && x.Aktif == true && x.Sil == false).SMSMetni;

                if (smsmetin != null)
                {
                    if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                        smsmetin = smsmetin.Replace("{FirmaAdi}", musteri.AdiSoyadi);
                    
                    if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
                    if (smsmetin.IndexOf("{Tarih}") != -1)
                        smsmetin = smsmetin.Replace("{Tarih}", OdemeTarihi.ToShortDateString());
                    if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                        smsmetin = smsmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                    if (smsmetin.IndexOf("{Tutar}") != -1)
                        smsmetin = smsmetin.Replace("{Tutar}", Tutar.ToString());
                    sonuc_musteri_randevu_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
                    return Json(new { Sonuc = true, Mesaj = sonuc_musteri_randevu_sms }, JsonRequestBehavior.AllowGet);
                }
            }
            #endregion
            return Json(new { Sonuc = false, Mesaj = "SMS Gönderilemedi." }, JsonRequestBehavior.AllowGet);
        }
    }
}