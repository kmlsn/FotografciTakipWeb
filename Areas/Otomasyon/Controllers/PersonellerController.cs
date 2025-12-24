using FotografciTakipWeb.App_Settings;
using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class PersonellerController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/Personeller
        public ActionResult Personeller()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Personeller";
            ViewBag.AltMenu = "Personel Listesi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 68 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            List<SubeToPersonel> subepersonel = dbContext.SubeToPersonels.Select(x => x).ToList();
            List<PersonelGorevleri> personelgorevleri = dbContext.PersonelGorevleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.gorevler = personelgorevleri;
            ViewBag.SubeListesi = sube;
            ViewBag.SubeKullanici = subekullanici;
            ViewBag.SubePersonel = subepersonel;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 68 && x.Aktif == true && x.Sil == false);
            List<Sube> subeler = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.Subeler = subeler;
            int BaslangicIzin = dbContext.AyarlarPersonels.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).BaslangicIzinSuresi;
            int personelSayisi = dbContext.Personels.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList().Count;
            int personelLimit = Convert.ToInt32(dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).PersonelLimit);
            ViewBag.PLimit = personelSayisi < personelLimit ? true : false;
            ViewBag.Ayar = dbContext.AyarlarFiltres.FirstOrDefault(x=>x.FirmaId==FirmaId).PersonelListesiPasifGizle;
            return View();
        }
        public ActionResult PersonelListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            List<Personel> personel = dbContext.Personels.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var perlist = personel.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                Sube = m.Sube.SubeAdi,
                GorevId = m.GorevId,
                Gorev = m.PersonelGorevleri.Gorev,
                AdSoyad = m.AdiSoyadi,
                TCKimlikNo = m.TCKimlikNo,
                BaslamaTarihi = m.BaslamaTarihi,
                BitisTarihi = m.BitisTarihi,
                SabitTel = m.SabitTel,
                CepTel = m.CepTel,
                Email = m.Email,
                YillikIzinHakki = m.YillikIzinHakki,
                ToplamIzin = m.ToplamIzin,
                KullanilanIzin = m.KullanilanIzin,
                KalanIzin = m.KalanIzin,
                Ucret = m.Ucret,
                CalismaSekli = m.CalismaSekli,
                Subeler = m.GorevliSubeler,
                Adres = m.Adres,
                SmsKabul = m.SMSKabul,
                EmailKabul = m.EmailKabul,
                Aktif = m.Aktif,
                KilitBit = m.KilitBit
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = perlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public string PersonelVarMi()
        {
            string AdSoyad = Request["AdSoyad"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string cep1 = Request["CepTel"];
            TelefonDüzelt td = new TelefonDüzelt();
            if (cep1 != null) { cep1 = td.düzelt(cep1); }
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            Personel per = dbContext.Personels.FirstOrDefault(x => x.SubeId == SubeId && x.AdiSoyadi == AdSoyad && x.TCKimlikNo == TCKimlikNo && x.Aktif == true && x.Sil == false);

            if (per != null)
            {
                return per.Id.ToString();
            }
            else
            {
                return "Yok";
            }
        }
        [HttpPost]
        public ActionResult PesonelEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long PersonelGorevId = Convert.ToInt64(Request["PersonelGorevId"]);
            string AdSoyad = Request["AdSoyad"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string cep = Request["CepTel"];
            string sabit = Request["SabitTel"];
            string Email = Request["Email"];
            decimal Ucret = Convert.ToDecimal(Request["Ucret"]);
            int IzinSuresi = Convert.ToInt32(Request["IzinSuresi"]);
            int toplamizin = 0;
            string CalismaSekli = Request["CalismaSekli"];
            string BaslamaTarihi = Request["BaslamaTarihi"];
            string SmsKabul = Request["SmsKabul"];
            string EmailKabul = Request["EmailKabul"];
            DateTime baslama = Convert.ToDateTime(BaslamaTarihi);
            string Adres = Request["Adres"];
            Adres = Adres.Replace("\r\n", "<br />");
            Adres = Adres.Replace("\n", "<br />");
            string[] SubeListesi = Request["SubeListesi"].Split(','); // SubetoKullanici tablosuna tek tek eklemek için
            string SubeListesi2 = Request["SubeListesi"]; // SubetoKullanici tablosuna tek tek eklemek için
            TelefonDüzelt td = new TelefonDüzelt();
            if (cep != null) { cep = td.düzelt(cep); }
            if (sabit != null) { sabit = td.düzelt(sabit); }

            Personel kayitvarmi = dbContext.Personels.FirstOrDefault(x => x.SubeId == SubeId && x.AdiSoyadi == AdSoyad && x.TCKimlikNo == TCKimlikNo && x.Aktif == false && x.Sil == true); // eklenmek istenilen kayıt daha önceden silinmişse aktif yapılıyor.
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
                    hata.Islem = "Personel Ekle, Silinmiş kaydı aktif yap, Kayıt Id: " + kayitvarmi.Id.ToString();
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
                Personel pers = new Personel();
                pers.FirmaId = FirmaId;
                pers.SubeId = SubeId;
                pers.AdiSoyadi = AdSoyad;
                pers.Adres = Adres;
                pers.BaslamaTarihi = baslama;
                pers.BitisTarihi = Convert.ToDateTime("01.01.2500");
                pers.CepTel = cep;
                pers.Email = Email;
                pers.GorevliSubeler = SubeListesi2;
                pers.GorevId = PersonelGorevId;
                pers.SabitTel = sabit;
                pers.TCKimlikNo = TCKimlikNo;
                pers.Ucret = Ucret;
                pers.YillikIzinHakki = IzinSuresi;
                pers.ToplamIzin = toplamizin + IzinSuresi;
                pers.KalanIzin = toplamizin + IzinSuresi;
                if (CalismaSekli == "1") { pers.CalismaSekli = "Tam Zamanlı (Full Time)"; }
                else if (CalismaSekli == "2") { pers.CalismaSekli = "Yarı Zamanlı (Part Time)"; }
                else if (CalismaSekli == "3") { pers.CalismaSekli = "Dönemsel"; }
                else if (CalismaSekli == "4") { pers.CalismaSekli = "Serbest (Freelance)"; }
                else if (CalismaSekli == "5") { pers.CalismaSekli = "Günlük (İş Başı)"; }
                if (SmsKabul == "Var")
                    pers.SMSKabul = true;
                else
                    pers.SMSKabul = false;
                if (EmailKabul == "Var")
                    pers.EmailKabul = true;
                else
                    pers.EmailKabul = false;
                if (string.IsNullOrEmpty(Email))
                    pers.EmailKabul = false;
                else
                    pers.EmailKabul = true;
                pers.OlusturanKullaniciId = KullaniciId;
                pers.OlusturmaTarih = DateTime.Now;
                pers.DegistirenKullaniciId = KullaniciId;
                pers.DegistirmeTarih = DateTime.Now;
                pers.Aktif = true;
                pers.Sil = false;
                try
                {
                    dbContext.Personels.Add(pers);
                    dbContext.SaveChanges();
                    #region Personel Şube görevlendirme
                    SubeToPersonel personelkullanici = new SubeToPersonel();
                    foreach (var sube in SubeListesi)
                    {
                        if (sube != "")
                        {
                            personelkullanici.SubeId = Convert.ToInt64(sube);
                            personelkullanici.PersonelId = pers.Id;
                            dbContext.SubeToPersonels.Add(personelkullanici);
                            try
                            {
                                dbContext.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                HataLoglari hata = new HataLoglari();
                                hata.FirmaId = FirmaId;
                                hata.SubeId = SubeId;
                                hata.Islem = "Personel Şube Görevlendirme, Şube Id: " + sube + ", Personel Id: " + pers.Id.ToString();
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
                    // Cep telefonu boşsa veya cep telefonu dolu mail adresi boşsa rehbere kaydetme
                    //if (!string.IsNullOrEmpty(cep) || (!string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(cep)))
                    //{
                        AyarlarGenel genelayar = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId);
                        RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Personeller");
                        TelefonRehberi rehber = new TelefonRehberi();
                        rehber.FirmaId = FirmaId;
                        rehber.RehberGrupId = rehbergrup.Id;
                        rehber.PersonelId = pers.Id;
                        rehber.FirmaAdi = "";
                        rehber.AdSoyad = AdSoyad;
                        rehber.SabitTel1 = sabit;
                        rehber.SabitTel2 = "";
                        rehber.CepTel1 = cep;
                        rehber.CepTel2 = "";
                        rehber.Fax = "";
                        rehber.Email = Email;
                        rehber.SmsKabul = true;
                        if (string.IsNullOrEmpty(Email))
                            rehber.EmailKabul = false;
                        else
                            rehber.EmailKabul = true;
                        rehber.Notlar = "Yeni Personel Kaydı";
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
                    //}
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Personel Ekle";
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
        public string EmailKontrol()
        {
            string Email = Request["Email"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Email == Email && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

            // email kontrolü firma bağılsız olarak yapılıyor.
            Personel per = dbContext.Personels.FirstOrDefault(x => x.Email == Email && x.Aktif == true && x.Sil == false);

            if (per != null)
                return per.Id.ToString();
            else
                return "Yok";

        }
        [HttpPost]
        public ActionResult PersonelGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long PersonelGorevId = Convert.ToInt64(Request["PersonelGorevId"]);
            string AdSoyad = Request["AdSoyad"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string cep = Request["CepTel"];
            string sabit = Request["SabitTel"];
            string Email = Request["Email"];
            decimal Ucret = Convert.ToDecimal(Request["Ucret"]);
            int IzinSuresi = Convert.ToInt32(Request["IzinSuresi"]);
            int EskiIzinSuresi = Convert.ToInt32(Request["EskiIzinSuresi"]);
            string CalismaSekli = Request["CalismaSekli"];
            string BaslamaTarihi = Request["BaslamaTarihi"];
            string BitisTarihi = Request["BitisTarihi"];
            string SmsKabul = Request["SmsKabul"];
            string EmailKabul = Request["EmailKabul"];
            DateTime bitis = Convert.ToDateTime("01.01.2500");
            DateTime baslama = Convert.ToDateTime(BaslamaTarihi);
            bool kilit = false;
            
            if (!string.IsNullOrEmpty(BitisTarihi))
            {
                bitis = Convert.ToDateTime(BitisTarihi);
                kilit = true;
            }
            string Adres = Request["Adres"];
            Adres = Adres.Replace("\r\n", "<br />");
            Adres = Adres.Replace("\n", "<br />");
            TelefonDüzelt td = new TelefonDüzelt();
            if (cep != null) { cep = td.düzelt(cep); }
            if (sabit != null) { sabit = td.düzelt(sabit); }
            string[] SubeListesi = Request["SubeListesi"].Split(','); // SubetoKullanici tablosuna tek tek eklemek için
            string SubeListesi2 = Request["SubeListesi"]; // SubetoKullanici tablosuna tek tek eklemek için
            Personel per = dbContext.Personels.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false);
            if (per == null)
                return Json(new { Sonuc = false, Mesaj = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            //per.GorevId = PersonelGorevId;
            //per.SubeId = SubeId;
            per.TCKimlikNo = TCKimlikNo;
            per.AdiSoyadi = AdSoyad;
            //per.BaslamaTarihi = baslama;
            per.BitisTarihi = bitis;
            per.SabitTel = sabit;
            per.CepTel = cep;
            //per.Email = Email;
            per.Ucret = Ucret;
            per.GorevliSubeler = SubeListesi2;
            if (SmsKabul == "Var")
                per.SMSKabul = true;
            else
                per.SMSKabul = false;
            if (EmailKabul == "Var")
                per.EmailKabul = true;
            else
                per.EmailKabul = false;
            if (bitis != Convert.ToDateTime("01.01.2500") && bitis <= DateTime.Now)
            {
                per.Aktif = false;
            }
            per.YillikIzinHakki = IzinSuresi;
            if (IzinSuresi > EskiIzinSuresi)
            {
                per.ToplamIzin = per.ToplamIzin + (IzinSuresi - EskiIzinSuresi);
            }
            else if (IzinSuresi < EskiIzinSuresi)
            {
                per.ToplamIzin = per.ToplamIzin - (EskiIzinSuresi - IzinSuresi);
            }
            if (CalismaSekli == "1") { per.CalismaSekli = "Tam Zamanlı (Full Time)"; }
            else if (CalismaSekli == "2") { per.CalismaSekli = "Yarı Zamanlı (Part Time)"; }
            else if (CalismaSekli == "3") { per.CalismaSekli = "Dönemsel"; }
            else if (CalismaSekli == "4") { per.CalismaSekli = "Serbest (Freelance)"; }
            else if (CalismaSekli == "5") { per.CalismaSekli = "Günlük (İş Başı)"; }
            per.Adres = Adres;
            per.KilitBit = kilit;
            per.DegistirenKullaniciId = KullaniciId;
            per.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                #region Şube yetkilisi ise Şube bilgilerini de güncelle
                long gorevid = dbContext.PersonelGorevleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true && x.Gorev == "Şube Yetkilisi").Id;
                if (per.GorevId == gorevid)
                {
                    Sube sb = dbContext.Subes.FirstOrDefault(x => x.Id == per.SubeId && x.FirmaId == FirmaId);
                    sb.Yetkili = AdSoyad;
                    sb.TCKimlikNo = TCKimlikNo;
                    sb.Email = Email;
                    sb.CepTel = cep;
                }
                #endregion
                #region Kullanıcı güncelle
                Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Email == per.Email && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                if (kl != null)
                {
                    kl.AdSoyad = AdSoyad;
                    dbContext.SaveChanges();
                }
                #endregion
                #region Rehber kaydını güncelle veya yeni rehber kaydı
                TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.PersonelId == id);
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
                        RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Personeller");
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
                        r.Notlar = "Personel Güncelle - Personel İletişim Bilgilerini Rehbere Ekle";
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
                #endregion
                #region Personel Şube Görevlendirme
                // var olan yetkiler kalacak yeni eklene yetki tabloya eklenecek, KullanıcıId ve ŞubeId ile sorgu yapılacak

                SubeToPersonel subepersonel = new SubeToPersonel();
                List<SubeToPersonel> yetkilisubeler = dbContext.SubeToPersonels.Where(x => x.PersonelId == id).ToList();
                // Kullanıcının herhangi bir şubeye yetkisi yoksa sayfadan gelen tüm subelere yetkilendiriliyor.
                long subeid;
                if (yetkilisubeler == null)
                {
                    foreach (var sube in SubeListesi)
                    {
                        subepersonel.SubeId = Convert.ToInt64(sube);
                        subepersonel.PersonelId = per.Id;
                        dbContext.SubeToPersonels.Add(subepersonel);
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Personel Şube Görevlendirme, Şube Id: " + sube + ", Personel Id: " + per.Id.ToString();
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
                        SubeToPersonel sb = dbContext.SubeToPersonels.FirstOrDefault(x => x.PersonelId == id && x.SubeId == subeid);
                        // yetkisinin olamadığı şubeye yetkilendiriliyor.
                        if (sb == null)
                        {
                            subepersonel.SubeId = Convert.ToInt64(sube);
                            subepersonel.PersonelId = per.Id;
                            dbContext.SubeToPersonels.Add(subepersonel);
                            try
                            {
                                dbContext.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                HataLoglari hata = new HataLoglari();
                                hata.FirmaId = FirmaId;
                                hata.SubeId = SubeId;
                                hata.Islem = "Personeli Şubeye Yetkilendirme, Şube Id: " + sube + ", Personel Id: " + per.Id.ToString();
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
                hata.Islem = "Personel Güncelle, Kayıt Id: " + id;
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
        public ActionResult PersonelSil(long? id)
        {
            // Personelin kullanıldığı tablolarda kayıtlar var ise personel silinemeyecek
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            
            List<PersonelIzin> izin = dbContext.PersonelIzins.Where(x => x.PersonelId == id && x.Sil == false).ToList();
            List<PersonelOdeme> persodeme = dbContext.PersonelOdemes.Where(x => x.PersonelId == id && x.Sil == false).ToList();
            //List<Randevu> rn = dbContext.Randevus.Where(x => x.PersonelId == id && x.Sil == false).ToList();
            List<RandevuToPersonel> randpers = dbContext.RandevuToPersonels.Where(x => x.PersonelId == id && x.Sil == false).ToList();
            if (izin.Count > 0 || randpers.Count > 0 || persodeme.Count > 0)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Personel Sil, Personel Kullanılıyor, Silinemez! Kayıt Id: " + id.ToString();
                hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Personele ait işlem detayları var, Silinemez!", JsonRequestBehavior.AllowGet });
            }
            else
            {
                //Personelin şube ilişikileri siliniyor.
                List<SubeToPersonel> subepersonel = dbContext.SubeToPersonels.Where(x => x.PersonelId == id).ToList();
                if (subepersonel.Count>0)
                {
                    foreach (var item in subepersonel)
                    {
                        dbContext.SubeToPersonels.Remove(item);
                    }
                    dbContext.SaveChanges();
                }
                Personel p = dbContext.Personels.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                if (p == null)
                    return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
                p.Aktif = false;
                p.Sil = true;
                p.GorevliSubeler = "";
                p.DegistirenKullaniciId = KullaniciId;
                p.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();

                    TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.PersonelId == id);
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
                            hata.Islem = "Personel Sil, Kayıt Id: " + id + ", Telefon rehberinden personel iletişim bilgilerini sil";
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
                    hata.Islem = "Personel Sil, Kayıt Id: " + id;
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
        public string IzınGunSayisiHesapla()
        {
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            string IzinBaslangic = Request["IzinBaslangic"];
            string IzinBitis = Request["IzinBitis"];
            TimeSpan IzinGunSayisi;
            DateTime izinbaslangic = Convert.ToDateTime(IzinBaslangic);
            DateTime izinbitis = Convert.ToDateTime(IzinBitis);
            AyarlarGenel genelayar = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId);
            IzinGunSayisi = izinbitis - izinbaslangic;
            int Gun = Convert.ToInt32(IzinGunSayisi.Days) + 1;
            var CumartesiAdet = 0;
            var PazarAdet = 0;
            if (!genelayar.CalismaGunuCumartesi) // Cumartesileri izinden düş
            {
                var tarihListe = Enumerable.Range(0, Gun).Select(e => izinbaslangic.AddDays(e));
                CumartesiAdet = tarihListe.Where(e => e.DayOfWeek == DayOfWeek.Saturday).Count();
            }
            if (!genelayar.CalismaGunuPazar) // Cumartesileri izinden düş
            {
                var tarihListe = Enumerable.Range(0, Gun).Select(e => izinbaslangic.AddDays(e));
                PazarAdet = tarihListe.Where(e => e.DayOfWeek == DayOfWeek.Sunday).Count();
            }
            Gun = Gun - CumartesiAdet - PazarAdet;
            return Gun.ToString();
        }
        [HttpPost]
        public string PersonelDurumDegistir(long? id)
        {
            Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == id && x.Sil == false);
            if (personel.Aktif == true)
                personel.Aktif = false;
            else
                personel.Aktif = true;
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
        public ActionResult PersonelIzinleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Personeller";
            ViewBag.AltMenu = "Personel İzinleri";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            ViewBag.AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 69 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).PersonelIzinTakibi;
            ViewBag.FiltreAyar = filtreayar;
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            List<SubeToPersonel> subepersonel = dbContext.SubeToPersonels.Where(x => x.SubeId == SubeId).ToList();
            List<Personel> pers = new List<Personel>();
            foreach (var item in subepersonel)
            {
                Personel p = dbContext.Personels.FirstOrDefault(x => x.Id == item.PersonelId && x.Aktif == true && x.Sil == false);
                if (p != null)
                {
                    pers.Add(dbContext.Personels.FirstOrDefault(x => x.Id == item.PersonelId && x.Aktif == true && x.Sil == false));
                }
            }
            //List<Personel> pers = dbContext.Personels.Where(x => x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();
            ViewBag.personeller = pers.Distinct().ToList();
            ViewBag.SubeListesi = sube;
            ViewBag.SubeKullanici = subekullanici;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 69 && x.Aktif == true && x.Sil == false);
            return View();
        }
        public ActionResult PersonelIzinListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).PersonelIzinTakibi;
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
            List<PersonelIzin> personelizin = new List<PersonelIzin>();
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            personelizin = dbContext.PersonelIzins.Where(x => x.FirmaId == FirmaId && ((x.IzinBaslama >= ilktarih) && (x.IzinBitis <= sontarih)) && x.Sil == false && x.Aktif == true).ToList();
            var izinler = personelizin.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                SubeAdi = m.Sube.SubeAdi,
                GorevliSubeler = m.GorevliSubeler,
                AdSoyad = m.Personel.AdiSoyadi,
                Gorev = m.Personel.PersonelGorevleri.Gorev,
                IzinBaslama = m.IzinBaslama,
                IzinBitis = m.IzinBitis,
                IseBaslama = m.IseBaslama,
                Gun = m.KullanilanIzinGun,
                KalanIzin = m.Personel.KalanIzin,
                Kilit = m.Personel.KilitBit,
                Aktif = m.Personel.Aktif,
                Aciklama = m.Aciklama
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = izinler }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonelIzinListesiFiltre(string IlkTarih, string SonTarih, int Sube, int PersonelId)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<PersonelIzin> pizin = new List<PersonelIzin>();
            List<PersonelIzin> personelizin = new List<PersonelIzin>();
            string[] gorenvlisubeler;
            long sb = 0;
            if (Sube == 0 && PersonelId == 0)
                personelizin = dbContext.PersonelIzins.Where(x => x.FirmaId == FirmaId && ((x.IzinBaslama >= ilktarih) && (x.IzinBitis <= sontarih)) && x.Sil == false && x.Aktif == true).ToList();
            else if (Sube == 0 && PersonelId != 0)
                personelizin = dbContext.PersonelIzins.Where(x => x.FirmaId == FirmaId && x.PersonelId == PersonelId && ((x.IzinBaslama >= ilktarih) && (x.IzinBitis <= sontarih))&& x.Sil == false && x.Aktif == true).ToList();
            else if (Sube != 0 && PersonelId == 0)
            {
                pizin= dbContext.PersonelIzins.Where(x => x.FirmaId == FirmaId && ((x.IzinBaslama >= ilktarih) && (x.IzinBitis <= sontarih)) && x.Sil == false && x.Aktif == true).ToList();
                foreach (var item in pizin)
                {
                    gorenvlisubeler = item.GorevliSubeler.Split(',');
                    foreach (var s in gorenvlisubeler)
                    {
                        if (Convert.ToInt64(s) == Sube)
                        {
                            sb = Convert.ToInt64(s);
                            personelizin.Add(item);
                        }
                    }
                }
            }
            else if (Sube != 0 && PersonelId != 0)
            {
                pizin = dbContext.PersonelIzins.Where(x => x.FirmaId == FirmaId && x.PersonelId == PersonelId && ((x.IzinBaslama >= ilktarih) && (x.IzinBitis <= sontarih)) && x.Sil == false && x.Aktif == true).ToList();
                foreach (var item in pizin)
                {
                    gorenvlisubeler = item.GorevliSubeler.Split(',');
                    foreach (var s in gorenvlisubeler)
                    {
                        if (Convert.ToInt64(s) == Sube)
                        {
                            sb = Convert.ToInt64(s);
                            personelizin.Add(item);
                        }
                    }
                }
            }
            var izinler = personelizin.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                SubeAdi = m.Sube.SubeAdi,
                GorevliSubeler = m.GorevliSubeler,
                AdSoyad = m.Personel.AdiSoyadi,
                Gorev = m.Personel.PersonelGorevleri.Gorev,
                IzinBaslama = m.IzinBaslama,
                IzinBitis = m.IzinBitis,
                IseBaslama = m.IseBaslama,
                Gun = m.KullanilanIzinGun,
                KalanIzin = m.Personel.KalanIzin,
                Kilit = m.Personel.KilitBit,
                Aktif = m.Personel.Aktif,
                Aciklama = m.Aciklama
            }).ToList().Distinct(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = izinler }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PersonelIzinEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
           
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            string IzinBaslangic = Request["IzinBaslangic"];
            string IzinBitis = Request["IzinBitis"];
            string IseBaslama = Request["IseBaslama"];
            string Aciklama = Request["Aciklama"];
            string aciklama = Aciklama;
            aciklama = aciklama.Replace("\r\n", "<br />");
            aciklama = aciklama.Replace("\n", "<br />");
            DateTime izinbaslangic = Convert.ToDateTime(IzinBaslangic);
            DateTime izinbitis = Convert.ToDateTime(IzinBitis);
            DateTime isebaslama = Convert.ToDateTime(IseBaslama);
            Personel per = dbContext.Personels.FirstOrDefault(x => x.Id == PersonelId && x.Aktif == true && x.Sil == false);
            PersonelIzin izinvarmi = dbContext.PersonelIzins.FirstOrDefault(x => x.PersonelId == PersonelId && x.IzinBaslama == izinbaslangic && x.IzinBitis == izinbitis && x.Aktif == true && x.Sil == false);
            TimeSpan IzinGunSayisi;
            AyarlarGenel genelayar = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId);

            IzinGunSayisi = izinbitis - izinbaslangic;
            int Gun = Convert.ToInt32(IzinGunSayisi.Days) + 1;
            int KalanIzın = Convert.ToInt32(per.KalanIzin);
            var CumartesiAdet = 0;
            var PazarAdet = 0;
            if (!genelayar.CalismaGunuCumartesi) // Cumartesileri izinden düş
            {
                var tarihListe = Enumerable.Range(0, Gun).Select(e => izinbaslangic.AddDays(e));
                CumartesiAdet = tarihListe.Where(e => e.DayOfWeek == DayOfWeek.Saturday).Count();
            }
            if (!genelayar.CalismaGunuPazar) // Cumartesileri izinden düş
            {
                var tarihListe = Enumerable.Range(0, Gun).Select(e => izinbaslangic.AddDays(e));
                PazarAdet = tarihListe.Where(e => e.DayOfWeek == DayOfWeek.Sunday).Count();
            }
            Gun = Gun - CumartesiAdet - PazarAdet;
            if (izinvarmi != null)
            {
                izinvarmi.Sil = false;
                izinvarmi.Aktif = true;
                izinvarmi.DegistirenKullaniciId = KullaniciId;
                izinvarmi.DegistirmeTarih = DateTime.Now;
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
                    hata.Islem = "Personel İzin Ekle, Silinmiş kaydı aktif yap, Kayıt Id: " + izinvarmi.Id.ToString();
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
                if (KalanIzın < Gun)
                {
                    return Json(new { Sonuc = false, Mesaj = "Personelin Yeterli İzni Yok" }, JsonRequestBehavior.AllowGet);
                }

                PersonelIzin izin = new PersonelIzin();
                izin.FirmaId = per.FirmaId; // Seçilen personelin Firma Id si
                izin.SubeId = per.SubeId; // Seçilen personelin Sube Id si
                izin.PersonelId = PersonelId;
                izin.IzinBaslama = izinbaslangic;
                izin.IzinBitis = izinbitis;
                izin.IseBaslama = isebaslama;
                izin.Aciklama = aciklama;
                izin.GorevliSubeler = per.GorevliSubeler;
                izin.KullanilanIzinGun = Gun;
                izin.OlusturmaTarih = DateTime.Now;
                izin.OlusturanKullaniciId = KullaniciId;
                izin.DegistirmeTarih = DateTime.Now;
                izin.DegistirenKullaniciId = KullaniciId;
                izin.Aktif = true;
                izin.Sil = false;
                try
                {
                    dbContext.PersonelIzins.Add(izin);
                    //dbContext.SaveChanges();
                    per.KalanIzin = KalanIzın - Gun;
                    dbContext.SaveChanges();
                    // Personel tablosundanki izin süresinden gün düşülecek
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Personel İzin Ekle";
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

            #region Tatil hesabı sonra yapılacak
            // İzinli gün sayısı hesaplama -- AyarlarGenel tablosundan çalışılan günleri çekerek ona göre hesaplama yapılıyor.
            //AyarlarGenel ayar = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId);
            //DateTime gecici = izinbaslangic;
            //int izinsuresi = 0;
            //int gunsayisi = 0;
            //string gun = string.Empty;
            //#region Haftasonu İzin Hesaplama
            //// Haftasonlarının izinden düşürlmesi
            //while (gecici <= izinbitis)
            //{
            //    gun = gecici.ToString("dddd");
            //    if (ayar.CalimaGunuCumartesi == true && ayar.CalimaGunuPazar == true) // Cumartesi ve Pazar Çalışma günü, izin süresine dahil edilir.
            //    {
            //        gunsayisi++;
            //    }
            //    else if (ayar.CalimaGunuCumartesi == true && ayar.CalimaGunuPazar == false) // Cumartesi çalışma günü, pazar tatil günü, Cumartesi izin süresine dahil edilir, Pazar günü izin süresine dahil edilmez.
            //    {
            //        if (gun != "Pazar")
            //            gunsayisi++;
            //    }
            //    else if (ayar.CalimaGunuCumartesi == false && ayar.CalimaGunuPazar == true) // Cumartesi tatil günü, pazar çalışma günü, Cumartesi izin süresine dahil edilmez, Pazar günü izin süresine dahil edilir.
            //    {
            //        if (gun != "Cumartesi")
            //            gunsayisi++;
            //    }
            //    else if (ayar.CalimaGunuCumartesi == false && ayar.CalimaGunuPazar == false) // Cumartesi ve Pazar tatil günü, izin süresine dahiledilmez.
            //    {
            //        if (gun != "Cumartesi" && gun != "Pazar")
            //            gunsayisi++;
            //    }
            //    gecici = gecici.AddDays(1);
            //}
            //#endregion
            //// İzin süresinden haftasonları düşüldükten sonra tatil günlerine bakılıyor. İzinden düşülecek tatil günleri toplanıyor.
            //#region İzinden Düşülecek Tatil Günleri
            //List<TatilGunleri> tatiller = dbContext.TatilGunleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false && x.IzindenDus == false).ToList();

            //int izindendus = 0;
            //if (tatiller != null) // tatil günleri içinde izinden düşülecek günler varsa
            //{
            //    for (int i = 0; i < tatiller.Count; i++)
            //    {
            //        DateTime tt = tatiller[i].Baslangic;
            //        DateTime tb = tatiller[i].Bitis;

            //    }
            //    foreach (var item in tatiller) // tatil günleri içerisinde gezilecek ve her tatil günü listeye eklenecek
            //    {
            //        DateTime tt = item.Baslangic;
            //        DateTime tb = item.Bitis;
            //        while (tt <= tb)
            //        {
            //            tt.AddDays(1);
            //            gun = tt.ToString("dddd");
            //            if (gun != "Cumartesi" && gun != "Pazar") // tatil günü hafta sonu dışında ise izinden düşülecek
            //            {
            //                izindendus++;
            //            }
            //        }
            //        //if (izinbaslangic <= tt && izinbitis >= tt) // tatil günü (tt) izin süresi içinde olmalı
            //        //{

            //        //}
            //    }
            //}
            //#endregion
            //// İzinli gün sayısı hesaplama
            //izinsuresi = gunsayisi - izindendus;
            #endregion

        }
        [HttpPost]
        public ActionResult PersonelIzinGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string IzinBaslangic = Request["IzinBaslangic"];
            string IzinBitis = Request["IzinBitis"];
            string IseBaslama = Request["IseBaslama"];
            string Aciklama = Request["Aciklama"];
            string aciklama = Aciklama;
            aciklama = aciklama.Replace("\r\n", "<br />");
            aciklama = aciklama.Replace("\n", "<br />");
            DateTime izinbaslangic = Convert.ToDateTime(IzinBaslangic);
            DateTime izinbitis = Convert.ToDateTime(IzinBitis);
            DateTime isebaslama = Convert.ToDateTime(IseBaslama);
            PersonelIzin izin = dbContext.PersonelIzins.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

            Personel per = dbContext.Personels.FirstOrDefault(x => x.Id == izin.PersonelId && x.Aktif == true && x.Sil == false);
            TimeSpan IzinGunSayisi;
            AyarlarGenel genelayar = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId);

            IzinGunSayisi = izinbitis - izinbaslangic;
            int Gun = Convert.ToInt32(IzinGunSayisi.Days) + 1;
            int? eskikullanilanizin = izin.KullanilanIzinGun;
            int KalanIzin = Convert.ToInt32(per.KalanIzin);
            int? farkizin = 0;
            var CumartesiAdet = 0;
            var PazarAdet = 0;
            if (!genelayar.CalismaGunuCumartesi) // Cumartesileri izinden düş
            {
                var tarihListe = Enumerable.Range(0, Gun).Select(e => izinbaslangic.AddDays(e));
                CumartesiAdet = tarihListe.Where(e => e.DayOfWeek == DayOfWeek.Saturday).Count();
            }
            if (!genelayar.CalismaGunuPazar) // Cumartesileri izinden düş
            {
                var tarihListe = Enumerable.Range(0, Gun).Select(e => izinbaslangic.AddDays(e));
                PazarAdet = tarihListe.Where(e => e.DayOfWeek == DayOfWeek.Sunday).Count();
            }
            Gun = Gun - CumartesiAdet - PazarAdet;
            farkizin = eskikullanilanizin - Gun;
            if (izin == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            //izin.PersonelId = PersonelId;
            izin.KullanilanIzinGun = Gun;
            izin.IzinBaslama = izinbaslangic;
            izin.IzinBitis = izinbitis;
            izin.IseBaslama = isebaslama;
            izin.Aciklama = aciklama;
            izin.DegistirmeTarih = DateTime.Now;
            izin.DegistirenKullaniciId = KullaniciId;
            per.KalanIzin = KalanIzin + farkizin;
            per.DegistirmeTarih = DateTime.Now;
            per.DegistirenKullaniciId = KullaniciId;
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
                hata.Islem = "Personel İzin Güncelle, Kayıt Id: " + id.ToString();
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
        public ActionResult PersonelIzinSil(long? id)
        {
            // Personelin kullanıldığı tablolarda kayıtlar var ise personel silinemeyecek
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            PersonelIzin izin = dbContext.PersonelIzins.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (izin == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == izin.PersonelId && x.Sil == false && x.Aktif == true);
            int? kalanizin = personel.KalanIzin;

            izin.Aktif = false;
            izin.Sil = true;
            izin.DegistirenKullaniciId = KullaniciId;
            izin.DegistirmeTarih = DateTime.Now;
            personel.KalanIzin = kalanizin + izin.KullanilanIzinGun; // Silinen gün kalan izine ekleniyor.
            personel.DegistirenKullaniciId = KullaniciId;
            personel.DegistirmeTarih = DateTime.Now;

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
                hata.Islem = "Personel İzin Sil, Kayıt Id: " + id;
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
        public string IzinVarMi()
        {
            long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            string IzinBaslangic = Request["IzinBaslangic"];
            string IzinBitis = Request["IzinBitis"];
            string IseBaslama = Request["IseBaslama"];
            string Aciklama = Request["Aciklama"];

            DateTime izinbaslangic = Convert.ToDateTime(IzinBaslangic);
            DateTime izinbitis = Convert.ToDateTime(IzinBitis);
            DateTime isebaslama = Convert.ToDateTime(IseBaslama);
            PersonelIzin izin = dbContext.PersonelIzins.FirstOrDefault(x => x.PersonelId == PersonelId && x.IzinBaslama == izinbaslangic && x.IzinBitis == izinbitis && x.Aktif == true && x.Sil == false);

            if (izin != null)
                return izin.Id.ToString();
            else
                return "Yok";
        }
        [HttpPost]
        public ActionResult IzinHakkiEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            int IzinHakkiSuresi = Convert.ToInt32(Request["IzinHakkiSuresi"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            Personel personel = dbContext.Personels.FirstOrDefault(x => x.Id == PersonelId && x.Aktif == true && x.Sil == false);
            int? kalanizin = 0;
            if (personel.KalanIzin == null)
                kalanizin = 0;
            else
                kalanizin = personel.KalanIzin;
            int? toplamizin = 0;
            if (personel.ToplamIzin == null)
                toplamizin = 0;
            else
                toplamizin = personel.ToplamIzin;

            toplamizin = toplamizin + IzinHakkiSuresi;
            kalanizin = kalanizin + IzinHakkiSuresi;
            personel.ToplamIzin = toplamizin;
            personel.KalanIzin = kalanizin;
            personel.DegistirenKullaniciId = KullaniciId;
            personel.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Personel İzin Süresi Ekle, Personel Id: " + PersonelId;
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
        public ActionResult PersonelOdemeleri()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Personeller";
            ViewBag.AltMenu = "Personel Ödemeleri";

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            ViewBag.AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 69 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            List<SubeToPersonel> subepersonel = dbContext.SubeToPersonels.Where(x => x.SubeId == SubeId).ToList();
            List<Personel> pers = new List<Personel>();
            foreach (var item in subepersonel)
            {
                Personel p = dbContext.Personels.FirstOrDefault(x => x.Id == item.PersonelId && x.Aktif == true && x.Sil == false);
                if (p != null)
                {
                    pers.Add(dbContext.Personels.FirstOrDefault(x => x.Id == item.PersonelId && x.Aktif == true && x.Sil == false));
                }
            }
            //List<Personel> pers = dbContext.Personels.Where(x => x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).PersonelOdemeleri;
            ViewBag.FiltreAyar = filtreayar;
            ViewBag.personeller = pers;
            ViewBag.SubeListesi = sube;
            ViewBag.SubeKullanici = subekullanici;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 69 && x.Aktif == true && x.Sil == false);
            return View();
        }
        public ActionResult PersonelOdemeTakibiListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);

            //DateTime IlkTarih = DateTime.Today;
            //DateTime SonTarih = DateTime.Today;
            //var songun = SonTarih.AddMonths(1).AddDays(SonTarih.AddMonths(1).Day).Date;
            //IlkTarih = new DateTime(DateTime.Now.Year, 1, 1);
            //SonTarih = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).PersonelOdemeleri;
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
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<PersonelOdeme> personel = dbContext.PersonelOdemes.Where(x => x.FirmaId == FirmaId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            var perlist = personel.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                Sube = m.Sube.SubeAdi,
                AdSoyad = m.Personel.AdiSoyadi,
                Gorev = m.Personel.PersonelGorevleri.Gorev,
                CalismaSekli = m.Personel.CalismaSekli,
                Aktif = m.Personel.Aktif,
                OdemeTarihi = m.OdemeTarihi,
                OdemeTuru = m.OdemeTuru,
                Tutar = m.Tutar,
                OdemeSekli=m.OdemeSekli,
                Aciklama = m.Aciklama,
                Kilit = m.Personel.KilitBit,
                Subeler = m.Personel.GorevliSubeler
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = perlist }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonelOdemeTakibiListesiFiltre(string IlkTarih, string SonTarih, int Sube, int OdemeTuruFiltre, int PersonelId)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            List<PersonelOdeme> personel = null;
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            string tur = "";
            if (OdemeTuruFiltre == 0)
                tur = "";
            else if (OdemeTuruFiltre == 1)
                tur = "Maaş Ödemesi";
            else if (OdemeTuruFiltre == 2)
                tur = "Avans";
            else if (OdemeTuruFiltre == 3)
                tur = "Prim";

            if (Sube == 0 && OdemeTuruFiltre == 0 && PersonelId == 0)
                personel = dbContext.PersonelOdemes.Where(x => x.FirmaId == FirmaId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && OdemeTuruFiltre == 0 && PersonelId != 0)
                personel = dbContext.PersonelOdemes.Where(x => x.FirmaId == FirmaId && x.PersonelId == PersonelId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && OdemeTuruFiltre != 0 && PersonelId == 0)
                personel = dbContext.PersonelOdemes.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == tur && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && OdemeTuruFiltre != 0 && PersonelId != 0)
                personel = dbContext.PersonelOdemes.Where(x => x.FirmaId == FirmaId && x.PersonelId == PersonelId && x.OdemeTuru == tur && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && OdemeTuruFiltre == 0 && PersonelId == 0)
                personel = dbContext.PersonelOdemes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && OdemeTuruFiltre == 0 && PersonelId != 0)
                personel = dbContext.PersonelOdemes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.PersonelId == PersonelId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && OdemeTuruFiltre != 0 && PersonelId == 0)
                personel = dbContext.PersonelOdemes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.OdemeTuru == tur && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && OdemeTuruFiltre != 0 && PersonelId != 0)
                personel = dbContext.PersonelOdemes.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.PersonelId == PersonelId && x.OdemeTuru == tur && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();

            var perlist = personel.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                Sube = m.Sube.SubeAdi,
                AdSoyad = m.Personel.AdiSoyadi,
                Gorev = m.Personel.PersonelGorevleri.Gorev,
                CalismaSekli = m.Personel.CalismaSekli,
                Aktif = m.Personel.Aktif,
                OdemeTarihi = m.OdemeTarihi,
                OdemeTuru = m.OdemeTuru,
                OdemeSekli = m.OdemeSekli,
                Tutar = m.Tutar,
                Aciklama = m.Aciklama,
                Kilit = m.Personel.KilitBit,
                Subeler = m.Personel.GorevliSubeler
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = perlist }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonelOdemeEkle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long PersonelId = Convert.ToInt64(Request["PersonelId"]);
            string OdemeTarihi = Request["OdemeTarihi"];
            DateTime odemetarih = Convert.ToDateTime(OdemeTarihi);
            string OdemeTuru = Request["OdemeTuru"];
            string OdemeSekli = Request["OdemeSekli"];
            decimal Ucret = Convert.ToDecimal(Request["Ucret"]);
            string Aciklama = Request["Aciklama"];

            PersonelOdeme personelOdeme = new PersonelOdeme();
            personelOdeme.FirmaId = FirmaId;
            personelOdeme.SubeId = SubeId;
            personelOdeme.PersonelId = PersonelId;
            personelOdeme.OdemeTarihi = odemetarih;
            personelOdeme.OdemeTuru = OdemeTuru; // Maaş Ödemesi - Avans - Prim
            personelOdeme.OdemeSekli = OdemeSekli; // Nakit-BankaHavaleri-KrediKartı
            personelOdeme.Tutar = Ucret;
            personelOdeme.Aciklama = Aciklama;
            personelOdeme.Aktif = true;
            personelOdeme.Sil = false;
            personelOdeme.OlusturanKullaniciId = KullaniciId;
            personelOdeme.OlusturmaTarih = DateTime.Now;
            personelOdeme.DegistirenKullaniciId = KullaniciId;
            personelOdeme.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.PersonelOdemes.Add(personelOdeme);
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Personel Ödeme Ekle";
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
        public ActionResult PersonelOdemeSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            PersonelOdeme odeme = dbContext.PersonelOdemes.FirstOrDefault(x => x.Id == id && x.FirmaId==FirmaId && x.Aktif == true && x.Sil == false);
            odeme.Sil = true;
            odeme.Aktif = false;
            odeme.DegistirenKullaniciId = KullaniciId;
            odeme.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Personel Ödemesi Silme, Ödeme Id: " + id;
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
        public ActionResult PersonelOdemeGuncelle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string Id = Request["Id"];
            long OdemeId = Convert.ToInt64(Id);
            string OdemeTarihi = Request["OdemeTarihi"];
            DateTime odemetarih = Convert.ToDateTime(OdemeTarihi);
            string OdemeTuru = Request["OdemeTuru"];
            string OdemeSekli = Request["OdemeSekli"];
            decimal Ucret = Convert.ToDecimal(Request["Ucret"]);
            string Aciklama = Request["Aciklama"];

            PersonelOdeme personelOdeme = dbContext.PersonelOdemes.FirstOrDefault(x => x.Id == OdemeId && x.Aktif == true && x.Sil == false);
            personelOdeme.FirmaId = FirmaId;
            personelOdeme.SubeId = SubeId;
            personelOdeme.OdemeTarihi = odemetarih;
            personelOdeme.OdemeTuru = OdemeTuru;
            personelOdeme.OdemeSekli = OdemeSekli;
            personelOdeme.Tutar = Ucret;
            personelOdeme.Aciklama = Aciklama;
            personelOdeme.Aktif = true;
            personelOdeme.Sil = false;
            personelOdeme.DegistirenKullaniciId = KullaniciId;
            personelOdeme.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Personel Ödeme Ekle";
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
        public ActionResult SubePersonelGetir(long? id)
        {
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            if (id > 0)
            {
                List<SubeToPersonel> subepersonel = dbContext.SubeToPersonels.Where(x => x.SubeId == id).ToList();
                List<Personel> pers = new List<Personel>();
                foreach (var item in subepersonel)
                {
                    pers.Add(dbContext.Personels.FirstOrDefault(x => x.Id == item.PersonelId && x.Aktif == true && x.Sil == false));
                }
                var perlist = pers.Select(m => new
                {
                    Id = m.Id,
                    //Firma = m.Firma.FirmaAdi,
                    //Sube = m.Sube.SubeAdi,
                    AdiSoyadi = m.AdiSoyadi
                }).ToList(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
                return Json(new { Sonuc = true, data = perlist }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //List<Sube> subeler = new List<Sube>();

                List<SubeToPersonel> subepersonel = new List<SubeToPersonel>();
                List<SubeToKullanici> kullanicisube = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList(); // Aktif kullanıcının yetkili olduğu şubeler
                List<Personel> pers = new List<Personel>();
                foreach (var item in kullanicisube) // Şubeler içinde dön, o anki şubedeki personelleri personel tablosuna ekle
                {
                    //List<SubeToPersonel> subepersonel1 = dbContext.SubeToPersonels.Where(x=>x.SubeId==item.SubeId).ToList();
                    subepersonel.AddRange(dbContext.SubeToPersonels.Where(x => x.SubeId == item.SubeId).ToList());
                }
                foreach (var item in subepersonel)
                {
                    pers.Add(dbContext.Personels.FirstOrDefault(x => x.Id == item.PersonelId && x.Aktif == true && x.Sil == false));
                }

                var perlist = pers.Select(m => new
                {
                    Id = m.Id,
                    //Firma = m.Firma.FirmaAdi,
                    //Sube = m.Sube.SubeAdi,
                    AdiSoyadi = m.AdiSoyadi
                }).Distinct(); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
                return Json(new { Sonuc = true, data = perlist }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PersonelIsTakibi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Personeller";
            ViewBag.AltMenu = "Personel İş Takibi";

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            ViewBag.AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 69 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            List<SubeToPersonel> subepersonel = dbContext.SubeToPersonels.Where(x => x.SubeId == SubeId).ToList();
            List<RezervasyonTurleri> rezervasyonTurleris = dbContext.RezervasyonTurleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<Personel> pers = new List<Personel>();
            foreach (var item in subepersonel)
            {
                Personel p = dbContext.Personels.FirstOrDefault(x => x.Id == item.PersonelId && x.Aktif == true && x.Sil == false);
                if (p != null)
                {
                    pers.Add(dbContext.Personels.FirstOrDefault(x => x.Id == item.PersonelId && x.Aktif == true && x.Sil == false));
                }
            }
            //List<Personel> pers = dbContext.Personels.Where(x => x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).PersonelIsTakibi;
            ViewBag.FiltreAyar = filtreayar;
            ViewBag.personeller = pers;
            ViewBag.SubeListesi = sube;
            ViewBag.SubeKullanici = subekullanici;
            ViewBag.RezervasyonTurleri = rezervasyonTurleris;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 69 && x.Aktif == true && x.Sil == false);
            return View();
        }

        public ActionResult PersonelRandevularFiltre(string IlkTarih, string SonTarih, int Sube, int RezervasyonTurId, int PersonelId)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<Randevu> randevus = null;
            List<Randevu> personelrandevus = new List<Randevu>();
            List<RandevuToPersonel> randevupersonel = new List<RandevuToPersonel>();

            if (Sube == 0 && RezervasyonTurId == 0 && PersonelId == 0)
                personelrandevus = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && ((x.Baslangic >= ilktarih) && (x.Bitis <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && RezervasyonTurId == 0 && PersonelId != 0)
            {
                randevus = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && ((x.Baslangic >= ilktarih) && (x.Bitis <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
                foreach (var item in randevus)
                {
                    string[] vs = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in vs)
                    {
                        if (Convert.ToInt64(persId) == PersonelId)
                        {
                            personelrandevus.Add(item);
                        }
                    }
                }
            }
            else if (Sube == 0 && RezervasyonTurId != 0 && PersonelId == 0)
                personelrandevus = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.RezervasyonTurId == RezervasyonTurId && ((x.Baslangic >= ilktarih) && (x.Bitis <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && RezervasyonTurId != 0 && PersonelId != 0)
            {
                randevus = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.RezervasyonTurId == RezervasyonTurId && ((x.Baslangic >= ilktarih) && (x.Bitis <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
                foreach (var item in randevus)
                {
                    string[] vs = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in vs)
                    {
                        if (Convert.ToInt64(persId) == PersonelId)
                        {
                            personelrandevus.Add(item);
                        }
                    }
                }
            }
            else if (Sube != 0 && RezervasyonTurId == 0 && PersonelId == 0)
                personelrandevus = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.Baslangic >= ilktarih) && (x.Bitis <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && RezervasyonTurId == 0 && PersonelId != 0)
            {
                randevus = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.Baslangic >= ilktarih) && (x.Bitis <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
                foreach (var item in randevus)
                {
                    string[] vs = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in vs)
                    {
                        if (Convert.ToInt64(persId) == PersonelId)
                        {
                            personelrandevus.Add(item);
                        }
                    }
                }
            }
            else if (Sube != 0 && RezervasyonTurId != 0 && PersonelId == 0)
                personelrandevus = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.RezervasyonTurId == RezervasyonTurId && ((x.Baslangic >= ilktarih) && (x.Bitis <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && RezervasyonTurId != 0 && PersonelId != 0)
            {
                randevus = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.RezervasyonTurId == RezervasyonTurId && ((x.Baslangic >= ilktarih) && (x.Bitis <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
                foreach (var item in randevus)
                {
                    string[] vs = item.GorevliPersonellerId.Split(',');
                    foreach (var persId in vs)
                    {
                        if (Convert.ToInt64(persId) == PersonelId)
                        {
                            personelrandevus.Add(item);
                        }
                    }
                }
            }
            foreach (var item in personelrandevus)
            {
                List<RandevuToPersonel> pers = dbContext.RandevuToPersonels.Where(x => x.RandevuId == item.Id).ToList();

                foreach (var randevu in pers)
                {
                    randevupersonel.Add(randevu);
                }
            }
            var randevular = personelrandevus.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                Sube = m.Sube.SubeAdi,
                SozlesmeNo = m.Sozlesme.SozlesmeNo,
                RandevuTuru = m.Baslik,
                Aciklama = m.Aciklama,
                Tarih=m.Baslangic,
                Baslangic = m.Baslangic.ToShortTimeString(),
                Bitis = m.Bitis.ToShortTimeString(),
                Iptal = m.Iptal,
                Aktif = m.Aktif,
                PersonelIdler = m.GorevliPersonellerId,
                Personeller = randevupersonel.Where(x => x.RandevuId == m.Id).Select(p => new
                {
                    RandevuId = p.RandevuId,
                    PersonelId = p.PersonelId,
                    PersonelAdSoyad = p.Personel.AdiSoyadi

                })
            }).ToList();
            return Json(new { Sonuc = true, data = randevular }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult PersonelRandevular()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).PersonelIzinTakibi;
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
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<RandevuToPersonel> randevupersonel = new List<RandevuToPersonel>();
            List<Randevu> randevus = new List<Randevu>();

            if (SubeId == 0)
                randevus = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && ((x.Baslangic >= ilktarih) && (x.Bitis <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else
                randevus = dbContext.Randevus.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && ((x.Baslangic >= ilktarih) && (x.Bitis <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();


            foreach (var item in randevus)
            {
                List<RandevuToPersonel> pers = dbContext.RandevuToPersonels.Where(x => x.RandevuId == item.Id).ToList();

                foreach (var randevu in pers)
                {
                    randevupersonel.Add(randevu);
                }
            }
            var randevular = randevus.Select(m => new
            {
                Id = m.Id,
                Firma = m.Firma.FirmaAdi,
                Sube = m.Sube.SubeAdi,
                SozlesmeNo = m.Sozlesme.SozlesmeNo,
                Bitti= m.Sozlesme.Bitti,
                RandevuTuru = m.Baslik,
                Aciklama = m.Aciklama,
                Tarih = m.Baslangic,
                Baslangic = m.Baslangic.ToShortTimeString(),
                Bitis = m.Bitis.ToShortTimeString(),
                Iptal = m.Iptal,
                Aktif = m.Aktif,
                PersonelIdler = m.GorevliPersonellerId,
                Personeller = randevupersonel.Where(x => x.RandevuId == m.Id).Select(p => new
                {
                    RandevuId = p.RandevuId,
                    PersonelId = p.PersonelId,
                    PersonelAdSoyad = p.Personel.AdiSoyadi

                })
            }).ToList();
            return Json(new { Sonuc = true, data = randevular }, JsonRequestBehavior.AllowGet);
        }
    }
}