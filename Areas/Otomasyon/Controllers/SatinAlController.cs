using FotografciTakipWeb.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using System.Net;
using System.Web;
using FotografciTakipWeb.App_Settings;

using Newtonsoft.Json.Linq; // Bu satırda hata alırsanız, site dosyalarınızın olduğu bölümde bin isimli bir klasör oluşturup içerisine Newtonsoft.Json.dll adlı DLL dosyasını kopyalayın.
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace FotografciTakipWeb.Areas.Otomasyon.Controllers
{
    public class SatinAlController : Controller
    {
        FotoTakipContext dbContext = new FotoTakipContext();
        string Mesaj = "";
        //string G_Token = "";
        //string G_SiparisNo = "";
        //string G_PaketTip = "";
        //string G_PaketAdi = "";
        public ActionResult PaketSec()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Satın Al";
            ViewBag.AltMenu = "Paket Seç";

            List<SatisFiyatlari> satisfiyatlari = dbContext.SatisFiyatlaris.Where(x => x.Aktif == true).ToList();
            ViewBag.SatisFiyatlari = satisfiyatlari;
            return View();
        }
        public ActionResult Odeme(long? id)
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            if (id == null)
                return RedirectToAction("SayfaBulunamadi", "Hata");
            ViewBag.UstMenu = "Ödeme";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string SiparisNo = "";
            Siparisler siparis = new Siparisler();
            List<SatisFiyatlari> SatisFiyatlari = dbContext.SatisFiyatlaris.Where(x => x.Aktif == true).ToList();
            SatisFiyatlari SatisFiyat = dbContext.SatisFiyatlaris.FirstOrDefault(x => x.Aktif == true && x.Id == id);
            decimal PaketTutar = 0;
            long? PaketSatisFiyatId = 0;
            string PaketTip = "", PaketAdi = "", PaketDetay = "", sn = "";
            ViewBag.Firma = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
            ViewBag.Kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KullaniciId && x.Sil == false && x.Aktif == true);

            if (SatisFiyat != null)
            {
                PaketTutar = SatisFiyat.SatisFiyati;
                PaketTip = SatisFiyat.PaketTip;
                PaketAdi = SatisFiyat.PaketAdi;
                PaketDetay = SatisFiyat.PaketDetayi;
                PaketSatisFiyatId = id;
            }
            else // herhangi bir paket seçilmemişse id sipariş no olarak gelmiştir.
            {
                siparis = dbContext.Siparislers.FirstOrDefault(x => x.SiparisNo == id && x.Aktif == true && x.Sil == false && x.Odendi == false);
                PaketTutar = Convert.ToDecimal(siparis.PaketTutar);
                PaketAdi = siparis.Paket;
                SiparisNo = id.ToString();
                SatisFiyatlari SiparisSatisFiyat = dbContext.SatisFiyatlaris.FirstOrDefault(x => x.Aktif == true && x.Id == siparis.SatisFiyatId);
                PaketTutar = SiparisSatisFiyat.SatisFiyati;
                PaketTip = SiparisSatisFiyat.PaketTip;
                PaketAdi = SiparisSatisFiyat.PaketAdi;
                PaketSatisFiyatId = SiparisSatisFiyat.Id;
            }

            if (string.IsNullOrEmpty(SiparisNo))
            {
                // Sipariş Numarası Oluşturuluyor.
                long sipno = 0;

                List<Siparisler> siparisvarmi = dbContext.Siparislers.Where(x => x.FirmaId == FirmaId).ToList();
                if (siparisvarmi.Count > 0)
                {
                    sipno = Convert.ToInt64(siparisvarmi.Max(x => x.Id)) + 1;
                    sn = FirmaId.ToString() + DateTime.Today.ToString("yyyy") + DateTime.Today.ToString("MM") + DateTime.Today.ToString("dd") + sipno.ToString();
                    //sn = FirmaId.ToString() + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + sipno.ToString();
                }
                else
                {
                    //sn = FirmaId.ToString() + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + sipno.ToString();
                    sn = FirmaId.ToString() + DateTime.Today.ToString("yyyy") + DateTime.Today.ToString("MM") + DateTime.Today.ToString("dd") + sipno.ToString();

                }
                // Sipariş Numarası Oluşturuluyor.
            }
            else
                sn = SiparisNo;

            ViewBag.PaketTutar = PaketTutar;
            ViewBag.PaketTip = PaketTip;
            ViewBag.PaketAdi = PaketAdi;
            ViewBag.PaketDetayi = PaketDetay;
            ViewBag.PaketSatisFiyatId = PaketSatisFiyatId;
            ViewBag.SiparisNo = sn;
            return View();
        }
        [HttpPost]
        public ActionResult BankaSiparisOnay()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string LisansSuresi = Request["LisansSuresi"]; //0: Tek Kullanımlık 1: 1AY, 2: 3AY, 3: 6AY, 4: 12AY(1YIL), 5: 24AY(2YIL), 6: 36AY(3YIL)
            string PaketAdi = Request["PaketAdi"]; // Basit Lisans (60 TL/Ay), Standart Lisans (75 TL/Ay), Profesyonel Lisans (90 TL/Ay), 1.000 SMS, 5.000 SMS, 10.000 SMS, 25.000 SMS 
            string PaketTip = Request["PaketTip"];
            long SiparisNo = Convert.ToInt64(Request["SiparisNo"]);
            long PaketSatisFiyatId = Convert.ToInt64(Request["PaketSatisFiyatId"]);

            string PaketDetay = "";
            decimal PaketTutar = 0;
            short sure = 1;
            if (LisansSuresi == "1")
                sure = 1;
            else if (LisansSuresi == "2")
                sure = 3;
            else if (LisansSuresi == "3")
                sure = 6;
            else if (LisansSuresi == "4")
                sure = 12;
            else if (LisansSuresi == "5")
                sure = 24;
            else if (LisansSuresi == "6")
                sure = 36;

            List<SatisFiyatlari> SatisFiyatlari = dbContext.SatisFiyatlaris.Where(x => x.Aktif == true).ToList();
            foreach (var paket in SatisFiyatlari)
            {

                if (PaketAdi == paket.PaketAdi)
                {
                    PaketDetay = paket.PaketDetayi;
                    if (paket.PaketTip == "Lisans")
                    {
                        PaketTutar = paket.SatisFiyati * sure;
                    }
                    else
                    {
                        PaketTutar = paket.SatisFiyati;
                        sure = 0;
                    }
                }
            }
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
            Siparisler sp = dbContext.Siparislers.FirstOrDefault(x => x.FirmaId == FirmaId && x.SiparisNo == SiparisNo && x.Iptal == false && x.Sil == false && x.Aktif == true);
            if (sp != null)
            {
                return RedirectToAction("Siparisler");
            }
            Siparisler siparis = new Siparisler();
            siparis.FirmaId = FirmaId;
            siparis.SatisFiyatId = PaketSatisFiyatId;
            siparis.SiparisNo = SiparisNo;
            siparis.Paket = PaketAdi;
            siparis.PaketDetay = PaketDetay;
            siparis.PaketTutar = PaketTutar;
            siparis.LisansSuresi = sure;
            siparis.Odendi = false;
            siparis.Durum = 0; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
            siparis.Tarih = DateTime.Now;
            siparis.Iptal = false;
            siparis.OdemeBildirim = false;  // Müşteri Bu Sipariş ile ilgili bildirimde bulunursa true oluyor
            siparis.OdemeTuru = "1";  // 0: Kredi Kartı, 1: Banka Havale/Eft
            siparis.OlusturanKullaniciId = KullaniciId;
            siparis.OlusturmaTarih = DateTime.Now;
            siparis.DegistirenKullaniciId = KullaniciId;
            siparis.DegistirmeTarih = DateTime.Now;
            siparis.Sil = false;
            siparis.Aktif = true;
            dbContext.Siparislers.Add(siparis);
            try
            {
                dbContext.SaveChanges();
                // Sirariş SMSi Gönder
                string smstutar = PaketTutar.ToString("C").Replace("₺", " ");
                string sonuc_uyeol_sms = "";
                string smsmetin = "Yeni Banka Ödemesi, Sipariş onayı bekliyor. Firma Id: " + FirmaId + ", Firma adı: " + frm.FirmaAdi + ", Ödeme Tutarı: " + smstutar + " TL, SiparisNo: " + SiparisNo + ".";
                sonuc_uyeol_sms = SMSGonder.Gonder_AtakSms(smsmetin, "05332625560", 1, 1, 1);
                //
                return Json(new { Sonuc = true, Mesaj = "Siparişiniz Oluşturuldu.<br />Siparişler menüsünden 'Ödeme Bildirimi' yaparak Lisans aktivasyonunuzu hızlandırabilirsiniz." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Sipariş Ekle";
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

        public ActionResult PayTROdeme()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KullaniciId);

            string LisansSuresi = Request["LisansSuresi"]; // 1: 1AY, 2: 3AY, 3: 6AY, 4: 12AY(1YIL), 5: 24AY(2YIL), 6: 36AY(3YIL)
            string PaketAdi = Request["PaketAdi"]; // Basit Lisans (60 TL/Ay), Standart Lisans (75 TL/Ay), Profesyonel Lisans (90 TL/Ay), 1.000 SMS, 5.000 SMS, 10.000 SMS, 25.000 SMS 
            string PaketTip = Request["PaketTip"]; // Lisans, SMS
            long SiparisNo = Convert.ToInt64(Request["SiparisNo"]);
            long PaketSatisFiyatId = Convert.ToInt64(Request["PaketSatisFiyatId"]);
            string PaketDetay = "";
            decimal PaketTutar = 0;
            short sure = 0; // LisansSüresi=0: Sms yüklemesi
            if (LisansSuresi == "1")
                sure = 1;
            else if (LisansSuresi == "2")
                sure = 3;
            else if (LisansSuresi == "3")
                sure = 6;
            else if (LisansSuresi == "4")
                sure = 12;
            else if (LisansSuresi == "5")
                sure = 24;
            else if (LisansSuresi == "6")
                sure = 36;
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            List<SatisFiyatlari> SatisFiyatlari = dbContext.SatisFiyatlaris.Where(x => x.Aktif == true).ToList();
            foreach (var paket in SatisFiyatlari)
            {
                if (PaketAdi == paket.PaketAdi)
                {
                    PaketDetay = paket.PaketDetayi;
                    if (paket.PaketTip == "Lisans")
                    {
                        PaketTutar = paket.SatisFiyati * sure;
                    }
                    else
                    {
                        PaketTutar = paket.SatisFiyati;
                        sure = 0;
                    }
                }
            }

            decimal sepettutar = PaketTutar / 1.2M;
            Siparisler sp = dbContext.Siparislers.FirstOrDefault(x => x.FirmaId == FirmaId && x.SiparisNo == SiparisNo && x.Iptal == false && x.Sil == false && x.Aktif == true);
            if (sp != null)
            {
                return RedirectToAction("Siparisler");
            }
            Siparisler siparis = new Siparisler();
            siparis.FirmaId = FirmaId;
            siparis.SatisFiyatId = PaketSatisFiyatId;
            siparis.SiparisNo = SiparisNo;
            siparis.Paket = PaketAdi;
            siparis.PaketDetay = PaketDetay;
            siparis.PaketTutar = PaketTutar;
            siparis.LisansSuresi = sure;
            siparis.Odendi = false;
            siparis.Durum = 5; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
            siparis.Tarih = DateTime.Now;
            siparis.Iptal = false;
            siparis.OdemeBildirim = true; // Müşteri Bu Sipariş ile ilgili bildirimde bulunursa true oluyor
            siparis.OdemeTuru = "0";  // 0: Kredi Kartı, 1: Banka Havale/Eft
            siparis.OlusturanKullaniciId = KullaniciId;
            siparis.OlusturmaTarih = DateTime.Now;
            siparis.DegistirenKullaniciId = KullaniciId;
            siparis.DegistirmeTarih = DateTime.Now;
            siparis.Sil = false;
            siparis.Aktif = true;
            dbContext.Siparislers.Add(siparis);
            try
            {
                dbContext.SaveChanges();
                // ####################### DÜZENLEMESİ ZORUNLU ALANLAR #######################
                //
                // API Entegrasyon Bilgileri - Mağaza paneline giriş yaparak BİLGİ sayfasından alabilirsiniz.
                string merchant_id = "581919";
                string merchant_key = "yrZqLon9iNPng1Qy";
                string merchant_salt = "A1RpsBLmxKkpncY7";
                //
                // Müşterinizin sitenizde kayıtlı veya form vasıtasıyla aldığınız eposta adresi
                string emailstr = kl.Email;
                //
                // Tahsil edilecek tutar. 9.99 için 9.99 * 100 = 999 gönderilmelidir.
                int payment_amountstr = Convert.ToInt32(PaketTutar * 100);
                //
                // Sipariş numarası: Her işlemde benzersiz olmalıdır!! Bu bilgi bildirim sayfanıza yapılacak bildirimde geri gönderilir.
                string merchant_oid = SiparisNo.ToString();
                //
                // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız ad ve soyad bilgisi
                string user_namestr = kl.AdSoyad;
                //
                // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız adres bilgisi
                string user_addressstr = "İSTANBUL";
                //
                // Müşterinizin sitenizde kayıtlı veya form aracılığıyla aldığınız telefon bilgisi
                string user_phonestr = kl.CepTel;
                //
                // Başarılı ödeme sonrası müşterinizin yönlendirileceği sayfa
                // !!! Bu sayfa siparişi onaylayacağınız sayfa değildir! Yalnızca müşterinizi bilgilendireceğiniz sayfadır!
                // !!! Siparişi onaylayacağız sayfa "Bildirim URL" sayfasıdır (Bakınız: 2.ADIM Klasörü).
                string merchant_ok_url = "https://www.fotografcitakip.com/Otomasyon/SatinAl/Siparisler";
                //string merchant_ok_url = "http://localhost:27347/Otomasyon/SatinAl/PayTROdemeOnay";
                //string merchant_ok_url = "http://www.tetrabilisim.com/Otomasyon/SatinAl/PayTROdemeOnay";

                //
                // Ödeme sürecinde beklenmedik bir hata oluşması durumunda müşterinizin yönlendirileceği sayfa
                // !!! Bu sayfa siparişi iptal edeceğiniz sayfa değildir! Yalnızca müşterinizi bilgilendireceğiniz sayfadır!
                // !!! Siparişi iptal edeceğiniz sayfa "Bildirim URL" sayfasıdır (Bakınız: 2.ADIM Klasörü).
                string merchant_fail_url = "https://fotografcitakip.com//basarisiz";
                //        
                // !!! Eğer bu örnek kodu sunucuda değil local makinanızda çalıştırıyorsanız
                // buraya dış ip adresinizi (https://www.whatismyip.com/) yazmalısınız. Aksi halde geçersiz paytr_token hatası alırsınız.
                string user_ip = Request.ServerVariables["https://fotografcitakip.com/"];
                if (user_ip == "" || user_ip == null)
                {
                    user_ip = Request.ServerVariables["REMOTE_ADDR"];
                }
                //
                // ÖRNEK user_basket oluşturma - Ürün adedine göre object'leri çoğaltabilirsiniz
                object[][] user_basket = {
            new object[] { PaketAdi, sepettutar.ToString(), 1}, // 1. ürün (Ürün Ad - Birim Fiyat - Adet)
            };
                /* ############################################################################################ */

                // İşlem zaman aşımı süresi - dakika cinsinden
                string timeout_limit = "30";
                //
                // Hata mesajlarının ekrana basılması için entegrasyon ve test sürecinde 1 olarak bırakın. Daha sonra 0 yapabilirsiniz.
                string debug_on = "1";
                //
                // Mağaza canlı modda iken test işlem yapmak için 1 olarak gönderilebilir.
                string test_mode = "0";
                //
                // Taksit yapılmasını istemiyorsanız, sadece tek çekim sunacaksanız 1 yapın
                string no_installment = "1";
                //
                // Sayfada görüntülenecek taksit adedini sınırlamak istiyorsanız uygun şekilde değiştirin.
                // Sıfır (0) gönderilmesi durumunda yürürlükteki en fazla izin verilen taksit geçerli olur.
                string max_installment = "0";
                //
                // Para birimi olarak TL, EUR, USD gönderilebilir. USD ve EUR kullanmak için kurumsal@paytr.com 
                // üzerinden bilgi almanız gerekmektedir. Boş gönderilirse TL geçerli olur.
                string currency = "TL";
                //
                // Türkçe için tr veya İngilizce için en gönderilebilir. Boş gönderilirse tr geçerli olur.
                string lang = "";

                // Gönderilecek veriler oluşturuluyor
                NameValueCollection data = new NameValueCollection();
                data["merchant_id"] = merchant_id;
                data["user_ip"] = user_ip;
                data["merchant_oid"] = merchant_oid;
                data["email"] = emailstr;
                data["payment_amount"] = payment_amountstr.ToString();


                // Sepet içerği oluşturma fonksiyonu, değiştirilmeden kullanılabilir.
                string user_basket_json = JsonConvert.SerializeObject(user_basket);
                string user_basketstr = Convert.ToBase64String(Encoding.UTF8.GetBytes(user_basket_json));
                data["user_basket"] = user_basketstr;
                //
                // Token oluşturma fonksiyonu, değiştirilmeden kullanılmalıdır.
                string Birlestir = string.Concat(merchant_id, user_ip, merchant_oid, emailstr, payment_amountstr.ToString(), user_basketstr, no_installment, max_installment, currency, test_mode, merchant_salt);
                HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchant_key));
                byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(Birlestir));
                data["paytr_token"] = Convert.ToBase64String(b);
                //
                data["debug_on"] = debug_on;
                data["test_mode"] = test_mode;
                data["no_installment"] = no_installment;
                data["max_installment"] = max_installment;
                data["user_name"] = user_namestr;
                data["user_address"] = user_addressstr;
                data["user_phone"] = user_phonestr;
                data["merchant_ok_url"] = merchant_ok_url;
                data["merchant_fail_url"] = merchant_fail_url;
                data["timeout_limit"] = timeout_limit;
                data["currency"] = currency;
                data["lang"] = lang;
                using (WebClient client = new WebClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    byte[] result = client.UploadValues("https://www.paytr.com/odeme/api/get-token", "POST", data);
                    string ResultAuthTicket = Encoding.UTF8.GetString(result);
                    dynamic json = JValue.Parse(ResultAuthTicket);
                    if (json.status == "success")
                    {
                        string tokenstatus = "success";
                        string tokenurl = "https://www.paytr.com/odeme/guvenli/" + json.token;
                        Session["qToken"] = json.token;
                        TempData["url"] = tokenurl;
                        TempData["tokenstatus"] = tokenstatus;
                        return Json(new { Sonuc = true, Mesaj = "Siparişiniz Oluşturuldu. Ödeme Formundan ödemenizi yapabilirsiniz.", tokenurl = tokenurl, tokenstatus = tokenstatus }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        TempData["tokenstatus"] = "PAYTR IFRAME failed. reason:" + json.reason;
                        //TempData["Error"] = "PAYTR IFRAME failed. reason:" + json.reason;
                        Siparisler siparissil = dbContext.Siparislers.FirstOrDefault(x => x.Id == siparis.Id);
                        dbContext.Siparislers.Remove(siparissil);
                        dbContext.SaveChanges();
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = SubeId;
                        hata.Islem = "Sipariş Ekle - İyzico ödeme formu oluştur.";
                        hata.HataMesajı = "PayTr Token alamadı - " + json.reason;
                        hata.OlusturanKullaniciId = 1;
                        hata.OlusturmaTarih = DateTime.Now;
                        hata.DegistirenKullaniciId = 1;
                        hata.DegistirmeTarih = DateTime.Now;
                        hata.Aktif = true;
                        hata.Sil = false;
                        dbContext.HataLoglaris.Add(hata);
                        dbContext.SaveChanges();
                        return Json(new { Sonuc = false, Mesaj = "Ödeme Formu oluşturulamadı, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                    }
                }

            }
            catch (Exception e)
            {
                Siparisler siparissil = dbContext.Siparislers.FirstOrDefault(x => x.Id == siparis.Id);
                dbContext.Siparislers.Remove(siparissil);
                dbContext.SaveChanges();
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Sipariş Ekle - Sipariş oluşturulamadı - PAYTR";
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Sipariş kaydı oluşturulamadı, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }
        }
        public ActionResult SiparisSil()
        {
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);

            long SiparisNo = Convert.ToInt64(Request["SiparisNo"]);
            Siparisler sp = dbContext.Siparislers.FirstOrDefault(x => x.FirmaId == FirmaId && x.SiparisNo == SiparisNo && x.Iptal == false && x.Sil == false && x.Aktif == true);
            if (sp != null)
            {
                dbContext.Siparislers.Remove(sp);
                try
                {
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = true, Mesaj = "Siparişiniz Silindi. Ödeme Formundan ödemenizi yapabilirsiniz." }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Sipariş Silinemedi - PAYTR";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = false, Mesaj = "Sipariş Silinemedi, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                }
            }
            else
            {
                return Json(new { Sonuc = false, Mesaj = "Sipariş Silinemedi, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }

        }
        public void PayTROdemeKontrol()
        {
            string merchant_key = "yrZqLon9iNPng1Qy";
            string merchant_salt = "A1RpsBLmxKkpncY7";
            //subid1 = 20250617 - 0556 - 2947 - accc - 72d7bb09de03
            // ####### Bu kısımda herhangi bir değişiklik yapmanıza gerek yoktur. #######
            // 
            // POST değerleri ile hash oluştur.
            string merchant_oid = Request.Form["merchant_oid"];
            string status = Request.Form["status"];
            string total_amount = Request.Form["total_amount"];
            string hash = Request.Form["hash"];
            string merchant_oid2 = Request["merchant_oid"];
            string Birlestir = string.Concat(merchant_oid, merchant_salt, status, total_amount);
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchant_key));
            byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(Birlestir));
            string token = Convert.ToBase64String(b);
            long SiparisNo = Convert.ToInt64(merchant_oid);

            //
            // Oluşturulan hash'i, paytr'dan gelen post içindeki hash ile karşılaştır (isteğin paytr'dan geldiğine ve değişmediğine emin olmak için)
            // Bu işlemi yapmazsanız maddi zarara uğramanız olasıdır.
            if (hash.ToString() != token)
            {
                Response.Write("PAYTR notification failed: bad hash");

            }
            //###########################################################################

            // BURADA YAPILMASI GEREKENLER
            // 1) Siparişin durumunu $post['merchant_oid'] değerini kullanarak veri tabanınızdan sorgulayın.
            // 2) Eğer sipariş zaten daha önceden onaylandıysa veya iptal edildiyse  echo "OK"; exit; yaparak sonlandırın.
            Siparisler siparis = dbContext.Siparislers.FirstOrDefault(x => x.SiparisNo == SiparisNo && x.Iptal == false && x.Aktif == true);
            SatisFiyatlari paket = dbContext.SatisFiyatlaris.FirstOrDefault(x => x.Id == siparis.SatisFiyatId);
            long FirmaId = siparis.FirmaId;
            long KullaniciId = siparis.OlusturanKullaniciId;
            string PaketAdi = siparis.Paket;
            int SmsMiktar = 0;
            if (siparis != null)
            {
                if (status == "success")
                { //Ödeme Onaylandı

                    siparis.Odendi = true;
                    siparis.Durum = 1; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                    siparis.Tarih = DateTime.Now;
                    siparis.Iptal = false;
                    siparis.OdemeBildirim = true; // Müşteri Bu Sipariş ile ilgili bildirimde bulunursa true oluyor
                    siparis.OdemeTuru = "0";  // 0: Kredi Kartı, 1: Banka Havale/Eft
                    dbContext.SaveChanges();
                    if (paket.PaketTip == "SMS")
                    {
                        // Sipariş edilen SMS miktarını bulma
                        //if (sip.Paket == "1.000 SMS")
                        //    SmsMiktar = 1000;
                        //else if (sip.Paket == "5.000 SMS")
                        //    SmsMiktar = 5000;
                        //else if (sip.Paket == "10.000 SMS")
                        //    SmsMiktar = 1000;
                        //else if (sip.Paket == "25.000 SMS")
                        //    SmsMiktar = 25000;

                        SmsMiktar = paket.SMSMiktar;

                        //---------
                        SmsHareket smshareket = new SmsHareket();
                        smshareket.FirmaId = FirmaId;
                        smshareket.YuklemeTarihi = DateTime.Now;
                        smshareket.Miktar = SmsMiktar;
                        smshareket.Açıklama = SiparisNo + " numaralı sipariş ile alınan SMS bakiyesi";
                        smshareket.OlusturanKullaniciId = KullaniciId;
                        smshareket.OlusturmaTarih = DateTime.Now;
                        smshareket.DegistirenKullaniciId = KullaniciId;
                        smshareket.DegistirmeTarih = DateTime.Now;
                        smshareket.Aktif = true;
                        smshareket.Sil = false;
                        dbContext.SmsHarekets.Add(smshareket);

                        SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                        smsbakiye.ToplamKontur = smsbakiye.Bakiye + SmsMiktar;
                        smsbakiye.Bakiye = smsbakiye.Bakiye + SmsMiktar;
                        smsbakiye.DegistirenKullaniciId = KullaniciId;
                        smsbakiye.DegistirmeTarih = DateTime.Now;
                    }
                    else if (paket.PaketTip == "Lisans")
                    {
                        Lisanslar lisans = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId);
                        //List<ModulSayfa> modulsayfa = dbContext.ModulSayfas.Where(x => x.FirmaId == FirmaId).ToList();
                        // Lisans Süresi işlemleri yapılıyor...
                        int sure = siparis.LisansSuresi;
                        DateTime lbitis = lisans.LisansBitisTarihi;
                        if (lbitis < DateTime.Now)
                        {
                            lisans.LisansBitisTarihi = DateTime.Now.AddMonths(sure);
                        }
                        else
                        {
                            lisans.LisansBitisTarihi = lbitis.AddMonths(sure);
                        }
                        lisans.LisansYenilemeTarihi = DateTime.Now;
                        // Lisans Modül işlemleri yapılıyor...

                        SmsMiktar = sure * paket.SMSMiktar;
                        lisans.Dashboard = paket.Dashboard;
                        lisans.RezervasyonIslemleri = paket.RezervasyonIslemleri;
                        lisans.RezervasyonIslemleri_Randevular = paket.RezervasyonIslemleri_Randevular;
                        lisans.RezervasyonIslemleri_RezervasyonListesi = paket.RezervasyonIslemleri_RezervasyonListesi;
                        lisans.RezervasyonIslemleri_RezervasyonTakvimi = paket.RezervasyonIslemleri_RezervasyonTakvimi;
                        lisans.RezervasyonIslemleri_RezervasyonTeklifleri = paket.RezervasyonIslemleri_RezervasyonTeklifleri;
                        lisans.Muhasebe = paket.Muhasebe;
                        lisans.Muhasebe_AlinanGelecekOdemeler = paket.Muhasebe_AlinanGelecekOdemeler;
                        lisans.Muhasebe_GunlukIsler = paket.Muhasebe_GunlukIsler;
                        lisans.Muhasebe_GelirGiderler = paket.Muhasebe_GelirGiderler;
                        lisans.Muhasebe_KalanBakiyeler = paket.Muhasebe_KalanBakiyeler;
                        lisans.Muhasebe_Kasa = paket.Muhasebe_Kasa;
                        lisans.Muhasebe_Cariler = paket.Muhasebe_Cariler;
                        lisans.Muhasebe_Cariler_CariHesapTakibi = paket.Muhasebe_Cariler_CariHesapTakibi;
                        lisans.Muhasebe_Cariler_CariListesi = paket.Muhasebe_Cariler_CariListesi;
                        lisans.Musteriler = paket.Musteriler;
                        lisans.Musteriler_MusteriHesapTakibi = paket.Musteriler_MusteriHesapTakibi;
                        lisans.Musteriler_MusteriListesi = paket.Musteriler_MusteriListesi;
                        lisans.Musteriler_FotografYukle = paket.Musteriler_FotografYukle;
                        lisans.Musteriler_MusteriMesajlari = paket.Musteriler_MusteriMesajlari;
                        lisans.Stok = paket.Stok;
                        lisans.Stok_DepoIslemleri = paket.Stok_DepoIslemleri;
                        lisans.Stok_DepoIslemleri_DepodanYapilmisCikislar = paket.Stok_DepoIslemleri_DepodanYapilmisCikislar;
                        lisans.Stok_DepoIslemleri_DepolarArasiTransfer = paket.Stok_DepoIslemleri_DepolarArasiTransfer;
                        lisans.Stok_DepoIslemleri_DepoListesi = paket.Stok_DepoIslemleri_DepoListesi;
                        lisans.Stok_DepoIslemleri_DepoStokDurumu = paket.Stok_DepoIslemleri_DepoStokDurumu;
                        lisans.Stok_DepoIslemleri_DepoyaYapılmışGirisler = paket.Stok_DepoIslemleri_DepoyaYapılmışGirisler;
                        lisans.Stok_Iade = paket.Stok_Iade;
                        lisans.Stok_StokCikisi = paket.Stok_StokCikisi;
                        lisans.Stok_StokGirisi = paket.Stok_StokGirisi;
                        lisans.Stok_StokHareketleri = paket.Stok_StokHareketleri;
                        lisans.Stok_StokKartlari = paket.Stok_StokKartlari;
                        lisans.Raporlar = paket.Raporlar;
                        lisans.Raporlar_GelecekOdemelerRaporu = paket.Raporlar_GelecekOdemelerRaporu;
                        lisans.Raporlar_GelirGiderRaporlari = paket.Raporlar_GelirGiderRaporlari;
                        lisans.Raporlar_RezervasyonIslemAdetleriRaporu = paket.Raporlar_RezervasyonIslemAdetleriRaporu;
                        lisans.Raporlar_RezervasyonTurleriRaporu = paket.Raporlar_RezervasyonTurleriRaporu;
                        lisans.Raporlar_HaftalikRapor = paket.Raporlar_HaftalikRapor;
                        lisans.Raporlar_RezervasyonEkHizmetleriRaporu = paket.Raporlar_RezervasyonEkHizmetleriRaporu;
                        lisans.Raporlar_CekimPaketleriRaporu = paket.Raporlar_CekimPaketleriRaporu;
                        lisans.Raporlar_PersonelPerformansRaporu = paket.Raporlar_PersonelPerformansRaporu;
                        lisans.TelefonRehberi = paket.TelefonRehberi;
                        lisans.TelefonRehberi_Rehber = paket.TelefonRehberi_Rehber;
                        lisans.TelefonRehberi_ExceldenEkle = paket.TelefonRehberi_ExceldenEkle;
                        lisans.Personeller = paket.Personeller;
                        lisans.Personeller_PersonelListesi = paket.Personeller_PersonelListesi;
                        lisans.Personeller_PersonelIzinleri = paket.Personeller_PersonelIzinleri;
                        lisans.Personeller_PersonelOdemeleri = paket.Personeller_PersonelOdemeleri;
                        lisans.Personeller_PersonelIsTakibi = paket.Personeller_PersonelIsTakibi;
                        lisans.Tanimlar = paket.Tanimlar;
                        lisans.Tanimlar_CekimPaketleri = paket.Tanimlar_CekimPaketleri;
                        lisans.Tanimlar_EmailMetinleri = paket.Tanimlar_EmailMetinleri;
                        lisans.Tanimlar_GelirGiderTurleri = paket.Tanimlar_GelirGiderTurleri;
                        lisans.Tanimlar_GunlukIsKategorileri = paket.Tanimlar_GunlukIsKategorileri;
                        lisans.Tanimlar_PersonelGorevleri = paket.Tanimlar_PersonelGorevleri;
                        lisans.Tanimlar_RehberGruplari = paket.Tanimlar_RehberGruplari;
                        lisans.Tanimlar_RezervasyonEkHizmetleri = paket.Tanimlar_RezervasyonEkHizmetleri;
                        lisans.Tanimlar_RezervasyonTurleri = paket.Tanimlar_RezervasyonTurleri;
                        lisans.Tanimlar_SmsMetinleri = paket.Tanimlar_SmsMetinleri;
                        lisans.Tanimlar_SozlesmeSartlari = paket.Tanimlar_SozlesmeSartlari;
                        lisans.Tanimlar_Sureler = paket.Tanimlar_Sureler;
                        lisans.Tanimlar_TatilGunleri = paket.Tanimlar_TatilGunleri;
                        lisans.Tanimlar_ZamanDilimleri = paket.Tanimlar_ZamanDilimleri;
                        lisans.Ayarlar = paket.Ayarlar;
                        lisans.Ayarlar_EmailGonderimAyarlari = paket.Ayarlar_EmailGonderimAyarlari;
                        lisans.Ayarlar_EmailHesapAyarlari = paket.Ayarlar_EmailHesapAyarlari;
                        lisans.Ayarlar_GenelAyarlar = paket.Ayarlar_GenelAyarlar;
                        lisans.Ayarlar_ListelemeFiltreAyarlari = paket.Ayarlar_ListelemeFiltreAyarlari;
                        lisans.Ayarlar_MusteriAyarlari = paket.Ayarlar_MusteriAyarlari;
                        lisans.Ayarlar_RezervasyonAyarlari = paket.Ayarlar_RezervasyonAyarlari;
                        lisans.Ayarlar_SmsGonderimAyarlari = paket.Ayarlar_SmsGonderimAyarlari;
                        lisans.Ayarlar_SozlesmeCiktiAyarlari = paket.Ayarlar_SozlesmeCiktiAyarlari;
                        lisans.Ayarlar_Personeller = paket.Ayarlar_Personeller;
                        lisans.FirmaSubeIslemleri = paket.FirmaSubeIslemleri;
                        lisans.FirmaSubeIslemleri_FirmaBilgileri = paket.FirmaSubeIslemleri_FirmaBilgileri;
                        lisans.FirmaSubeIslemleri_SubeIslemleri = paket.FirmaSubeIslemleri_SubeIslemleri;
                        lisans.KullaniciYetkiIslemleri = paket.KullaniciYetkiIslemleri;
                        lisans.KullaniciYetkiIslemleri_KullaniciListesi = paket.KullaniciYetkiIslemleri_KullaniciListesi;
                        lisans.KullaniciYetkiIslemleri_KullaniciYetkilendirme = paket.KullaniciYetkiIslemleri_KullaniciYetkilendirme;
                        lisans.SmsGonder = paket.SmsGonder;
                        lisans.SatinAl = paket.SatinAl;
                        lisans.SatinAl_PaketSec = paket.SatinAl_PaketSec;
                        lisans.SatinAl_Siparisler = paket.SatinAl_Siparisler;
                        lisans.Destek = paket.Destek;
                        lisans.Destek_DestekDetay = paket.Destek_DestekDetay;
                        lisans.Destek_DestekTalepleri = paket.Destek_DestekTalepleri;
                        lisans.SubeLimit = paket.SubeLimit;
                        lisans.KullaniciLimit = paket.KullaniciLimit;
                        lisans.PersonelLimit = paket.PersonelLimit;
                        lisans.DegistirenKullaniciId = 1;
                        lisans.DegistirmeTarih = DateTime.Now;


                        #region Hediye SMS Yüklemesi
                        SmsHareket smshareket = new SmsHareket();
                        smshareket.FirmaId = FirmaId;
                        smshareket.YuklemeTarihi = DateTime.Now;
                        smshareket.Miktar = SmsMiktar;
                        smshareket.Açıklama = SiparisNo + " numaralı sipariş ile alınan SMS bakiyesi";
                        smshareket.OlusturanKullaniciId = KullaniciId;
                        smshareket.OlusturmaTarih = DateTime.Now;
                        smshareket.DegistirenKullaniciId = KullaniciId;
                        smshareket.DegistirmeTarih = DateTime.Now;
                        smshareket.Aktif = true;
                        smshareket.Sil = false;
                        dbContext.SmsHarekets.Add(smshareket);

                        SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                        smsbakiye.ToplamKontur = smsbakiye.Bakiye + SmsMiktar;
                        smsbakiye.Bakiye = smsbakiye.Bakiye + SmsMiktar;
                        smsbakiye.DegistirenKullaniciId = KullaniciId;
                        smsbakiye.DegistirmeTarih = DateTime.Now;
                        #endregion
                    }
                    else
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = 1;
                        hata.Islem = "Lisans Güncelleme";
                        hata.HataMesajı = "Lisasn güncelleme - PaketTip: Belirsiz ";
                        hata.OlusturanKullaniciId = 1;
                        hata.OlusturmaTarih = DateTime.Now;
                        hata.DegistirenKullaniciId = 1;
                        hata.DegistirmeTarih = DateTime.Now;
                        hata.Aktif = true;
                        hata.Sil = false;
                        dbContext.HataLoglaris.Add(hata);
                        dbContext.SaveChanges();
                        ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                    }
                    try
                    {
                        dbContext.SaveChanges();
                        siparis.Durum = 2; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                        siparis.DegistirenKullaniciId = KullaniciId;
                        siparis.DegistirmeTarih = DateTime.Now;
                        dbContext.SaveChanges();
                        Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
                        // Sirariş SMSi Gönder
                        string smstutar = siparis.PaketTutar.ToString("C").Replace("₺", " ");
                        string sonuc_uyeol_sms = "";
                        string smsmetin = "Yeni Kredikartı Ödemesi, Firma Id: " + FirmaId + ", Firma adı: " + frm.FirmaAdi + ", Ödeme Tutarı: " + smstutar + " TL, SiparisNo: " + SiparisNo + ".";
                        sonuc_uyeol_sms = SMSGonder.Gonder_AtakSms(smsmetin, "05332625560", 1, 1, 1);
                        //
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = FirmaId;
                        hata.SubeId = 1;
                        hata.Islem = "Lisasn Güncelleme";
                        hata.HataMesajı = e.Message;
                        hata.OlusturanKullaniciId = 1;
                        hata.OlusturmaTarih = DateTime.Now;
                        hata.DegistirenKullaniciId = 1;
                        hata.DegistirmeTarih = DateTime.Now;
                        hata.Aktif = true;
                        hata.Sil = false;
                        dbContext.HataLoglaris.Add(hata);
                        dbContext.SaveChanges();
                        ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                    }

                    Response.Write("OK");

                    // Bildirimin alındığını PayTR sistemine bildir.  
                    // BURADA YAPILMASI GEREKENLER ONAY İŞLEMLERİDİR.
                    // 1) Siparişi onaylayın.
                    // 2) iframe çağırma adımında merchant_oid ve diğer bilgileri veri tabanınıza kayıp edip bu aşamada karşılaştırarak eğer var ise bilgieri çekebilir ve otomatik sipariş tamamlama işlemleri yaptırabilirsiniz.
                    // 2) Eğer müşterinize mesaj / SMS / e-posta gibi bilgilendirme yapacaksanız bu aşamada yapabilirsiniz. Bu işlemide yine iframe çağırma adımında merchant_oid bilgisini kayıt edip bu aşamada sorgulayarak verilere ulaşabilirsiniz.
                    // 3) 1. ADIM'da gönderilen payment_amount sipariş tutarı taksitli alışveriş yapılması durumunda
                    // değişebilir. Güncel tutarı Request.Form['total_amount'] değerinden alarak muhasebe işlemlerinizde kullanabilirsiniz.

                }
                else
                { //Ödemeye Onay Verilmedi

                    siparis.Odendi = false;
                    siparis.Durum = 4; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                    siparis.Tarih = DateTime.Now;
                    siparis.Iptal = false;
                    siparis.OdemeBildirim = true; // Müşteri Bu Sipariş ile ilgili bildirimde bulunursa true oluyor
                    siparis.OdemeTuru = "0";  // 0: Kredi Kartı, 1: Banka Havale/Eft
                    siparis.OdemeHata = "Hata Kodu: " + Request.Form["failed_reason_code"] + " - Açıklama: " + Request.Form["failed_reason_msg"];
                    dbContext.SaveChanges();

                    // Bildirimin alındığını PayTR sistemine bildir.  
                    Response.Write("OK");

                    // BURADA YAPILMASI GEREKENLER
                    // 1) Siparişi iptal edin.
                    // 2) Eğer ödemenin onaylanmama sebebini kayıt edecekseniz aşağıdaki değerleri kullanabilirsiniz.
                    // $post['failed_reason_code'] - başarısız hata kodu
                    // $post['failed_reason_msg'] - başarısız hata mesajı
                }
            }
            else
            {
                Response.Write("OK");
            }


        }
        public ActionResult PayTROdemeOnay()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Id == KullaniciId);
            string tokenurl = TempData["url"].ToString();
            string tokenstatus = TempData["tokenstatus"].ToString();
  
            //string Error = TempData["Error"].ToString();
            ViewBag.url = tokenurl;
            ViewBag.status = tokenstatus;
            //ViewBag.ErrorMessage = Error;
            return View();


        }

        public ActionResult KrediKartSiparisFormOlustur()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            string LisansSuresi = Request["LisansSuresi"]; // 1: 1AY, 2: 3AY, 3: 6AY, 4: 12AY(1YIL), 5: 24AY(2YIL), 6: 36AY(3YIL)
            string PaketAdi = Request["PaketAdi"]; // Basit Lisans (60 TL/Ay), Standart Lisans (75 TL/Ay), Profesyonel Lisans (90 TL/Ay), 1.000 SMS, 5.000 SMS, 10.000 SMS, 25.000 SMS 
            string PaketTip = Request["PaketTip"]; // Lisans, SMS
            long SiparisNo = Convert.ToInt64(Request["SiparisNo"]);
            long PaketSatisFiyatId = Convert.ToInt64(Request["PaketSatisFiyatId"]);
            string PaketDetay = "";
            decimal PaketTutar = 0;
            short sure = 0; // LisansSüresi=0: Sms yüklemesi
            if (LisansSuresi == "1")
                sure = 1;
            else if (LisansSuresi == "2")
                sure = 3;
            else if (LisansSuresi == "3")
                sure = 6;
            else if (LisansSuresi == "4")
                sure = 12;
            else if (LisansSuresi == "5")
                sure = 24;
            else if (LisansSuresi == "6")
                sure = 36;
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);
            List<SatisFiyatlari> SatisFiyatlari = dbContext.SatisFiyatlaris.Where(x => x.Aktif == true).ToList();
            foreach (var paket in SatisFiyatlari)
            {
                if (PaketAdi == paket.PaketAdi)
                {
                    PaketDetay = paket.PaketDetayi;
                    if (paket.PaketTip == "Lisans")
                    {
                        PaketTutar = paket.SatisFiyati * sure;
                    }
                    else
                    {
                        PaketTutar = paket.SatisFiyati;
                        sure = 0;
                    }
                }
            }

            decimal sepettutar = PaketTutar / 1.2M;
            Siparisler sp = dbContext.Siparislers.FirstOrDefault(x => x.FirmaId == FirmaId && x.SiparisNo == SiparisNo && x.Iptal == false && x.Sil == false && x.Aktif == true);
            if (sp != null)
            {
                return RedirectToAction("Siparisler");
            }
            Siparisler siparis = new Siparisler();
            siparis.FirmaId = FirmaId;
            siparis.SatisFiyatId = PaketSatisFiyatId;
            siparis.SiparisNo = SiparisNo;
            siparis.Paket = PaketAdi;
            siparis.PaketDetay = PaketDetay;
            siparis.PaketTutar = PaketTutar;
            siparis.LisansSuresi = sure;
            siparis.Odendi = false;
            siparis.Durum = 5; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
            siparis.Tarih = DateTime.Now;
            siparis.Iptal = false;
            siparis.OdemeBildirim = true; // Müşteri Bu Sipariş ile ilgili bildirimde bulunursa true oluyor
            siparis.OdemeTuru = "0";  // 0: Kredi Kartı, 1: Banka Havale/Eft
            siparis.OlusturanKullaniciId = KullaniciId;
            siparis.OlusturmaTarih = DateTime.Now;
            siparis.DegistirenKullaniciId = KullaniciId;
            siparis.DegistirmeTarih = DateTime.Now;
            siparis.Sil = false;
            siparis.Aktif = true;
            dbContext.Siparislers.Add(siparis);
            try
            {
                dbContext.SaveChanges();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Options options = new Options();
                options.ApiKey = "GkCc31H6ssYvSvzmCiWPIU7qdQG6xjpN";
                options.SecretKey = "cCdTFeGhbmteLzevJdnxZ1DtnCvuJiQc";
                options.BaseUrl = "https://api.iyzipay.com";

                CreateCheckoutFormInitializeRequest request = new CreateCheckoutFormInitializeRequest();
                request.Locale = Locale.TR.ToString();
                request.ConversationId = SiparisNo.ToString();
                request.Price = Decimal.Round(sepettutar, 2).ToString().Replace(',', '.');
                request.PaidPrice = PaketTutar.ToString().Replace(',', '.');
                request.Currency = Currency.TRY.ToString();
                request.BasketId = siparis.Id.ToString();
                request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
                request.CallbackUrl = "https://www.fotografcitakip.com/Otomasyon/SatinAl/KrediKartiOdemeOnay";
                //request.CallbackUrl = "https://www.merchant.com/callback";

                List<int> enabledInstallments = new List<int>();
                enabledInstallments.Add(2);
                enabledInstallments.Add(3);
                enabledInstallments.Add(6);
                enabledInstallments.Add(9);
                request.EnabledInstallments = enabledInstallments;

                // Firma yetkili Ad-Soyad bulma
                string[] yetkili = frm.Yetkili.Split(' ');
                string YetkiliAd = "";
                string YetkiliSoyad = "";
                if (yetkili.Length > 2)
                {
                    foreach (var item in yetkili[yetkili.Length - 2])
                    {
                        YetkiliAd += item;
                    }
                    YetkiliSoyad = yetkili[yetkili.Length - 1];
                }
                else if (yetkili.Length == 2)
                {
                    YetkiliAd = yetkili[0];
                    YetkiliSoyad = yetkili[1];
                }
                else
                {
                    YetkiliAd = yetkili[0];
                    YetkiliSoyad = yetkili[0];
                }
                // Alıcı Firma Bilgileri
                Buyer buyer = new Buyer();
                buyer.Id = frm.Id.ToString();
                buyer.Name = YetkiliAd;
                buyer.Surname = YetkiliSoyad;
                buyer.GsmNumber = "+9" + frm.CepTel;
                buyer.Email = frm.Email;
                buyer.IdentityNumber = frm.TCKimlikNo;
                buyer.LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                buyer.RegistrationDate = frm.OlusturmaTarih.ToString("yyyy-MM-dd HH:mm:ss");
                buyer.RegistrationAddress = frm.Adres;
                buyer.Ip = "85.34.78.112";
                buyer.City = frm.Il.Il1;
                buyer.Country = "Turkey";
                buyer.ZipCode = "34732";
                request.Buyer = buyer;

                Address shippingAddress = new Address();
                shippingAddress.ContactName = frm.Yetkili;
                shippingAddress.City = frm.Il.Il1;
                shippingAddress.Country = "Turkey";
                shippingAddress.Description = frm.Adres;
                shippingAddress.ZipCode = "34742";
                request.ShippingAddress = shippingAddress;

                Address billingAddress = new Address();
                billingAddress.ContactName = frm.Yetkili;
                billingAddress.City = frm.Il.Il1;
                billingAddress.Country = "Turkey";
                billingAddress.Description = frm.Adres;
                billingAddress.ZipCode = "34742";
                request.BillingAddress = billingAddress;

                List<BasketItem> basketItems = new List<BasketItem>();
                BasketItem firstBasketItem = new BasketItem();
                firstBasketItem.Id = PaketAdi;
                firstBasketItem.Name = PaketAdi;
                firstBasketItem.Category1 = "Dijital Lisans";
                firstBasketItem.ItemType = BasketItemType.VIRTUAL.ToString();
                firstBasketItem.Price = Decimal.Round(sepettutar, 2).ToString().Replace(',', '.');
                basketItems.Add(firstBasketItem);

                request.BasketItems = basketItems;
                try
                {
                    CheckoutFormInitialize checkoutFormInitialize = CheckoutFormInitialize.Create(request, options);

                    Session["Token"] = checkoutFormInitialize.Token.ToString();
                    Session["SiparisNo"] = SiparisNo.ToString();
                    Session["PaketTip"] = PaketTip.ToString();
                    Session["PaketAdi"] = PaketAdi.ToString();
                    return Json(new { Sonuc = true, Mesaj = "Siparişiniz Oluşturuldu. Ödeme Formundan ödemenizi yapabilirsiniz.", forum = checkoutFormInitialize.CheckoutFormContent }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    Siparisler siparissil = dbContext.Siparislers.FirstOrDefault(x => x.Id == siparis.Id);
                    dbContext.Siparislers.Remove(siparissil);
                    dbContext.SaveChanges();
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "Sipariş Ekle - İyzico ödeme formu oluştur.";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = false, Mesaj = "Ödeme Formu oluşturulamadı, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception e)
            {
                Siparisler siparissil = dbContext.Siparislers.FirstOrDefault(x => x.Id == siparis.Id);
                dbContext.Siparislers.Remove(siparissil);
                dbContext.SaveChanges();
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Sipariş Ekle - Sipariş oluşturulamadı - İyzico";
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Sipariş kaydı oluşturulamadı, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }

        }
        //[HttpPost]
        public ActionResult KrediKartiOdemeOnay()
        {

            string Token = Request["Token"].ToString();

            Options options = new Options();
            options.ApiKey = "GkCc31H6ssYvSvzmCiWPIU7qdQG6xjpN";
            options.SecretKey = "cCdTFeGhbmteLzevJdnxZ1DtnCvuJiQc";
            options.BaseUrl = "https://api.iyzipay.com";
            RetrieveCheckoutFormRequest request = new RetrieveCheckoutFormRequest();
            request.Token = Token;
            CheckoutForm checkoutForm = CheckoutForm.Retrieve(request, options);
            long SiparisId = Convert.ToInt64(checkoutForm.BasketId);
            Siparisler sip = dbContext.Siparislers.FirstOrDefault(x => x.Id == SiparisId);
            SatisFiyatlari paket = dbContext.SatisFiyatlaris.FirstOrDefault(x => x.Id == sip.SatisFiyatId);
            long FirmaId = sip.FirmaId;
            long KullaniciId = sip.OlusturanKullaniciId;
            string PaketAdi = sip.Paket;
            long SiparisNo = sip.SiparisNo;
            //if (PaketAdi == "Basit Lisans (60 TL/Ay)" || PaketAdi == "Standart Lisans (75 TL/Ay)" || PaketAdi == "Profesyonel Lisans (90 TL/Ay)")
            //    PaketTip = "Lisans";
            //else if (PaketAdi == "1.000 SMS" || PaketAdi == "5.000 SMS" || PaketAdi == "10.000 SMS" || PaketAdi == "25.000 SMS")
            //    PaketTip = "SMS";


            int SmsMiktar = 0;
            if (checkoutForm.PaymentStatus == "SUCCESS")
            {
                // Sipariş durumu güncelleniyor...
                sip.Odendi = true;
                sip.Durum = 1; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                sip.DegistirenKullaniciId = KullaniciId;
                sip.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();

                if (paket.PaketTip == "SMS")
                {
                    // Sipariş edilen SMS miktarını bulma
                    //if (sip.Paket == "1.000 SMS")
                    //    SmsMiktar = 1000;
                    //else if (sip.Paket == "5.000 SMS")
                    //    SmsMiktar = 5000;
                    //else if (sip.Paket == "10.000 SMS")
                    //    SmsMiktar = 1000;
                    //else if (sip.Paket == "25.000 SMS")
                    //    SmsMiktar = 25000;

                    SmsMiktar = paket.SMSMiktar;

                    //---------
                    SmsHareket smshareket = new SmsHareket();
                    smshareket.FirmaId = FirmaId;
                    smshareket.YuklemeTarihi = DateTime.Now;
                    smshareket.Miktar = SmsMiktar;
                    smshareket.Açıklama = SiparisNo + " numaralı sipariş ile alınan SMS bakiyesi";
                    smshareket.OlusturanKullaniciId = KullaniciId;
                    smshareket.OlusturmaTarih = DateTime.Now;
                    smshareket.DegistirenKullaniciId = KullaniciId;
                    smshareket.DegistirmeTarih = DateTime.Now;
                    smshareket.Aktif = true;
                    smshareket.Sil = false;
                    dbContext.SmsHarekets.Add(smshareket);

                    SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                    smsbakiye.ToplamKontur = smsbakiye.Bakiye + SmsMiktar;
                    smsbakiye.Bakiye = smsbakiye.Bakiye + SmsMiktar;
                    smsbakiye.DegistirenKullaniciId = KullaniciId;
                    smsbakiye.DegistirmeTarih = DateTime.Now;
                }
                else if (paket.PaketTip == "Lisans")
                {
                    Lisanslar lisans = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId);
                    //List<ModulSayfa> modulsayfa = dbContext.ModulSayfas.Where(x => x.FirmaId == FirmaId).ToList();
                    // Lisans Süresi işlemleri yapılıyor...
                    int sure = sip.LisansSuresi;
                    DateTime lbitis = lisans.LisansBitisTarihi;
                    if (lbitis < DateTime.Now)
                    {
                        lisans.LisansBitisTarihi = DateTime.Now.AddMonths(sure);
                    }
                    else
                    {
                        lisans.LisansBitisTarihi = lbitis.AddMonths(sure);
                    }
                    lisans.LisansYenilemeTarihi = DateTime.Now;
                    // Lisans Modül işlemleri yapılıyor...

                    SmsMiktar = sure * paket.SMSMiktar;
                    lisans.Dashboard = paket.Dashboard;
                    lisans.RezervasyonIslemleri = paket.RezervasyonIslemleri;
                    lisans.RezervasyonIslemleri_Randevular = paket.RezervasyonIslemleri_Randevular;
                    lisans.RezervasyonIslemleri_RezervasyonListesi = paket.RezervasyonIslemleri_RezervasyonListesi;
                    lisans.RezervasyonIslemleri_RezervasyonTakvimi = paket.RezervasyonIslemleri_RezervasyonTakvimi;
                    lisans.RezervasyonIslemleri_RezervasyonTeklifleri = paket.RezervasyonIslemleri_RezervasyonTeklifleri;
                    lisans.Muhasebe = paket.Muhasebe;
                    lisans.Muhasebe_AlinanGelecekOdemeler = paket.Muhasebe_AlinanGelecekOdemeler;
                    lisans.Muhasebe_GunlukIsler = paket.Muhasebe_GunlukIsler;
                    lisans.Muhasebe_GelirGiderler = paket.Muhasebe_GelirGiderler;
                    lisans.Muhasebe_KalanBakiyeler = paket.Muhasebe_KalanBakiyeler;
                    lisans.Muhasebe_Kasa = paket.Muhasebe_Kasa;
                    lisans.Muhasebe_Cariler = paket.Muhasebe_Cariler;
                    lisans.Muhasebe_Cariler_CariHesapTakibi = paket.Muhasebe_Cariler_CariHesapTakibi;
                    lisans.Muhasebe_Cariler_CariListesi = paket.Muhasebe_Cariler_CariListesi;
                    lisans.Musteriler = paket.Musteriler;
                    lisans.Musteriler_MusteriHesapTakibi = paket.Musteriler_MusteriHesapTakibi;
                    lisans.Musteriler_MusteriListesi = paket.Musteriler_MusteriListesi;
                    lisans.Musteriler_FotografYukle = paket.Musteriler_FotografYukle;
                    lisans.Musteriler_MusteriMesajlari = paket.Musteriler_MusteriMesajlari;
                    lisans.Stok = paket.Stok;
                    lisans.Stok_DepoIslemleri = paket.Stok_DepoIslemleri;
                    lisans.Stok_DepoIslemleri_DepodanYapilmisCikislar = paket.Stok_DepoIslemleri_DepodanYapilmisCikislar;
                    lisans.Stok_DepoIslemleri_DepolarArasiTransfer = paket.Stok_DepoIslemleri_DepolarArasiTransfer;
                    lisans.Stok_DepoIslemleri_DepoListesi = paket.Stok_DepoIslemleri_DepoListesi;
                    lisans.Stok_DepoIslemleri_DepoStokDurumu = paket.Stok_DepoIslemleri_DepoStokDurumu;
                    lisans.Stok_DepoIslemleri_DepoyaYapılmışGirisler = paket.Stok_DepoIslemleri_DepoyaYapılmışGirisler;
                    lisans.Stok_Iade = paket.Stok_Iade;
                    lisans.Stok_StokCikisi = paket.Stok_StokCikisi;
                    lisans.Stok_StokGirisi = paket.Stok_StokGirisi;
                    lisans.Stok_StokHareketleri = paket.Stok_StokHareketleri;
                    lisans.Stok_StokKartlari = paket.Stok_StokKartlari;
                    lisans.Raporlar = paket.Raporlar;
                    lisans.Raporlar_GelecekOdemelerRaporu = paket.Raporlar_GelecekOdemelerRaporu;
                    lisans.Raporlar_GelirGiderRaporlari = paket.Raporlar_GelirGiderRaporlari;
                    lisans.Raporlar_RezervasyonIslemAdetleriRaporu = paket.Raporlar_RezervasyonIslemAdetleriRaporu;
                    lisans.Raporlar_RezervasyonTurleriRaporu = paket.Raporlar_RezervasyonTurleriRaporu;
                    lisans.Raporlar_HaftalikRapor = paket.Raporlar_HaftalikRapor;
                    lisans.Raporlar_RezervasyonEkHizmetleriRaporu = paket.Raporlar_RezervasyonEkHizmetleriRaporu;
                    lisans.Raporlar_CekimPaketleriRaporu = paket.Raporlar_CekimPaketleriRaporu;
                    lisans.Raporlar_PersonelPerformansRaporu = paket.Raporlar_PersonelPerformansRaporu;
                    lisans.TelefonRehberi = paket.TelefonRehberi;
                    lisans.TelefonRehberi_Rehber = paket.TelefonRehberi_Rehber;
                    lisans.TelefonRehberi_ExceldenEkle = paket.TelefonRehberi_ExceldenEkle;
                    lisans.Personeller = paket.Personeller;
                    lisans.Personeller_PersonelListesi = paket.Personeller_PersonelListesi;
                    lisans.Personeller_PersonelIzinleri = paket.Personeller_PersonelIzinleri;
                    lisans.Personeller_PersonelOdemeleri = paket.Personeller_PersonelOdemeleri;
                    lisans.Personeller_PersonelIsTakibi = paket.Personeller_PersonelIsTakibi;
                    lisans.Tanimlar = paket.Tanimlar;
                    lisans.Tanimlar_CekimPaketleri = paket.Tanimlar_CekimPaketleri;
                    lisans.Tanimlar_EmailMetinleri = paket.Tanimlar_EmailMetinleri;
                    lisans.Tanimlar_GelirGiderTurleri = paket.Tanimlar_GelirGiderTurleri;
                    lisans.Tanimlar_GunlukIsKategorileri = paket.Tanimlar_GunlukIsKategorileri;
                    lisans.Tanimlar_PersonelGorevleri = paket.Tanimlar_PersonelGorevleri;
                    lisans.Tanimlar_RehberGruplari = paket.Tanimlar_RehberGruplari;
                    lisans.Tanimlar_RezervasyonEkHizmetleri = paket.Tanimlar_RezervasyonEkHizmetleri;
                    lisans.Tanimlar_RezervasyonTurleri = paket.Tanimlar_RezervasyonTurleri;
                    lisans.Tanimlar_SmsMetinleri = paket.Tanimlar_SmsMetinleri;
                    lisans.Tanimlar_SozlesmeSartlari = paket.Tanimlar_SozlesmeSartlari;
                    lisans.Tanimlar_Sureler = paket.Tanimlar_Sureler;
                    lisans.Tanimlar_TatilGunleri = paket.Tanimlar_TatilGunleri;
                    lisans.Tanimlar_ZamanDilimleri = paket.Tanimlar_ZamanDilimleri;
                    lisans.Ayarlar = paket.Ayarlar;
                    lisans.Ayarlar_EmailGonderimAyarlari = paket.Ayarlar_EmailGonderimAyarlari;
                    lisans.Ayarlar_EmailHesapAyarlari = paket.Ayarlar_EmailHesapAyarlari;
                    lisans.Ayarlar_GenelAyarlar = paket.Ayarlar_GenelAyarlar;
                    lisans.Ayarlar_ListelemeFiltreAyarlari = paket.Ayarlar_ListelemeFiltreAyarlari;
                    lisans.Ayarlar_MusteriAyarlari = paket.Ayarlar_MusteriAyarlari;
                    lisans.Ayarlar_RezervasyonAyarlari = paket.Ayarlar_RezervasyonAyarlari;
                    lisans.Ayarlar_SmsGonderimAyarlari = paket.Ayarlar_SmsGonderimAyarlari;
                    lisans.Ayarlar_SozlesmeCiktiAyarlari = paket.Ayarlar_SozlesmeCiktiAyarlari;
                    lisans.Ayarlar_Personeller = paket.Ayarlar_Personeller;
                    lisans.FirmaSubeIslemleri = paket.FirmaSubeIslemleri;
                    lisans.FirmaSubeIslemleri_FirmaBilgileri = paket.FirmaSubeIslemleri_FirmaBilgileri;
                    lisans.FirmaSubeIslemleri_SubeIslemleri = paket.FirmaSubeIslemleri_SubeIslemleri;
                    lisans.KullaniciYetkiIslemleri = paket.KullaniciYetkiIslemleri;
                    lisans.KullaniciYetkiIslemleri_KullaniciListesi = paket.KullaniciYetkiIslemleri_KullaniciListesi;
                    lisans.KullaniciYetkiIslemleri_KullaniciYetkilendirme = paket.KullaniciYetkiIslemleri_KullaniciYetkilendirme;
                    lisans.SmsGonder = paket.SmsGonder;
                    lisans.SatinAl = paket.SatinAl;
                    lisans.SatinAl_PaketSec = paket.SatinAl_PaketSec;
                    lisans.SatinAl_Siparisler = paket.SatinAl_Siparisler;
                    lisans.Destek = paket.Destek;
                    lisans.Destek_DestekDetay = paket.Destek_DestekDetay;
                    lisans.Destek_DestekTalepleri = paket.Destek_DestekTalepleri;
                    lisans.SubeLimit = paket.SubeLimit;
                    lisans.KullaniciLimit = paket.KullaniciLimit;
                    lisans.PersonelLimit = paket.PersonelLimit;
                    lisans.DegistirenKullaniciId = 1;
                    lisans.DegistirmeTarih = DateTime.Now;


                    #region Hediye SMS Yüklemesi
                    SmsHareket smshareket = new SmsHareket();
                    smshareket.FirmaId = FirmaId;
                    smshareket.YuklemeTarihi = DateTime.Now;
                    smshareket.Miktar = SmsMiktar;
                    smshareket.Açıklama = SiparisNo + " numaralı sipariş ile alınan SMS bakiyesi";
                    smshareket.OlusturanKullaniciId = KullaniciId;
                    smshareket.OlusturmaTarih = DateTime.Now;
                    smshareket.DegistirenKullaniciId = KullaniciId;
                    smshareket.DegistirmeTarih = DateTime.Now;
                    smshareket.Aktif = true;
                    smshareket.Sil = false;
                    dbContext.SmsHarekets.Add(smshareket);

                    SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                    smsbakiye.ToplamKontur = smsbakiye.Bakiye + SmsMiktar;
                    smsbakiye.Bakiye = smsbakiye.Bakiye + SmsMiktar;
                    smsbakiye.DegistirenKullaniciId = KullaniciId;
                    smsbakiye.DegistirmeTarih = DateTime.Now;
                    #endregion
                }
                else
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "Lisans Güncelleme";
                    hata.HataMesajı = "Lisasn güncelleme - PaketTip: Belirsiz ";
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                }
                try
                {
                    dbContext.SaveChanges();
                    sip.Durum = 2; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                    sip.DegistirenKullaniciId = KullaniciId;
                    sip.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                    Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
                    // Sirariş SMSi Gönder
                    string smstutar = sip.PaketTutar.ToString("C").Replace("₺", " ");
                    string sonuc_uyeol_sms = "";
                    string smsmetin = "Yeni Kredikartı Ödemesi, Firma Id: " + FirmaId + ", Firma adı: " + frm.FirmaAdi + ", Ödeme Tutarı: " + smstutar + " TL, SiparisNo: " + SiparisNo + ".";
                    sonuc_uyeol_sms = SMSGonder.Gonder_AtakSms(smsmetin, "05332625560", 1, 1, 1);
                    //
                    //ViewBag.Sonuc = "FAILURE";
                    Mesaj = "SUCCESS";
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "Lisasn Güncelleme";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                }
            }
            else if (checkoutForm.PaymentStatus == "FAILURE")
            {
                sip.Odendi = false;
                sip.Durum = 4; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                sip.DegistirenKullaniciId = KullaniciId;
                sip.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();
                Mesaj = "FAILURE";
            }
            else
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Lisasn Güncelleme";
                hata.HataMesajı = "Lisasn güncelleme - Ödeme sonucu:Başarısız-Başarılı'dan farklıbir durum.";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });

            }
            return RedirectToAction("Siparisler");
            //return Json(new { Sonuc = true, Mesaj = "Siparişiniz Oluşturuldu", forum = checkoutForm }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Siparisler()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            ViewBag.UstMenu = "Satın Al";
            ViewBag.AltMenu = "Siparişler";
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            if (Mesaj != "")
                ViewBag.Mesaj = Mesaj;
            ViewBag.FiltreAyar = dbContext.AyarlarFiltres.FirstOrDefault(x => x.FirmaId == FirmaId).Siparisler;
            ViewBag.KullaniciYetki = dbContext.KullaniciYetkis.FirstOrDefault(x => x.FirmaId == FirmaId && x.SayfaId == 108 && x.Aktif == true && x.Sil == false);
            return View();
        }
        [HttpPost]
        public ActionResult SiparisListesi()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long KullaniciId = Convert.ToInt64(Session["Id"]);
           
            int RolId = Convert.ToInt32(Session["RolId"]);
            List<Siparisler> siparisler;
            if (FirmaId == 1)
                siparisler = dbContext.Siparislers.Where(x => x.Aktif == true && x.Sil == false).ToList(); // aktif şubeye ait müşteri listesi
            else
                siparisler = dbContext.Siparislers.Where(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false).ToList(); // aktif şubeye ait müşteri listesi
            var sip = siparisler.Select(m => new
            {
                Id = m.Id,
                FirmaId = m.FirmaId,
                SiparisNo = m.SiparisNo,
                PaketAdi = m.Paket,
                PaketDetay = m.PaketDetay,
                PaketTutar = m.PaketTutar,
                LisansSuresi = m.LisansSuresi,
                Odendi = m.Odendi,
                Dosya = m.Dosya,
                Durum = m.Durum, // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödenmedi
                Tarih = m.Tarih.ToShortDateString(),
                Iptal = m.Iptal,
                OdemeBildirim = m.OdemeBildirim,
                OdemeTuru = m.OdemeTuru
            }); // Json data dönüşünde hata alınıyordu. İhtiyaç duyulan kolon isimleri verilerek sorun çözüldü.
            return Json(new { data = sip }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult OdemeBildirim()
        {
            if (Session.Count == 0)
                return RedirectToAction("GirisYap", "Giris");
            long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            if (SubeId == 0)
                SubeId = dbContext.Subes.FirstOrDefault(x => x.FirmaId == FirmaId && x.KilitBit == true && x.OlusturanKullaniciId == 1).Id;
            long KullaniciId = Convert.ToInt64(Session["Id"]);
            long SiparisId = Convert.ToInt64(Request["SiparisId"]);
            string Aciklama = Request["Aciklama"];
            string dosyaadi = "";
            Siparisler siparis = dbContext.Siparislers.FirstOrDefault(x => x.Id == SiparisId && x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
            if (Request.Files.Count > 0) // Dosya seçilmişse
            {
                HttpPostedFileBase dosya = Request.Files[0]; //Uploaded file
                System.IO.FileInfo ff = new System.IO.FileInfo(dosya.FileName);
                string uzanti = ff.Extension; // dosya uzantısı
                dosyaadi = "OdemeBildirim_" + siparis.SiparisNo + uzanti;

                if (System.IO.File.Exists(Server.MapPath("/Areas/Otomasyon/Dosyalar/OdemeBildirimleri/" + dosyaadi))) // bu resimden var mı?
                {
                    System.IO.File.Delete(Server.MapPath("/Areas/Otomasyon/Dosyalar/OdemeBildirimleri/" + dosyaadi)); // Bu isimdeki dosya siliniyor.
                }
                dosya.SaveAs(Server.MapPath("/Areas/Otomasyon/Dosyalar/OdemeBildirimleri/" + dosyaadi)); // dosya kaydet
                siparis.Dosya = "/Areas/Otomasyon/Dosyalar/OdemeBildirimleri/" + dosyaadi;
            }
            siparis.OdemeBildirimAciklama = Aciklama.TrimEnd();
            siparis.Durum = 0;  // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
            siparis.OdemeBildirim = true;
            siparis.DegistirenKullaniciId = KullaniciId;
            siparis.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();

                // Yöneticiye SMS ve Mail gönder
                string metin = siparis.SiparisNo + " Numaralı siparişin ödemesi Banka Havalesi ile yapıldı.";
                Firma Yonetici = dbContext.Firmas.FirstOrDefault(x => x.Id == 1);
                string SMSSonuc = SMSGonder.Gonder_AtakSms(metin, Yonetici.CepTel, 1, 1, 1);  // Yöneticiye SMS Gönder
                bool MailSonu = MailGonder.Gonder_Text(1, 1, 1, Yonetici.Email, "Ödeme Bildirimi", metin); // Yöneticiye Mail Gönder

                return Json(new { Sonuc = true, Mesaj = "Ödeme bildiriminiz tarafımıza iletildi.<br />Ödemeniz kontrol edilecek ve en kısa sürede ödemeniz onaylanacaktır." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Ödeme Bildirimi";
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
        public ActionResult SiparisIptal(long? id)
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

            Siparisler siparis = dbContext.Siparislers.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id && x.Aktif == true && x.Sil == false);
            siparis.Iptal = true;
            siparis.Durum = 3;
            siparis.DegistirenKullaniciId = KullaniciId;
            siparis.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Siparişiniz İptal Edildi." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Sipariş İptal. Sipariş Id:" + id;
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
        public ActionResult OdemeOnay(long? id)
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

            Siparisler siparis = dbContext.Siparislers.FirstOrDefault(x => x.FirmaId == FirmaId && x.Id == id && x.Aktif == true && x.Sil == false);
            SatisFiyatlari paket = dbContext.SatisFiyatlaris.FirstOrDefault(x => x.Id == siparis.SatisFiyatId);
            int SmsMiktar = 0;
            int sure = siparis.LisansSuresi;
            string PaketAdi = siparis.Paket;
            long SiparisNo = siparis.SiparisNo;
            //if (PaketAdi == "Basit Lisans (60 TL/Ay)" || PaketAdi == "Standart Lisans (75 TL/Ay)" || PaketAdi == "Profesyonel Lisans (90 TL/Ay)")
            //    PaketTip = "Lisans";
            //else if (PaketAdi == "1.000 SMS" || PaketAdi == "5.000 SMS" || PaketAdi == "10.000 SMS" || PaketAdi == "25.000 SMS")
            //    PaketTip = "SMS";

            // Sipariş durumu güncelleniyor...
            siparis.Odendi = true;
            siparis.Durum = 1; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
            siparis.DegistirenKullaniciId = KullaniciId;
            siparis.DegistirmeTarih = DateTime.Now;
            dbContext.SaveChanges();

            if (paket.PaketTip == "SMS")
            {
                // Sipariş edilen SMS miktarını bulma
                //if (siparis.Paket == "1.000 SMS")
                //    SmsMiktar = 1000;
                //else if (siparis.Paket == "5.000 SMS")
                //    SmsMiktar = 5000;
                //else if (siparis.Paket == "10.000 SMS")
                //    SmsMiktar = 1000;
                //else if (siparis.Paket == "25.000 SMS")
                //    SmsMiktar = 25000;
                SmsMiktar = paket.SMSMiktar;
                //---------
                SmsHareket smshareket = new SmsHareket();
                smshareket.FirmaId = FirmaId;
                smshareket.YuklemeTarihi = DateTime.Now;
                smshareket.Miktar = SmsMiktar;
                smshareket.Açıklama = SiparisNo + " numaralı sipariş ile alınan SMS bakiyesi";
                smshareket.OlusturanKullaniciId = KullaniciId;
                smshareket.OlusturmaTarih = DateTime.Now;
                smshareket.DegistirenKullaniciId = KullaniciId;
                smshareket.DegistirmeTarih = DateTime.Now;
                smshareket.Aktif = true;
                smshareket.Sil = false;
                dbContext.SmsHarekets.Add(smshareket);

                SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                smsbakiye.ToplamKontur = smsbakiye.Bakiye + SmsMiktar;
                smsbakiye.Bakiye = smsbakiye.Bakiye + SmsMiktar;
                smsbakiye.DegistirenKullaniciId = KullaniciId;
                smsbakiye.DegistirmeTarih = DateTime.Now;
            }
            else if (paket.PaketTip == "Lisans")
            {
                Lisanslar lisans = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId);
                //List<ModulSayfa> modulsayfa = dbContext.ModulSayfas.Where(x => x.FirmaId == FirmaId).ToList();
                // Lisans Süresi işlemleri yapılıyor...

                DateTime lbitis = lisans.LisansBitisTarihi;
                lisans.LisansYenilemeTarihi = DateTime.Now;
                lisans.LisansBitisTarihi = lbitis.AddMonths(sure);

                SmsMiktar = sure * paket.SMSMiktar;
                lisans.Dashboard = paket.Dashboard;
                lisans.RezervasyonIslemleri = paket.RezervasyonIslemleri;
                lisans.RezervasyonIslemleri_Randevular = paket.RezervasyonIslemleri_Randevular;
                lisans.RezervasyonIslemleri_RezervasyonListesi = paket.RezervasyonIslemleri_RezervasyonListesi;
                lisans.RezervasyonIslemleri_RezervasyonTakvimi = paket.RezervasyonIslemleri_RezervasyonTakvimi;
                lisans.RezervasyonIslemleri_RezervasyonTeklifleri = paket.RezervasyonIslemleri_RezervasyonTeklifleri;
                lisans.Muhasebe = paket.Muhasebe;
                lisans.Muhasebe_AlinanGelecekOdemeler = paket.Muhasebe_AlinanGelecekOdemeler;
                lisans.Muhasebe_GunlukIsler = paket.Muhasebe_GunlukIsler;
                lisans.Muhasebe_GelirGiderler = paket.Muhasebe_GelirGiderler;
                lisans.Muhasebe_KalanBakiyeler = paket.Muhasebe_KalanBakiyeler;
                lisans.Muhasebe_Kasa = paket.Muhasebe_Kasa;
                lisans.Muhasebe_Cariler = paket.Muhasebe_Cariler;
                lisans.Muhasebe_Cariler_CariHesapTakibi = paket.Muhasebe_Cariler_CariHesapTakibi;
                lisans.Muhasebe_Cariler_CariListesi = paket.Muhasebe_Cariler_CariListesi;
                lisans.Musteriler = paket.Musteriler;
                lisans.Musteriler_MusteriHesapTakibi = paket.Musteriler_MusteriHesapTakibi;
                lisans.Musteriler_MusteriListesi = paket.Musteriler_MusteriListesi;
                lisans.Musteriler_FotografYukle = paket.Musteriler_FotografYukle;
                lisans.Musteriler_MusteriMesajlari = paket.Musteriler_MusteriMesajlari;
                lisans.Stok = paket.Stok;
                lisans.Stok_DepoIslemleri = paket.Stok_DepoIslemleri;
                lisans.Stok_DepoIslemleri_DepodanYapilmisCikislar = paket.Stok_DepoIslemleri_DepodanYapilmisCikislar;
                lisans.Stok_DepoIslemleri_DepolarArasiTransfer = paket.Stok_DepoIslemleri_DepolarArasiTransfer;
                lisans.Stok_DepoIslemleri_DepoListesi = paket.Stok_DepoIslemleri_DepoListesi;
                lisans.Stok_DepoIslemleri_DepoStokDurumu = paket.Stok_DepoIslemleri_DepoStokDurumu;
                lisans.Stok_DepoIslemleri_DepoyaYapılmışGirisler = paket.Stok_DepoIslemleri_DepoyaYapılmışGirisler;
                lisans.Stok_Iade = paket.Stok_Iade;
                lisans.Stok_StokCikisi = paket.Stok_StokCikisi;
                lisans.Stok_StokGirisi = paket.Stok_StokGirisi;
                lisans.Stok_StokHareketleri = paket.Stok_StokHareketleri;
                lisans.Stok_StokKartlari = paket.Stok_StokKartlari;
                lisans.Raporlar = paket.Raporlar;
                lisans.Raporlar_GelecekOdemelerRaporu = paket.Raporlar_GelecekOdemelerRaporu;
                lisans.Raporlar_GelirGiderRaporlari = paket.Raporlar_GelirGiderRaporlari;
                lisans.Raporlar_RezervasyonIslemAdetleriRaporu = paket.Raporlar_RezervasyonIslemAdetleriRaporu;
                lisans.Raporlar_RezervasyonTurleriRaporu = paket.Raporlar_RezervasyonTurleriRaporu;
                lisans.Raporlar_HaftalikRapor = paket.Raporlar_HaftalikRapor;
                lisans.Raporlar_RezervasyonEkHizmetleriRaporu = paket.Raporlar_RezervasyonEkHizmetleriRaporu;
                lisans.Raporlar_CekimPaketleriRaporu = paket.Raporlar_CekimPaketleriRaporu;
                lisans.Raporlar_PersonelPerformansRaporu = paket.Raporlar_PersonelPerformansRaporu;
                lisans.TelefonRehberi = paket.TelefonRehberi;
                lisans.TelefonRehberi_Rehber = paket.TelefonRehberi_Rehber;
                lisans.TelefonRehberi_ExceldenEkle = paket.TelefonRehberi_ExceldenEkle;
                lisans.Personeller = paket.Personeller;
                lisans.Personeller_PersonelListesi = paket.Personeller_PersonelListesi;
                lisans.Personeller_PersonelIzinleri = paket.Personeller_PersonelIzinleri;
                lisans.Personeller_PersonelOdemeleri = paket.Personeller_PersonelOdemeleri;
                lisans.Personeller_PersonelIsTakibi = paket.Personeller_PersonelIsTakibi;
                lisans.Tanimlar = paket.Tanimlar;
                lisans.Tanimlar_CekimPaketleri = paket.Tanimlar_CekimPaketleri;
                lisans.Tanimlar_EmailMetinleri = paket.Tanimlar_EmailMetinleri;
                lisans.Tanimlar_GelirGiderTurleri = paket.Tanimlar_GelirGiderTurleri;
                lisans.Tanimlar_GunlukIsKategorileri = paket.Tanimlar_GunlukIsKategorileri;
                lisans.Tanimlar_PersonelGorevleri = paket.Tanimlar_PersonelGorevleri;
                lisans.Tanimlar_RehberGruplari = paket.Tanimlar_RehberGruplari;
                lisans.Tanimlar_RezervasyonEkHizmetleri = paket.Tanimlar_RezervasyonEkHizmetleri;
                lisans.Tanimlar_RezervasyonTurleri = paket.Tanimlar_RezervasyonTurleri;
                lisans.Tanimlar_SmsMetinleri = paket.Tanimlar_SmsMetinleri;
                lisans.Tanimlar_SozlesmeSartlari = paket.Tanimlar_SozlesmeSartlari;
                lisans.Tanimlar_Sureler = paket.Tanimlar_Sureler;
                lisans.Tanimlar_TatilGunleri = paket.Tanimlar_TatilGunleri;
                lisans.Tanimlar_ZamanDilimleri = paket.Tanimlar_ZamanDilimleri;
                lisans.Ayarlar = paket.Ayarlar;
                lisans.Ayarlar_EmailGonderimAyarlari = paket.Ayarlar_EmailGonderimAyarlari;
                lisans.Ayarlar_EmailHesapAyarlari = paket.Ayarlar_EmailHesapAyarlari;
                lisans.Ayarlar_GenelAyarlar = paket.Ayarlar_GenelAyarlar;
                lisans.Ayarlar_ListelemeFiltreAyarlari = paket.Ayarlar_ListelemeFiltreAyarlari;
                lisans.Ayarlar_MusteriAyarlari = paket.Ayarlar_MusteriAyarlari;
                lisans.Ayarlar_RezervasyonAyarlari = paket.Ayarlar_RezervasyonAyarlari;
                lisans.Ayarlar_SmsGonderimAyarlari = paket.Ayarlar_SmsGonderimAyarlari;
                lisans.Ayarlar_SozlesmeCiktiAyarlari = paket.Ayarlar_SozlesmeCiktiAyarlari;
                lisans.Ayarlar_Personeller = paket.Ayarlar_Personeller;
                lisans.FirmaSubeIslemleri = paket.FirmaSubeIslemleri;
                lisans.FirmaSubeIslemleri_FirmaBilgileri = paket.FirmaSubeIslemleri_FirmaBilgileri;
                lisans.FirmaSubeIslemleri_SubeIslemleri = paket.FirmaSubeIslemleri_SubeIslemleri;
                lisans.KullaniciYetkiIslemleri = paket.KullaniciYetkiIslemleri;
                lisans.KullaniciYetkiIslemleri_KullaniciListesi = paket.KullaniciYetkiIslemleri_KullaniciListesi;
                lisans.KullaniciYetkiIslemleri_KullaniciYetkilendirme = paket.KullaniciYetkiIslemleri_KullaniciYetkilendirme;
                lisans.SmsGonder = paket.SmsGonder;
                lisans.SatinAl = paket.SatinAl;
                lisans.SatinAl_PaketSec = paket.SatinAl_PaketSec;
                lisans.SatinAl_Siparisler = paket.SatinAl_Siparisler;
                lisans.Destek = paket.Destek;
                lisans.Destek_DestekDetay = paket.Destek_DestekDetay;
                lisans.Destek_DestekTalepleri = paket.Destek_DestekTalepleri;
                lisans.SubeLimit = paket.SubeLimit;
                lisans.KullaniciLimit = paket.KullaniciLimit;
                lisans.PersonelLimit = paket.PersonelLimit;
                lisans.DegistirenKullaniciId = 1;
                lisans.DegistirmeTarih = DateTime.Now;

                #region Hediye SMS Yüklemesi
                SmsHareket smshareket = new SmsHareket();
                smshareket.FirmaId = FirmaId;
                smshareket.YuklemeTarihi = DateTime.Now;
                smshareket.Miktar = SmsMiktar;
                smshareket.Açıklama = SiparisNo + " numaralı sipariş ile alınan SMS bakiyesi";
                smshareket.OlusturanKullaniciId = KullaniciId;
                smshareket.OlusturmaTarih = DateTime.Now;
                smshareket.DegistirenKullaniciId = KullaniciId;
                smshareket.DegistirmeTarih = DateTime.Now;
                smshareket.Aktif = true;
                smshareket.Sil = false;
                dbContext.SmsHarekets.Add(smshareket);

                SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                smsbakiye.ToplamKontur = smsbakiye.Bakiye + SmsMiktar;
                smsbakiye.Bakiye = smsbakiye.Bakiye + SmsMiktar;
                smsbakiye.DegistirenKullaniciId = KullaniciId;
                smsbakiye.DegistirmeTarih = DateTime.Now;
                #endregion
            }
            else
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Lisans Güncelleme";
                hata.HataMesajı = "Lisasn güncelleme - PaketTip: Belirsiz ";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }
            try
            {
                dbContext.SaveChanges();
                siparis.Durum = 2; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                siparis.DegistirenKullaniciId = KullaniciId;
                siparis.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();
                return Json(new { Sonuc = true, Mesaj = "Ödeme onaylandı <br/> Sipariş Tamamlandı." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Lisasn Güncelleme";
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }
        }
        //Giriş Ekranından Lisans Süresi bitmiş uyarısı ile yönlendirilen firmalar için
        public ActionResult FPaketSec(long? id)
        {
            // id= FirmaId
            if (id == null || id == 0)
                return RedirectToAction("FYetkisizGiris");
            ViewBag.UstMenu = "Satın Al";
            ViewBag.AltMenu = "Paket Seç";
            TempData["FirmaId"] = id;
            List<SatisFiyatlari> satisfiyatlari = dbContext.SatisFiyatlaris.Where(x => x.Aktif == true).ToList();
            ViewBag.SatisFiyatlari = satisfiyatlari;
            return View();
        }
        public ActionResult FYetkisizGiris()
        {
            return View();
        }
        public ActionResult FOdeme(long? id)
        {
            if (TempData["FirmaId"] == null)
                return RedirectToAction("FYetkisizGiris");
            if (id == null || id == 0)
                return RedirectToAction("FYetkisizGiris");

            long FirmaId = Convert.ToInt64(TempData["FirmaId"]);
            ViewBag.UstMenu = "Ödeme";
            string SiparisNo = "";
            Siparisler siparis = new Siparisler();
            List<SatisFiyatlari> SatisFiyatlari = dbContext.SatisFiyatlaris.Where(x => x.Aktif == true).ToList();
            SatisFiyatlari SatisFiyat = dbContext.SatisFiyatlaris.FirstOrDefault(x => x.Aktif == true && x.Id == id);
            decimal PaketTutar = 0;
            long? PaketSatisFiyatId = 0;
            string PaketTip = "", PaketAdi = "", PaketDetay = "", sn = "";
            ViewBag.Firma = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
            ViewBag.Kullanici = dbContext.Kullanicis.FirstOrDefault(x => x.FirmaId == FirmaId && x.PersonelGorevleri.Gorev == "Firma Sahibi");
            if (SatisFiyat != null)
            {
                PaketTutar = SatisFiyat.SatisFiyati;
                PaketTip = SatisFiyat.PaketTip;
                PaketAdi = SatisFiyat.PaketAdi;
                PaketDetay = SatisFiyat.PaketDetayi;
                PaketSatisFiyatId = id;
            }
            else // herhangi bir paket seçilmemişse id sipariş no olarak gelmiştir.
            {
                siparis = dbContext.Siparislers.FirstOrDefault(x => x.SiparisNo == id && x.Aktif == true && x.Sil == false && x.Odendi == false);
                PaketTutar = Convert.ToDecimal(siparis.PaketTutar);
                PaketAdi = siparis.Paket;
                SiparisNo = id.ToString();
                SatisFiyatlari SiparisSatisFiyat = dbContext.SatisFiyatlaris.FirstOrDefault(x => x.Aktif == true && x.Id == siparis.SatisFiyatId);
                PaketTutar = SiparisSatisFiyat.SatisFiyati;
                PaketTip = SiparisSatisFiyat.PaketTip;
                PaketAdi = SiparisSatisFiyat.PaketAdi;
                PaketSatisFiyatId = SiparisSatisFiyat.Id;
            }

            if (string.IsNullOrEmpty(SiparisNo))
            {
                // Sipariş Numarası Oluşturuluyor.
                long sipno = 0;

                List<Siparisler> siparisvarmi = dbContext.Siparislers.Where(x => x.FirmaId == FirmaId).ToList();
                if (siparisvarmi.Count > 0)
                {
                    sipno = Convert.ToInt64(siparisvarmi.Max(x => x.Id));
                    sn = FirmaId.ToString() + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + sipno.ToString();
                }
                else
                {
                    sn = FirmaId.ToString() + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + sipno.ToString();
                }
                // Sipariş Numarası Oluşturuluyor.
            }
            else
                sn = SiparisNo;

            ViewBag.PaketTutar = PaketTutar;
            ViewBag.PaketTip = PaketTip;
            ViewBag.PaketAdi = PaketAdi;
            ViewBag.PaketDetayi = PaketDetay;
            ViewBag.PaketSatisFiyatId = PaketSatisFiyatId;
            ViewBag.SiparisNo = sn;
            TempData["FirmaId"] = FirmaId;

            return View();
        }
        public ActionResult FKrediKartSiparisFormOlustur()
        {
            if (TempData["FirmaId"] == null)
                return RedirectToAction("FYetkisizGiris");

            long FirmaId = Convert.ToInt64(TempData["FirmaId"]);
            //long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            //long KullaniciId = Convert.ToInt64(Session["Id"]);
            string LisansSuresi = Request["LisansSuresi"]; // 1: 1AY, 2: 3AY, 3: 6AY, 4: 12AY(1YIL), 5: 24AY(2YIL), 6: 36AY(3YIL)
            string PaketAdi = Request["PaketAdi"]; // Basit Lisans (60 TL/Ay), Standart Lisans (75 TL/Ay), Profesyonel Lisans (90 TL/Ay), 1.000 SMS, 5.000 SMS, 10.000 SMS, 25.000 SMS 
            string PaketTip = Request["PaketTip"]; // Lisans, SMS
            long SiparisNo = Convert.ToInt64(Request["SiparisNo"]);
            long PaketSatisFiyatId = Convert.ToInt64(Request["PaketSatisFiyatId"]);
            string PaketDetay = "";
            decimal PaketTutar = 0;
            short sure = 0; // LisansSüresi=0: Sms yüklemesi
            if (LisansSuresi == "1")
                sure = 1;
            else if (LisansSuresi == "2")
                sure = 3;
            else if (LisansSuresi == "3")
                sure = 6;
            else if (LisansSuresi == "4")
                sure = 12;
            else if (LisansSuresi == "5")
                sure = 24;
            else if (LisansSuresi == "6")
                sure = 36;
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Aktif == true && x.Sil == false);

            List<SatisFiyatlari> SatisFiyatlari = dbContext.SatisFiyatlaris.Where(x => x.Aktif == true).ToList();
            foreach (var paket in SatisFiyatlari)
            {

                if (PaketAdi == paket.PaketAdi)
                {
                    PaketDetay = paket.PaketDetayi;
                    if (paket.PaketTip == "Lisans")
                    {
                        PaketTutar = paket.SatisFiyati * sure;
                    }
                    else
                    {
                        PaketTutar = paket.SatisFiyati;
                        sure = 0;
                    }
                }
            }

            decimal sepettutar = PaketTutar / 1.2M;
            Siparisler sp = dbContext.Siparislers.FirstOrDefault(x => x.FirmaId == FirmaId && x.SiparisNo == SiparisNo && x.Iptal == false && x.Sil == false && x.Aktif == true);
            if (sp != null)
            {
                return RedirectToAction("Siparisler");
            }
            Siparisler siparis = new Siparisler();
            siparis.FirmaId = FirmaId;
            siparis.SatisFiyatId = PaketSatisFiyatId;
            siparis.SiparisNo = SiparisNo;
            siparis.Paket = PaketAdi;
            siparis.PaketDetay = PaketDetay;
            siparis.PaketTutar = PaketTutar;
            siparis.LisansSuresi = sure;
            siparis.Odendi = false;
            siparis.Durum = 5; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
            siparis.Tarih = DateTime.Now;
            siparis.Iptal = false;
            siparis.OdemeBildirim = true; // Müşteri Bu Sipariş ile ilgili bildirimde bulunursa true oluyor
            siparis.OdemeTuru = "0";  // 0: Kredi Kartı, 1: Banka Havale/Eft
            siparis.OlusturanKullaniciId = 1;
            siparis.OlusturmaTarih = DateTime.Now;
            siparis.DegistirenKullaniciId = 1;
            siparis.DegistirmeTarih = DateTime.Now;
            siparis.Sil = false;
            siparis.Aktif = true;
            dbContext.Siparislers.Add(siparis);
            try
            {
                dbContext.SaveChanges();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Options options = new Options();
                options.ApiKey = "GkCc31H6ssYvSvzmCiWPIU7qdQG6xjpN";
                options.SecretKey = "cCdTFeGhbmteLzevJdnxZ1DtnCvuJiQc";
                options.BaseUrl = "https://api.iyzipay.com";

                CreateCheckoutFormInitializeRequest request = new CreateCheckoutFormInitializeRequest();
                request.Locale = Locale.TR.ToString();
                request.ConversationId = SiparisNo.ToString();
                request.Price = Decimal.Round(sepettutar, 2).ToString().Replace(',', '.');
                request.PaidPrice = PaketTutar.ToString().Replace(',', '.');
                request.Currency = Currency.TRY.ToString();
                request.BasketId = siparis.Id.ToString();
                request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
                request.CallbackUrl = "https://www.fotografcitakip.com/Otomasyon/SatinAl/FKrediKartiOdemeOnay";
                //request.CallbackUrl = "https://www.merchant.com/callback";

                List<int> enabledInstallments = new List<int>();
                enabledInstallments.Add(2);
                enabledInstallments.Add(3);
                enabledInstallments.Add(6);
                enabledInstallments.Add(9);
                request.EnabledInstallments = enabledInstallments;

                // Firma yetkili Ad-Soyad bulma
                string[] yetkili = frm.Yetkili.Split(' ');
                string YetkiliAd = "";
                string YetkiliSoyad = "";
                if (yetkili.Length > 2)
                {
                    foreach (var item in yetkili[yetkili.Length - 2])
                    {
                        YetkiliAd += item;
                    }
                    YetkiliSoyad = yetkili[yetkili.Length - 1];
                }
                else if (yetkili.Length == 2)
                {
                    YetkiliAd = yetkili[0];
                    YetkiliSoyad = yetkili[1];
                }
                else
                {
                    YetkiliAd = yetkili[0];
                    YetkiliSoyad = yetkili[0];
                }
                // Alıcı Firma Bilgileri
                Buyer buyer = new Buyer();
                buyer.Id = frm.Id.ToString();
                buyer.Name = YetkiliAd;
                buyer.Surname = YetkiliSoyad;
                buyer.GsmNumber = "+9" + frm.CepTel;
                buyer.Email = frm.Email;
                buyer.IdentityNumber = frm.TCKimlikNo;
                buyer.LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                buyer.RegistrationDate = frm.OlusturmaTarih.ToString("yyyy-MM-dd HH:mm:ss");
                buyer.RegistrationAddress = frm.Adres;
                buyer.Ip = "85.34.78.112";
                buyer.City = frm.Il.Il1;
                buyer.Country = "Turkey";
                buyer.ZipCode = "34732";
                request.Buyer = buyer;

                Address shippingAddress = new Address();
                shippingAddress.ContactName = frm.Yetkili;
                shippingAddress.City = frm.Il.Il1;
                shippingAddress.Country = "Turkey";
                shippingAddress.Description = frm.Adres;
                shippingAddress.ZipCode = "34742";
                request.ShippingAddress = shippingAddress;

                Address billingAddress = new Address();
                billingAddress.ContactName = frm.Yetkili;
                billingAddress.City = frm.Il.Il1;
                billingAddress.Country = "Turkey";
                billingAddress.Description = frm.Adres;
                billingAddress.ZipCode = "34742";
                request.BillingAddress = billingAddress;

                List<BasketItem> basketItems = new List<BasketItem>();
                BasketItem firstBasketItem = new BasketItem();
                firstBasketItem.Id = PaketAdi;
                firstBasketItem.Name = PaketAdi;
                firstBasketItem.Category1 = "Dijital Lisans";
                firstBasketItem.ItemType = BasketItemType.VIRTUAL.ToString();
                firstBasketItem.Price = Decimal.Round(sepettutar, 2).ToString().Replace(',', '.');
                basketItems.Add(firstBasketItem);

                request.BasketItems = basketItems;
                try
                {
                    CheckoutFormInitialize checkoutFormInitialize = CheckoutFormInitialize.Create(request, options);

                    Session["Token"] = checkoutFormInitialize.Token.ToString();
                    Session["SiparisNo"] = SiparisNo.ToString();
                    Session["PaketTip"] = PaketTip.ToString();
                    Session["PaketAdi"] = PaketAdi.ToString();
                    return Json(new { Sonuc = true, Mesaj = "Siparişiniz Oluşturuldu. Ödeme Formundan ödemenizi yapabilirsiniz.", forum = checkoutFormInitialize.CheckoutFormContent }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    Siparisler siparissil = dbContext.Siparislers.FirstOrDefault(x => x.Id == siparis.Id);
                    dbContext.Siparislers.Remove(siparissil);
                    dbContext.SaveChanges();
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "Sipariş Ekle - İyzico ödeme formu oluştur.";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    return Json(new { Sonuc = false, Mesaj = "Ödeme Formu oluşturulamadı, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception e)
            {
                Siparisler siparissil = dbContext.Siparislers.FirstOrDefault(x => x.Id == siparis.Id);
                dbContext.Siparislers.Remove(siparissil);
                dbContext.SaveChanges();
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Sipariş Ekle";
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return Json(new { Sonuc = false, Mesaj = "Sipariş kaydı oluşturulamadı, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
            }

        }
        public ActionResult FKrediKartiOdemeOnay()
        {
            string Token = Request["Token"].ToString();

            Options options = new Options();
            options.ApiKey = "GkCc31H6ssYvSvzmCiWPIU7qdQG6xjpN";
            options.SecretKey = "cCdTFeGhbmteLzevJdnxZ1DtnCvuJiQc";
            options.BaseUrl = "https://api.iyzipay.com";
            RetrieveCheckoutFormRequest request = new RetrieveCheckoutFormRequest();
            request.Token = Token;
            CheckoutForm checkoutForm = CheckoutForm.Retrieve(request, options);
            long SiparisId = Convert.ToInt64(checkoutForm.BasketId);
            Siparisler sip = dbContext.Siparislers.FirstOrDefault(x => x.Id == SiparisId);
            SatisFiyatlari paket = dbContext.SatisFiyatlaris.FirstOrDefault(x => x.Id == sip.SatisFiyatId);
            long FirmaId = sip.FirmaId;
            long KullaniciId = sip.OlusturanKullaniciId;
            string PaketAdi = sip.Paket;
            long SiparisNo = sip.SiparisNo;

            //if (PaketAdi == "Basit Lisans (60 TL/Ay)" || PaketAdi == "Standart Lisans (75 TL/Ay)" || PaketAdi == "Profesyonel Lisans (90 TL/Ay)")
            //    PaketTip = "Lisans";
            //else if (PaketAdi == "1.000 SMS" || PaketAdi == "5.000 SMS" || PaketAdi == "10.000 SMS" || PaketAdi == "25.000 SMS")
            //    PaketTip = "SMS";


            int SmsMiktar = 0;
            if (checkoutForm.PaymentStatus == "SUCCESS")
            {
                // Sipariş durumu güncelleniyor...
                sip.Odendi = true;
                sip.Durum = 1; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                sip.DegistirenKullaniciId = KullaniciId;
                sip.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();

                if (paket.PaketTip == "SMS")
                {
                    // Sipariş edilen SMS miktarını bulma
                    //if (sip.Paket == "1.000 SMS")
                    //    SmsMiktar = 1000;
                    //else if (sip.Paket == "5.000 SMS")
                    //    SmsMiktar = 5000;
                    //else if (sip.Paket == "10.000 SMS")
                    //    SmsMiktar = 1000;
                    //else if (sip.Paket == "25.000 SMS")
                    //    SmsMiktar = 25000;

                    SmsMiktar = paket.SMSMiktar;
                    //---------
                    SmsHareket smshareket = new SmsHareket();
                    smshareket.FirmaId = FirmaId;
                    smshareket.YuklemeTarihi = DateTime.Now;
                    smshareket.Miktar = SmsMiktar;
                    smshareket.Açıklama = SiparisNo + " numaralı sipariş ile alınan SMS bakiyesi";
                    smshareket.OlusturanKullaniciId = KullaniciId;
                    smshareket.OlusturmaTarih = DateTime.Now;
                    smshareket.DegistirenKullaniciId = KullaniciId;
                    smshareket.DegistirmeTarih = DateTime.Now;
                    smshareket.Aktif = true;
                    smshareket.Sil = false;
                    dbContext.SmsHarekets.Add(smshareket);

                    SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                    smsbakiye.ToplamKontur = smsbakiye.Bakiye + SmsMiktar;
                    smsbakiye.Bakiye = smsbakiye.Bakiye + SmsMiktar;
                    smsbakiye.DegistirenKullaniciId = KullaniciId;
                    smsbakiye.DegistirmeTarih = DateTime.Now;
                }
                else if (paket.PaketTip == "Lisans")
                {
                    Lisanslar lisans = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == FirmaId);
                    //List<ModulSayfa> modulsayfa = dbContext.ModulSayfas.Where(x => x.FirmaId == FirmaId).ToList();
                    // Lisans Süresi işlemleri yapılıyor...
                    int sure = sip.LisansSuresi;
                    DateTime lbitis = lisans.LisansBitisTarihi;
                    if (lbitis < DateTime.Now)
                    {
                        lisans.LisansBitisTarihi = DateTime.Now.AddMonths(sure);
                    }
                    else
                    {
                        lisans.LisansBitisTarihi = lbitis.AddMonths(sure);
                    }
                    lisans.LisansYenilemeTarihi = DateTime.Now;

                    // Lisans Modül işlemleri yapılıyor...

                    SmsMiktar = sure * paket.SMSMiktar;
                    lisans.Dashboard = paket.Dashboard;
                    lisans.RezervasyonIslemleri = paket.RezervasyonIslemleri;
                    lisans.RezervasyonIslemleri_Randevular = paket.RezervasyonIslemleri_Randevular;
                    lisans.RezervasyonIslemleri_RezervasyonListesi = paket.RezervasyonIslemleri_RezervasyonListesi;
                    lisans.RezervasyonIslemleri_RezervasyonTakvimi = paket.RezervasyonIslemleri_RezervasyonTakvimi;
                    lisans.RezervasyonIslemleri_RezervasyonTeklifleri = paket.RezervasyonIslemleri_RezervasyonTeklifleri;
                    lisans.Muhasebe = paket.Muhasebe;
                    lisans.Muhasebe_AlinanGelecekOdemeler = paket.Muhasebe_AlinanGelecekOdemeler;
                    lisans.Muhasebe_GunlukIsler = paket.Muhasebe_GunlukIsler;
                    lisans.Muhasebe_GelirGiderler = paket.Muhasebe_GelirGiderler;
                    lisans.Muhasebe_KalanBakiyeler = paket.Muhasebe_KalanBakiyeler;
                    lisans.Muhasebe_Kasa = paket.Muhasebe_Kasa;
                    lisans.Muhasebe_Cariler = paket.Muhasebe_Cariler;
                    lisans.Muhasebe_Cariler_CariHesapTakibi = paket.Muhasebe_Cariler_CariHesapTakibi;
                    lisans.Muhasebe_Cariler_CariListesi = paket.Muhasebe_Cariler_CariListesi;
                    lisans.Musteriler = paket.Musteriler;
                    lisans.Musteriler_MusteriHesapTakibi = paket.Musteriler_MusteriHesapTakibi;
                    lisans.Musteriler_MusteriListesi = paket.Musteriler_MusteriListesi;
                    lisans.Musteriler_FotografYukle = paket.Musteriler_FotografYukle;
                    lisans.Musteriler_MusteriMesajlari = paket.Musteriler_MusteriMesajlari;
                    lisans.Stok = paket.Stok;
                    lisans.Stok_DepoIslemleri = paket.Stok_DepoIslemleri;
                    lisans.Stok_DepoIslemleri_DepodanYapilmisCikislar = paket.Stok_DepoIslemleri_DepodanYapilmisCikislar;
                    lisans.Stok_DepoIslemleri_DepolarArasiTransfer = paket.Stok_DepoIslemleri_DepolarArasiTransfer;
                    lisans.Stok_DepoIslemleri_DepoListesi = paket.Stok_DepoIslemleri_DepoListesi;
                    lisans.Stok_DepoIslemleri_DepoStokDurumu = paket.Stok_DepoIslemleri_DepoStokDurumu;
                    lisans.Stok_DepoIslemleri_DepoyaYapılmışGirisler = paket.Stok_DepoIslemleri_DepoyaYapılmışGirisler;
                    lisans.Stok_Iade = paket.Stok_Iade;
                    lisans.Stok_StokCikisi = paket.Stok_StokCikisi;
                    lisans.Stok_StokGirisi = paket.Stok_StokGirisi;
                    lisans.Stok_StokHareketleri = paket.Stok_StokHareketleri;
                    lisans.Stok_StokKartlari = paket.Stok_StokKartlari;
                    lisans.Raporlar = paket.Raporlar;
                    lisans.Raporlar_GelecekOdemelerRaporu = paket.Raporlar_GelecekOdemelerRaporu;
                    lisans.Raporlar_GelirGiderRaporlari = paket.Raporlar_GelirGiderRaporlari;
                    lisans.Raporlar_RezervasyonIslemAdetleriRaporu = paket.Raporlar_RezervasyonIslemAdetleriRaporu;
                    lisans.Raporlar_RezervasyonTurleriRaporu = paket.Raporlar_RezervasyonTurleriRaporu;
                    lisans.Raporlar_HaftalikRapor = paket.Raporlar_HaftalikRapor;
                    lisans.Raporlar_RezervasyonEkHizmetleriRaporu = paket.Raporlar_RezervasyonEkHizmetleriRaporu;
                    lisans.Raporlar_CekimPaketleriRaporu = paket.Raporlar_CekimPaketleriRaporu;
                    lisans.Raporlar_PersonelPerformansRaporu = paket.Raporlar_PersonelPerformansRaporu;
                    lisans.TelefonRehberi = paket.TelefonRehberi;
                    lisans.TelefonRehberi_Rehber = paket.TelefonRehberi_Rehber;
                    lisans.TelefonRehberi_ExceldenEkle = paket.TelefonRehberi_ExceldenEkle;
                    lisans.Personeller = paket.Personeller;
                    lisans.Personeller_PersonelListesi = paket.Personeller_PersonelListesi;
                    lisans.Personeller_PersonelIzinleri = paket.Personeller_PersonelIzinleri;
                    lisans.Personeller_PersonelOdemeleri = paket.Personeller_PersonelOdemeleri;
                    lisans.Personeller_PersonelIsTakibi = paket.Personeller_PersonelIsTakibi;
                    lisans.Tanimlar = paket.Tanimlar;
                    lisans.Tanimlar_CekimPaketleri = paket.Tanimlar_CekimPaketleri;
                    lisans.Tanimlar_EmailMetinleri = paket.Tanimlar_EmailMetinleri;
                    lisans.Tanimlar_GelirGiderTurleri = paket.Tanimlar_GelirGiderTurleri;
                    lisans.Tanimlar_GunlukIsKategorileri = paket.Tanimlar_GunlukIsKategorileri;
                    lisans.Tanimlar_PersonelGorevleri = paket.Tanimlar_PersonelGorevleri;
                    lisans.Tanimlar_RehberGruplari = paket.Tanimlar_RehberGruplari;
                    lisans.Tanimlar_RezervasyonEkHizmetleri = paket.Tanimlar_RezervasyonEkHizmetleri;
                    lisans.Tanimlar_RezervasyonTurleri = paket.Tanimlar_RezervasyonTurleri;
                    lisans.Tanimlar_SmsMetinleri = paket.Tanimlar_SmsMetinleri;
                    lisans.Tanimlar_SozlesmeSartlari = paket.Tanimlar_SozlesmeSartlari;
                    lisans.Tanimlar_Sureler = paket.Tanimlar_Sureler;
                    lisans.Tanimlar_TatilGunleri = paket.Tanimlar_TatilGunleri;
                    lisans.Tanimlar_ZamanDilimleri = paket.Tanimlar_ZamanDilimleri;
                    lisans.Ayarlar = paket.Ayarlar;
                    lisans.Ayarlar_EmailGonderimAyarlari = paket.Ayarlar_EmailGonderimAyarlari;
                    lisans.Ayarlar_EmailHesapAyarlari = paket.Ayarlar_EmailHesapAyarlari;
                    lisans.Ayarlar_GenelAyarlar = paket.Ayarlar_GenelAyarlar;
                    lisans.Ayarlar_ListelemeFiltreAyarlari = paket.Ayarlar_ListelemeFiltreAyarlari;
                    lisans.Ayarlar_MusteriAyarlari = paket.Ayarlar_MusteriAyarlari;
                    lisans.Ayarlar_RezervasyonAyarlari = paket.Ayarlar_RezervasyonAyarlari;
                    lisans.Ayarlar_SmsGonderimAyarlari = paket.Ayarlar_SmsGonderimAyarlari;
                    lisans.Ayarlar_SozlesmeCiktiAyarlari = paket.Ayarlar_SozlesmeCiktiAyarlari;
                    lisans.Ayarlar_Personeller = paket.Ayarlar_Personeller;
                    lisans.FirmaSubeIslemleri = paket.FirmaSubeIslemleri;
                    lisans.FirmaSubeIslemleri_FirmaBilgileri = paket.FirmaSubeIslemleri_FirmaBilgileri;
                    lisans.FirmaSubeIslemleri_SubeIslemleri = paket.FirmaSubeIslemleri_SubeIslemleri;
                    lisans.KullaniciYetkiIslemleri = paket.KullaniciYetkiIslemleri;
                    lisans.KullaniciYetkiIslemleri_KullaniciListesi = paket.KullaniciYetkiIslemleri_KullaniciListesi;
                    lisans.KullaniciYetkiIslemleri_KullaniciYetkilendirme = paket.KullaniciYetkiIslemleri_KullaniciYetkilendirme;
                    lisans.SmsGonder = paket.SmsGonder;
                    lisans.SatinAl = paket.SatinAl;
                    lisans.SatinAl_PaketSec = paket.SatinAl_PaketSec;
                    lisans.SatinAl_Siparisler = paket.SatinAl_Siparisler;
                    lisans.Destek = paket.Destek;
                    lisans.Destek_DestekDetay = paket.Destek_DestekDetay;
                    lisans.Destek_DestekTalepleri = paket.Destek_DestekTalepleri;
                    lisans.SubeLimit = paket.SubeLimit;
                    lisans.KullaniciLimit = paket.KullaniciLimit;
                    lisans.PersonelLimit = paket.PersonelLimit;
                    lisans.DegistirenKullaniciId = 1;
                    lisans.DegistirmeTarih = DateTime.Now;

                    #region KAPATILAN KOD
                    //if (sip.Paket == "Basit Lisans (60 TL/Ay)")
                    //{
                    //    SmsMiktar = sure * 100;
                    //    lisans.Dashboard = true;
                    //    lisans.RezervasyonIslemleri = true;
                    //    lisans.RezervasyonIslemleri_Randevular = true;
                    //    lisans.RezervasyonIslemleri_RezervasyonListesi = true;
                    //    lisans.RezervasyonIslemleri_RezervasyonTakvimi = true;
                    //    lisans.RezervasyonIslemleri_RezervasyonTeklifleri = true;
                    //    lisans.Muhasebe = true;
                    //    lisans.Muhasebe_AlinanGelecekOdemeler = true;
                    //    lisans.Muhasebe_GunlukIsler = true;
                    //    lisans.Muhasebe_GelirGiderler = true;
                    //    lisans.Muhasebe_KalanBakiyeler = false;
                    //    lisans.Muhasebe_Kasa = true;
                    //    lisans.Muhasebe_Cariler = false;
                    //    lisans.Muhasebe_Cariler_CariHesapTakibi = false;
                    //    lisans.Muhasebe_Cariler_CariListesi = false;
                    //    lisans.Musteriler = true;
                    //    lisans.Musteriler_MusteriHesapTakibi = true;
                    //    lisans.Musteriler_MusteriListesi = true;
                    //    lisans.Musteriler_FotografYukle = false;
                    //    lisans.Musteriler_MusteriMesajlari = false;
                    //    lisans.Stok = false;
                    //    lisans.Stok_DepoIslemleri = false;
                    //    lisans.Stok_DepoIslemleri_DepodanYapilmisCikislar = false;
                    //    lisans.Stok_DepoIslemleri_DepolarArasiTransfer = false;
                    //    lisans.Stok_DepoIslemleri_DepoListesi = false;
                    //    lisans.Stok_DepoIslemleri_DepoStokDurumu = false;
                    //    lisans.Stok_DepoIslemleri_DepoyaYapılmışGirisler = false;
                    //    lisans.Stok_Iade = false;
                    //    lisans.Stok_StokCikisi = false;
                    //    lisans.Stok_StokGirisi = false;
                    //    lisans.Stok_StokHareketleri = false;
                    //    lisans.Stok_StokKartlari = false;
                    //    lisans.Raporlar = true;
                    //    lisans.Raporlar_GelecekOdemelerRaporu = true;
                    //    lisans.Raporlar_GelirGiderRaporlari = true;
                    //    lisans.Raporlar_RezervasyonIslemAdetleriRaporu = true;
                    //    lisans.Raporlar_RezervasyonTurleriRaporu = true;
                    //    lisans.Raporlar_HaftalikRapor = true;
                    //    lisans.Raporlar_RezervasyonEkHizmetleriRaporu = true;
                    //    lisans.Raporlar_RezervasyonRaporlari = true;
                    //    lisans.TelefonRehberi = true;
                    //    lisans.TelefonRehberi_Rehber = true;
                    //    lisans.TelefonRehberi_ExceldenEkle = false;
                    //    lisans.Personeller = true;
                    //    lisans.Personeller_PersonelListesi = true;
                    //    lisans.Personeller_PersonelIzinleri = false;
                    //    lisans.Personeller_PersonelTakibi = false;
                    //    lisans.Tanimlar = true;
                    //    lisans.Tanimlar_CekimPaketleri = true;
                    //    lisans.Tanimlar_EmailMetinleri = true;
                    //    lisans.Tanimlar_GelirGiderTurleri = true;
                    //    lisans.Tanimlar_GunlukIsKategorileri = true;
                    //    lisans.Tanimlar_PersonelGorevleri = true;
                    //    lisans.Tanimlar_RehberGruplari = true;
                    //    lisans.Tanimlar_RezervasyonEkHizmetleri = true;
                    //    lisans.Tanimlar_RezervasyonTurleri = true;
                    //    lisans.Tanimlar_SmsMetinleri = true;
                    //    lisans.Tanimlar_SozlesmeSartlari = true;
                    //    lisans.Tanimlar_Sureler = true;
                    //    lisans.Tanimlar_TatilGunleri = true;
                    //    lisans.Tanimlar_ZamanDilimleri = true;
                    //    lisans.Ayarlar = true;
                    //    lisans.Ayarlar_EmailGonderimAyarlari = true;
                    //    lisans.Ayarlar_EmailHesapAyarlari = true;
                    //    lisans.Ayarlar_GenelAyarlar = true;
                    //    lisans.Ayarlar_ListelemeFiltreAyarlari = true;
                    //    lisans.Ayarlar_MusteriAyarlari = true;
                    //    lisans.Ayarlar_RezervasyonAyarlari = true;
                    //    lisans.Ayarlar_SmsGonderimAyarlari = true;
                    //    lisans.Ayarlar_SozlesmeCiktiAyarlari = true;
                    //    lisans.FirmaSubeIslemleri = true;
                    //    lisans.FirmaSubeIslemleri_FirmaBilgileri = true;
                    //    lisans.FirmaSubeIslemleri_SubeIslemleri = true;
                    //    lisans.KullaniciYetkiIslemleri = true;
                    //    lisans.KullaniciYetkiIslemleri_KullaniciListesi = true;
                    //    lisans.KullaniciYetkiIslemleri_KullaniciYetkilendirme = true;
                    //    lisans.SmsGonder = true;
                    //    lisans.SatinAl = true;
                    //    lisans.SatinAl_PaketSec = true;
                    //    lisans.SatinAl_Siparisler = true;
                    //    lisans.Destek = true;
                    //    lisans.Destek_DestekDetay = true;
                    //    lisans.Destek_DestekTalepleri = true;
                    //    lisans.SubeLimit = "1";
                    //    lisans.KullaniciLimit = "5";
                    //    lisans.PersonelLimit = "5";

                    //    //foreach (var modul in modulsayfa)
                    //    //{
                    //    //    #region Rezervasyonİşlemleri
                    //    //    if (modul.SayfaAdi == "Rezervasyon İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Randevular")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Takvimi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Teklifleri")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Muhasebe
                    //    //    if (modul.SayfaAdi == "Muhasebe")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Günlük İşler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelirler - Giderler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Alınan/Gelecek Ödemeler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kalan Bakiyeler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kasa")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Cariler")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Cari Listesi")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Cari Hesap Takibi")
                    //    //        modul.Aktif = false;
                    //    //    #endregion
                    //    //    #region Müşteriler
                    //    //    if (modul.SayfaAdi == "Müşteriler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Hesap Takibi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Mesajları")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Fotoğraf Yükle")
                    //    //        modul.Aktif = false;
                    //    //    #endregion
                    //    //    #region Stok
                    //    //    if (modul.SayfaAdi == "Stok")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Stok Kartları")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Stok Girişi")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Stok Çıkışı")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "StokHareketleri")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "İade")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depo İşlemleri")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depo Listesi")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depolar Arası Trasfer")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depo Stok Durumu")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depoya Yapılmış Girişler")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depodan Yapılmış Çıkışlar")
                    //    //        modul.Aktif = false;
                    //    //    #endregion
                    //    //    #region Raporlar
                    //    //    if (modul.SayfaAdi == "Raporlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Raporları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon İşlem Adetleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelir - Gider Raporları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Türleri Raporu")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelecek Ödemeler Raporu")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Haftalık Rapor")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyok Ek Hizmetler Raporu")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Rehber
                    //    //    if (modul.SayfaAdi == "Telefon Rehberi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rehber")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Excel'den Aktar")
                    //    //        modul.Aktif = false;
                    //    //    #endregion
                    //    //    #region Personeller
                    //    //    if (modul.SayfaAdi == "Personeller")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel İzinleri")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Personel Takibi")
                    //    //        modul.Aktif = false;
                    //    //    #endregion
                    //    //    #region Tanımlar
                    //    //    if (modul.SayfaAdi == "Tanımlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Çekim Paketleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelir/Gider Türleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Günlük İş Kategorileri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel Görevleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rehber Grupları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Ek Hizmetleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Türleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Email Metinleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "SMS Metinleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Sözleşme Şartları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Süreçler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Tatil Günleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Zaman Dilimleri")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Ayarlar
                    //    //    if (modul.SayfaAdi == "Ayarlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Genel Ayarlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Email Gönderim Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Email Hesap Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Listeleme Filtre Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "SMS Gönderim Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Sözleşme Çıktı Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region FirmaŞubeİşlemleri
                    //    //    if (modul.SayfaAdi == "Firma/Şube İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Firma Bilgileri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Şube İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region KullanıcıYetkiİşlemleri
                    //    //    if (modul.SayfaAdi == "Kullanıcı/Yetki İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kullanıcı Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kullanıcı Yetkilendir")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region SatınAl
                    //    //    if (modul.SayfaAdi == "Satın Al")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Paket Seç")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Siparişlerim")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Destek
                    //    //    if (modul.SayfaAdi == "Destek")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Destek Talepleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Destek Talepleri Detay")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    if (modul.SayfaAdi == "SMS Gönder")
                    //    //        modul.Aktif = true;
                    //    //}
                    //}
                    //else if (sip.Paket == "Standart Lisans (75 TL/Ay)")
                    //{
                    //    SmsMiktar = sure * 250;
                    //    lisans.Dashboard = true;
                    //    lisans.RezervasyonIslemleri = true;
                    //    lisans.RezervasyonIslemleri_Randevular = true;
                    //    lisans.RezervasyonIslemleri_RezervasyonListesi = true;
                    //    lisans.RezervasyonIslemleri_RezervasyonTakvimi = true;
                    //    lisans.RezervasyonIslemleri_RezervasyonTeklifleri = true;
                    //    lisans.Muhasebe = true;
                    //    lisans.Muhasebe_AlinanGelecekOdemeler = true;
                    //    lisans.Muhasebe_GunlukIsler = true;
                    //    lisans.Muhasebe_GelirGiderler = true;
                    //    lisans.Muhasebe_KalanBakiyeler = false;
                    //    lisans.Muhasebe_Kasa = true;
                    //    lisans.Muhasebe_Cariler = true;
                    //    lisans.Muhasebe_Cariler_CariHesapTakibi = true;
                    //    lisans.Muhasebe_Cariler_CariListesi = true;
                    //    lisans.Musteriler = true;
                    //    lisans.Musteriler_MusteriHesapTakibi = true;
                    //    lisans.Musteriler_MusteriListesi = true;
                    //    lisans.Musteriler_FotografYukle = false;
                    //    lisans.Musteriler_MusteriMesajlari = false;
                    //    lisans.Stok = false;
                    //    lisans.Stok_DepoIslemleri = false;
                    //    lisans.Stok_DepoIslemleri_DepodanYapilmisCikislar = false;
                    //    lisans.Stok_DepoIslemleri_DepolarArasiTransfer = false;
                    //    lisans.Stok_DepoIslemleri_DepoListesi = false;
                    //    lisans.Stok_DepoIslemleri_DepoStokDurumu = false;
                    //    lisans.Stok_DepoIslemleri_DepoyaYapılmışGirisler = false;
                    //    lisans.Stok_Iade = false;
                    //    lisans.Stok_StokCikisi = false;
                    //    lisans.Stok_StokGirisi = false;
                    //    lisans.Stok_StokHareketleri = false;
                    //    lisans.Stok_StokKartlari = false;
                    //    lisans.Raporlar = true;
                    //    lisans.Raporlar_GelecekOdemelerRaporu = true;
                    //    lisans.Raporlar_GelirGiderRaporlari = true;
                    //    lisans.Raporlar_RezervasyonIslemAdetleriRaporu = true;
                    //    lisans.Raporlar_RezervasyonTurleriRaporu = true;
                    //    lisans.Raporlar_HaftalikRapor = true;
                    //    lisans.Raporlar_RezervasyonEkHizmetleriRaporu = true;
                    //    lisans.Raporlar_RezervasyonRaporlari = true;
                    //    lisans.TelefonRehberi = true;
                    //    lisans.TelefonRehberi_Rehber = true;
                    //    lisans.TelefonRehberi_ExceldenEkle = false;
                    //    lisans.Personeller = true;
                    //    lisans.Personeller_PersonelListesi = true;
                    //    lisans.Personeller_PersonelIzinleri = true;
                    //    lisans.Personeller_PersonelTakibi = true;
                    //    lisans.Tanimlar = true;
                    //    lisans.Tanimlar_CekimPaketleri = true;
                    //    lisans.Tanimlar_EmailMetinleri = true;
                    //    lisans.Tanimlar_GelirGiderTurleri = true;
                    //    lisans.Tanimlar_GunlukIsKategorileri = true;
                    //    lisans.Tanimlar_PersonelGorevleri = true;
                    //    lisans.Tanimlar_RehberGruplari = true;
                    //    lisans.Tanimlar_RezervasyonEkHizmetleri = true;
                    //    lisans.Tanimlar_RezervasyonTurleri = true;
                    //    lisans.Tanimlar_SmsMetinleri = true;
                    //    lisans.Tanimlar_SozlesmeSartlari = true;
                    //    lisans.Tanimlar_Sureler = true;
                    //    lisans.Tanimlar_TatilGunleri = true;
                    //    lisans.Tanimlar_ZamanDilimleri = true;
                    //    lisans.Ayarlar = true;
                    //    lisans.Ayarlar_EmailGonderimAyarlari = true;
                    //    lisans.Ayarlar_EmailHesapAyarlari = true;
                    //    lisans.Ayarlar_GenelAyarlar = true;
                    //    lisans.Ayarlar_ListelemeFiltreAyarlari = true;
                    //    lisans.Ayarlar_MusteriAyarlari = true;
                    //    lisans.Ayarlar_RezervasyonAyarlari = true;
                    //    lisans.Ayarlar_SmsGonderimAyarlari = true;
                    //    lisans.Ayarlar_SozlesmeCiktiAyarlari = true;
                    //    lisans.FirmaSubeIslemleri = true;
                    //    lisans.FirmaSubeIslemleri_FirmaBilgileri = true;
                    //    lisans.FirmaSubeIslemleri_SubeIslemleri = true;
                    //    lisans.KullaniciYetkiIslemleri = true;
                    //    lisans.KullaniciYetkiIslemleri_KullaniciListesi = true;
                    //    lisans.KullaniciYetkiIslemleri_KullaniciYetkilendirme = true;
                    //    lisans.SmsGonder = true;
                    //    lisans.SatinAl = true;
                    //    lisans.SatinAl_PaketSec = true;
                    //    lisans.SatinAl_Siparisler = true;
                    //    lisans.Destek = true;
                    //    lisans.Destek_DestekDetay = true;
                    //    lisans.Destek_DestekTalepleri = true;
                    //    lisans.SubeLimit = "5";
                    //    lisans.KullaniciLimit = "25";
                    //    lisans.PersonelLimit = "25";

                    //    //foreach (var modul in modulsayfa)
                    //    //{
                    //    //    #region Rezervasyonİşlemleri
                    //    //    if (modul.SayfaAdi == "Rezervasyon İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Randevular")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Takvimi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Teklifleri")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Muhasebe
                    //    //    if (modul.SayfaAdi == "Muhasebe")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Günlük İşler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelirler - Giderler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Alınan/Gelecek Ödemeler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kalan Bakiyeler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kasa")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Cariler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Cari Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Cari Hesap Takibi")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Müşteriler
                    //    //    if (modul.SayfaAdi == "Müşteriler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Hesap Takibi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Mesajları")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Fotoğraf Yükle")
                    //    //        modul.Aktif = false;
                    //    //    #endregion
                    //    //    #region Stok
                    //    //    if (modul.SayfaAdi == "Stok")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Stok Kartları")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Stok Girişi")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Stok Çıkışı")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "StokHareketleri")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "İade")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depo İşlemleri")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depo Listesi")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depolar Arası Trasfer")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depo Stok Durumu")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depoya Yapılmış Girişler")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depodan Yapılmış Çıkışlar")
                    //    //        modul.Aktif = false;
                    //    //    #endregion
                    //    //    #region Raporlar
                    //    //    if (modul.SayfaAdi == "Raporlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Raporları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon İşlem Adetleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelir - Gider Raporları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Türleri Raporu")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelecek Ödemeler Raporu")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Haftalık Rapor")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyok Ek Hizmetler Raporu")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Rehber
                    //    //    if (modul.SayfaAdi == "Telefon Rehberi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rehber")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Excel'den Aktar")
                    //    //        modul.Aktif = false;
                    //    //    #endregion
                    //    //    #region Personeller
                    //    //    if (modul.SayfaAdi == "Personeller")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel İzinleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel Takibi")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Tanımlar
                    //    //    if (modul.SayfaAdi == "Tanımlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Çekim Paketleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelir/Gider Türleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Günlük İş Kategorileri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel Görevleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rehber Grupları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Ek Hizmetleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Türleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Email Metinleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "SMS Metinleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Sözleşme Şartları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Süreçler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Tatil Günleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Zaman Dilimleri")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Ayarlar
                    //    //    if (modul.SayfaAdi == "Ayarlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Genel Ayarlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Email Gönderim Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Email Hesap Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Listeleme Filtre Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "SMS Gönderim Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Sözleşme Çıktı Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region FirmaŞubeİşlemleri
                    //    //    if (modul.SayfaAdi == "Firma/Şube İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Firma Bilgileri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Şube İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region KullanıcıYetkiİşlemleri
                    //    //    if (modul.SayfaAdi == "Kullanıcı/Yetki İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kullanıcı Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kullanıcı Yetkilendir")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region SatınAl
                    //    //    if (modul.SayfaAdi == "Satın Al")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Paket Seç")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Siparişlerim")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Destek
                    //    //    if (modul.SayfaAdi == "Destek")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Destek Talepleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Destek Talepleri Detay")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    if (modul.SayfaAdi == "SMS Gönder")
                    //    //        modul.Aktif = true;
                    //    //}
                    //}
                    //else if (sip.Paket == "Profesyonel Lisans (90 TL/Ay)")
                    //{
                    //    SmsMiktar = sure * 500;
                    //    lisans.Dashboard = true;
                    //    lisans.RezervasyonIslemleri = true;
                    //    lisans.RezervasyonIslemleri_Randevular = true;
                    //    lisans.RezervasyonIslemleri_RezervasyonListesi = true;
                    //    lisans.RezervasyonIslemleri_RezervasyonTakvimi = true;
                    //    lisans.RezervasyonIslemleri_RezervasyonTeklifleri = true;
                    //    lisans.Muhasebe = true;
                    //    lisans.Muhasebe_AlinanGelecekOdemeler = true;
                    //    lisans.Muhasebe_GunlukIsler = true;
                    //    lisans.Muhasebe_GelirGiderler = true;
                    //    lisans.Muhasebe_KalanBakiyeler = false;
                    //    lisans.Muhasebe_Kasa = true;
                    //    lisans.Muhasebe_Cariler = true;
                    //    lisans.Muhasebe_Cariler_CariHesapTakibi = true;
                    //    lisans.Muhasebe_Cariler_CariListesi = true;
                    //    lisans.Musteriler = true;
                    //    lisans.Musteriler_MusteriHesapTakibi = true;
                    //    lisans.Musteriler_MusteriListesi = true;
                    //    lisans.Musteriler_FotografYukle = true;
                    //    lisans.Musteriler_MusteriMesajlari = true;
                    //    lisans.Stok = false;
                    //    lisans.Stok_DepoIslemleri = false;
                    //    lisans.Stok_DepoIslemleri_DepodanYapilmisCikislar = false;
                    //    lisans.Stok_DepoIslemleri_DepolarArasiTransfer = false;
                    //    lisans.Stok_DepoIslemleri_DepoListesi = false;
                    //    lisans.Stok_DepoIslemleri_DepoStokDurumu = false;
                    //    lisans.Stok_DepoIslemleri_DepoyaYapılmışGirisler = false;
                    //    lisans.Stok_Iade = false;
                    //    lisans.Stok_StokCikisi = false;
                    //    lisans.Stok_StokGirisi = false;
                    //    lisans.Stok_StokHareketleri = false;
                    //    lisans.Stok_StokKartlari = false;
                    //    lisans.Raporlar = true;
                    //    lisans.Raporlar_GelecekOdemelerRaporu = true;
                    //    lisans.Raporlar_GelirGiderRaporlari = true;
                    //    lisans.Raporlar_RezervasyonIslemAdetleriRaporu = true;
                    //    lisans.Raporlar_RezervasyonTurleriRaporu = true;
                    //    lisans.Raporlar_HaftalikRapor = true;
                    //    lisans.Raporlar_RezervasyonEkHizmetleriRaporu = true;
                    //    lisans.Raporlar_RezervasyonRaporlari = true;
                    //    lisans.TelefonRehberi = true;
                    //    lisans.TelefonRehberi_Rehber = true;
                    //    lisans.TelefonRehberi_ExceldenEkle = false;
                    //    lisans.Personeller = true;
                    //    lisans.Personeller_PersonelListesi = true;
                    //    lisans.Personeller_PersonelIzinleri = true;
                    //    lisans.Personeller_PersonelTakibi = true;
                    //    lisans.Tanimlar = true;
                    //    lisans.Tanimlar_CekimPaketleri = true;
                    //    lisans.Tanimlar_EmailMetinleri = true;
                    //    lisans.Tanimlar_GelirGiderTurleri = true;
                    //    lisans.Tanimlar_GunlukIsKategorileri = true;
                    //    lisans.Tanimlar_PersonelGorevleri = true;
                    //    lisans.Tanimlar_RehberGruplari = true;
                    //    lisans.Tanimlar_RezervasyonEkHizmetleri = true;
                    //    lisans.Tanimlar_RezervasyonTurleri = true;
                    //    lisans.Tanimlar_SmsMetinleri = true;
                    //    lisans.Tanimlar_SozlesmeSartlari = true;
                    //    lisans.Tanimlar_Sureler = true;
                    //    lisans.Tanimlar_TatilGunleri = true;
                    //    lisans.Tanimlar_ZamanDilimleri = true;
                    //    lisans.Ayarlar = true;
                    //    lisans.Ayarlar_EmailGonderimAyarlari = true;
                    //    lisans.Ayarlar_EmailHesapAyarlari = true;
                    //    lisans.Ayarlar_GenelAyarlar = true;
                    //    lisans.Ayarlar_ListelemeFiltreAyarlari = true;
                    //    lisans.Ayarlar_MusteriAyarlari = true;
                    //    lisans.Ayarlar_RezervasyonAyarlari = true;
                    //    lisans.Ayarlar_SmsGonderimAyarlari = true;
                    //    lisans.Ayarlar_SozlesmeCiktiAyarlari = true;
                    //    lisans.FirmaSubeIslemleri = true;
                    //    lisans.FirmaSubeIslemleri_FirmaBilgileri = true;
                    //    lisans.FirmaSubeIslemleri_SubeIslemleri = true;
                    //    lisans.KullaniciYetkiIslemleri = true;
                    //    lisans.KullaniciYetkiIslemleri_KullaniciListesi = true;
                    //    lisans.KullaniciYetkiIslemleri_KullaniciYetkilendirme = true;
                    //    lisans.SmsGonder = true;
                    //    lisans.SatinAl = true;
                    //    lisans.SatinAl_PaketSec = true;
                    //    lisans.SatinAl_Siparisler = true;
                    //    lisans.Destek = true;
                    //    lisans.Destek_DestekDetay = true;
                    //    lisans.Destek_DestekTalepleri = true;
                    //    lisans.SubeLimit = "10";
                    //    lisans.KullaniciLimit = "Sınırsız";
                    //    lisans.PersonelLimit = "Sınırsız";

                    //    //foreach (var modul in modulsayfa)
                    //    //{
                    //    //    #region Rezervasyonİşlemleri
                    //    //    if (modul.SayfaAdi == "Rezervasyon İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Randevular")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Takvimi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Teklifleri")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Muhasebe
                    //    //    if (modul.SayfaAdi == "Muhasebe")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Günlük İşler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelirler - Giderler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Alınan/Gelecek Ödemeler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kalan Bakiyeler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kasa")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Cariler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Cari Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Cari Hesap Takibi")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Müşteriler
                    //    //    if (modul.SayfaAdi == "Müşteriler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Hesap Takibi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Mesajları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Fotoğraf Yükle")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Stok
                    //    //    if (modul.SayfaAdi == "Stok")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Stok Kartları")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Stok Girişi")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Stok Çıkışı")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "StokHareketleri")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "İade")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depo İşlemleri")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depo Listesi")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depolar Arası Trasfer")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depo Stok Durumu")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depoya Yapılmış Girişler")
                    //    //        modul.Aktif = false;
                    //    //    if (modul.SayfaAdi == "Depodan Yapılmış Çıkışlar")
                    //    //        modul.Aktif = false;
                    //    //    #endregion
                    //    //    #region Raporlar
                    //    //    if (modul.SayfaAdi == "Raporlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Raporları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon İşlem Adetleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelir - Gider Raporları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Türleri Raporu")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelecek Ödemeler Raporu")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Haftalık Rapor")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyok Ek Hizmetler Raporu")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Rehber
                    //    //    if (modul.SayfaAdi == "Telefon Rehberi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rehber")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Excel'den Aktar")
                    //    //        modul.Aktif = false;
                    //    //    #endregion
                    //    //    #region Personeller
                    //    //    if (modul.SayfaAdi == "Personeller")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel İzinleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel Takibi")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Tanımlar
                    //    //    if (modul.SayfaAdi == "Tanımlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Çekim Paketleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Gelir/Gider Türleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Günlük İş Kategorileri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Personel Görevleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rehber Grupları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Ek Hizmetleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Türleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Email Metinleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "SMS Metinleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Sözleşme Şartları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Süreçler")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Tatil Günleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Zaman Dilimleri")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Ayarlar
                    //    //    if (modul.SayfaAdi == "Ayarlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Genel Ayarlar")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Email Gönderim Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Email Hesap Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Listeleme Filtre Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "SMS Gönderim Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Müşteri Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Rezervasyon Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Sözleşme Çıktı Ayarları")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region FirmaŞubeİşlemleri
                    //    //    if (modul.SayfaAdi == "Firma/Şube İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Firma Bilgileri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Şube İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region KullanıcıYetkiİşlemleri
                    //    //    if (modul.SayfaAdi == "Kullanıcı/Yetki İşlemleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kullanıcı Listesi")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Kullanıcı Yetkilendir")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region SatınAl
                    //    //    if (modul.SayfaAdi == "Satın Al")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Paket Seç")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Siparişlerim")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    #region Destek
                    //    //    if (modul.SayfaAdi == "Destek")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Destek Talepleri")
                    //    //        modul.Aktif = true;
                    //    //    if (modul.SayfaAdi == "Destek Talepleri Detay")
                    //    //        modul.Aktif = true;
                    //    //    #endregion
                    //    //    if (modul.SayfaAdi == "SMS Gönder")
                    //    //        modul.Aktif = true;
                    //    //}
                    //}
                    #endregion

                    #region Hediye SMS Yüklemesi
                    SmsHareket smshareket = new SmsHareket();
                    smshareket.FirmaId = FirmaId;
                    smshareket.YuklemeTarihi = DateTime.Now;
                    smshareket.Miktar = SmsMiktar;
                    smshareket.Açıklama = SiparisNo + " numaralı sipariş ile alınan SMS bakiyesi";
                    smshareket.OlusturanKullaniciId = KullaniciId;
                    smshareket.OlusturmaTarih = DateTime.Now;
                    smshareket.DegistirenKullaniciId = KullaniciId;
                    smshareket.DegistirmeTarih = DateTime.Now;
                    smshareket.Aktif = true;
                    smshareket.Sil = false;
                    dbContext.SmsHarekets.Add(smshareket);

                    SmsBakiye smsbakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Sil == false && x.Aktif == true);
                    smsbakiye.ToplamKontur = smsbakiye.Bakiye + SmsMiktar;
                    smsbakiye.Bakiye = smsbakiye.Bakiye + SmsMiktar;
                    smsbakiye.DegistirenKullaniciId = KullaniciId;
                    smsbakiye.DegistirmeTarih = DateTime.Now;
                    #endregion
                }
                else
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "Lisans Güncelleme";
                    hata.HataMesajı = "Lisasn güncelleme - PaketTip: Belirsiz ";
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                }
                try
                {
                    dbContext.SaveChanges();
                    sip.Durum = 2; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                    sip.DegistirenKullaniciId = KullaniciId;
                    sip.DegistirmeTarih = DateTime.Now;
                    dbContext.SaveChanges();
                    Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
                    // Sirariş SMSi Gönder
                    string smstutar = sip.PaketTutar.ToString("C").Replace("₺", " ");
                    string sonuc_uyeol_sms = "";
                    string smsmetin = "Yeni Kredikartı Ödemesi, Lisans süresi biten firma, Firma Id: " + FirmaId + ", Firma adı: " + frm.FirmaAdi + ", Ödeme Tutarı: " + smstutar + " TL, SiparisNo: " + SiparisNo + ".";
                    sonuc_uyeol_sms = SMSGonder.Gonder_AtakSms(smsmetin, "05332625560", 1, 1, 1);
                    //
                    //ViewBag.Sonuc = "FAILURE";
                    Mesaj = "SUCCESS";
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = 1;
                    hata.Islem = "Lisasn Güncelleme";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                    ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                    return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });
                }
            }
            else if (checkoutForm.PaymentStatus == "FAILURE")
            {
                sip.Odendi = false;
                sip.Durum = 4; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
                sip.DegistirenKullaniciId = KullaniciId;
                sip.DegistirmeTarih = DateTime.Now;
                dbContext.SaveChanges();
                Mesaj = "FAILURE";
            }
            else
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Lisasn Güncelleme";
                hata.HataMesajı = "Lisasn güncelleme - Ödeme sonucu:Başarısız-Başarılı'dan farklıbir durum.";
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                ViewBag.Sonuc = "Lisans Güncelleme Hatası";
                return Json(new { Sonuc = false, Mesaj = "Kayıt esnasında bit hata oluştu, Lütfen Sistem Yöneticisine bilgi veriniz.", JsonRequestBehavior.AllowGet });

            }
            return RedirectToAction("GirisYap", "Giris");
            //return Json(new { Sonuc = true, Mesaj = "Siparişiniz Oluşturuldu", forum = checkoutForm }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FBankaSiparisOnay()
        {
            if (TempData["FirmaId"] == null)
                return RedirectToAction("FYetkisizGiris");

            long FirmaId = Convert.ToInt64(TempData["FirmaId"]);
            //long FirmaId = Convert.ToInt64(Session["FirmaId"]);
            //long SubeId = Convert.ToInt64(Session["AktifSubeId"]);
            //long KullaniciId = Convert.ToInt64(Session["Id"]);
            string LisansSuresi = Request["LisansSuresi"]; //0: Tek Kullanımlık 1: 1AY, 2: 3AY, 3: 6AY, 4: 12AY(1YIL), 5: 24AY(2YIL), 6: 36AY(3YIL)
            string PaketAdi = Request["PaketAdi"]; // Basit Lisans (60 TL/Ay), Standart Lisans (75 TL/Ay), Profesyonel Lisans (90 TL/Ay), 1.000 SMS, 5.000 SMS, 10.000 SMS, 25.000 SMS 
            string PaketTip = Request["PaketTip"];
            long SiparisNo = Convert.ToInt64(Request["SiparisNo"]);
            long PaketSatisFiyatId = Convert.ToInt64(Request["PaketSatisFiyatId"]);
            string PaketDetay = "";
            decimal PaketTutar = 0;
            short sure = 1;
            if (LisansSuresi == "1")
                sure = 1;
            else if (LisansSuresi == "2")
                sure = 3;
            else if (LisansSuresi == "3")
                sure = 6;
            else if (LisansSuresi == "4")
                sure = 12;
            else if (LisansSuresi == "5")
                sure = 24;
            else if (LisansSuresi == "6")
                sure = 36;

            List<SatisFiyatlari> SatisFiyatlari = dbContext.SatisFiyatlaris.Where(x => x.Aktif == true).ToList();
            foreach (var paket in SatisFiyatlari)
            {

                if (PaketAdi == paket.PaketAdi)
                {
                    PaketDetay = paket.PaketDetayi;
                    if (paket.PaketTip == "Lisans")
                    {
                        PaketTutar = paket.SatisFiyati * sure;
                    }
                    else
                    {
                        PaketTutar = paket.SatisFiyati;
                        sure = 0;
                    }
                }
            }
            Firma frm = dbContext.Firmas.FirstOrDefault(x => x.Id == FirmaId && x.Sil == false && x.Aktif == true);
            Siparisler sp = dbContext.Siparislers.FirstOrDefault(x => x.FirmaId == FirmaId && x.SiparisNo == SiparisNo && x.Iptal == false && x.Sil == false && x.Aktif == true);
            if (sp != null)
            {
                return RedirectToAction("Siparisler");
            }
            Siparisler siparis = new Siparisler();
            siparis.FirmaId = FirmaId;
            siparis.SatisFiyatId = PaketSatisFiyatId;
            siparis.SiparisNo = SiparisNo;
            siparis.Paket = PaketAdi;
            siparis.PaketDetay = PaketDetay;
            siparis.PaketTutar = PaketTutar;
            siparis.LisansSuresi = sure;
            siparis.Odendi = false;
            siparis.Durum = 0; // 0: Onay Bekliyor, 1: Onaylandı, 2: Tamamlandı, 3: İptal Edildi, 4: Ödeme Başarısız, 5: Ödeme Bekliyor
            siparis.Tarih = DateTime.Now;
            siparis.Iptal = false;
            siparis.OdemeBildirim = false;  // Müşteri Bu Sipariş ile ilgili bildirimde bulunursa true oluyor
            siparis.OdemeTuru = "1";  // 0: Kredi Kartı, 1: Banka Havale/Eft
            siparis.OlusturanKullaniciId = 1;
            siparis.OlusturmaTarih = DateTime.Now;
            siparis.DegistirenKullaniciId = 1;
            siparis.DegistirmeTarih = DateTime.Now;
            siparis.Sil = false;
            siparis.Aktif = true;
            dbContext.Siparislers.Add(siparis);
            try
            {
                dbContext.SaveChanges();
                // Sirariş SMSi Gönder ToString("C").Replace("₺", " ") TL
                string smstutar = PaketTutar.ToString("C").Replace("₺", " ");
                string sonuc_uyeol_sms = "";
                string smsmetin = "Yeni Banka Ödemesi, Sipariş onayı bekliyor. Lisans süresi biten firma, Firma Id: " + FirmaId + ", Firma adı: " + frm.FirmaAdi + ", Ödeme Tutarı: " + smstutar + " TL, SiparisNo: " + SiparisNo + ".";
                sonuc_uyeol_sms = SMSGonder.Gonder_AtakSms(smsmetin, "05332625560", 1, 1, 1);
                //
                return Json(new { Sonuc = true, Mesaj = "Siparişiniz Oluşturuldu.<br />Ödemeniz kontrol edilerek en kısa sürede Lisans Aktivasyonunuz gerçekleştirilecektir." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = 1;
                hata.Islem = "Sipariş Ekle";
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