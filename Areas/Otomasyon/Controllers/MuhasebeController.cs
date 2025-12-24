using Rotativa;
using FotografciTakipWeb.App_Settings;
using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class MuhasebeController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        // GET: Otomasyon/Muhasebe
        #region Günlük İşler
        public ActionResult GunlukIsler()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Muhasebe";
            ViewBag.AltMenu = "Günlük İşler";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            //List<VW_Kullanici_Gorevler_Resim> kullanicilar = dbContext.VW_Kullanici_Gorevler_Resim.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            //ViewBag.kullanicilar = kullanicilar;
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 26 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).GunlukIsler;
            ViewBag.FiltreAyar = filtreayar;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 26 && x.Aktif == true && x.Sil == false);
            ViewBag.kullaniciId = KullaniciId;
            ViewBag.GunlukIsKategoriler = dbContext.GunlukIsKategoris.Where(x => x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId).ToList();
            ViewBag.Musteriler = dbContext.Musteris.Where(x => x.Aktif == true && x.Sil == false && x.FirmaId == FirmaId).ToList();
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            ViewBag.SubeListesi = sube;
            ViewBag.SubeKullanici = subekullanici;
            return View();
        }
        public ActionResult GunlukIsFiltre(string IlkTarih, string SonTarih, int Sube, int Kategori)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<GunlukIsler> gis = null;

            if (Sube == 0 && Kategori == 0)
                gis = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && Kategori != 0)
                gis = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && x.KategoriId == Kategori && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && Kategori == 0)
                gis = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && Kategori != 0)
                gis = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.KategoriId == Kategori && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();

            var gunlukislist = gis.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                SubeAdi = m.Sube.SubeAdi,
                TakipNo = m.TakipNo,
                Tarih = m.Tarih.ToShortDateString(),
                KategoriId = m.KategoriId,
                Kategori = m.GunlukIsKategori.KategoriAdi,
                MusteriId = m.MusteriId,
                AdSoyad = m.AdSoyad,
                TcKimlik = m.TCKimlikNo,
                SabitTel = m.SabitTel,
                CepTel = m.CepTel,
                Email = m.Email,
                Adres = m.Adres,
                Adet = m.Adet,
                BirimUcret = m.BirimUcret,
                Ucret = m.Ucret,
                Notlar = m.Notlar,
                Aciklama = m.Notlar,
                OdemeSekli = m.OdemeTuru,
                Iptal = m.Iptal,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            });
            return Json(new { data = gunlukislist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GunlukIsEkle()
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
            DateTime Tarih = Convert.ToDateTime(Request["Tarih"]);
            long KategoriId = Convert.ToInt64(Request["KategoriId"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            string AdSoyad = Request["AdSoyad"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string cep = Request["CepTel"];
            TelefonDüzelt td = new TelefonDüzelt();
            if (!string.IsNullOrEmpty(cep)) { cep = td.düzelt(cep); }
            string Email = Request["Email"];
            string Adres = Request["Adres"];
            decimal Ucret = 0;
            int Adet = Convert.ToInt32(Request["Adet"]);
            decimal BirimUcret = Convert.ToDecimal(Request["BirimUcret"]);
            string Notlar = Request["Notlar"];
            string OdemeSekli = Request["OdemeSekli"];
            Notlar = Notlar.Replace("\r\n", "<br />");
            Notlar = Notlar.Replace("\n", "<br />");
            string AnaSayfa = Request["AnaSayfa"];
            long takipno = 0;
            string tno;
            bool mailsonuc;
            string smssonuc = "";
            string makbuzno = "";
            List<Models.GunlukIsler> gunluk = dbContext.GunlukIslers.Select(x => x).ToList();
            if (gunluk.Count > 0)
            {
                takipno = dbContext.GunlukIslers.Max(x => x.Id);
            }
            takipno = takipno + 1;
            tno = FirmaId.ToString() + "0" + takipno.ToString();
            Ucret = BirimUcret * Adet;
            Tarih = Tarih.AddHours(DateTime.Now.Hour);
            Tarih = Tarih.AddMinutes(DateTime.Now.Minute);
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
            GunlukIsler Gis = new GunlukIsler();
            Gis.TakipNo = Convert.ToInt64(tno);
            Gis.FirmaId = FirmaId;
            Gis.SubeId = SubeId;
            Gis.Tarih = Tarih;
            Gis.KategoriId = KategoriId;
            Gis.MusteriId = MusteriId;
            Gis.AdSoyad = AdSoyad;
            Gis.TCKimlikNo = TCKimlikNo;
            Gis.CepTel = cep;
            Gis.Adres = Adres;
            Gis.Email = Email;
            Gis.Adet = Adet;
            Gis.BirimUcret = BirimUcret;
            Gis.Ucret = Ucret;
            Gis.Notlar = Notlar;
            Gis.OdemeTuru = OdemeSekli;
            Gis.Odenen = 0;
            Gis.KalanBakiye = 0;
            Gis.OlusturanKullaniciId = KullaniciId;
            Gis.OlusturmaTarih = DateTime.Now;
            Gis.DegistirenKullaniciId = KullaniciId;
            Gis.DegistirmeTarih = DateTime.Now;
            Gis.Aktif = true;
            Gis.Sil = false;
            try
            {
                dbContext.GunlukIslers.Add(Gis);
                dbContext.SaveChanges();
                string MusteriAdSoyad = AdSoyad;
                Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                bool smskabul = true, emailkabul = true;
                if (musteri == null)
                {
                    smskabul = true; emailkabul = true;
                }
                else
                {
                    smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul; MusteriAdSoyad = musteri.AdiSoyadi;
                }
                makbuzno = dbContext.GelirGiders.FirstOrDefault(x => x.FirmaId == FirmaId && x.GisId == Gis.Id).MakbuzNo.ToString();
                #region Sms Bilgi Mesajı
                if (AdSoyad != "-- Müşterisiz İşlem --" && cep != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
                {
                    AyarlarSmsGonderim smsayar = null;
                    string smsmetin = "";

                    smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.GunlukIsOdemeBilgiGonderimSuresi == 6 && x.GunlukIsOdemeBilgiMesaji != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (smsayar != null)
                        smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.GunlukIsOdemeBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                    else
                        smsmetin = null;
                    if (smsayar != null && smsmetin != null)
                    {
                        if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", MusteriAdSoyad);
                        if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            smsmetin = smsmetin.Replace("{AdSoyad}", MusteriAdSoyad);
                        if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                            smsmetin = smsmetin.Replace("{FirmaAdi}", frm.FirmaAdi);
                        if (smsmetin.IndexOf("{Tarih}") != -1)
                            smsmetin = smsmetin.Replace("{Tarih}", Tarih.ToShortDateString());
                        if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{OdemeTarihi}", Tarih.ToShortDateString());
                        if (smsmetin.IndexOf("{Tutar}") != -1)
                            smsmetin = smsmetin.Replace("{Tutar}", Ucret.ToString());
                        if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{GelinAdSoyad}", "");
                        if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{DamatAdSoyad}", "");
                        if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{RandevuTarihi}", "");
                        if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{OpsiyonTarihi}", "");
                        if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                            smsmetin = smsmetin.Replace("{SozlesmeNo}", makbuzno);
                        if (smsmetin.IndexOf("{RezervasyonTuru}") != -1)
                            smsmetin = smsmetin.Replace("{RezervasyonTuru}", "Günlük İş");
                        if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                            smsmetin = smsmetin.Replace("{MakbuzNo}", makbuzno);
                        if (smsmetin.IndexOf("{CekimYeri}") != -1)
                            smsmetin = smsmetin.Replace("{CekimYeri}", "");
                        if (smsmetin.IndexOf("{CekimSaati}") != -1)
                            smsmetin = smsmetin.Replace("{CekimSaati}", "");
                        smssonuc = SMSGonder.Gonder_AtakSms(smsmetin, cep, FirmaId, SubeId, KullaniciId);
                    }
                }
                #endregion

                #region Email Bilgi Maili
                if (AdSoyad != "-- Müşterisiz İşlem --" && Email != "" && emailkabul) // Müşteri bir cep telefonu girilmiş ise SMS gönder
                {
                    AyarlarMailGonderim mailayar = null;
                    MailMetinleri mmetin = null;
                    string mailmetin = "";
                    mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.GunlukIsOdemeBilgiGonderimSuresi == 6 && x.GunlukIsOdemeBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (mailayar == null)
                        return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                    else
                        mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.GunlukIsOdemeBilgiMaili && x.Aktif == true && x.Sil == false);

                    if (mailayar != null && mailmetin != null)
                    {
                        mailmetin = mmetin.MailMetni;
                        if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", MusteriAdSoyad);
                        if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            mailmetin = mailmetin.Replace("{AdSoyad}", MusteriAdSoyad);
                        if (mailmetin.IndexOf("{FirmaAdi}") != -1)
                            mailmetin = mailmetin.Replace("{FirmaAdi}", frm.FirmaAdi);
                        if (mailmetin.IndexOf("{Tarih}") != -1)
                            mailmetin = mailmetin.Replace("{Tarih}", Tarih.ToShortDateString());
                        if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OdemeTarihi}", Tarih.ToShortDateString());
                        if (mailmetin.IndexOf("{Tutar}") != -1)
                            mailmetin = mailmetin.Replace("{Tutar}", Ucret.ToString());
                        if (mailmetin.IndexOf("{GelinAdSoyad}") != -1)
                            mailmetin = mailmetin.Replace("{GelinAdSoyad}", "");
                        if (mailmetin.IndexOf("{DamatAdSoyad}") != -1)
                            mailmetin = mailmetin.Replace("{DamatAdSoyad}", "");
                        if (mailmetin.IndexOf("{RandevuTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{RandevuTarihi}", "");
                        if (mailmetin.IndexOf("{OpsiyonTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OpsiyonTarihi}", "");
                        if (mailmetin.IndexOf("{SozlesmeNo}") != -1)
                            mailmetin = mailmetin.Replace("{SozlesmeNo}", makbuzno);
                        if (mailmetin.IndexOf("{RezervasyonTuru}") != -1)
                            mailmetin = mailmetin.Replace("{RezervasyonTuru}", "Günlük İş");
                        if (mailmetin.IndexOf("{MakbuzNo}") != -1)
                            mailmetin = mailmetin.Replace("{MakbuzNo}", makbuzno);
                        if (mailmetin.IndexOf("{CekimYeri}") != -1)
                            mailmetin = mailmetin.Replace("{CekimYeri}", "");
                        if (mailmetin.IndexOf("{CekimSaati}") != -1)
                            mailmetin = mailmetin.Replace("{CekimSaati}", "");


                        string konu = mmetin.MailKonu;
                        string baslik = mmetin.MailBaslik;
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
                            body = body.Replace("{MailBaslik}", baslik);
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

                            mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, Email, konu, body, htmlView);
                        }
                        else
                            mailsonuc = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, Email, konu, mailmetin);

                    }
                }

                #endregion
                var gis = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && (x.Tarih.Day == DateTime.Today.Day && x.Tarih.Month == DateTime.Today.Month && x.Tarih.Year == DateTime.Today.Year) && x.Aktif == true && x.Sil == false)
                    .OrderByDescending(x => x.Id).Take(10)
                    .Select(m => new
                    {
                        Id = m.Id,
                        TakipNo = m.TakipNo,
                        Kategori = m.GunlukIsKategori.KategoriAdi,
                        Adet = m.Adet,
                        BirimUcret = m.BirimUcret,
                        Ucret = m.Ucret,
                        Notlar = m.Notlar,
                        Iptal = m.Iptal,

                    }).ToList();
                // Dashboar da kullanılıyor. 
                List<GelirGider> gelir;
                List<GelirGider> gider;
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gelir" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gider" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
                decimal toplamgelir = gelir.Sum(x => x.Tutar);
                decimal toplamgider = gider.Sum(x => x.Tutar);
                decimal kasa = toplamgelir - toplamgider;
                //Dashboard da kullanılıyor.
                return Json(new { Sonuc = true, Gelir = toplamgelir, Gider = toplamgider, Kasa = kasa, Mesaj = "Kayıt Başarılı", data = gis }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Günlük İş Ekle";
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
            #region Trigger'e Alındı
            //// Makbuz Numarası Oluşturuluyor.
            //long makbuzno = 0;
            //string mn;
            //List<GelirGider> alinanodemevarmi = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId).ToList();
            //if (alinanodemevarmi.Count > 0)
            //{
            //    makbuzno = Convert.ToInt64(alinanodemevarmi.Max(x => x.MakbuzNo));
            //    makbuzno = makbuzno + 1;
            //    mn = makbuzno.ToString();
            //}
            //else
            //    mn = FirmaId.ToString() + "101" + makbuzno.ToString();
            //// Makbuz Numarası Oluşturuluyor.

            //Odemeler odeme = new Odemeler();
            //odeme.FirmaId = FirmaId;
            //odeme.SubeId = SubeId;
            //odeme.SozlesmeId = 1;
            //odeme.GisId = Gis.Id;
            ////if (MusteriId==0)
            ////    odeme.MusteriId = 1;
            ////else
            //odeme.MusteriId = MusteriId;
            //odeme.CariHareketId = 1;
            //odeme.OdemeYapanAdSoyad = AdSoyad;
            //odeme.Tarih = Tarih;
            //odeme.OdemeTarihi = Tarih;
            //odeme.Tutar = Ucret;
            //odeme.Kapora = false;
            //odeme.OdemeAl = false; // Ödeme Türü: GelecekÖdeme den bir ödeme alınıyor ise true olacak.
            //odeme.OdemeTuru = "AlinanOdeme";
            //odeme.OdemeSekli = OdemeSekli;
            //odeme.AlinanOdemeMakbuzNo = Convert.ToInt64(mn);
            //odeme.Notlar = Notlar;
            //odeme.Iptal = false;
            //odeme.Aktif = true;
            //odeme.Sil = false;
            //odeme.KilitBit = false;
            //odeme.OlusturanKullaniciId = KullaniciId;
            //odeme.OlusturmaTarih = DateTime.Now;
            //odeme.DegistirenKullaniciId = KullaniciId;
            //odeme.DegistirmeTarih = DateTime.Now;

            //try
            //{
            //    dbContext.Odemelers.Add(odeme);
            //    dbContext.SaveChanges();
            //}
            //catch (Exception e)
            //{
            //    HataLoglari hata = new HataLoglari();
            //    hata.FirmaId = FirmaId;
            //    hata.SubeId = SubeId;
            //    hata.Islem = "Günlük İş Ekle, Alinan Ödeme Ekle";
            //    hata.HataMesajı = e.Message;
            //    hata.OlusturanKullaniciId = 1;
            //    hata.OlusturmaTarih = DateTime.Now;
            //    hata.DegistirenKullaniciId = 1;
            //    hata.DegistirmeTarih = DateTime.Now;
            //    hata.Aktif = true;
            //    hata.Sil = false;
            //    dbContext.HataLoglaris.Add(hata);
            //    try
            //    {
            //        dbContext.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        string mesaj = ex.Message;
            //    }
            //    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            //}

            //GelirGiderTurleri gtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.GelirGiderTur == "Günlük İşler");
            //GelirGider gg = new GelirGider();
            //if (gtur != null)
            //{
            //    gg.GelirGiderTurId = gtur.Id;
            //}
            //else
            //{
            //    GelirGiderTurleri ggtur = new GelirGiderTurleri();
            //    ggtur.FirmaId = FirmaId; // sessiondaki FirmaID
            //    ggtur.GelirGiderTur = "Günlük İşler";
            //    ggtur.OlusturanKullaniciId = KullaniciId;
            //    ggtur.OlusturmaTarih = DateTime.Now;
            //    ggtur.DegistirenKullaniciId = KullaniciId;
            //    ggtur.DegistirmeTarih = DateTime.Now;
            //    ggtur.Aktif = true;
            //    ggtur.Sil = false;
            //    ggtur.KilitBit = true;
            //    dbContext.GelirGiderTurleris.Add(ggtur);
            //    dbContext.SaveChanges();
            //    gg.GelirGiderTurId = ggtur.Id;
            //}
            //gg.FirmaId = FirmaId;
            //gg.SubeId = SubeId;
            //gg.Tarih = Tarih;
            //gg.SozlesmeId = 1;
            //gg.GisId = Gis.Id;
            //gg.CariHareketId = 1;
            //gg.OdemeId = 0;
            //gg.Tip = "Gelir";
            //gg.Tutar = Ucret;
            //gg.MakbuzNo = Convert.ToInt64(mn); // GelirGider tablosundaki MakbuzNo = Odemler tablosundaki MakbuzNo
            //gg.Notlar = tno + " Takip Numaralı Günlük Satışa ait ödeme";
            //gg.OlusturanKullaniciId = KullaniciId;
            //gg.Aktif = true;
            //gg.Sil = false;
            //gg.OlusturmaTarih = DateTime.Now;
            //gg.DegistirenKullaniciId = KullaniciId;
            //gg.DegistirmeTarih = DateTime.Now;
            #endregion

            #region Gerek Kalmadı. Geri Alınabilir


            //try
            //{
            //    string MusteriAdSoyad = AdSoyad;
            //    dbContext.GelirGiders.Add(gg);
            //    dbContext.SaveChanges();
            //    Models.Musteri musteri = dbContext.Musteris.FirstOrDefault(x => x.Id == MusteriId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            //    bool smskabul = true, emailkabul = true;
            //    if (musteri == null)
            //    {
            //        smskabul = true; emailkabul = true;
            //    }
            //    else
            //    {
            //        smskabul = musteri.SMSKabul; emailkabul = musteri.EmailKabul; MusteriAdSoyad = musteri.AdiSoyadi;
            //    }
            //    #region Sms Bilgi Mesajı

            //    if (AdSoyad != "-- Müşterisiz İşlem --" && cep != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
            //    {
            //        AyarlarSmsGonderim smsayar = null;
            //        string smsmetin = "";

            //        smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.GunlukIsOdemeBilgiGonderimSuresi == 6 && x.GunlukIsOdemeBilgiMesaji != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
            //        if (smsayar != null)
            //            smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.GunlukIsOdemeBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
            //        else
            //            smsmetin = null;
            //        if (smsayar != null && smsmetin != null)
            //        {
            //            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
            //                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", MusteriAdSoyad);
            //            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
            //                smsmetin = smsmetin.Replace("{AdSoyad}", MusteriAdSoyad);
            //            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
            //                smsmetin = smsmetin.Replace("{FirmaAdi}", frm.FirmaAdi);
            //            if (smsmetin.IndexOf("{Tarih}") != -1)
            //                smsmetin = smsmetin.Replace("{Tarih}", Tarih.ToShortDateString());
            //            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
            //                smsmetin = smsmetin.Replace("{OdemeTarihi}", Tarih.ToShortDateString());
            //            if (smsmetin.IndexOf("{Tutar}") != -1)
            //                smsmetin = smsmetin.Replace("{Tutar}", Ucret.ToString());
            //            if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
            //                smsmetin = smsmetin.Replace("{GelinAdSoyad}", "");
            //            if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
            //                smsmetin = smsmetin.Replace("{DamatAdSoyad}", "");
            //            if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
            //                smsmetin = smsmetin.Replace("{RandevuTarihi}", "");
            //            if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
            //                smsmetin = smsmetin.Replace("{OpsiyonTarihi}", "");
            //            if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
            //                smsmetin = smsmetin.Replace("{SozlesmeNo}", mn.ToString());
            //            if (smsmetin.IndexOf("{MakbuzNo}") != -1)
            //                smsmetin = smsmetin.Replace("{MakbuzNo}", mn.ToString());
            //            if (smsmetin.IndexOf("{CekimYeri}") != -1)
            //                smsmetin = smsmetin.Replace("{CekimYeri}", "");
            //            if (smsmetin.IndexOf("{CekimSaati}") != -1)
            //                smsmetin = smsmetin.Replace("{CekimSaati}", "");
            //            smssonuc = SMSGonder.Gonder_AtakSms(smsmetin, cep, FirmaId, SubeId, KullaniciId);
            //        }
            //    }
            //    #endregion

            //    #region Email Bilgi Maili
            //    if (AdSoyad != "-- Müşterisiz İşlem --" && Email != "" && emailkabul) // Müşteri bir cep telefonu girilmiş ise SMS gönder
            //    {
            //        AyarlarMailGonderim mailayar = null;
            //        MailMetinleri mmetin = null;
            //        string mailmetin = "";
            //        mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.GunlukIsOdemeBilgiGonderimSuresi == 6 && x.GunlukIsOdemeBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
            //        if (mailayar == null)
            //        {
            //            return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Bilgi Maili gönderilemedi" }, JsonRequestBehavior.AllowGet);
            //        }
            //        mailmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.GunlukIsOdemeBilgiMaili && x.Aktif == true && x.Sil == false).MailMetni;

            //        mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.GunlukIsOdemeBilgiMaili && x.Aktif == true && x.Sil == false);


            //        if (mailayar != null && mailmetin != null)
            //        {
            //            if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
            //                mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", MusteriAdSoyad);
            //            if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
            //                mailmetin = mailmetin.Replace("{AdSoyad}", MusteriAdSoyad);
            //            if (mailmetin.IndexOf("{FirmaAdi}") != -1)
            //                mailmetin = mailmetin.Replace("{FirmaAdi}", frm.FirmaAdi);
            //            if (mailmetin.IndexOf("{Tarih}") != -1)
            //                mailmetin = mailmetin.Replace("{Tarih}", Tarih.ToShortDateString());
            //            if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
            //                mailmetin = mailmetin.Replace("{OdemeTarihi}", Tarih.ToShortDateString());
            //            if (mailmetin.IndexOf("{Tutar}") != -1)
            //                mailmetin = mailmetin.Replace("{Tutar}", Ucret.ToString());
            //            if (mailmetin.IndexOf("{GelinAdSoyad}") != -1)
            //                mailmetin = mailmetin.Replace("{GelinAdSoyad}", "");
            //            if (mailmetin.IndexOf("{DamatAdSoyad}") != -1)
            //                mailmetin = mailmetin.Replace("{DamatAdSoyad}", "");
            //            if (mailmetin.IndexOf("{RandevuTarihi}") != -1)
            //                mailmetin = mailmetin.Replace("{RandevuTarihi}", "");
            //            if (mailmetin.IndexOf("{OpsiyonTarihi}") != -1)
            //                mailmetin = mailmetin.Replace("{OpsiyonTarihi}", "");
            //            if (mailmetin.IndexOf("{SozlesmeNo}") != -1)
            //                mailmetin = mailmetin.Replace("{SozlesmeNo}", mn.ToString());
            //            if (mailmetin.IndexOf("{MakbuzNo}") != -1)
            //                mailmetin = mailmetin.Replace("{MakbuzNo}", mn.ToString());
            //            if (mailmetin.IndexOf("{CekimYeri}") != -1)
            //                mailmetin = mailmetin.Replace("{CekimYeri}", "");
            //            if (mailmetin.IndexOf("{CekimSaati}") != -1)
            //                mailmetin = mailmetin.Replace("{CekimSaati}", "");


            //            string konu = mmetin.MailKonu;
            //            string baslik = mmetin.MailBaslik;
            //            string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
            //            string IcerikResim = Server.MapPath(mmetin.IcerikResim);
            //            string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");
            //            string body = string.Empty;

            //            if (mmetin.TemaYol != "")
            //            {
            //                using (StreamReader reader = new StreamReader(Server.MapPath(mmetin.TemaYol)))
            //                {
            //                    body = reader.ReadToEnd();
            //                }
            //                body = body.Replace("{FirmaLogo}", "src=cid:firmalogo");
            //                body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
            //                body = body.Replace("{IcerikResim}", "src=cid:icerikresim");
            //                body = body.Replace("{MailBaslik}", baslik);
            //                body = body.Replace("{FirmaAdi}", frm.FirmaAdi);
            //                body = body.Replace("{FirmaWeb}", frm.WebSitesi);
            //                body = body.Replace("{FirmaFacebook}", frm.Facebook);
            //                body = body.Replace("{FirmaInstagram}", frm.Instagram);
            //                body = body.Replace("{FirmaTwitter}", frm.Twitter);
            //                body = body.Replace("{EmailMetni}", mailmetin);

            //                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
            //                LinkedResource firmalogo = new LinkedResource(FirmaLogo);
            //                firmalogo.ContentId = "firmalogo";
            //                LinkedResource fotografcitakiplogo = new LinkedResource(FotografciTakipLogo);
            //                fotografcitakiplogo.ContentId = "icerikresim";
            //                LinkedResource icerikresim = new LinkedResource(IcerikResim);
            //                icerikresim.ContentId = "icerikresim";
            //                htmlView.LinkedResources.Add(firmalogo);
            //                htmlView.LinkedResources.Add(fotografcitakiplogo);
            //                htmlView.LinkedResources.Add(icerikresim);

            //                mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, Email, konu, body, htmlView);
            //            }
            //            else
            //                mailsonuc = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, Email, konu, mailmetin);

            //        }
            //    }

            //    #endregion
            //    var gis = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && (x.Tarih.Day == DateTime.Today.Day && x.Tarih.Month == DateTime.Today.Month && x.Tarih.Year == DateTime.Today.Year) && x.Aktif == true && x.Sil == false)
            //        .OrderByDescending(x => x.Id).Take(10)
            //        .Select(m => new
            //        {
            //            Id = m.Id,
            //            TakipNo = m.TakipNo,
            //            Kategori = m.GunlukIsKategori.KategoriAdi,
            //            Ucret = m.Ucret,
            //            Notlar = m.Notlar,
            //            Iptal = m.Iptal,

            //        }).ToList();
            //    // Dashboar da kullanılıyor. 
            //    List<GelirGider> gelir;
            //    List<GelirGider> gider;
            //    gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gelir" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
            //    gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gider" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
            //    decimal toplamgelir = gelir.Sum(x => x.Tutar);
            //    decimal toplamgider = gider.Sum(x => x.Tutar);
            //    decimal kasa = toplamgelir - toplamgider;
            //    //Dashboard da kullanılıyor.
            //    return Json(new { Sonuc = true, Gelir = toplamgelir, Gider = toplamgider, Kasa = kasa, Mesaj = "Kayıt Başarılı", data = gis }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception e)
            //{
            //    HataLoglari hata = new HataLoglari();
            //    hata.FirmaId = FirmaId;
            //    hata.SubeId = SubeId;
            //    hata.Islem = "Günlük İş Ekle, Gelir Gider Ekle";
            //    hata.HataMesajı = e.Message;
            //    hata.OlusturanKullaniciId = 1;
            //    hata.OlusturmaTarih = DateTime.Now;
            //    hata.DegistirenKullaniciId = 1;
            //    hata.DegistirmeTarih = DateTime.Now;
            //    hata.Aktif = true;
            //    hata.Sil = false;
            //    dbContext.HataLoglaris.Add(hata);
            //    try
            //    {
            //        dbContext.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        string mesaj = ex.Message;
            //    }
            //    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            //}
            #endregion
        }
        [HttpPost]
        public ActionResult GunlukIsListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);

            int RolId = Convert.ToInt32(Session["RolId"]);
            List<GunlukIsler> gis;
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).GunlukIsler;
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
            if (SubeId == 0)
            {
                gis = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && ((x.Tarih.Day >= IlkTarih.Day && x.Tarih.Month >= IlkTarih.Month && x.Tarih.Year >= IlkTarih.Year) && (x.Tarih.Day <= SonTarih.Day && x.Tarih.Month <= SonTarih.Month && x.Tarih.Year <= SonTarih.Year)) && x.Aktif == true && x.Sil == false).ToList();
            }
            else
            {
                gis = dbContext.GunlukIslers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && ((x.Tarih.Day >= IlkTarih.Day && x.Tarih.Month >= IlkTarih.Month && x.Tarih.Year >= IlkTarih.Year) && (x.Tarih.Day <= SonTarih.Day && x.Tarih.Month <= SonTarih.Month && x.Tarih.Year <= SonTarih.Year)) && x.Aktif == true && x.Sil == false).ToList();
            }


            var gunlukisler = gis.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                SubeAdi = m.Sube.SubeAdi,
                TakipNo = m.TakipNo,
                Tarih = m.Tarih.ToString("dd/MM/yyyy"),
                KategoriId = m.KategoriId,
                Kategori = m.GunlukIsKategori.KategoriAdi,
                MusteriId = m.MusteriId,
                AdSoyad = m.AdSoyad,
                TcKimlik = m.TCKimlikNo,
                SabitTel = m.SabitTel,
                CepTel = m.CepTel,
                Email = m.Email,
                Adres = m.Adres,
                Adet = m.Adet,
                BirimUcret = m.BirimUcret,
                Ucret = m.Ucret,
                Notlar = m.Notlar,
                Aciklama = m.Notlar,
                OdemeSekli = m.OdemeTuru,
                Iptal = m.Iptal,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            });
            return Json(new { data = gunlukisler }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GunlukIsGuncelle()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long KategoriId = Convert.ToInt64(Request["KategoriId"]);
            DateTime Tarih = Convert.ToDateTime(Request["Tarih"]);
            long GisId = Convert.ToInt64(Request["GisId"]);
            long MusteriId = Convert.ToInt64(Request["MusteriId"]);
            string AdSoyad = Request["AdSoyad"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string cep = Request["CepTel"];
            TelefonDüzelt td = new TelefonDüzelt();
            if (!string.IsNullOrEmpty(cep)) { cep = td.düzelt(cep); }
            string Email = Request["Email"];
            string Adres = Request["Adres"];
            decimal Ucret = 0;
            int Adet = Convert.ToInt32(Request["Adet"]);
            decimal BirimUcret = Convert.ToDecimal(Request["BirimUcret"]);
            string Notlar = Request["Notlar"];
            string OdemeSekli = Request["OdemeSekli"];
            Notlar = Notlar.Replace("\r\n", "<br />");
            Notlar = Notlar.Replace("\n", "<br />");
            Tarih = Tarih.AddHours(DateTime.Now.Hour);
            Tarih = Tarih.AddMinutes(DateTime.Now.Minute);
            GunlukIsler Gis = dbContext.GunlukIslers.FirstOrDefault(x => x.Id == GisId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (Gis == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            Ucret = BirimUcret * Adet;
            Gis.Tarih = Tarih;
            Gis.KategoriId = KategoriId;
            Gis.MusteriId = MusteriId;
            Gis.AdSoyad = AdSoyad;
            Gis.TCKimlikNo = TCKimlikNo;
            Gis.CepTel = cep;
            Gis.Adres = Adres;
            Gis.Email = Email;
            Gis.Adet = Adet;
            Gis.BirimUcret = BirimUcret;
            Gis.Ucret = Ucret;
            Gis.OdemeTuru = OdemeSekli;
            Gis.Notlar = Notlar;
            Gis.Odenen = 0;
            Gis.KalanBakiye = 0;
            Gis.DegistirenKullaniciId = KullaniciId;
            Gis.DegistirmeTarih = DateTime.Now;

            #region Trigger'e Alındı
            //Odemeler alinanodeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == FirmaId && x.GisId == Gis.Id && x.OdemeTuru == "AlinanOdeme");
            //GelirGider gg = dbContext.GelirGiders.FirstOrDefault(x => x.FirmaId == FirmaId && x.GisId == Gis.Id);
            //alinanodeme.Tutar = Ucret;
            //alinanodeme.OdemeYapanAdSoyad = AdSoyad;
            //alinanodeme.OdemeSekli = OdemeSekli;
            //alinanodeme.Notlar = Notlar;
            //alinanodeme.Tarih = Tarih;

            //gg.Tarih = Tarih;
            //gg.Tutar = Ucret;
            //gg.DegistirenKullaniciId = KullaniciId;
            //gg.DegistirmeTarih = DateTime.Now;
            #endregion
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Güncelleme Başarılı", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Günlük İş Güncelle, Kayıt Id: " + GisId;
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
        public ActionResult GisSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);


            GunlukIsler gis = dbContext.GunlukIslers.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (gis == null)
                return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            gis.Sil = true;
            gis.Aktif = false;
            gis.DegistirenKullaniciId = KullaniciId;
            gis.DegistirmeTarih = DateTime.Now;
            #region Trigger'e Alındı         
            //Odemeler alinanodeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == FirmaId && x.GisId == gis.Id && x.OdemeSekli == "AlinanOdeme");
            //List<GelirGider> gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.GisId == gis.Id).ToList();
            //alinanodeme.Sil = true;
            //alinanodeme.Aktif = false;
            //alinanodeme.DegistirenKullaniciId = KullaniciId;
            //alinanodeme.DegistirmeTarih = DateTime.Now;
            //if (gg.Count > 0)
            //{
            //    foreach (var item in gg)
            //    {
            //        item.Sil = true;
            //        item.Aktif = false;
            //        item.DegistirenKullaniciId = KullaniciId;
            //        item.DegistirmeTarih = DateTime.Now;
            //    }
            //}
            #endregion
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Günlük İş Sil, Kayıt Id: " + id;
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
        public ActionResult GisIptal(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            GunlukIsler gis = dbContext.GunlukIslers.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (gis == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            gis.Iptal = true;
            gis.DegistirenKullaniciId = KullaniciId;
            gis.DegistirmeTarih = DateTime.Now;

            #region Trigger'e Alındı
            //Odemeler alinanodeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == FirmaId && x.GisId == gis.Id && x.OdemeTuru == "AlinanOdeme");
            //GelirGider gg = dbContext.GelirGiders.FirstOrDefault(x => x.FirmaId == FirmaId && x.GisId == gis.Id);
            //alinanodeme.Iptal = true;
            //alinanodeme.DegistirenKullaniciId = KullaniciId;
            //alinanodeme.DegistirmeTarih = DateTime.Now;
            //GelirGider gider = new GelirGider();
            //gider.SubeId = SubeId;
            //gider.FirmaId = FirmaId;
            //gider.Tarih = DateTime.Now;
            //gider.SozlesmeId = 0;
            //gider.GisId = gis.Id;
            //gider.OdemeId = 0;
            //gider.Tip = "Gider";
            //gider.GelirGiderTurId = gg.GelirGiderTurId;
            //gider.Tutar = gg.Tutar;
            //gider.Notlar = gis.TakipNo + "Takip Numaralı Günlük İşin İptaliden oluşan geri ödeme";
            //gider.OlusturanKullaniciId = KullaniciId;
            //gider.OlusturmaTarih = DateTime.Now;
            //gider.DegistirenKullaniciId = KullaniciId;
            //gider.DegistirmeTarih = DateTime.Now;
            //gider.Aktif = true;
            //gider.Sil = false;
            //dbContext.GelirGiders.Add(gider);
            #endregion

            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt İptal Edildi", JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Günlük İş İptal Et, Kayıt Id: " + id;
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
        #region Günlük İşler Makbuz Yazdırma İşemleri
        public ActionResult GunlukMakbuztoPDF(long? id)
        {
            // id = gunlukisid
            //if (Session.Count == 0)
            //    return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            GunlukIsler gis = dbContext.GunlukIslers.FirstOrDefault(x => x.TakipNo == id && x.Aktif == true && x.Sil == false);
            if (gis == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == gis.FirmaId);
            ViewBag.SozlesmeAyar = dbContext.AyarlarSozlesmeCiktis.FirstOrDefault(x => x.FirmaId == gis.FirmaId);
            ViewBag.Firma = frm;
            return View(gis);
        }
        public ActionResult GunlukMakbuzYazdir(long id)
        {
            // id = gunlukisid
            var p = new ActionAsPdf("GunlukMakbuztoPDF", new { id = id })
            {
                CustomSwitches = "--page-offset 0 --footer-center [page]/[toPage] --footer-font-size 10",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 }// ayarlar sayfasından alınabilir
            };
            return p;
        }
        #endregion
        #region Gelir Gider İşlemleri
        public ActionResult GelirlerGiderler()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Muhasebe";
            ViewBag.AltMenu = "Gelirler-Giderler";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 27 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            List<GelirGiderTurleri> ggtur = dbContext.GelirGiderTurleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).GelirlerGiderler;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 27 && x.Aktif == true && x.Sil == false);
            ViewBag.FiltreAyar = filtreayar;
            ViewBag.SubeListesi = sube;
            ViewBag.SubeKullanici = subekullanici;
            ViewBag.GGTur = ggtur;
            return View();
        }
        public ActionResult GelirGiderFiltre(string IlkTarih, string SonTarih, int Sube, int GoruntuleTip, int GoruntuleGGTur)
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<GelirGider> gg = null;

            string tip = "";
            if (GoruntuleTip == 0)
                tip = "";
            else if (GoruntuleTip == 1)
                tip = "Gelir";
            else if (GoruntuleTip == 2)
                tip = "Gider";

            if (Sube == 0 && GoruntuleTip == 0 && GoruntuleGGTur == 0)
                gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && GoruntuleTip == 0 && GoruntuleGGTur != 0)
                gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.GelirGiderTurId == GoruntuleGGTur && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && GoruntuleTip != 0 && GoruntuleGGTur == 0)
                gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == tip && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube == 0 && GoruntuleTip != 0 && GoruntuleGGTur != 0)
                gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == tip && x.GelirGiderTurId == GoruntuleGGTur && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && GoruntuleTip == 0 && GoruntuleGGTur == 0)
                gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && GoruntuleTip == 0 && GoruntuleGGTur != 0)
                gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.GelirGiderTurId == GoruntuleGGTur && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && GoruntuleTip != 0 && GoruntuleGGTur == 0)
                gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.Tip == tip && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();
            else if (Sube != 0 && GoruntuleTip != 0 && GoruntuleGGTur != 0)
                gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.Tip == tip && x.GelirGiderTurId == GoruntuleGGTur && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false).ToList();

            var gelirgiderlist = gg.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                SubeAdi = m.Sube.SubeAdi,
                Tarih = m.Tarih,
                SozlesmeId = m.SozlesmeId,
                GisId = m.GisId,
                Tip = m.Tip,
                GelirGiderTurId = m.GelirGiderTurId,
                GelirGiderTur = m.GelirGiderTurleri.GelirGiderTur,
                Tutar = m.Tutar,
                Aciklama = m.Notlar,
                KilitBit = m.KilitBit,
                OdemeSekli = m.OdemeTuru,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            });
            return Json(new { data = gelirgiderlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GelirGiderListesi()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            List<GelirGider> gg;
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).GelirlerGiderler;
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

            if (SubeId == 0)
            {
                gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && ((x.Tarih.Day >= IlkTarih.Day && x.Tarih.Month >= IlkTarih.Month && x.Tarih.Year >= IlkTarih.Year) && (x.Tarih.Day <= SonTarih.Day && x.Tarih.Month <= SonTarih.Month && x.Tarih.Year <= SonTarih.Year)) && x.Aktif == true && x.Sil == false).ToList();
            }
            else
            {
                gg = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && ((x.Tarih.Day >= IlkTarih.Day && x.Tarih.Month >= IlkTarih.Month && x.Tarih.Year >= IlkTarih.Year) && (x.Tarih.Day <= SonTarih.Day && x.Tarih.Month <= SonTarih.Month && x.Tarih.Year <= SonTarih.Year)) && x.Aktif == true && x.Sil == false).ToList();
            }

            var gelirgiderlist = gg.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                SubeAdi = m.Sube.SubeAdi,
                Tarih = m.Tarih,
                SozlesmeId = m.SozlesmeId,
                GisId = m.GisId,
                Tip = m.Tip,
                GelirGiderTurId = m.GelirGiderTurId,
                GelirGiderTur = m.GelirGiderTurleri.GelirGiderTur,
                Tutar = m.Tutar,
                Aciklama = m.Notlar,
                KilitBit = m.KilitBit,
                OdemeSekli = m.OdemeTuru,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            });
            return Json(new { data = gelirgiderlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GelirGiderEkle()
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
            DateTime Tarih = Convert.ToDateTime(Request["Tarih"]);
            //Tarih = Tarih.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute).AddSeconds(DateTime.Now.Second);
            string GGTip = Request["GGTip"];
            long GGTur = Convert.ToInt64(Request["GGTur"]);
            decimal Tutar = Convert.ToDecimal(Request["Tutar"]);
            string OdemeSekli = Request["OdemeSekli"];
            string Aciklama = Request["Aciklama"];

            GelirGider gelirgider = new GelirGider();
            gelirgider.FirmaId = FirmaId;
            gelirgider.SubeId = SubeId;
            gelirgider.Tarih = Tarih;
            gelirgider.SozlesmeId = 0;
            gelirgider.GisId = 0;
            gelirgider.CariHareketId = 0;
            gelirgider.PersonelOdemeId = 0;
            gelirgider.OdemeId = 0;
            gelirgider.Tip = GGTip;
            gelirgider.GelirGiderTurId = GGTur;
            gelirgider.Tutar = Tutar;
            gelirgider.OdemeTuru = OdemeSekli;
            gelirgider.Notlar = Aciklama;
            gelirgider.KilitBit = false;
            gelirgider.Aktif = true;
            gelirgider.Sil = false;
            gelirgider.OlusturanKullaniciId = KullaniciId;
            gelirgider.OlusturmaTarih = DateTime.Now;
            gelirgider.DegistirenKullaniciId = KullaniciId;
            gelirgider.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.GelirGiders.Add(gelirgider);
                dbContext.SaveChanges();
                // Dashboar da kullanılıyor.
                List<GelirGider> gelir;
                List<GelirGider> gider;
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == "Gelir" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == "Gider" && ((x.Tarih.Day >= DateTime.Today.Day && x.Tarih.Month >= DateTime.Today.Month && x.Tarih.Year >= DateTime.Today.Year) && (x.Tarih.Day <= DateTime.Today.Day && x.Tarih.Month <= DateTime.Today.Month && x.Tarih.Year <= DateTime.Today.Year)) && x.Aktif == true && x.Sil == false).ToList();

                decimal toplamgelir = gelir.Sum(x => x.Tutar);
                decimal toplamgider = gider.Sum(x => x.Tutar);
                decimal kasa = toplamgelir - toplamgider;
                //Dashboard da kullanılıyor.

                return Json(new { Sonuc = true, Gelir = toplamgelir, Gider = toplamgider, Kasa = kasa, Mesaj = "Kayıt Başarılı", JsonRequestBehavior.AllowGet });

                //return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Gelir - Gider Ekle";
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
        public ActionResult GelirGiderGuncelle(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");

            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            DateTime Tarih = Convert.ToDateTime(Request["Tarih"]);
            string GGTip = Request["GGTip"];
            long GGTur = Convert.ToInt64(Request["GGTur"]);
            decimal Tutar = Convert.ToDecimal(Request["Tutar"]);
            string OdemeSekli = Request["OdemeSekli"];
            string Aciklama = Request["Aciklama"];

            GelirGider gelirgider = dbContext.GelirGiders.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (gelirgider == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            gelirgider.Tarih = Tarih;
            gelirgider.Tip = GGTip;
            gelirgider.GelirGiderTurId = GGTur;
            gelirgider.Tutar = Tutar;
            gelirgider.OdemeTuru = OdemeSekli;
            gelirgider.Notlar = Aciklama;
            gelirgider.DegistirenKullaniciId = KullaniciId;
            gelirgider.DegistirmeTarih = DateTime.Now;
            try
            {
                if (gelirgider.GisId > 0)
                {
                    Models.GunlukIsler gis = dbContext.GunlukIslers.FirstOrDefault(x => x.Id == gelirgider.GisId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                    gis.Tarih = Tarih;
                    gis.Ucret = Tutar;
                    gis.OdemeTuru = OdemeSekli;
                    gis.Notlar = Aciklama;
                    gis.DegistirenKullaniciId = KullaniciId;
                    gis.DegistirmeTarih = DateTime.Now;
                }
                else if (gelirgider.PersonelOdemeId > 0)
                {
                    PersonelOdeme personelOdeme = dbContext.PersonelOdemes.FirstOrDefault(x => x.Id == gelirgider.GisId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                    personelOdeme.OdemeTarihi = Tarih;
                    personelOdeme.Tutar = Tutar;
                    personelOdeme.OdemeTuru = OdemeSekli;
                    personelOdeme.Aciklama = Aciklama;
                    personelOdeme.DegistirenKullaniciId = KullaniciId;
                    personelOdeme.DegistirmeTarih = DateTime.Now;
                }
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Güncelleme Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Gelir - Gider Güncelle, Kayıt Id: " + id;
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
        public ActionResult GelirGiderSil(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");

            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            string DetaySil = Request["DetaySil"];
            if (DetaySil == "Evet") // Detay Kayıt Var, Detaylar ile birlikte sil
            {
                GelirGider gg = dbContext.GelirGiders.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                if (gg == null)
                    return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
                //if (gg.SozlesmeId != 0 || gg.SozlesmeId != null)
                if (gg.SozlesmeId != 0)
                {
                    // OdemelerAlinan tablosundan silinecek
                    try
                    {
                        Odemeler odeme = dbContext.Odemelers.FirstOrDefault(x => x.Id == gg.OdemeId && x.FirmaId == FirmaId);
                        odeme.Aktif = false;
                        odeme.Sil = true;
                        odeme.DegistirenKullaniciId = KullaniciId;
                        odeme.DegistirmeTarih = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = 1;
                        hata.Islem = "Gelir - Gider Sil, Sözleşmeden alınan ödeme, Sözleşme Id: " + gg.SozlesmeId.ToString() + " Alınan Ödeme Id: " + gg.OdemeId.ToString();
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
                //else if (gg.GisId != 0 || gg.GisId != null)
                else if (gg.GisId != 0)
                {
                    // GunlukIs tablosundan silinecek
                    try
                    {
                        GunlukIsler gis = dbContext.GunlukIslers.FirstOrDefault(x => x.Id == gg.GisId && x.FirmaId == FirmaId);
                        gis.Aktif = false;
                        gis.Sil = true;
                        gis.DegistirenKullaniciId = KullaniciId;
                        gis.DegistirmeTarih = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = 1;
                        hata.Islem = "Gelir - Gider Sil, Günlük İşten alınan ödeme, Günlük İş Id: " + gg.GisId.ToString();
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
                //else if (gg.CariHareketId != 0 || gg.CariHareketId != null)
                else if (gg.CariHareketId != 0)
                {
                    // CariHareket tablosundan silinecek
                    try
                    {
                        CariHareket carihareket = dbContext.CariHarekets.FirstOrDefault(x => x.Id == gg.CariHareketId && x.FirmaId == FirmaId);
                        carihareket.Aktif = false;
                        carihareket.Sil = true;
                        carihareket.DegistirenKullaniciId = KullaniciId;
                        carihareket.DegistirmeTarih = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = 1;
                        hata.Islem = "Gelir - Gider Sil, Cari Harekete ait işlem detayı silme, Cari Hareket Id: " + gg.CariHareketId.ToString();
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
                else if (gg.PersonelOdemeId != 0)
                {
                    PersonelOdeme persodeme = dbContext.PersonelOdemes.FirstOrDefault(x => x.Id == gg.PersonelOdemeId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                    persodeme.Sil = true;
                    persodeme.Aktif = false;
                    persodeme.DegistirenKullaniciId = KullaniciId;
                    persodeme.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = 1;
                        hata.Islem = "Gelir - Gider Sil, Personel Odeme Hareketine ait detayı silme, Personel Odeme Id: " + gg.PersonelOdemeId.ToString();
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
                //pkt.FirmaId = FirmaId; // sesiondaki firmaID
                gg.DegistirenKullaniciId = KullaniciId;
                gg.DegistirmeTarih = DateTime.Now;
                gg.Aktif = false;
                gg.Sil = true;
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
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
            else // Detay Kayıt Yok, Doğrudan Sil
            {
                GelirGider gg = dbContext.GelirGiders.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false); // giriş yapan kullanıcı ve firmaya göre kısıtlandırılacak
                if (gg == null)
                    return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
                gg.DegistirenKullaniciId = KullaniciId;
                gg.DegistirmeTarih = DateTime.Now;
                gg.Aktif = false;
                gg.Sil = true;
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Silindi" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "Gelir-Gider Sil, Kayıt Id: " + id.ToString();
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
        public ActionResult GelirGiderDetayKontrol(long? id)
        {
            // sonuc:True => Detay Kayıt Var, Detay Kayıtlar ile birlikte mi silinecek?
            // sonuc:True => Detay Kayıt Yok, doğrudan Silinecek?
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            GelirGider gg = dbContext.GelirGiders.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (gg == null)
                return Json(new { Sonuc = false, Bilgi = "Detay bilgisi istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            if (gg.SozlesmeId > 0)
            {
                string sozlesmeno = dbContext.Sozlesmes.FirstOrDefault(x => x.Id == gg.SozlesmeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).SozlesmeNo.ToString();
                return Json(new { Sonuc = true, Mesaj = "<h2 style='font-size:16px; color:red'>Silmek istediğiniz kayıt, " + sozlesmeno + " numaralı Sözleşmeye ait.</h2></p><p><h3><strong>Bu kaydı silmeniz durumunda Sözleşme ödemelerinde tutarsızlık yaşayabilirsiniz. </br> Bu işlemi 'Sözleşme Düzenle' Bölümünden yapmanızı tavsiye ederiz. </strong></h3></p><p><h2> Sözleşmedeki Ödeme kaydı ile birlikte silinsin mi? </h2></p>" }, JsonRequestBehavior.AllowGet);
            }
            else if (gg.GisId > 0)
            {
                string takipno = dbContext.GunlukIslers.FirstOrDefault(x => x.Id == gg.GisId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).TakipNo.ToString();
                return Json(new { Sonuc = true, Mesaj = "<h2 style='font-size:16px; color:red'>Silmek istediğiniz kayıt, " + takipno + " takip numaralı Günlük İşe ait.</h2></p> <p><h2> Günlük İş kaydı ile birlikte silinsin mi? </h2></p>" }, JsonRequestBehavior.AllowGet);
            }
            else if (gg.CariHareketId > 0)
            {
                string cari = dbContext.CariHarekets.FirstOrDefault(x => x.Id == gg.CariHareketId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).Cari.FirmaAdi;
                if (string.IsNullOrEmpty(cari))
                    cari = dbContext.CariHarekets.FirstOrDefault(x => x.Id == gg.CariHareketId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).Cari.AdSoyad;
                else
                    cari = cari + " - " + dbContext.CariHarekets.FirstOrDefault(x => x.Id == gg.CariHareketId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).Cari.Yetkili;

                return Json(new { Sonuc = true, Mesaj = "<h2 style='font-size:16px; color:red'>Silmek istediğiniz kayıt, Cari: " + cari + "'ye ait.</h2></p> <p><h3><strong>Bu kaydı silmeniz durumunda Cari Hesap Takibinde tutarsızlık yaşayabilirsiniz. </br> Bu işlemi 'Cari Hesap Takibi' Bölümünden yapmanızı tavsiye ederiz. </strong></h3></p> <p><h2> Cari İşlem kaydı ile birlikte silinsin mi? </h2></p>" }, JsonRequestBehavior.AllowGet);
            }
            else if (gg.PersonelOdemeId > 0)
            {
                string personel = dbContext.PersonelOdemes.FirstOrDefault(x => x.Id == gg.PersonelOdemeId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).Personel.AdiSoyadi;

                return Json(new { Sonuc = true, Mesaj = "<h2 style='font-size:16px; color:red'>Silmek istediğiniz kayıt, Personel: " + personel + "'e yapılan ödemeye ait.</h2></p> <p><h3><strong>Bu kaydı silmeniz durumunda Personel ödemelerinde tutarsızlık yaşayabilirsiniz. </br> Bu işlemi 'Personel Ödemeleri' Bölümünden yapmanızı tavsiye ederiz. </strong></h3></p> <p><h2> Perssonle Ödeme kaydı ile birlikte silinsin mi? </h2></p>" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Sonuc = false, Mesaj = "Gelir - Gider detayı bulunamadı" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Gelecek Ödemeler İşlemleri
        public ActionResult GelecekOdemeler()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Muhasebe";
            ViewBag.AltMenu = "Gelecek Ödemeler";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            ViewBag.AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 28 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            List<SmsMetinleri> smsmetinleri = dbContext.SmsMetinleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<Sozlesme> Sozlesmeler;
            List<Models.Musteri> Musteriler;
            if (SubeId == 0)
            {
                Sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.Bitti == false && x.Iptal == false && x.Aktif == true && x.Sil == false).ToList();
                Musteriler = dbContext.Musteris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            }
            else
            {
                Sozlesmeler = dbContext.Sozlesmes.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Bitti == false && x.Iptal == false && x.Aktif == true && x.Sil == false).ToList();
                Musteriler = dbContext.Musteris.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();
            }

            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).AlinanGelecekOdemeler;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 28 && x.Aktif == true && x.Sil == false);
            ViewBag.FiltreAyar = filtreayar;
            ViewBag.SmsMetinleri = smsmetinleri;
            ViewBag.SubeListesi = sube;
            ViewBag.Sozlesmeler = Sozlesmeler;
            ViewBag.Musteriler = Musteriler;
            ViewBag.SubeKullanici = subekullanici;
            return View();
        }
        public ActionResult GelecekOdemelerFiltre(string TarihSecim, string IlkTarih, string SonTarih, int Sube, int GoruntuleTip, int FiltreSozlesmeId, int FiltreMusteriId)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            DateTime ilktarih = Convert.ToDateTime(IlkTarih);
            DateTime sontarih = Convert.ToDateTime(SonTarih).AddDays(1);
            List<Odemeler> odemeler = null;
            string OdemeTuru = "";
            if (GoruntuleTip == 1)
                OdemeTuru = "AlinanOdeme";
            else if (GoruntuleTip == 2)
                OdemeTuru = "GelecekOdeme";
            if (TarihSecim == "on") // Filter tarihleri İşlem tarihi
            {
                if (Sube == 0 && GoruntuleTip == 0 && FiltreSozlesmeId == 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip == 0 && FiltreSozlesmeId == 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.MusteriId == FiltreMusteriId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip == 0 && FiltreSozlesmeId != 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == FiltreSozlesmeId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip == 0 && FiltreSozlesmeId != 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == FiltreSozlesmeId && x.MusteriId == FiltreMusteriId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip != 0 && FiltreSozlesmeId == 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == OdemeTuru && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip != 0 && FiltreSozlesmeId == 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == OdemeTuru && x.MusteriId == FiltreMusteriId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip != 0 && FiltreSozlesmeId != 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == OdemeTuru && x.SozlesmeId == FiltreSozlesmeId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip != 0 && FiltreSozlesmeId != 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == OdemeTuru && x.SozlesmeId == FiltreSozlesmeId && x.MusteriId == FiltreMusteriId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip == 0 && FiltreSozlesmeId == 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip == 0 && FiltreSozlesmeId == 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.MusteriId == FiltreMusteriId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip == 0 && FiltreSozlesmeId != 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.SozlesmeId == FiltreSozlesmeId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip == 0 && FiltreSozlesmeId != 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.SozlesmeId == FiltreSozlesmeId && x.MusteriId == FiltreMusteriId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip != 0 && FiltreSozlesmeId == 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.OdemeTuru == OdemeTuru && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip != 0 && FiltreSozlesmeId == 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.OdemeTuru == OdemeTuru && x.MusteriId == FiltreMusteriId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip != 0 && FiltreSozlesmeId != 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.OdemeTuru == OdemeTuru && x.SozlesmeId == FiltreSozlesmeId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip != 0 && FiltreSozlesmeId != 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.OdemeTuru == OdemeTuru && x.SozlesmeId == FiltreSozlesmeId && x.MusteriId == FiltreMusteriId && ((x.Tarih >= ilktarih) && (x.Tarih <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
            }
            else // Filter tarihleri ödeme tarihi
            {
                if (Sube == 0 && GoruntuleTip == 0 && FiltreSozlesmeId == 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip == 0 && FiltreSozlesmeId == 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.MusteriId == FiltreMusteriId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip == 0 && FiltreSozlesmeId != 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == FiltreSozlesmeId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip == 0 && FiltreSozlesmeId != 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SozlesmeId == FiltreSozlesmeId && x.MusteriId == FiltreMusteriId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip != 0 && FiltreSozlesmeId == 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == OdemeTuru && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip != 0 && FiltreSozlesmeId == 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == OdemeTuru && x.MusteriId == FiltreMusteriId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip != 0 && FiltreSozlesmeId != 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == OdemeTuru && x.SozlesmeId == FiltreMusteriId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube == 0 && GoruntuleTip != 0 && FiltreSozlesmeId != 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.OdemeTuru == OdemeTuru && x.SozlesmeId == FiltreMusteriId && x.MusteriId == FiltreMusteriId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                if (Sube != 0 && GoruntuleTip == 0 && FiltreSozlesmeId == 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip == 0 && FiltreSozlesmeId == 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.MusteriId == FiltreMusteriId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip == 0 && FiltreSozlesmeId != 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.SozlesmeId == FiltreSozlesmeId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip == 0 && FiltreSozlesmeId != 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.SozlesmeId == FiltreSozlesmeId && x.MusteriId == FiltreMusteriId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip != 0 && FiltreSozlesmeId == 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.OdemeTuru == OdemeTuru && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip != 0 && FiltreSozlesmeId == 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.OdemeTuru == OdemeTuru && x.MusteriId == FiltreMusteriId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip != 0 && FiltreSozlesmeId != 0 && FiltreMusteriId == 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.OdemeTuru == OdemeTuru && x.SozlesmeId == FiltreMusteriId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                else if (Sube != 0 && GoruntuleTip != 0 && FiltreSozlesmeId != 0 && FiltreMusteriId != 0)
                    odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == Sube && x.OdemeTuru == OdemeTuru && x.SozlesmeId == FiltreMusteriId && x.MusteriId == FiltreMusteriId && ((x.OdemeTarihi >= ilktarih) && (x.OdemeTarihi <= sontarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
            }
            var odemelist = odemeler.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                SubeAdi = m.Sube.SubeAdi,
                Tarih = m.Tarih.ToString("dd/MM/yyyy"),
                OdemeTarihi = m.OdemeTarihi.ToString("dd/MM/yyyy"),
                OdemeTuru = m.OdemeTuru,
                GelecekOdemeId = m.GelecekOdemeID,
                // Sözleşme Detayı İçin
                SozlesmeId = m.SozlesmeId,
                SozlesmeNo = m.Sozlesme.SozlesmeNo,
                SozlesmeFirmaAdi = m.Sozlesme.Firma.FirmaAdi,
                SozlesmeTarihi = m.Sozlesme.SozlesmeTarihi.ToString("dd/MM/yyyy"),
                OpsiyonTarihi = Convert.ToDateTime(m.Sozlesme.OpsiyonTarihi).ToString("dd/MM/yyyy"),
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
                Iptal = m.Sozlesme.Iptal,
                Durum = m.Sozlesme.Durum,
                // Sözleşme Detayı İçin
                // Müşteri Detayı İçin
                MusteriId = m.MusteriId,
                MusteriKodu = m.Musteri.MusteriKodu,
                MusteriAdiSoyadi = m.Musteri.AdiSoyadi,
                MusteriTC = m.Musteri.TCKimlikNo,
                MusteriSabitTel = m.Musteri.SabitTel,
                MusteriCepTel = m.Musteri.CepTel,
                MusteriEmail = m.Musteri.Email,
                MusteriAdres = m.Musteri.Adres,
                MusteriNotlar = m.Musteri.Notlar,
                // Müşteri Detayı İçin
                GisId = m.GisId,
                GisTakipNo = m.GunlukIsler.TakipNo,
                GisKategori = m.GunlukIsler.GunlukIsKategori.KategoriAdi,
                AlinanOdemeMakbuzNo = m.AlinanOdemeMakbuzNo,
                OdemeYapanAdSoyad = m.OdemeYapanAdSoyad,
                OdemeSekli = m.OdemeSekli,
                Tutar = m.Tutar,
                Aciklama = m.Notlar,
                KilitBit = m.KilitBit,
                OdemeAl = m.OdemeAl,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            }).ToList();
            return Json(new { data = odemelist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GelecekOdemelerListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).AlinanGelecekOdemeler;
            DateTime IlkTarih = DateTime.Today;
            DateTime SonTarih = DateTime.Today;
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
                    SonTarih = DateTime.Today;
                    break;
                case "5": // Yılın başından bu güne
                    IlkTarih = new DateTime(DateTime.Now.Year, 1, 1);
                    SonTarih = DateTime.Now;
                    break;
            }
            List<Odemeler> odemeler = null;
            if (SubeId == 0)
                odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                //odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && ((x.Tarih.Day >= IlkTarih.Day && x.Tarih.Month >= IlkTarih.Month && x.Tarih.Year >= IlkTarih.Year) && (x.Tarih.Day <= SonTarih.Day && x.Tarih.Month <= SonTarih.Month && x.Tarih.Year <= SonTarih.Year)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
            else
                odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
                //odemeler = dbContext.Odemelers.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && ((x.Tarih.Day >= IlkTarih.Day && x.Tarih.Month >= IlkTarih.Month && x.Tarih.Year >= IlkTarih.Year) && (x.Tarih.Day <= SonTarih.Day && x.Tarih.Month <= SonTarih.Month && x.Tarih.Year <= SonTarih.Year)) && x.Aktif == true && x.Sil == false && x.OdemeAl == false).ToList();
            var odemelist = odemeler.Select(m => new
            {
                Id = m.Id,
                FirmaAdi = m.Firma.FirmaAdi,
                SubeAdi = m.Sube.SubeAdi,
                Tarih = m.Tarih.ToString("dd/MM/yyyy"),
                OdemeTarihi = m.OdemeTarihi.ToString("dd/MM/yyyy"),
                OdemeTuru = m.OdemeTuru,
                GelecekOdemeId = m.GelecekOdemeID,
                // Sözleşme Detayı İçin
                SozlesmeId = m.SozlesmeId,
                SozlesmeNo = m.Sozlesme.SozlesmeNo,
                SozlesmeFirmaAdi = m.Sozlesme.Firma.FirmaAdi,
                SozlesmeTarihi = m.Sozlesme.SozlesmeTarihi.ToString("dd/MM/yyyy"),
                OpsiyonTarihi = Convert.ToDateTime(m.Sozlesme.OpsiyonTarihi).ToString("dd/MM/yyyy"),
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
                Iptal = m.Sozlesme.Iptal,
                Durum = m.Sozlesme.Durum,
                // Sözleşme Detayı İçin
                // Müşteri Detayı İçin
                MusteriId = m.MusteriId,
                MusteriKodu = m.Musteri.MusteriKodu,
                MusteriAdiSoyadi = m.Musteri.AdiSoyadi,
                MusteriTC = m.Musteri.TCKimlikNo,
                MusteriSabitTel = m.Musteri.SabitTel,
                MusteriCepTel = m.Musteri.CepTel,
                MusteriEmail = m.Musteri.Email,
                MusteriAdres = m.Musteri.Adres,
                MusteriNotlar = m.Musteri.Notlar,
                // Müşteri Detayı İçin
                GisId = m.GisId,
                GisTakipNo = m.GunlukIsler.TakipNo,
                GisKategori = m.GunlukIsler.GunlukIsKategori.KategoriAdi,
                AlinanOdemeMakbuzNo = m.AlinanOdemeMakbuzNo,
                OdemeYapanAdSoyad = m.OdemeYapanAdSoyad,
                OdemeSekli = m.OdemeSekli,
                Tutar = m.Tutar,
                Aciklama = m.Notlar,
                KilitBit = m.KilitBit,
                OdemeAl = m.OdemeAl,
                OlusturanKullanici = m.OlusturanKullaniciId,
                OlusturmaTarih = m.OlusturmaTarih,
                DegistirenKullanici = m.DegistirenKullaniciId,
                DegistirmeTarih = m.DegistirmeTarih,
                Aktif = m.Aktif,
                Sil = m.Sil
            }).ToList();
            return Json(new { data = odemelist }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult OdemeAl(long? id)
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
            DateTime OdemeTarihi = Convert.ToDateTime(Request["OdemeTarihi"]);
            DateTime KalanOdemeTarihi = Convert.ToDateTime(Request["KalanOdemeTarihi"]);
            decimal OdenenTutar = Convert.ToDecimal(Request["OdenenTutar"]);
            decimal KalanTutar = Convert.ToDecimal(Request["KalanTutar"]);
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

            OdemeTarihi = OdemeTarihi.AddHours(DateTime.Now.Hour);
            OdemeTarihi = OdemeTarihi.AddMinutes(DateTime.Now.Minute);

            Odemeler alinanodeme = dbContext.Odemelers.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id && x.Sil == false && x.Aktif == true);
            if (alinanodeme == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });

            Odemeler odeme = new Odemeler();
            odeme.FirmaId = FirmaId;
            odeme.SubeId = SubeId;
            odeme.SozlesmeId = alinanodeme.SozlesmeId;
            odeme.GisId = alinanodeme.GisId;
            odeme.MusteriId = alinanodeme.MusteriId;
            odeme.CariHareketId = alinanodeme.CariHareketId;
            odeme.GelecekOdemeID = id;
            odeme.OdemeYapanAdSoyad = OdemeYapan;
            odeme.Tarih = OdemeTarihi;
            odeme.Tutar = OdenenTutar;
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
                // OdenecekTutar sıfırdan büyükse bir gelecek ödeme kaydı oluşturulacak.
                if (KalanTutar > 0)
                {
                    Odemeler kalanodeme = new Odemeler();
                    kalanodeme.FirmaId = FirmaId;
                    kalanodeme.SubeId = SubeId;
                    kalanodeme.SozlesmeId = alinanodeme.SozlesmeId;
                    kalanodeme.GisId = alinanodeme.GisId;
                    kalanodeme.MusteriId = alinanodeme.MusteriId;
                    kalanodeme.CariHareketId = alinanodeme.CariHareketId;
                    //kalanodeme.OdemeYapanAdSoyad = OdemeYapan;
                    kalanodeme.Tarih = KalanOdemeTarihi;
                    kalanodeme.Tutar = KalanTutar;
                    kalanodeme.OdemeTarihi = KalanOdemeTarihi;
                    kalanodeme.OdemeTuru = "GelecekOdeme";
                    //kalanodeme.OdemeSekli = OdemeSekli;
                    kalanodeme.OdemeAl = false;
                    kalanodeme.Kapora = false;
                    kalanodeme.KilitBit = false;
                    kalanodeme.Notlar = Aciklama;
                    kalanodeme.Iptal = false;
                    kalanodeme.Aktif = true;
                    kalanodeme.Sil = false;
                    kalanodeme.OlusturanKullaniciId = KullaniciId;
                    kalanodeme.OlusturmaTarih = DateTime.Now;
                    kalanodeme.DegistirenKullaniciId = KullaniciId;
                    kalanodeme.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.Odemelers.Add(kalanodeme);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Odemeler, Alınan odemeden kalan tutarın Gelecek Ödeme olarak kaydedilmesi, Kayıt Id: " + id;
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

                #region GelirGidet tür Tanımı
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
                    dbContext.GelirGiderTurleris.Add(ggtur);
                    dbContext.SaveChanges();
                    gg.GelirGiderTurId = ggtur.Id;
                }
                #endregion
                gg.FirmaId = FirmaId;
                gg.SubeId = SubeId;
                gg.Tarih = OdemeTarihi;
                gg.SozlesmeId = alinanodeme.SozlesmeId;
                gg.GisId = alinanodeme.GisId;
                gg.CariHareketId = alinanodeme.CariHareketId;
                gg.PersonelOdemeId = 1;
                gg.OdemeId = odeme.Id;
                gg.Tip = "Gelir";
                gg.Tutar = OdenenTutar;
                gg.OdemeTuru = OdemeSekli;
                gg.MakbuzNo = Convert.ToInt64(mn); // GelirGider tablosundaki MakbuzNo = Odemler tablosundaki MakbuzNo
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
            {
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            }
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
        #endregion
        public ActionResult KalanBakiyeler()
        {
            ViewBag.UstMenu = "Muhasebe";
            ViewBag.AltMenu = "Kalan Bakiyeler";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            return View();
        }
        #region Cari İşlemler
        public ActionResult CariListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Muhasebe";
            ViewBag.AltMenu = "Cariler";
            ViewBag.AltMenu2 = "Cari Listesi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 31 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 31 && x.Aktif == true && x.Sil == false);
            ViewBag.SubeListesi = sube;
            ViewBag.SubeKullanici = subekullanici;
            ViewBag.Ayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).CariListesiPasifGizle;

            return View();
        }
        public ActionResult CarilerListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<Cari> c;
            int RolId = Convert.ToInt32(Session["RolId"]);

            c = dbContext.Caris.Where(x => x.FirmaId == FirmaId && x.Sil == false).ToList();
            var carilist = c.Select(m => new
            {
                Id = m.Id,
                SubeAdi = m.Sube.SubeAdi,
                Firma = m.Firma.FirmaAdi,
                FirmaAdi = m.FirmaAdi,
                Yetkili = m.Yetkili,
                VergiDairesi = m.VergiDairesi,
                VergiNo = m.VergiNo,
                AdSoyad = m.AdSoyad,
                TCKimlikNo = m.TCKimlikNo,
                SabitTel = m.SabitTel,
                CepTel = m.CepTel,
                Fax = m.Fax,
                Email = m.Email,
                Adres = m.Adres,
                Notlar = m.Notlar,
                Aktif = m.Aktif,
                KilitBit = m.KilitBit
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = carilist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public string CariVarMi()
        {
            string FirmaAd = Request["FirmaAd"];
            string Yetkili = Request["Yetkili"];
            string AdSoyad = Request["AdSoyad"];
            string TCKimlikNo = Request["TCKimlikNo"];
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);

            //Cari cr = dbContext.Caris.FirstOrDefault(x => x.SubeId == SubeId && ((x.AdSoyad == AdSoyad && x.TCKimlikNo == TCKimlikNo) || (x.FirmaAdi == FirmaAd && x.Yetkili == Yetkili)) && x.Aktif == true && x.Sil == false);
            Cari cr = new Cari();
            if (FirmaAd == "")
                cr = dbContext.Caris.FirstOrDefault(x => x.FirmaId == FirmaId && x.AdSoyad == AdSoyad && x.TCKimlikNo == TCKimlikNo && x.Aktif == true && x.Sil == false);
            else if (AdSoyad == "")
                cr = dbContext.Caris.FirstOrDefault(x => x.FirmaId == FirmaId && x.FirmaAdi == FirmaAd && x.Yetkili == Yetkili && x.Aktif == true && x.Sil == false);
            if (cr != null)
            {
                return cr.Id.ToString();
            }
            else
            {
                return "Yok";
            }
        }
        [HttpPost]
        public ActionResult CariEkle()
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
            string FirmaAd = Request["FirmaAd"];
            string Yetkili = Request["Yetkili"];
            string VergiDairesi = Request["VergiDairesi"];
            string VergiNo = Request["VergiNo"];
            string AdSoyad = Request["AdSoyad"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string cep = Request["CepTel"];
            string sabit = Request["SabitTel"];
            string fax = Request["Fax"];
            string Email = Request["Email"];
            string Adres = Request["Adres"];
            Adres = Adres.Replace("\r\n", "<br />");
            Adres = Adres.Replace("\n", "<br />");
            string Notlar = Request["Notlar"];
            Notlar = Notlar.Replace("\r\n", "<br />");
            Notlar = Notlar.Replace("\n", "<br />");
            TelefonDüzelt td = new TelefonDüzelt();
            //if (cep != null) { cep = td.düzelt(cep); }
            //if (sabit != null) { sabit = td.düzelt(sabit); }
            if (!string.IsNullOrEmpty(cep)) { cep = td.düzelt(cep); }
            if (!string.IsNullOrEmpty(sabit)) { sabit = td.düzelt(sabit); }
            if (!string.IsNullOrEmpty(fax)) { fax = td.düzelt(fax); }
            Cari kayitvarmi = new Cari();
            if (FirmaAd == "")
                kayitvarmi = dbContext.Caris.FirstOrDefault(x => x.AdSoyad == AdSoyad && x.TCKimlikNo == TCKimlikNo && x.Aktif == false && x.Sil == true);
            else if (AdSoyad == "")
                kayitvarmi = dbContext.Caris.FirstOrDefault(x => x.FirmaAdi == FirmaAd && x.Yetkili == Yetkili && x.Aktif == false && x.Sil == true);
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
                    hata.Islem = "Cari Ekle, Silinmiş olan cari kaydı aktifleştirilirken oluşan hata, Kayıt Id: " + kayitvarmi.Id.ToString();
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
                Cari cari = new Cari();
                cari.FirmaId = FirmaId;
                cari.SubeId = SubeId;
                cari.FirmaAdi = FirmaAd;
                cari.Yetkili = Yetkili;
                cari.VergiDairesi = VergiDairesi;
                cari.VergiNo = VergiNo;
                cari.AdSoyad = AdSoyad;
                cari.TCKimlikNo = TCKimlikNo;
                cari.CepTel = cep;
                cari.SabitTel = sabit;
                cari.Fax = fax;
                cari.Email = Email;
                cari.Adres = Adres;
                cari.Notlar = Notlar;
                if (string.IsNullOrEmpty(cep))
                    cari.SMSKabul = false;
                else
                    cari.SMSKabul = true;
                if (string.IsNullOrEmpty(Email))
                    cari.EmailKabul = false;
                else
                    cari.EmailKabul = true;
                cari.OlusturanKullaniciId = KullaniciId;
                cari.OlusturmaTarih = DateTime.Now;
                cari.DegistirenKullaniciId = KullaniciId;
                cari.DegistirmeTarih = DateTime.Now;
                cari.Aktif = true;
                cari.Sil = false;
                cari.KilitBit = false;
                try
                {
                    dbContext.Caris.Add(cari);
                    dbContext.SaveChanges();
                    // Cep telefonu boşsa veya cep telefonu dolu mail adresi boşsa rehbere kaydetme
                    #region Triggera Alındı
                    //if (!string.IsNullOrEmpty(cep) || (!string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(cep)))
                    //{
                    //    AyarlarGenel genelayar = dbContext.AyarlarGenels.FirstOrDefault(x => x.FirmaId == FirmaId);
                    //    RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Cariler");
                    //    TelefonRehberi rehber = new TelefonRehberi();
                    //    rehber.FirmaId = FirmaId;
                    //    rehber.RehberGrupId = rehbergrup.Id;
                    //    rehber.CariId = cari.Id;
                    //    rehber.FirmaAdi = FirmaAd;
                    //    if (FirmaAd == "")
                    //        rehber.AdSoyad = AdSoyad;
                    //    else
                    //        rehber.AdSoyad = Yetkili;
                    //    rehber.SabitTel1 = sabit;
                    //    rehber.SabitTel2 = "";
                    //    rehber.CepTel1 = cep;
                    //    rehber.CepTel2 = "";
                    //    rehber.Fax = "";
                    //    rehber.Email = Email;
                    //    rehber.SmsKabul = true;
                    //    rehber.EmailKabul = true;
                    //    rehber.Notlar = "Yeni Cari Kaydı";
                    //    rehber.OlusturanKullaniciId = KullaniciId;
                    //    rehber.OlusturmaTarih = DateTime.Now;
                    //    rehber.DegistirenKullaniciId = KullaniciId;
                    //    rehber.DegistirmeTarih = DateTime.Now;
                    //    if (genelayar.PersonelRehberKayit == true)
                    //        rehber.Aktif = true;
                    //    else
                    //        rehber.Aktif = false;
                    //    rehber.Sil = false;
                    //    dbContext.TelefonRehberis.Add(rehber);
                    //    try
                    //    {
                    //        //dbContext.SaveChanges();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        HataLoglari hata = new HataLoglari();
                    //        hata.FirmaId = FirmaId;
                    //        hata.SubeId = SubeId;
                    //        hata.Islem = "Cari İletişim Bilgilerini Rehbere Ekle";
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
                    #endregion

                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Cari Ekle";
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
        public ActionResult CariGuncelle(long? id)
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
            string FirmaAd = Request["FirmaAd"];
            string Yetkili = Request["Yetkili"];
            string VergiDairesi = Request["VergiDairesi"];
            string VergiNo = Request["VergiNo"];
            string AdSoyad = Request["AdSoyad"];
            string TCKimlikNo = Request["TCKimlikNo"];
            string cep = Request["CepTel"];
            string sabit = Request["SabitTel"];
            string fax = Request["Fax"];
            string Email = Request["Email"];
            string Adres = Request["Adres"];
            Adres = Adres.Replace("\r\n", "<br />");
            Adres = Adres.Replace("\n", "<br />");
            string Notlar = Request["Notlar"];
            Notlar = Notlar.Replace("\r\n", "<br />");
            Notlar = Notlar.Replace("\n", "<br />");
            TelefonDüzelt td = new TelefonDüzelt();
            //if (cep != null) { cep = td.düzelt(cep); }
            //if (sabit != null) { sabit = td.düzelt(sabit); }
            if (!string.IsNullOrEmpty(cep)) { cep = td.düzelt(cep); }
            if (!string.IsNullOrEmpty(sabit)) { sabit = td.düzelt(sabit); }
            if (!string.IsNullOrEmpty(fax)) { fax = td.düzelt(fax); }

            Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (cari == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            cari.SubeId = SubeId;
            cari.FirmaAdi = FirmaAd;
            cari.Yetkili = Yetkili;
            cari.VergiDairesi = VergiDairesi;
            cari.VergiNo = VergiNo;
            cari.TCKimlikNo = TCKimlikNo;
            cari.AdSoyad = AdSoyad;
            cari.SabitTel = sabit;
            cari.CepTel = cep;
            cari.Fax = fax;
            cari.Email = Email;
            cari.Adres = Adres;
            cari.Notlar = Notlar;
            cari.DegistirenKullaniciId = KullaniciId;
            cari.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariId == id);
                if (rehber != null)
                {
                    rehber.SabitTel1 = sabit;
                    rehber.AdSoyad = AdSoyad;
                    rehber.FirmaAdi = FirmaAd;
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
                        RehberGrup rehbergrup = dbContext.RehberGrups.FirstOrDefault(x => x.FirmaId == FirmaId && x.GrupAdi == "Cariler");
                        TelefonRehberi r = new TelefonRehberi();
                        r.FirmaId = FirmaId;
                        r.RehberGrupId = rehbergrup.Id;
                        r.CariId = id;
                        r.FirmaAdi = FirmaAd;
                        r.AdSoyad = AdSoyad;
                        r.SabitTel1 = sabit;
                        r.SabitTel2 = "";
                        r.CepTel1 = cep;
                        r.CepTel2 = "";
                        r.Fax = "";
                        r.Email = Email;
                        r.SmsKabul = true;
                        r.EmailKabul = true;
                        r.Notlar = "Cari Güncelle - Cari İletişim Bilgilerini Rehbere Ekle";
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
                hata.Islem = "Cari Güncelle, Kayıt Id: " + id.ToString();
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
        public ActionResult CariSil(long? id)
        {
            // Personelin kullanıldığı tablolarda kayıtlar var ise personel silinemeyecek
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

            // Cariye ait kayıt var mı?
            List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.CariId == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            if (carihareket.Count > 0) // Cariye ait hareket kaydı varsa silme
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Cari Sil, Cari Kullanılıyor, Silinemez! Kayıt Id: " + id.ToString();
                hata.HataMesajı = "Silinmek istenilen Kayıt Kullanılıyor.";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Silinmek istenilen Cariye ait Hareket kayıtları var, Silinemez!", JsonRequestBehavior.AllowGet });
            }
            else // Cariye ait hareket kaydı yoksa sil
            {
                Cari c = dbContext.Caris.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                if (c == null)
                    return Json(new { Sonuc = false, Bilgi = "Silinmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
                c.Aktif = false;
                c.Sil = true;
                c.DegistirenKullaniciId = KullaniciId;
                c.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();

                    TelefonRehberi rehber = dbContext.TelefonRehberis.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariId == id);
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
                            hata.Islem = "Cari Sil, Kayıt Id: " + id + ", Telefon rehberinden cari iletişim bilgilerini sil";
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
                    hata.Islem = "Cari Sil, Kayıt Id: " + id.ToString();
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
        public ActionResult CariHareketKontrol(long? id)
        {
            // Cari kullanıldığı tablolarda kayıtlar var ise personel silinemeyecek
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

            List<CariHareket> carihareket = dbContext.CariHarekets.Where(x => x.CariId == id && x.Sil == false && x.Aktif == true).ToList();
            if (carihareket.Count == 0)
                return Json(new { Sonuc = true }, JsonRequestBehavior.AllowGet); // cariye  ait hareket yoksa silinebilir
            else
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Cari Sil, Kayıt Id: " + id.ToString();
                hata.HataMesajı = "Cariye Ait Hareket kayıtları var";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false }, JsonRequestBehavior.AllowGet); // cariye  ait hareket varsa silinemez
            }
        }
        [HttpPost]
        public string CariAktifDurumDegistir(long? id)
        {
            Models.Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == id && x.Sil == false);

            if (cari.Aktif == true)
                cari.Aktif = false;
            else
                cari.Aktif = true;
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
        #region Cari Hareket İşlemleri
        public ActionResult CariHesapTakibi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Muhasebe";
            ViewBag.AltMenu = "Cariler";
            ViewBag.AltMenu2 = "Cari Hesap Takibi";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            int RolId = Convert.ToInt32(Session["RolId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 32 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> sube;
            List<Cari> cari;
            sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            if (SubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                cari = dbContext.Caris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            else
                cari = dbContext.Caris.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();
            List<SmsMetinleri> smsmetinleri = dbContext.SmsMetinleris.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).CariHesapTakibi;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 32 && x.Aktif == true && x.Sil == false);
            ViewBag.FiltreAyar = filtreayar;
            ViewBag.SubeListesi = sube;
            ViewBag.CariListesi = cari;
            ViewBag.SubeKullanici = subekullanici;
            ViewBag.SmsMetinleri = smsmetinleri;
            return View();
        }
        [HttpPost]
        public ActionResult CariHesapBakiyeler(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            List<CariHareket> c = new List<CariHareket>();
            if (id == 0)
                if (SubeId == 0)
                    c = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
                else
                    c = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();
            else
                if (SubeId == 0)
                c = dbContext.CariHarekets.Where(x => x.CariId == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            else
                c = dbContext.CariHarekets.Where(x => x.CariId == id && x.FirmaId == FirmaId && x.SubeId == SubeId && x.Aktif == true && x.Sil == false).ToList();

            var carilist = c.Select(m => new
            {
                Id = m.Id,
                SubeAdi = m.Sube.SubeAdi,
                FirmaAdi = m.Firma.FirmaAdi,
                CariId = m.CariId,
                CariFirma = m.Cari.FirmaAdi,
                CariYetkili = m.Cari.Yetkili,
                CariAdSoyad = m.Cari.AdSoyad,
                CariSabitTel = m.Cari.SabitTel,
                CariCepTel = m.Cari.CepTel,
                CariFax = m.Cari.Fax,
                CariEmail = m.Cari.Email,
                CariVergiDairesi = m.Cari.VergiDairesi,
                CariVergiNo = m.Cari.VergiNo,
                CariTcKimlikNo = m.Cari.TCKimlikNo,
                CariAdres = m.Cari.Adres,
                CariAciklama = m.Cari.Notlar,
                IslemTarihi = m.IslemTarihi.ToString("dd/MM/yyyy"),
                OdemeTarihi = m.OdemeTarihi.ToString("dd/MM/yyyy"),
                OdemeYapan = m.OdemeYapanAdSoyad,
                Tip = m.Tip,
                TahsilatOdemeBit = m.TahsilatOdemeBit,
                Tutar = m.Tutar,
                OdemeSekli = m.OdemeTuru,
                Aciklama = m.Notlar
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = carilist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CariHareketEkle()
        {
            // Cari Hareket işlemi herhangi bir gelir-gider yada para girişi değildir.
            // Cari Hareket, cariye olan borcu yada cariden alacağı takip etmek için tasarlandı

            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long CariId = Convert.ToInt64(Request["CariId"]);
            DateTime IslemTarihi = Convert.ToDateTime(Request["IslemTarihi"]);
            DateTime OdemeTarihi = Convert.ToDateTime(Request["OdemeTarihi"]);
            decimal Tutar = Convert.ToDecimal(Request["Tutar"]);
            string Tip = Request["Tip"];
            string Aciklama = Request["Aciklama"];
            string OdemeSekli = Request["OdemeSekli"];
            string OdemeIslem = Request["OdemeIslem"];
            string TahsilatIslem = Request["TahsilatIslem"];
            string OdemeYapan = Request["OdemeYapan"];
            bool mailsonuc;
            bool borcodendi = false, tahsilatyapildi = false;
            string GGTip = "", carihareketaciklama = "";
            if (Tip == "Alacak")
            {
                GGTip = "Gelir";
                if (TahsilatIslem == "yapıldı")
                    tahsilatyapildi = true;
                else
                    tahsilatyapildi = false;
            }

            else if (Tip == "Borç")
            {
                GGTip = "Gider";
                if (OdemeIslem == "yapıldı")
                    borcodendi = true;
                else
                    borcodendi = false;
            }
            Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == CariId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (cari == null)
                return Json(new { Sonuc = false, Bilgi = "Hareket eklenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
            if (cari.FirmaAdi == "")
                carihareketaciklama = cari.AdSoyad;
            else if (cari.AdSoyad == "")
                carihareketaciklama = cari.FirmaAdi + " - " + cari.Yetkili;
            #region CariHareket Ekleme işlemleri
            CariHareket carihareket = new CariHareket();
            carihareket.FirmaId = FirmaId;
            carihareket.SubeId = SubeId;
            carihareket.CariId = CariId;
            carihareket.IslemTarihi = IslemTarihi;
            carihareket.OdemeTarihi = OdemeTarihi;
            carihareket.Tip = Tip;
            carihareket.OdemeTuru = OdemeSekli;
            carihareket.Tutar = Tutar;
            carihareket.Notlar = Aciklama;
            carihareket.OdemeYapanAdSoyad = OdemeYapan;
            if (tahsilatyapildi == true || borcodendi == true)
                carihareket.TahsilatOdemeBit = true;
            else
                carihareket.TahsilatOdemeBit = false;
            carihareket.Aktif = true;
            carihareket.Sil = false;
            carihareket.OlusturanKullaniciId = KullaniciId;
            carihareket.OlusturmaTarih = DateTime.Now;
            carihareket.DegistirenKullaniciId = KullaniciId;
            carihareket.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.CariHarekets.Add(carihareket);
                dbContext.SaveChanges();
                #region Gelir yada giler olarak ekleme işlemleri
                if (tahsilatyapildi == true || borcodendi == true)
                {
                    CariHareket eskihareket = dbContext.CariHarekets.FirstOrDefault(x => x.Id == carihareket.Id);
                    eskihareket.TahsilatOdemeBit = true;
                    eskihareket.DegistirenKullaniciId = KullaniciId;
                    eskihareket.DegistirmeTarih = DateTime.Now;
                    //dbContext.SaveChanges();
                    #region Tahsilattan veya Ödemeden kalan tutarı yeniden ekle
                    if (Tutar < carihareket.Tutar)
                    {
                        CariHareket yenihareket = new CariHareket();
                        yenihareket.FirmaId = FirmaId;
                        yenihareket.SubeId = SubeId;
                        yenihareket.CariId = CariId;
                        yenihareket.IslemTarihi = IslemTarihi;
                        yenihareket.OdemeTarihi = OdemeTarihi;
                        yenihareket.Tip = Tip;
                        yenihareket.OdemeTuru = "---";
                        yenihareket.Tutar = carihareket.Tutar - Tutar;
                        if (Tip == "Alacak")
                            yenihareket.Notlar = IslemTarihi.ToShortDateString() + " Tarihli Alacağın " + OdemeTarihi.ToShortDateString() + " tarihindeki Tahsilatından kalan";
                        else
                            yenihareket.Notlar = IslemTarihi.ToShortDateString() + " Tarihli Borcun " + OdemeTarihi.ToShortDateString() + " tarihindeki Ödemesinden kalan";
                        yenihareket.OdemeYapanAdSoyad = "";
                        yenihareket.TahsilatOdemeBit = false;
                        yenihareket.Aktif = true;
                        yenihareket.Sil = false;
                        yenihareket.OlusturanKullaniciId = KullaniciId;
                        yenihareket.OlusturmaTarih = DateTime.Now;
                        yenihareket.DegistirenKullaniciId = KullaniciId;
                        yenihareket.DegistirmeTarih = DateTime.Now;
                        try
                        {
                            dbContext.CariHarekets.Add(yenihareket);
                            dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Cari Hareket Ekle, Tahsilattan kalan tutarı Alacak olarak yeniden ekle";
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
                    GelirGiderTurleri gtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.GelirGiderTur == "Cari İşlem Ödemesi");
                    GelirGider gelirgider = new GelirGider();
                    if (gtur != null)
                    {
                        gelirgider.GelirGiderTurId = gtur.Id;
                    }
                    else
                    {
                        GelirGiderTurleri ggtur = new GelirGiderTurleri();
                        ggtur.FirmaId = FirmaId; // sessiondaki FirmaID
                        ggtur.GelirGiderTur = "Cari İşlem Ödemesi";
                        ggtur.OlusturanKullaniciId = KullaniciId;
                        ggtur.OlusturmaTarih = DateTime.Now;
                        ggtur.DegistirenKullaniciId = KullaniciId;
                        ggtur.DegistirmeTarih = DateTime.Now;
                        ggtur.Aktif = true;
                        ggtur.Sil = false;
                        ggtur.KilitBit = true;
                        try
                        {
                            dbContext.GelirGiderTurleris.Add(ggtur);
                            dbContext.SaveChanges();
                            gelirgider.GelirGiderTurId = ggtur.Id;
                        }
                        catch (Exception e)
                        {
                            HataLoglari hata = new HataLoglari();
                            hata.FirmaId = FirmaId;
                            hata.SubeId = SubeId;
                            hata.Islem = "Cari Hareket Ekle, Gelir gider Eklemek için Gelir Gider Türü: Cari İşlem Ödemesi, ekle işleminde oluşan hata";
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
                    gelirgider.FirmaId = FirmaId;
                    gelirgider.SubeId = SubeId;
                    gelirgider.Tarih = OdemeTarihi;
                    gelirgider.SozlesmeId = 1;
                    gelirgider.GisId = 1;
                    gelirgider.OdemeId = 0;
                    gelirgider.CariHareketId = carihareket.Id;
                    gelirgider.Tip = GGTip;
                    gelirgider.Tutar = Tutar;
                    gelirgider.OdemeTuru = OdemeSekli;
                    if (tahsilatyapildi)
                        gelirgider.Notlar = carihareketaciklama + " isimli cariden yapılan Tahsilat.";
                    else if (borcodendi)
                        gelirgider.Notlar = carihareketaciklama + " isimli cariye yapılan Ödeme.";
                    gelirgider.KilitBit = false;
                    gelirgider.Aktif = true;
                    gelirgider.Sil = false;
                    gelirgider.OlusturanKullaniciId = KullaniciId;
                    gelirgider.OlusturmaTarih = DateTime.Now;
                    gelirgider.DegistirenKullaniciId = KullaniciId;
                    gelirgider.DegistirmeTarih = DateTime.Now;

                    try
                    {
                        dbContext.GelirGiders.Add(gelirgider);
                        dbContext.SaveChanges();
                        // Borç tahsilatı yapıldığında yada Alacak peşin olarak alındığında SMS Gönder
                        #region Sms Bilgi Mesajı
                        if (cari.CepTel != "" && cari.SMSKabul == true) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            AyarlarSmsGonderim smsayar = null;
                            string smsmetin = "";
                            if (tahsilatyapildi == true) // Alacak kaydı oluşturulduğunda SMS Gönder
                            {
                                smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariTahsilatBilgiGonderimSuresi == 6 && x.CariTahsilatBilgiMesaji != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                                if (smsayar != null)
                                    smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.CariTahsilatBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                                else
                                    smsmetin = null;
                            }
                            else if (borcodendi == true) // Borç Ödeme kaydı oluşturulduğunda SMS Gönder
                            {
                                smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariyeYapilanOdemeBilgiGonderimSuresi == 6 && x.CariyeYapilanOdemeBilgiMesaji != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                                if (smsayar != null)
                                    smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.CariyeYapilanOdemeBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                                else
                                    smsmetin = null;
                            }

                            if (smsayar != null && smsmetin != null)
                            {
                                if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    if (cari.AdSoyad == "")  // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                                    else                   // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                                if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    if (cari.AdSoyad == "") // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                                    else                    // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                        smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                                if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                    smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi);
                                if (smsmetin.IndexOf("{Tarih}") != -1)
                                    smsmetin = smsmetin.Replace("{Tarih}", IslemTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                    smsmetin = smsmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                                if (smsmetin.IndexOf("{Tutar}") != -1)
                                    smsmetin = smsmetin.Replace("{Tutar}", Tutar.ToString());

                                SMSGonder.Gonder_AtakSms(smsmetin, cari.CepTel, FirmaId, SubeId, KullaniciId);
                            }
                        }
                        #endregion
                        #region Email Bilgi Maili
                        if (cari.Email != "" && cari.EmailKabul == true) // Cariye bir cep telefonu girilmiş ise SMS gönder
                        {
                            AyarlarMailGonderim mailayar = null;
                            MailMetinleri mailmetinleri = null;
                            string mailmetin = "";
                            string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
                            string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");


                            if (tahsilatyapildi == true) // Alacak kaydı oluşturulduğunda SMS Gönder
                            {
                                mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariTahsilatBilgiGonderimSuresi == 6 && x.CariTahsilatBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                                if (mailayar == null)
                                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                                else
                                    mailmetinleri = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.CariTahsilatBilgiMaili && x.Aktif == true && x.Sil == false);
                            }
                            else if (borcodendi == true) // Borç Ödeme kaydı oluşturulduğunda SMS Gönder
                            {
                                mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariyeYapilanOdemeBilgiGonderimSuresi == 6 && x.CariyeYapilanOdemeBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                                if (mailayar == null)
                                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                                else
                                    mailmetinleri = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.CariyeYapilanOdemeBilgiMaili && x.Aktif == true && x.Sil == false);
                            }
                            if (mailayar != null && mailmetinleri != null)
                            {
                                string body = string.Empty;
                                string konu = mailmetinleri.MailKonu;
                                string baslik = mailmetinleri.MailBaslik;
                                string IcerikResim = Server.MapPath(mailmetinleri.IcerikResim);
                                mailmetin = mailmetinleri.MailMetni;
                                if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                    if (cari.AdSoyad == "")  // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                        mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                                    else                   // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                        mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                                if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                    if (cari.AdSoyad == "") // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                        mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                                    else                    // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                        mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                                if (mailmetin.IndexOf("{FirmaAdi}") != -1)
                                    mailmetin = mailmetin.Replace("{FirmaAdi}", cari.FirmaAdi);
                                if (mailmetin.IndexOf("{Tarih}") != -1)
                                    mailmetin = mailmetin.Replace("{Tarih}", IslemTarihi.ToShortDateString());
                                if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                                    mailmetin = mailmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                                if (mailmetin.IndexOf("{Tutar}") != -1)
                                    mailmetin = mailmetin.Replace("{Tutar}", Tutar.ToString());

                                if (mailmetinleri.TemaYol != "")
                                {
                                    using (StreamReader reader = new StreamReader(Server.MapPath(mailmetinleri.TemaYol)))
                                    {
                                        body = reader.ReadToEnd();
                                    }
                                    body = body.Replace("{FirmaLogo}", "src=cid:firmalogo");
                                    body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
                                    body = body.Replace("{IcerikResim}", "src=cid:icerikresim");
                                    body = body.Replace("{MailBaslik}", baslik);
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

                                    mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, cari.Email, mailmetinleri.MailKonu, body, htmlView);
                                }
                                else
                                    mailsonuc = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, cari.Email, mailmetinleri.MailKonu, mailmetin);
                                //MailGonder.Gonder(smsmetin, cari.CepTel, FirmaId, SubeId, KullaniciId);
                            }
                        }
                        #endregion
                        return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Cari Hareket Ekle, Gelir Gider Olarak Ekle";
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
                    // Cari borçlu ise borç hatırlatma SMSi
                    #region Sms Bilgi Mesajı
                    if (cari.CepTel != "" && cari.SMSKabul == true) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {
                        AyarlarSmsGonderim smsayar = null;
                        string smsmetin = "";
                        if (tahsilatyapildi == false) // Alacak kaydı oluşturulduğunda SMS Gönder
                        {
                            smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariAlacakHatirlatmaGonderimSuresi == 6 && x.CariAlacakHatirlatmaMesaji != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                            if (smsayar != null)
                                smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.CariAlacakHatirlatmaMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                            else
                                smsmetin = null;
                        }
                        if (smsayar != null && smsmetin != null)
                        {
                            if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                if (cari.AdSoyad == "")  // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                                else                   // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                            if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                if (cari.AdSoyad == "") // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                                else                    // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                    smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                            if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                                smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi);
                            if (smsmetin.IndexOf("{Tarih}") != -1)
                                smsmetin = smsmetin.Replace("{Tarih}", IslemTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                                smsmetin = smsmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                            if (smsmetin.IndexOf("{Tutar}") != -1)
                                smsmetin = smsmetin.Replace("{Tutar}", Tutar.ToString());

                            SMSGonder.Gonder_AtakSms(smsmetin, cari.CepTel, FirmaId, SubeId, KullaniciId);
                        }
                    }
                    #endregion
                    #region Email Bilgi Maili
                    if (cari.Email != "" && cari.EmailKabul == true) // Cariye bir cep telefonu girilmiş ise SMS gönder
                    {

                        AyarlarMailGonderim mailayar = null;
                        MailMetinleri mailmetinleri = null;
                        string mailmetin = "";
                        string FirmaLogo = Server.MapPath(frm.Resim.ResimAdres);
                        string FotografciTakipLogo = Server.MapPath("/Areas/Otomasyon/Dosyalar/FirmaLogolar/Fotografci_Takip_Logo_Mail_Sablon_1.png");

                        if (tahsilatyapildi == false) // Alacak kaydı oluşturulduğunda SMS Gönder
                        {
                            mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariTahsilatBilgiGonderimSuresi == 6 && x.CariTahsilatBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                            if (mailayar == null)
                                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                            else
                                mailmetinleri = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.CariTahsilatBilgiMaili && x.Aktif == true && x.Sil == false);
                        }

                        if (mailayar != null && mailmetinleri != null)
                        {
                            string body = string.Empty;
                            string konu = mailmetinleri.MailKonu;
                            string baslik = mailmetinleri.MailBaslik;
                            string IcerikResim = Server.MapPath(mailmetinleri.IcerikResim);
                            mailmetin = mailmetinleri.MailMetni;
                            if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                                if (cari.AdSoyad == "")  // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                    mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                                else                   // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                    mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                            if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                                if (cari.AdSoyad == "") // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                    mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                                else                    // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                    mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                            if (mailmetin.IndexOf("{FirmaAdi}") != -1)
                                mailmetin = mailmetin.Replace("{FirmaAdi}", cari.FirmaAdi);
                            if (mailmetin.IndexOf("{Tarih}") != -1)
                                mailmetin = mailmetin.Replace("{Tarih}", IslemTarihi.ToShortDateString());
                            if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                                mailmetin = mailmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                            if (mailmetin.IndexOf("{Tutar}") != -1)
                                mailmetin = mailmetin.Replace("{Tutar}", Tutar.ToString());

                            if (mailmetinleri.TemaYol != "")
                            {
                                using (StreamReader reader = new StreamReader(Server.MapPath(mailmetinleri.TemaYol)))
                                {
                                    body = reader.ReadToEnd();
                                }
                                body = body.Replace("{FirmaLogo}", "src=cid:firmalogo");
                                body = body.Replace("{FotografciTakipLogo}", "src=cid:fotografcitakiplogo");
                                body = body.Replace("{IcerikResim}", "src=cid:icerikresim");
                                body = body.Replace("{MailBaslik}", baslik);
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

                                mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, cari.Email, mailmetinleri.MailKonu, body, htmlView);
                            }
                            else
                                mailsonuc = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, cari.Email, mailmetinleri.MailKonu, mailmetin);

                            //MailGonder.Gonder(smsmetin, cari.CepTel, FirmaId, SubeId, KullaniciId);
                        }
                    }
                    #endregion
                    return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Cari Hareket Ekle";
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
        [HttpPost]
        public ActionResult TahsilatYap(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long CariId = Convert.ToInt64(Request["CariId"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            DateTime IslemTarihi = Convert.ToDateTime(Request["IslemTarihi"]);
            DateTime OdemeTarihi = Convert.ToDateTime(Request["OdemeTarihi"]);
            decimal Tutar = Convert.ToDecimal(Request["Tutar"]);
            string Tip = Request["Tip"];
            string Aciklama = Request["Aciklama"];
            string OdemeSekli = Request["OdemeSekli"];
            string OdemeIslem = Request["OdemeIslem"];
            //string TahsilatIslem = Request["TahsilatIslem"];
            string OdemeYapan = Request["OdemeYapan"];
            bool mailsonuc;
            string GGTip = "Gelir", carihareketaciklama = "";
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
            Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == CariId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (cari == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            if (cari.FirmaAdi == "")
                carihareketaciklama = cari.AdSoyad;
            else if (cari.AdSoyad == "")
                carihareketaciklama = cari.FirmaAdi + " - " + cari.Yetkili;
            #region CariHareket Ekleme işlemleri
            CariHareket carihareket = dbContext.CariHarekets.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id && x.Aktif == true && x.Sil == false);

            carihareket.IslemTarihi = IslemTarihi;
            carihareket.OdemeTarihi = OdemeTarihi;
            carihareket.Tip = Tip;
            carihareket.OdemeTuru = OdemeSekli;
            carihareket.Notlar = Aciklama;
            carihareket.OdemeYapanAdSoyad = OdemeYapan;
            carihareket.TahsilatOdemeBit = true;
            carihareket.DegistirenKullaniciId = KullaniciId;
            carihareket.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                #region MakbuzNo
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
                #endregion

                #region Tahsilattan kalan tutarı yeniden ekle
                if (Tutar < carihareket.Tutar)
                {
                    if (SubeId == 0)
                        SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
                    CariHareket yenihareket = new CariHareket();
                    yenihareket.FirmaId = FirmaId;
                    yenihareket.SubeId = SubeId;
                    yenihareket.CariId = CariId;
                    yenihareket.IslemTarihi = IslemTarihi;
                    yenihareket.OdemeTarihi = OdemeTarihi;
                    yenihareket.Tip = "Alacak";
                    yenihareket.OdemeTuru = "---";
                    yenihareket.Tutar = carihareket.Tutar - Tutar;
                    yenihareket.Notlar = IslemTarihi.ToShortDateString() + " Tarihli Alacağın " + OdemeTarihi.ToShortDateString() + " tarihindeki Tahsilatından kalan";
                    yenihareket.OdemeYapanAdSoyad = "";
                    yenihareket.TahsilatOdemeBit = false;
                    yenihareket.Aktif = true;
                    yenihareket.Sil = false;
                    yenihareket.OlusturanKullaniciId = KullaniciId;
                    yenihareket.OlusturmaTarih = DateTime.Now;
                    yenihareket.DegistirenKullaniciId = KullaniciId;
                    yenihareket.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.CariHarekets.Add(yenihareket);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Cari Hareket Ekle, Tahsilattan kalan tutarı Alacak olarak yeniden ekle";
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

                #region Gelir yada giler olarak ekleme işlemleri
                GelirGiderTurleri gtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.GelirGiderTur == "Cari İşlem Ödemesi");
                GelirGider gelirgider = new GelirGider();
                if (gtur != null)
                {
                    gelirgider.GelirGiderTurId = gtur.Id;
                }
                else
                {
                    GelirGiderTurleri ggtur = new GelirGiderTurleri();
                    ggtur.FirmaId = FirmaId; // sessiondaki FirmaID
                    ggtur.GelirGiderTur = "Cari İşlem Ödemesi";
                    ggtur.OlusturanKullaniciId = KullaniciId;
                    ggtur.OlusturmaTarih = DateTime.Now;
                    ggtur.DegistirenKullaniciId = KullaniciId;
                    ggtur.DegistirmeTarih = DateTime.Now;
                    ggtur.Aktif = true;
                    ggtur.Sil = false;
                    ggtur.KilitBit = true;
                    try
                    {
                        dbContext.GelirGiderTurleris.Add(ggtur);
                        dbContext.SaveChanges();
                        gelirgider.GelirGiderTurId = ggtur.Id;
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Cari Hareket Ekle, Gelir gider Eklemek için Gelir Gider Türü: Cari İşlem Ödemesi, ekle işleminde oluşan hata";
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
                gelirgider.FirmaId = FirmaId;
                gelirgider.SubeId = SubeId;
                gelirgider.Tarih = OdemeTarihi;
                gelirgider.SozlesmeId = 1;
                gelirgider.GisId = 1;
                gelirgider.OdemeId = 0;
                gelirgider.PersonelOdemeId = 0;
                gelirgider.CariHareketId = carihareket.Id;
                gelirgider.Tip = GGTip;
                gelirgider.Tutar = Tutar;
                gelirgider.OdemeTuru = OdemeSekli;
                gelirgider.MakbuzNo = Convert.ToInt64(mn); // GelirGider tablosundaki MakbuzNo = Odemler tablosundaki MakbuzNo
                gelirgider.Notlar = carihareketaciklama + " isimli cariden yapılan Tahsilat.";
                gelirgider.KilitBit = false;
                gelirgider.Aktif = true;
                gelirgider.Sil = false;
                gelirgider.OlusturanKullaniciId = KullaniciId;
                gelirgider.OlusturmaTarih = DateTime.Now;
                gelirgider.DegistirenKullaniciId = KullaniciId;
                gelirgider.DegistirmeTarih = DateTime.Now;

                try
                {
                    dbContext.GelirGiders.Add(gelirgider);
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Tahsilat Yap, Gelir Gider Olarak Ekle, Kayıt Id:" + id;
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
                SubeId = Convert.ToInt64(Session["AktifSubeId"]);
                List<CariHareket> CariHareketListesi = new List<CariHareket>();
                if (SubeId == 0) // Şube TÜMÜ seçilmişse
                    CariHareketListesi = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
                else
                    CariHareketListesi = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.OdemeTarihi == DateTime.Today && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();

                var carilist = CariHareketListesi.Select(m => new
                {
                    Id = m.Id,
                    SubeAdi = m.Sube.SubeAdi,
                    FirmaAdi = m.Firma.FirmaAdi,
                    CariId = m.CariId,
                    CariFirma = m.Cari.FirmaAdi,
                    CariYetkili = m.Cari.Yetkili,
                    CariAdSoyad = m.Cari.AdSoyad,
                    CariSabitTel = m.Cari.SabitTel,
                    CariCepTel = m.Cari.CepTel,
                    CariFax = m.Cari.Fax,
                    CariEmail = m.Cari.Email,
                    CariVergiDairesi = m.Cari.VergiDairesi,
                    CariVergiNo = m.Cari.VergiNo,
                    CariTcKimlikNo = m.Cari.TCKimlikNo,
                    CariAdres = m.Cari.Adres,
                    CariAciklama = m.Cari.Notlar,
                    IslemTarihi = m.IslemTarihi.ToString("dd/MM/yyyy"),
                    OdemeTarihi = m.OdemeTarihi.ToString("dd/MM/yyyy"),
                    OdemeYapan = m.OdemeYapanAdSoyad,
                    Tip = m.Tip,
                    TahsilatOdemeBit = m.TahsilatOdemeBit,
                    Tutar = m.Tutar,
                    OdemeSekli = m.OdemeTuru,
                    Aciklama = m.Notlar
                }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.


                #region Sms Bilgi Mesajı
                if (cari.CepTel != "" && cari.SMSKabul == true) // Cariye bir cep telefonu girilmiş ise SMS gönder
                {
                    AyarlarSmsGonderim smsayar = null;
                    string smsmetin = "";
                    smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariTahsilatBilgiGonderimSuresi == 6 && x.CariyeYapilanOdemeBilgiMesaji != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (smsayar != null)
                        smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.CariTahsilatBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                    else
                        smsmetin = null;
                    if (smsayar != null && smsmetin != null)
                    {
                        if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            if (cari.AdSoyad == "")  // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                            else                   // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                        if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            if (cari.AdSoyad == "") // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                            else                    // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                        if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                            smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi);
                        if (smsmetin.IndexOf("{Tarih}") != -1)
                            smsmetin = smsmetin.Replace("{Tarih}", IslemTarihi.ToShortDateString());
                        if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                        if (smsmetin.IndexOf("{Tutar}") != -1)
                            smsmetin = smsmetin.Replace("{Tutar}", Tutar.ToString());
                        if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{GelinAdSoyad}", "");
                        if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{DamatAdSoyad}", "");
                        if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{RandevuTarihi}", "");
                        if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{OpsiyonTarihi}", "");
                        if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                            smsmetin = smsmetin.Replace("{SozlesmeNo}", mn.ToString());
                        if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                            smsmetin = smsmetin.Replace("{MakbuzNo}", mn.ToString());
                        if (smsmetin.IndexOf("{CekimYeri}") != -1)
                            smsmetin = smsmetin.Replace("{CekimYeri}", "");
                        if (smsmetin.IndexOf("{CekimSaati}") != -1)
                            smsmetin = smsmetin.Replace("{CekimSaati}", "");
                        SMSGonder.Gonder_AtakSms(smsmetin, cari.CepTel, FirmaId, SubeId, KullaniciId);
                    }
                }
                #endregion
                #region Email Bilgi Maili
                if (cari.Email != "" && cari.EmailKabul == true) // Cariye bir cep telefonu girilmiş ise SMS gönder
                {
                    AyarlarMailGonderim mailayar = null;
                    MailMetinleri mmetin = null;
                    string mailmetin = "";
                    mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariTahsilatBilgiGonderimSuresi == 6 && x.CariyeYapilanOdemeBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (mailayar == null)
                        return Json(new { Sonuc = true, CariList = carilist, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                    else
                        mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.CariTahsilatBilgiMaili && x.Aktif == true && x.Sil == false);
                    if (mailayar != null && mmetin != null)
                    {
                        mailmetin = mmetin.MailMetni;
                        if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            if (cari.AdSoyad == "")  // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                            else                   // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                        if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            if (cari.AdSoyad == "") // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                            else                    // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                        if (mailmetin.IndexOf("{FirmaAdi}") != -1)
                            mailmetin = mailmetin.Replace("{FirmaAdi}", cari.FirmaAdi);
                        if (mailmetin.IndexOf("{Tarih}") != -1)
                            mailmetin = mailmetin.Replace("{Tarih}", IslemTarihi.ToShortDateString());
                        if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                        if (mailmetin.IndexOf("{Tutar}") != -1)
                            mailmetin = mailmetin.Replace("{Tutar}", Tutar.ToString());
                        if (mailmetin.IndexOf("{GelinAdSoyad}") != -1)
                            mailmetin = mailmetin.Replace("{GelinAdSoyad}", "");
                        if (mailmetin.IndexOf("{DamatAdSoyad}") != -1)
                            mailmetin = mailmetin.Replace("{DamatAdSoyad}", "");
                        if (mailmetin.IndexOf("{RandevuTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{RandevuTarihi}", "");
                        if (mailmetin.IndexOf("{OpsiyonTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OpsiyonTarihi}", "");
                        if (mailmetin.IndexOf("{SozlesmeNo}") != -1)
                            mailmetin = mailmetin.Replace("{SozlesmeNo}", mn.ToString());
                        if (mailmetin.IndexOf("{MakbuzNo}") != -1)
                            mailmetin = mailmetin.Replace("{MakbuzNo}", mn.ToString());
                        if (mailmetin.IndexOf("{CekimYeri}") != -1)
                            mailmetin = mailmetin.Replace("{CekimYeri}", "");
                        if (mailmetin.IndexOf("{CekimSaati}") != -1)
                            mailmetin = mailmetin.Replace("{CekimSaati}", "");


                        string konu = mmetin.MailKonu;
                        string baslik = mmetin.MailBaslik;
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
                            body = body.Replace("{MailBaslik}", baslik);
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

                            mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, cari.Email, konu, body, htmlView);
                        }
                        else
                            mailsonuc = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, cari.Email, konu, mailmetin);
                    }
                }

                #endregion

                return Json(new { Sonuc = true, CariList = carilist, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Tahsilat Yap, Kayıt Id: " + id;
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
        [HttpPost]
        public ActionResult OdemeYap(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long CariId = Convert.ToInt64(Request["CariId"]);
            long RolId = Convert.ToInt64(Session["RolId"]);
            DateTime IslemTarihi = Convert.ToDateTime(Request["IslemTarihi"]);
            DateTime OdemeTarihi = Convert.ToDateTime(Request["OdemeTarihi"]);
            decimal Tutar = Convert.ToDecimal(Request["Tutar"]);
            string Tip = Request["Tip"];
            string Aciklama = Request["Aciklama"];
            string OdemeSekli = Request["OdemeSekli"];
            //string OdemeIslem = Request["OdemeIslem"];
            //string TahsilatIslem = Request["TahsilatIslem"];
            string OdemeYapan = Request["OdemeYapan"];

            string GGTip = "Gider", carihareketaciklama = "";
            bool mailsonuc;
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
            Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == CariId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (cari == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            if (cari.FirmaAdi == "")
                carihareketaciklama = cari.AdSoyad;
            else if (cari.AdSoyad == "")
                carihareketaciklama = cari.FirmaAdi + " - " + cari.Yetkili;
            #region CariHareket Ekleme işlemleri
            CariHareket carihareket = dbContext.CariHarekets.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id && x.Aktif == true && x.Sil == false);

            carihareket.IslemTarihi = IslemTarihi;
            carihareket.OdemeTarihi = OdemeTarihi;
            carihareket.Tip = Tip;
            carihareket.OdemeTuru = OdemeSekli;
            carihareket.Notlar = Aciklama;
            carihareket.OdemeYapanAdSoyad = OdemeYapan;
            carihareket.TahsilatOdemeBit = true;
            carihareket.DegistirenKullaniciId = KullaniciId;
            carihareket.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                #region MakbuzNo
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
                #endregion

                #region Tahsilattan kalan tutarı yeniden ekle
                if (Tutar < carihareket.Tutar)
                {
                    if (SubeId == 0)
                        SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
                    CariHareket yenihareket = new CariHareket();
                    yenihareket.FirmaId = FirmaId;
                    yenihareket.SubeId = SubeId;
                    yenihareket.CariId = CariId;
                    yenihareket.IslemTarihi = IslemTarihi;
                    yenihareket.OdemeTarihi = OdemeTarihi;
                    yenihareket.Tip = "Borç";
                    yenihareket.OdemeTuru = "---";
                    yenihareket.Tutar = carihareket.Tutar - Tutar;
                    yenihareket.Notlar = IslemTarihi.ToShortDateString() + " Tarihli Borcun " + OdemeTarihi.ToShortDateString() + " tarihindeki Ödemesinden kalan";
                    yenihareket.OdemeYapanAdSoyad = "";
                    yenihareket.TahsilatOdemeBit = false;
                    yenihareket.Aktif = true;
                    yenihareket.Sil = false;
                    yenihareket.OlusturanKullaniciId = KullaniciId;
                    yenihareket.OlusturmaTarih = DateTime.Now;
                    yenihareket.DegistirenKullaniciId = KullaniciId;
                    yenihareket.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.CariHarekets.Add(yenihareket);
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

                #region Gelir yada giler olarak ekleme işlemleri
                if (SubeId == 0)
                    SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
                GelirGiderTurleri gtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.GelirGiderTur == "Cari İşlem Ödemesi");
                GelirGider gelirgider = new GelirGider();
                if (gtur != null)
                {
                    gelirgider.GelirGiderTurId = gtur.Id;
                }
                else
                {
                    GelirGiderTurleri ggtur = new GelirGiderTurleri();
                    ggtur.FirmaId = FirmaId; // sessiondaki FirmaID
                    ggtur.GelirGiderTur = "Cari İşlem Ödemesi";
                    ggtur.OlusturanKullaniciId = KullaniciId;
                    ggtur.OlusturmaTarih = DateTime.Now;
                    ggtur.DegistirenKullaniciId = KullaniciId;
                    ggtur.DegistirmeTarih = DateTime.Now;
                    ggtur.Aktif = true;
                    ggtur.Sil = false;
                    ggtur.KilitBit = true;
                    try
                    {
                        dbContext.GelirGiderTurleris.Add(ggtur);
                        dbContext.SaveChanges();
                        gelirgider.GelirGiderTurId = ggtur.Id;
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Cari Hareket Ekle, Gelir gider Eklemek için Gelir Gider Türü: Cari İşlem Ödemesi, ekle işleminde oluşan hata";
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
                gelirgider.FirmaId = FirmaId;
                gelirgider.SubeId = SubeId;
                gelirgider.Tarih = OdemeTarihi;
                gelirgider.SozlesmeId = 1;
                gelirgider.GisId = 1;
                gelirgider.OdemeId = 0;
                gelirgider.PersonelOdemeId = 0;
                gelirgider.CariHareketId = carihareket.Id;
                gelirgider.Tip = GGTip;
                gelirgider.Tutar = Tutar;
                gelirgider.OdemeTuru = OdemeSekli;
                gelirgider.MakbuzNo = Convert.ToInt64(mn); // GelirGider tablosundaki MakbuzNo = Odemler tablosundaki MakbuzNo
                gelirgider.Notlar = carihareketaciklama + " isimli cariye yapılan Ödeme.";
                gelirgider.KilitBit = false;
                gelirgider.Aktif = true;
                gelirgider.Sil = false;
                gelirgider.OlusturanKullaniciId = KullaniciId;
                gelirgider.OlusturmaTarih = DateTime.Now;
                gelirgider.DegistirenKullaniciId = KullaniciId;
                gelirgider.DegistirmeTarih = DateTime.Now;

                try
                {
                    dbContext.GelirGiders.Add(gelirgider);
                    dbContext.SaveChanges();

                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Ödeme Yap, Gelir Gider Olarak Ekle, Kayıt Id:" + id;
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
                #region Sms Bilgi Mesajı
                if (cari.CepTel != "" && cari.SMSKabul == true) // Cariye bir cep telefonu girilmiş ise SMS gönder
                {
                    AyarlarSmsGonderim smsayar = null;
                    string smsmetin = "";
                    smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariyeYapilanOdemeBilgiGonderimSuresi == 6 && x.CariyeYapilanOdemeBilgiMesaji != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (smsayar != null)
                        smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == smsayar.CariyeYapilanOdemeBilgiMesaji && x.Aktif == true && x.Sil == false).SMSMetni;
                    else
                        smsmetin = null;
                    if (smsayar != null && smsmetin != null)
                    {
                        if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            if (cari.AdSoyad == "")  // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                            else                   // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                        if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            if (cari.AdSoyad == "") // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                            else                    // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                        if (smsmetin.IndexOf("{FirmaAdi}") != -1)
                            smsmetin = smsmetin.Replace("{FirmaAdi}", cari.FirmaAdi);
                        if (smsmetin.IndexOf("{Tarih}") != -1)
                            smsmetin = smsmetin.Replace("{Tarih}", IslemTarihi.ToShortDateString());
                        if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                        if (smsmetin.IndexOf("{Tutar}") != -1)
                            smsmetin = smsmetin.Replace("{Tutar}", Tutar.ToString());
                        if (smsmetin.IndexOf("{GelinAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{GelinAdSoyad}", "");
                        if (smsmetin.IndexOf("{DamatAdSoyad}") != -1)
                            smsmetin = smsmetin.Replace("{DamatAdSoyad}", "");
                        if (smsmetin.IndexOf("{RandevuTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{RandevuTarihi}", "");
                        if (smsmetin.IndexOf("{OpsiyonTarihi}") != -1)
                            smsmetin = smsmetin.Replace("{OpsiyonTarihi}", "");
                        if (smsmetin.IndexOf("{SozlesmeNo}") != -1)
                            smsmetin = smsmetin.Replace("{SozlesmeNo}", mn.ToString());
                        if (smsmetin.IndexOf("{MakbuzNo}") != -1)
                            smsmetin = smsmetin.Replace("{MakbuzNo}", mn.ToString());
                        if (smsmetin.IndexOf("{CekimYeri}") != -1)
                            smsmetin = smsmetin.Replace("{CekimYeri}", "");
                        if (smsmetin.IndexOf("{CekimSaati}") != -1)
                            smsmetin = smsmetin.Replace("{CekimSaati}", "");
                        SMSGonder.Gonder_AtakSms(smsmetin, cari.CepTel, FirmaId, SubeId, KullaniciId);
                    }
                }
                #endregion
                #region Email Bilgi Maili
                if (cari.Email != "" && cari.EmailKabul == true) // Cariye bir cep telefonu girilmiş ise SMS gönder
                {
                    AyarlarMailGonderim mailayar = null;
                    MailMetinleri mmetin = null;
                    string mailmetin = "";
                    mailayar = dbContext.AyarlarMailGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.CariyeYapilanOdemeBilgiGonderimSuresi == 6 && x.CariyeYapilanOdemeBilgiMaili != 0);  // Sms gönderim ayarı CariOdemeBilgiMesajı için "Kayıt Yapıldığında" Seçilmişse
                    if (mailayar == null)
                        return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı. Mail gönderim ayarları yapılandırımmaış. Bilgi Maili gönderilemedi." }, JsonRequestBehavior.AllowGet);
                    else
                        mmetin = dbContext.MailMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == mailayar.CariyeYapilanOdemeBilgiMaili && x.Aktif == true && x.Sil == false);
                    if (mailayar != null && mmetin != null)
                    {
                        mailmetin = mmetin.MailMetni;
                        if (mailmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                            if (cari.AdSoyad == "")  // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                            else                   // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                        if (mailmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                            if (cari.AdSoyad == "") // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                                mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.Yetkili);
                            else                    // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                                mailmetin = mailmetin.Replace("{YetkiliAdSoyad}", cari.AdSoyad);
                        if (mailmetin.IndexOf("{FirmaAdi}") != -1)
                            mailmetin = mailmetin.Replace("{FirmaAdi}", cari.FirmaAdi);
                        if (mailmetin.IndexOf("{Tarih}") != -1)
                            mailmetin = mailmetin.Replace("{Tarih}", IslemTarihi.ToShortDateString());
                        if (mailmetin.IndexOf("{OdemeTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
                        if (mailmetin.IndexOf("{Tutar}") != -1)
                            mailmetin = mailmetin.Replace("{Tutar}", Tutar.ToString());
                        if (mailmetin.IndexOf("{GelinAdSoyad}") != -1)
                            mailmetin = mailmetin.Replace("{GelinAdSoyad}", "");
                        if (mailmetin.IndexOf("{DamatAdSoyad}") != -1)
                            mailmetin = mailmetin.Replace("{DamatAdSoyad}", "");
                        if (mailmetin.IndexOf("{RandevuTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{RandevuTarihi}", "");
                        if (mailmetin.IndexOf("{OpsiyonTarihi}") != -1)
                            mailmetin = mailmetin.Replace("{OpsiyonTarihi}", "");
                        if (mailmetin.IndexOf("{SozlesmeNo}") != -1)
                            mailmetin = mailmetin.Replace("{SozlesmeNo}", mn.ToString());
                        if (mailmetin.IndexOf("{MakbuzNo}") != -1)
                            mailmetin = mailmetin.Replace("{MakbuzNo}", mn.ToString());
                        if (mailmetin.IndexOf("{CekimYeri}") != -1)
                            mailmetin = mailmetin.Replace("{CekimYeri}", "");
                        if (mailmetin.IndexOf("{CekimSaati}") != -1)
                            mailmetin = mailmetin.Replace("{CekimSaati}", "");


                        string konu = mmetin.MailKonu;
                        string baslik = mmetin.MailBaslik;
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
                            body = body.Replace("{MailBaslik}", baslik);
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

                            mailsonuc = MailGonder.Gonder(FirmaId, SubeId, KullaniciId, cari.Email, konu, body, htmlView);
                        }
                        else
                            mailsonuc = MailGonder.Gonder_Text(FirmaId, SubeId, KullaniciId, cari.Email, konu, mailmetin);

                    }
                }

                #endregion
                SubeId = Convert.ToInt64(Session["AktifSubeId"]);
                List<CariHareket> CariHareketListesi = new List<CariHareket>();
                if (SubeId == 0) // Giriş Yapan Kullanıcı Firma Yetkilis ise
                    CariHareketListesi = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.OdemeTarihi == DateTime.Today && x.TahsilatOdemeBit == false && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();
                else
                    CariHareketListesi = dbContext.CariHarekets.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.OdemeTarihi == DateTime.Today && x.TahsilatOdemeBit == false && x.Aktif == true && x.Sil == false).OrderByDescending(x => x.Id).Take(10).ToList();

                var carilist = CariHareketListesi.Select(m => new
                {
                    Id = m.Id,
                    SubeAdi = m.Sube.SubeAdi,
                    FirmaAdi = m.Firma.FirmaAdi,
                    CariId = m.CariId,
                    CariFirma = m.Cari.FirmaAdi,
                    CariYetkili = m.Cari.Yetkili,
                    CariAdSoyad = m.Cari.AdSoyad,
                    CariSabitTel = m.Cari.SabitTel,
                    CariCepTel = m.Cari.CepTel,
                    CariFax = m.Cari.Fax,
                    CariEmail = m.Cari.Email,
                    CariVergiDairesi = m.Cari.VergiDairesi,
                    CariVergiNo = m.Cari.VergiNo,
                    CariTcKimlikNo = m.Cari.TCKimlikNo,
                    CariAdres = m.Cari.Adres,
                    CariAciklama = m.Cari.Notlar,
                    IslemTarihi = m.IslemTarihi.ToString("dd/MM/yyyy"),
                    OdemeTarihi = m.OdemeTarihi.ToString("dd/MM/yyyy"),
                    OdemeYapan = m.OdemeYapanAdSoyad,
                    Tip = m.Tip,
                    TahsilatOdemeBit = m.TahsilatOdemeBit,
                    Tutar = m.Tutar,
                    OdemeSekli = m.OdemeTuru,
                    Aciklama = m.Notlar
                }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.

                return Json(new { Sonuc = true, CariList = carilist, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Ödeme Yap, Kayıt Id: " + id;
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
        [HttpPost]
        public ActionResult CariSMSGonder()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long CariId = Convert.ToInt64(Request["CariId"]);
            long SmsMetinId = Convert.ToInt64(Request["SmsMetinId"]);
            DateTime OdemeTarihi = Convert.ToDateTime(Request["OdemeTarihi"]);
            //string tutar1 = Request["Tutar"];
            //tutar1 = tutar1.Replace("TL", "");
            decimal Tutar = Convert.ToDecimal(Request["Tutar"]);
            string SmsMetin = Request["SmsMetin"];
            // Alacak Metni için SMS metinleri içinden seçim yapılacak.

            string sonuc_musteri_randevu_sms = "";
            Models.Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == CariId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            bool smskabul = true;
            if (cari == null)
                return Json(new { Sonuc = false, Mesaj = "SMS Gönderilemedi. Cari Silinmiş veya Cari Yok." }, JsonRequestBehavior.AllowGet);
            else
                smskabul = cari.SMSKabul;
            sonuc_musteri_randevu_sms = SMSGonder.Gonder_AtakSms(SmsMetin, cari.CepTel, FirmaId, SubeId, KullaniciId);
            return Json(new { Sonuc = true, Mesaj = sonuc_musteri_randevu_sms }, JsonRequestBehavior.AllowGet);
            #region Alınan Ödeme Bilgi Smsi
            //if (musteri.AdiSoyadi != "-- Müşterisiz İşlem --" && musteri.CepTel != "" && smskabul) // Cariye bir cep telefonu girilmiş ise SMS gönder
            //{

            //    string smsmetin = "";

            //    smsmetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == SmsMetinId && x.Aktif == true && x.Sil == false).SMSMetni;

            //    if (smsmetin != null)
            //    {
            //        if (smsmetin.IndexOf("{FirmaAdi}") != -1)
            //            smsmetin = smsmetin.Replace("{FirmaAdi}", musteri.AdiSoyadi);

            //        if (smsmetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
            //            smsmetin = smsmetin.Replace("{YetkiliAdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
            //        if (smsmetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
            //            smsmetin = smsmetin.Replace("{AdSoyad}", musteri.AdiSoyadi);  //Müşteri AdSoyad bilgisini Parametre olarak ekle
            //        if (smsmetin.IndexOf("{Tarih}") != -1)
            //            smsmetin = smsmetin.Replace("{Tarih}", OdemeTarihi.ToShortDateString());
            //        if (smsmetin.IndexOf("{OdemeTarihi}") != -1)
            //            smsmetin = smsmetin.Replace("{OdemeTarihi}", OdemeTarihi.ToShortDateString());
            //        if (smsmetin.IndexOf("{Tutar}") != -1)
            //            smsmetin = smsmetin.Replace("{Tutar}", Tutar.ToString());
            //        sonuc_musteri_randevu_sms = SMSGonder.Gonder_AtakSms(smsmetin, musteri.CepTel, FirmaId, SubeId, KullaniciId);
            //        return Json(new { Sonuc = true, Mesaj = sonuc_musteri_randevu_sms }, JsonRequestBehavior.AllowGet);
            //    }
            //}
            #endregion
            //return Json(new { Sonuc = false, Mesaj = "SMS Gönderilemedi." }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CariHareketGuncelle(long? id)
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
            long CariId = Convert.ToInt64(Request["CariId"]);
            DateTime IslemTarihi = Convert.ToDateTime(Request["IslemTarihi"]);
            DateTime OdemeTarihi = Convert.ToDateTime(Request["OdemeTarihi"]);
            decimal Tutar = Convert.ToDecimal(Request["Tutar"]);
            string Tip = Request["Tip"];
            string Aciklama = Request["Aciklama"];
            string OdemeSekli = Request["OdemeSekli"];
            string OdemeIslem = Request["OdemeIslem"];
            string TahsilatIslem = Request["TahsilatIslem"];
            string OdemeYapan = Request["OdemeYapan"];

            bool borcodendi = false, tahsilatyapildi = false;
            string GGTip = "", carihareketaciklama = "";
            if (Tip == "Alacak")
            {
                GGTip = "Gelir";
                if (TahsilatIslem == "yapıldı")
                    tahsilatyapildi = true;
                else
                    tahsilatyapildi = false;
            }

            else if (Tip == "Borç")
            {
                GGTip = "Gider";
                if (OdemeIslem == "yapıldı")
                    borcodendi = true;
                else
                    borcodendi = false;
            }

            Cari cari = dbContext.Caris.FirstOrDefault(x => x.Id == CariId && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (cari == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            if (cari.FirmaAdi == "")
                carihareketaciklama = cari.AdSoyad;
            else if (cari.AdSoyad == "")
                carihareketaciklama = cari.FirmaAdi + " - " + cari.Yetkili;

            #region CariHareket Güncelleme işlemleri
            CariHareket carihareket = dbContext.CariHarekets.FirstOrDefault(x => x.Id == id && x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (carihareket == null)
                return Json(new { Sonuc = false, Bilgi = "Güncellenmek istenilen kayıt bulunamadı", JsonRequestBehavior.AllowGet });
            #region Gelir yada giler olarak ekleme işlemleri
            if (tahsilatyapildi == true || borcodendi == true)
            {
                #region Tahsilattan veya Ödemeden kalan tutarı yeniden ekle
                if (Tutar < carihareket.Tutar)
                {
                    CariHareket yenihareket = new CariHareket();
                    yenihareket.FirmaId = FirmaId;
                    yenihareket.SubeId = SubeId;
                    yenihareket.CariId = CariId;
                    yenihareket.IslemTarihi = IslemTarihi;
                    yenihareket.OdemeTarihi = OdemeTarihi;
                    yenihareket.Tip = Tip;
                    yenihareket.OdemeTuru = "---";
                    yenihareket.Tutar = carihareket.Tutar - Tutar;
                    if (Tip == "Alacak")
                        yenihareket.Notlar = IslemTarihi.ToShortDateString() + " Tarihli Alacağın " + OdemeTarihi.ToShortDateString() + " tarihindeki Tahsilatından kalan";
                    else
                        yenihareket.Notlar = IslemTarihi.ToShortDateString() + " Tarihli Borcun " + OdemeTarihi.ToShortDateString() + " tarihindeki Ödemesinden kalan";
                    yenihareket.OdemeYapanAdSoyad = "";
                    yenihareket.TahsilatOdemeBit = false;
                    yenihareket.Aktif = true;
                    yenihareket.Sil = false;
                    yenihareket.OlusturanKullaniciId = KullaniciId;
                    yenihareket.OlusturmaTarih = DateTime.Now;
                    yenihareket.DegistirenKullaniciId = KullaniciId;
                    yenihareket.DegistirmeTarih = DateTime.Now;
                    try
                    {
                        dbContext.CariHarekets.Add(yenihareket);
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Cari Hareket Ekle, Tahsilattan kalan tutarı Alacak olarak yeniden ekle";
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
                GelirGiderTurleri gtur = dbContext.GelirGiderTurleris.FirstOrDefault(x => x.FirmaId == FirmaId && x.GelirGiderTur == "Cari İşlem Ödemesi");
                GelirGider gelirgider = new GelirGider();
                if (gtur != null)
                {
                    gelirgider.GelirGiderTurId = gtur.Id;
                }
                else
                {
                    GelirGiderTurleri ggtur = new GelirGiderTurleri();
                    ggtur.FirmaId = FirmaId; // sessiondaki FirmaID
                    ggtur.GelirGiderTur = "Cari İşlem Ödemesi";
                    ggtur.OlusturanKullaniciId = KullaniciId;
                    ggtur.OlusturmaTarih = DateTime.Now;
                    ggtur.DegistirenKullaniciId = KullaniciId;
                    ggtur.DegistirmeTarih = DateTime.Now;
                    ggtur.Aktif = true;
                    ggtur.Sil = false;
                    ggtur.KilitBit = true;
                    try
                    {
                        dbContext.GelirGiderTurleris.Add(ggtur);
                        dbContext.SaveChanges();
                        gelirgider.GelirGiderTurId = ggtur.Id;
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Cari Hareket Ekle, Gelir gider Eklemek için Gelir Gider Türü: Cari İşlem Ödemesi, ekle işleminde oluşan hata";
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
                gelirgider.FirmaId = FirmaId;
                gelirgider.SubeId = SubeId;
                gelirgider.Tarih = OdemeTarihi;
                gelirgider.SozlesmeId = 1;
                gelirgider.GisId = 1;
                gelirgider.OdemeId = 0;
                gelirgider.CariHareketId = carihareket.Id;
                gelirgider.Tip = GGTip;
                gelirgider.Tutar = Tutar;
                gelirgider.OdemeTuru = OdemeSekli;
                if (tahsilatyapildi)
                    gelirgider.Notlar = carihareketaciklama + " isimli cariyden yapılan Tahsilat.";
                else if (borcodendi)
                    gelirgider.Notlar = carihareketaciklama + " isimli cariye yapılan Ödeme.";
                gelirgider.KilitBit = false;
                gelirgider.Aktif = true;
                gelirgider.Sil = false;
                gelirgider.OlusturanKullaniciId = KullaniciId;
                gelirgider.OlusturmaTarih = DateTime.Now;
                gelirgider.DegistirenKullaniciId = KullaniciId;
                gelirgider.DegistirmeTarih = DateTime.Now;

                try
                {
                    dbContext.GelirGiders.Add(gelirgider);
                    dbContext.SaveChanges();
                    //return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Cari Hareket Ekle, Gelir Gider Olarak Ekle";
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

            carihareket.IslemTarihi = IslemTarihi;
            carihareket.OdemeTarihi = OdemeTarihi;
            carihareket.Tip = Tip;
            carihareket.OdemeTuru = OdemeSekli;
            carihareket.Tutar = Tutar;
            carihareket.Notlar = Aciklama;
            carihareket.OdemeYapanAdSoyad = OdemeYapan;
            if (tahsilatyapildi == true || borcodendi == true)
                carihareket.TahsilatOdemeBit = true;
            else
                carihareket.TahsilatOdemeBit = false;
            carihareket.DegistirenKullaniciId = KullaniciId;
            carihareket.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                // Alacağın Tahsilatı veya Borcun Ödemesi Yapıldı seçildi ise Gelir-Gider e Tahislat/Borç Tutarı eklenecek.

            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Cari Hareket Güncelle, Kayıt Id: " + id.ToString();
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
        [HttpPost]
        public ActionResult CariHareketSil(long? id)
        {
            // Personelin kullanıldığı tablolarda kayıtlar var ise personel silinemeyecek
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            CariHareket c = dbContext.CariHarekets.FirstOrDefault(x => x.Id == id);
            c.Aktif = false;
            c.Sil = true;
            c.DegistirenKullaniciId = KullaniciId;
            c.DegistirmeTarih = DateTime.Now;
            GelirGider gg = dbContext.GelirGiders.FirstOrDefault(x => x.CariHareketId == id && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (gg != null)
            {
                gg.Aktif = false;
                gg.Sil = true;
                gg.DegistirenKullaniciId = KullaniciId;
                gg.DegistirmeTarih = DateTime.Now;
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
                hata.Islem = "Cari Hareket Sil, Gelir Gider kaydı veya Cari hareket kaydı silinirken oluşan hata, Kayıt Id: " + id.ToString();
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
        public ActionResult Kasa()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Muhasebe";
            ViewBag.AltMenu = "Kasa";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            ViewBag.AktifSubeId = Convert.ToInt64(Session["AktifSubeId"]);
            KullaniciYetki SayfaYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 33 && x.Aktif == true && x.Sil == false);
            if (!SayfaYetki.SayfaYetki || SayfaYetki == null)
                return RedirectToAction("YetkisizGiris", "Hata");
            List<Sube> sube = dbContext.Subes.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList();
            string filtreayar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).Kasa;
            List<SubeToKullanici> subekullanici = dbContext.SubeToKullanicis.Where(x => x.KullaniciId == KullaniciId).ToList();
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

            List<GelirGider> gelir;
            List<GelirGider> gider;
            if (SubeId == 0)
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == "Gelir" && ((x.Tarih.Day >= IlkTarih.Day && x.Tarih.Month >= IlkTarih.Month && x.Tarih.Year >= IlkTarih.Year) && (x.Tarih.Day <= SonTarih.Day && x.Tarih.Month <= SonTarih.Month && x.Tarih.Year <= SonTarih.Year)) && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == "Gider" && ((x.Tarih.Day >= IlkTarih.Day && x.Tarih.Month >= IlkTarih.Month && x.Tarih.Year >= IlkTarih.Year) && (x.Tarih.Day <= SonTarih.Day && x.Tarih.Month <= SonTarih.Month && x.Tarih.Year <= SonTarih.Year)) && x.Aktif == true && x.Sil == false).ToList();
            }
            else
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gelir" && ((x.Tarih.Day >= IlkTarih.Day && x.Tarih.Month >= IlkTarih.Month && x.Tarih.Year >= IlkTarih.Year) && (x.Tarih.Day <= SonTarih.Day && x.Tarih.Month <= SonTarih.Month && x.Tarih.Year <= SonTarih.Year)) && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gider" && ((x.Tarih.Day >= IlkTarih.Day && x.Tarih.Month >= IlkTarih.Month && x.Tarih.Year >= IlkTarih.Year) && (x.Tarih.Day <= SonTarih.Day && x.Tarih.Month <= SonTarih.Month && x.Tarih.Year <= SonTarih.Year)) && x.Aktif == true && x.Sil == false).ToList();
            }


            decimal toplamgelir = gelir.Sum(x => x.Tutar);
            decimal toplamgider = gider.Sum(x => x.Tutar);
            decimal kasa = toplamgelir - toplamgider;

            ViewBag.FiltreAyar = filtreayar;
            ViewBag.ToplamGelir = toplamgelir;
            ViewBag.ToplamGider = toplamgider;
            ViewBag.Kasa = kasa;
            ViewBag.SubeListesi = sube;
            ViewBag.SubeKullanici = subekullanici;
            return View();
        }
        [HttpPost]
        public ActionResult KasaFiltrele()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            DateTime IlkTarih = Convert.ToDateTime(Request["IlkTarih"]);
            DateTime SonTarih = Convert.ToDateTime(Request["SonTarih"]).AddHours(23).AddMinutes(59).AddSeconds(59);
            SubeId = Convert.ToInt64(Request["SubeId"]);
            string OdemeSekli = Request["OdemeSekli"];
            List<GelirGider> gelir;
            List<GelirGider> gider;

            if (SubeId == 0 && OdemeSekli == "Tümü")
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == "Gelir" && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == "Gider" && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.Aktif == true && x.Sil == false).ToList();
            }
            else if (SubeId != 0 && OdemeSekli == "Tümü")
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gelir" && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gider" && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.Aktif == true && x.Sil == false).ToList();
            }
            else if (SubeId != 0 && OdemeSekli != "Tümü")
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gelir" && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.OdemeTuru == OdemeSekli && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gider" && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.OdemeTuru == OdemeSekli && x.Aktif == true && x.Sil == false).ToList();
            }
            else if (SubeId == 0 && OdemeSekli != "Tümü")
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == "Gelir" && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.OdemeTuru == OdemeSekli && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.Tip == "Gider" && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.OdemeTuru == OdemeSekli && x.Aktif == true && x.Sil == false).ToList();
            }
            else
            {
                gelir = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gelir" && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.OdemeTuru == OdemeSekli && x.Aktif == true && x.Sil == false).ToList();
                gider = dbContext.GelirGiders.Where(x => x.FirmaId == FirmaId && x.SubeId == SubeId && x.Tip == "Gider" && ((x.Tarih >= IlkTarih) && (x.Tarih <= SonTarih)) && x.OdemeTuru == OdemeSekli && x.Aktif == true && x.Sil == false).ToList();
            }

            decimal toplamgelir = gelir.Sum(x => x.Tutar);
            decimal toplamgider = gider.Sum(x => x.Tutar);
            decimal kasa = toplamgelir - toplamgider;

            return Json(new { Sonuc = true, Mesaj = "Filtreleme Başarılı", ToplamGelir = toplamgelir, ToplamGider = toplamgider, Kasa = kasa }, JsonRequestBehavior.AllowGet);
        }
    }
}