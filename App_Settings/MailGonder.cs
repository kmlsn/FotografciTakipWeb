using FotografciTakipWeb.Models;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using FotografciTakipWeb.App_Settings;

namespace FotografciTakipWeb.App_Settings
{

    public class MailGonder
    {
        public static bool Gonder(long FirmaId, long SubeId, long KullaniciId, string aliciMail, string konu, string mailMesaj, AlternateView htmlView)
        {
            // 1. Yol
            //var gonderenadres = new MailAddress(gonderenMail, gonderenIsim);
            //var alici = new MailAddress(aliciMail);
            ////string konu = "Konu: Mesajınız var!";

            //using (var smtp = new SmtpClient
            //{
            //    Host = smtpAdres,
            //    Port = smtpPort,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(smtpMail, smtpMailSifre)
            //})
            //{
            //    using (var mesaj = new MailMessage(gonderenadres, alici) { Subject = konu, IsBodyHtml = true, Body = mailMesaj })
            //    {
            //        smtp.Send(mesaj);
            //    }
            //}

            // 2. Yol
            FotoTakipContext dbContext = new FotoTakipContext();
            AyarlarMailHesap ayar = new AyarlarMailHesap();
            MD5Sifreleme sifrele = new MD5Sifreleme();
            // Firma eposta ayarları yapılmamış ise TeTRa Bilişim e tanımlanan ayarlar (info@fotografcitakip.com) ile mail gönderilecek.
            ayar = dbContext.AyarlarMailHesaps.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (ayar == null)
            {
                ayar = dbContext.AyarlarMailHesaps.FirstOrDefault(x => x.FirmaId == 1 && x.Sil == false && x.Aktif == true);
                //return Json(new { Sonuc = false, mesaj = "Mail Gönderim Ayarları yapılmamış.<br/> Lütfen Mail Gönderim Ayarlarını yapınız. ", JsonRequestBehavior.AllowGet });
            }
           
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ayar.GonderenMail, ayar.GonderenAdSoyad);
            mail.To.Add(aliciMail);
            mail.Subject = (konu);
            //mail.Body = (mailMesaj);
            mail.AlternateViews.Add(htmlView);
            mail.IsBodyHtml = true;
           
            // gönderen maile ait mail ayarları
            SmtpClient smtp2 = new SmtpClient();
            if (ayar.Id==1)
                smtp2.Credentials = new System.Net.NetworkCredential(ayar.GonderenMail, ayar.GonderenSifre);
            else
                smtp2.Credentials = new System.Net.NetworkCredential(ayar.GonderenMail, sifrele.SifreCoz(ayar.GonderenSifre));
            smtp2.Port = Convert.ToInt32(ayar.SmtpPort);
            smtp2.Host = ayar.SmtpAdres;
            smtp2.EnableSsl =ayar.Ssl;
            // Mail Gönderme işlemi
            //smtp2.SendAsync(mail, (object)mail);
            try
            {
                smtp2.Send(mail);
                GonderilenEmailler gonderilenmail = new GonderilenEmailler();
                gonderilenmail.FirmaId = FirmaId;
                gonderilenmail.SubeId = SubeId;
                gonderilenmail.MailKonu = konu;
                gonderilenmail.AliciEposta = aliciMail;
                gonderilenmail.MailIcerik = mailMesaj;
                gonderilenmail.TemaYol = "";
                gonderilenmail.OlusturanKullaniciId = KullaniciId;
                gonderilenmail.OlusturmaTarih = DateTime.Now;
                gonderilenmail.DegistirenKullaniciId = KullaniciId;
                gonderilenmail.DegistirmeTarih = DateTime.Now;
                gonderilenmail.Aktif = true;
                gonderilenmail.Sil = false;
                dbContext.GonderilenEmaillers.Add(gonderilenmail);
                try
                {
                    return true;
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Gönderilen Mail Kaydedilemedi";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return false;
                }
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Mail Gönderilemedi";
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return false;
            }
        }
        public static bool Gonder_Text(long FirmaId, long SubeId, long KullaniciId, string aliciMail, string konu, string mailMesaj)
        {
            // 1. Yol
            //var gonderenadres = new MailAddress(gonderenMail, gonderenIsim);
            //var alici = new MailAddress(aliciMail);
            ////string konu = "Konu: Mesajınız var!";

            //using (var smtp = new SmtpClient
            //{
            //    Host = smtpAdres,
            //    Port = smtpPort,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(smtpMail, smtpMailSifre)
            //})
            //{
            //    using (var mesaj = new MailMessage(gonderenadres, alici) { Subject = konu, IsBodyHtml = true, Body = mailMesaj })
            //    {
            //        smtp.Send(mesaj);
            //    }
            //}

            // 2. Yol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            FotoTakipContext dbContext = new FotoTakipContext();
            AyarlarMailHesap ayar = new AyarlarMailHesap();
            MD5Sifreleme sifrele = new MD5Sifreleme();
            // Firma eposta ayarları yapılmamış ise TeTRa Bilişim e tanımlanan ayarlar (info@fotografcitakip.com) ile mail gönderilecek.
            ayar = dbContext.AyarlarMailHesaps.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (ayar == null)
            {
                ayar = dbContext.AyarlarMailHesaps.FirstOrDefault(x => x.FirmaId == 1 && x.Sil == false && x.Aktif == true);
                //return Json(new { Sonuc = false, mesaj = "Mail Gönderim Ayarları yapılmamış.<br/> Lütfen Mail Gönderim Ayarlarını yapınız. ", JsonRequestBehavior.AllowGet });
            }

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ayar.GonderenMail, ayar.GonderenAdSoyad);
            mail.To.Add(aliciMail);
            mail.Subject = (konu);
            mail.Body = (mailMesaj);
            //mail.AlternateViews.Add(htmlView);
            mail.IsBodyHtml = true;

            // gönderen maile ait mail ayarları
            SmtpClient smtp2 = new SmtpClient();
            smtp2.Credentials = new System.Net.NetworkCredential(ayar.GonderenMail, sifrele.SifreCoz(ayar.GonderenSifre));
            smtp2.Port = Convert.ToInt32(ayar.SmtpPort);
            smtp2.Host = ayar.SmtpAdres;
            smtp2.EnableSsl = ayar.Ssl;
            // Mail Gönderme işlemi
            //smtp2.SendAsync(mail, (object)mail);
            try
            {
                smtp2.Send(mail);
                GonderilenEmailler gonderilenmail = new GonderilenEmailler();
                gonderilenmail.FirmaId = FirmaId;
                gonderilenmail.SubeId = SubeId;
                gonderilenmail.MailKonu = konu;
                gonderilenmail.AliciEposta = aliciMail;
                gonderilenmail.MailIcerik = mailMesaj;
                gonderilenmail.TemaYol = "";
                gonderilenmail.OlusturanKullaniciId = KullaniciId;
                gonderilenmail.OlusturmaTarih = DateTime.Now;
                gonderilenmail.DegistirenKullaniciId = KullaniciId;
                gonderilenmail.DegistirmeTarih = DateTime.Now;
                gonderilenmail.Aktif = true;
                gonderilenmail.Sil = false;
                dbContext.GonderilenEmaillers.Add(gonderilenmail);
                try
                {
                    return true;
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Gönderilen Mail Kaydedilemedi";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return false;
                }
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Mail Gönderilemedi";
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return false;
            }
        }

        //public static void Gonder2(string mailMesaj)
        //{
        //    // 1.Yol
        //    var gonderenadres = new MailAddress("kamil@alternatifmed.com.tr");
        //    var alici = new MailAddress("kamil@alternatifmed.com.tr");
        //    //string konu = "Konu: Mesajınız var!";

        //    using (var smtp = new SmtpClient
        //    {
        //        Host = "srvm01.turhost.com",
        //        Port = 465,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(gonderenadres.Address, "442Bir726")
        //    })
        //    {
        //        using (var mesaj = new MailMessage(gonderenadres, alici) { Subject = "Demene", Body = mailMesaj })
        //        {
        //            smtp.Send(mesaj);
        //        }
        //    }

        //    // 2. Yol
        //    //MailMessage mail = new MailMessage();
        //    //mail.From = new MailAddress("kamil@vefaliparasayma.com", "Kamil ŞEN - Deneme");
        //    //mail.To.Add("kamil@vefaliparasayma.com");
        //    //mail.Subject = ("Mesaj Deneme");
        //    //mail.Body = (mailMesaj);
        //    //mail.IsBodyHtml = true;

        //    //// gönderen maile ait mail ayarları
        //    //SmtpClient smtp2 = new SmtpClient();
        //    //smtp2.Credentials = new System.Net.NetworkCredential("kamil@vefaliparasayma.com", "442Bir726");
        //    //smtp2.Port = 465;
        //    //smtp2.Host = "srvm02.turhost.com";
        //    //smtp2.EnableSsl = true;
        //    //// Mail Gönderme işlemi
        //    ////smtp2.SendAsync(mail, (object)mail);
        //    //smtp2.Send(mail);

        //}

    }
}