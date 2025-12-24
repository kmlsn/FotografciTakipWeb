
using Newtonsoft.Json;
using FotografciTakipWeb.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using FotografciTakipWeb.App_Settings;

namespace FotografciTakipWeb.App_Settings
{

    public class SMSGonder
    {
        public static string Gonder(string SmsMetin, string GSMNo, long FirmaId, long SubeId, long KullaniciId)
        {
            FotoTakipContext dbContext = new FotoTakipContext();
            AyarlarSmsGonderim smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            string SmsMetinXml = "";
            if (!smsayar.SmsTurkceKarakter)
            {
                // Türkçe Karakterler: ğ, Ğ, ç, ş, Ş, ı, İ 
                SmsMetinXml = SmsMetin.Replace("ğ", "g");
                SmsMetinXml = SmsMetin.Replace("Ğ", "G");
                SmsMetinXml = SmsMetin.Replace("ç", "c");
                SmsMetinXml = SmsMetin.Replace("ş", "s");
                SmsMetinXml = SmsMetin.Replace("Ş", "S");
                SmsMetinXml = SmsMetin.Replace("ı", "I");
                SmsMetinXml = SmsMetin.Replace("İ", "I");
            }
            string Sifre = "***";
            string Kullanici_adi = "Kullanici_adi";
            string ss = "";
            string PostAddress = "https://api.netgsm.com.tr/sms/send/xml";
            ss += "<?xml version='1.0' encoding='UTF-8'?>";
            ss += "<mainbody>";
            ss += "<header>";
            ss += "<company dil='TR'>Netgsm</company>";
            ss += "<usercode>" + Kullanici_adi + "</usercode>";
            ss += "<password>" + Sifre + "</password>";
            ss += "<type>1:n</type>";
            ss += "<msgheader>StudyoTakip</msgheader>";
            ss += "</header>";
            ss += "<body>";
            ss += "<msg>";
            ss += "<![CDATA[" + SmsMetin + "]]>"; // Maximum 917 karakter.
            ss += "</msg>";
            ss += "<no>" + GSMNo + "</no>";
            //ss += "<no>051212312312</no>";
            ss += "</body>  ";
            ss += "</mainbody>";

            #region XML_Dosya Kayıt İşlemleri
            string dosyaadi = "";
            dosyaadi = "SMS_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString() + ".xml";

            string xmldosya = System.Web.HttpContext.Current.Server.MapPath("/Areas/Otomasyon/Dosyalar/" + dosyaadi);
            //
            XmlTextWriter xmlYaz = new XmlTextWriter(xmldosya, UTF8Encoding.UTF8);
            //XmlWriter xmlYaz = new XmlWriter(xmldosya);
            xmlYaz.WriteStartDocument();
            xmlYaz.WriteStartElement("mainbody");
            xmlYaz.WriteEndDocument();
            xmlYaz.Close();

            XmlDocument doc = new XmlDocument();
            doc.Load(xmldosya);

            XmlElement header = doc.CreateElement("header");
            XmlElement company = doc.CreateElement("company");
            company.SetAttribute("dil", "TR");
            company.InnerText = "Netgsm";
            XmlElement usercode = doc.CreateElement("usercode");
            usercode.InnerText = Kullanici_adi;
            XmlElement password = doc.CreateElement("password");
            password.InnerText = Sifre;
            XmlElement type = doc.CreateElement("type");
            type.InnerText = "1:n";
            XmlElement msgheader = doc.CreateElement("msgheader");
            msgheader.InnerText = "StudyoTakip";


            header.AppendChild(company); // header içine company alınıyor. 
            header.AppendChild(usercode); // header içine usercode alınıyor. 
            header.AppendChild(password); // header içine password alınıyor. 
            header.AppendChild(type); // header içine type alınıyor. 
            header.AppendChild(msgheader); // header içine msgheader alınıyor. 

            XmlElement body = doc.CreateElement("body");
            XmlElement msg = doc.CreateElement("msg");
            //XmlElement CDATA = doc.CreateElement("![CDATA[" + SmsMetin + "]]");
            XmlCDataSection cd = doc.CreateCDataSection(SmsMetin);
            XmlElement no = doc.CreateElement("no");
            no.InnerText = GSMNo;

            msg.AppendChild(cd); // msg içine CDATA alınıyor. 
            body.AppendChild(msg); // body içine msg alınıyor. 
            body.AppendChild(no); // body içine msg alınıyor. 

            doc.DocumentElement.AppendChild(header);
            doc.DocumentElement.AppendChild(body);

            XmlTextWriter xmlset = new XmlTextWriter(xmldosya, null);
            //xmlset.Formatting = Formatting.Indented;
            doc.WriteContentTo(xmlset);
            xmlset.Close();
            #endregion

            #region SMS_Gonderim ve Veritabanına Kayıt İşlemleri
            #region Karakter hesabı ve Bakiye düşüm işlemleri

            if (smsayar == null)
            {
                return "SMS Gönderilemedi";
            }
            int karakter_sayisi = SmsHesap.SmsKarakterHesap(SmsMetin, smsayar.SmsTurkceKarakter);
            int sms_adet = SmsHesap.SmsSayisi(karakter_sayisi, smsayar.SmsTurkceKarakter);

            SmsBakiye bakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            bakiye.Bakiye = bakiye.Bakiye - sms_adet;
            bakiye.DegistirenKullaniciId = KullaniciId;
            bakiye.DegistirmeTarih = DateTime.Now;
            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "SMS Bakiye Güncelleme";
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return "SMS Gönderilemedi";
            }
            #endregion
            string Sonuc = XMLPOST(PostAddress, ss);
            string[] GecerliSonuc;
            GonderilenSmsler sms = new GonderilenSmsler();
            //20  Mesaj metninde ki problemden dolayı gönderilemediğini veya standart maksimum mesaj karakter sayısını (640) geçtiğini ifade eder.
            //30  Geçersiz kullanıcı adı , şifre veya kullanıcınızın API erişim izninin olmadığını gösterir.Ayrıca eğer API erişiminizde IP sınırlaması yaptıysanız ve sınırladığınız ip dışında gönderim sağlıyorsanız 30 hata kodunu alırsınız.API erişim izninizi veya IP sınırlamanızı, web arayüzden; sağ üst köşede bulunan ayarlar > API işlemleri menüsunden kontrol edebilirsiniz.
            //40  Mesaj başlığınızın (gönderici adınızın) sistemde tanımlı olmadığını ifade eder.Gönderici adlarınızı API ile sorgulayarak kontrol edebilirsiniz.
            //50  Abone hesabınız ile İYS kontrollü gönderimler yapılamamaktadır.
            //51  Aboneliğinize tanımlı İYS Marka bilgisi bulunamadığını ifade eder.
            //70  Hatalı sorgulama. Gönderdiğiniz parametrelerden birisi hatalı veya zorunlu alanlardan birinin eksik olduğunu ifade eder.
            //80  Gönderim sınır aşımı.
            //85  Mükerrer Gönderim sınır aşımı. Aynı numaraya 1 dakika içerisinde 20'den fazla görev oluşturulamaz.
            sms.FirmaId = FirmaId;
            sms.SubeId = SubeId;
            sms.GonderimTarihi = DateTime.Now;
            if (smsayar.SmsTurkceKarakter)
                sms.SmsMetni = SmsMetin;
            else
                sms.SmsMetni = SmsMetinXml;
            sms.Telefon = GSMNo;
            if (Sonuc == "20")
                sms.HataKodu = "20";
            else if (Sonuc == "30")
                sms.HataKodu = "30";
            else if (Sonuc == "40")
                sms.HataKodu = "40";
            else if (Sonuc == "50")
                sms.HataKodu = "50";
            else if (Sonuc == "51")
                sms.HataKodu = "51";
            else if (Sonuc == "70")
                sms.HataKodu = "70";
            else if (Sonuc == "80")
                sms.HataKodu = "80";
            else if (Sonuc == "85")
                sms.HataKodu = "85";
            else if (Sonuc == "-1")
                sms.HataKodu = "-1";
            else
            {
                GecerliSonuc = Sonuc.Split(' ');
                sms.HataKodu = GecerliSonuc[0];
                sms.SmsGorevId = GecerliSonuc[1];
            }
            sms.Durum = "0"; // İletilmeyi Bekliyor.
            sms.OlusturanKullaniciId = KullaniciId;
            sms.OlusturmaTarih = DateTime.Now;
            sms.DegistirenKullaniciId = KullaniciId;
            sms.DegistirmeTarih = DateTime.Now;
            sms.Aktif = true;
            sms.Sil = false;
            try
            {
                dbContext.GonderilenSmslers.Add(sms);
                dbContext.SaveChanges();
                //return XMLPOST(PostAddress, ss);
                return "Başarılı";
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Göderilen SMS kaydet";
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return "SMS Gönderilemedi";
            }
            #endregion
        }
        private static string XMLPOST(string PostAddress, string xmlData)
        {
            try
            {
                PostAddress = "https://ataksms.com/Api/#/v1/xml/syncreply/Submit";
                WebClient wUpload = new WebClient();
                HttpWebRequest request = WebRequest.Create(PostAddress) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Byte[] bPostArray = Encoding.UTF8.GetBytes(xmlData);
                Byte[] bResponse = wUpload.UploadData(PostAddress, "POST", bPostArray);
                Char[] sReturnChars = Encoding.UTF8.GetChars(bResponse);
                string sWebPage = new string(sReturnChars);
                return sWebPage;
            }
            catch (Exception e)
            {
                string hata = e.Message;
                return "-1";
            }
        }

        public static string Gonder_AtakSms(string SmsMetin, string GSMNo, long FirmaId, long SubeId, long KullaniciId)
        {
            string PostAddress = "http://panel4.ekomesaj.com:9587/sms/create";
            string pkgID = "";
            string err = "";
            string returnmesaj = "";


            #region Yöneticiye Sms Gönder
            if (FirmaId == 1)
            {
                SmsJsonData msjYonetici = new SmsJsonData()
                {
                    type = 1,
                    sendingType = 0,
                    title = "FotografciTakip",
                    content = SmsMetin,
                    number = Convert.ToInt64(GSMNo),
                    encoding = 1, // Karakter Kodlaması =  0: Varsayılan(Ascii) 1: Türkçe 2: UTF - 8
                    sender = "KAMiL SEN",
                    //sender = "FOTOTAKiP",
                    periodicSettings = null,
                    sendingDate = null,
                    validity = 60,
                    pushSettings = null
                };
                string YoneticiSmsSonuc = JSONPOST_ATAKSMS(PostAddress, msjYonetici);
                if (YoneticiSmsSonuc == "Uzak ad çözülemedi: 'panel4.ekomesaj.com'")
                {
                    return "SMS sunucusuna bağlanılamadı";
                }
                SmsSonuc YoneticiSMS = JsonConvert.DeserializeObject<SmsSonuc>(YoneticiSmsSonuc);
                if (YoneticiSMS.err == null) // Sms başarıyla gönderildi ise kontör düşümü gerçekleşecek aksi taktirde kontör düşmeyedek.
                {
                    returnmesaj = "Mesaj Başarıyla Göderildi";
                }
                else if (YoneticiSMS.data == null)
                {
                    pkgID = "";
                    err = YoneticiSMS.err.message;
                    returnmesaj = "HATA - Mesaj Gönderilemedi (" + err + ")";
                }
                return returnmesaj;
            }
            #endregion

            short karakterkodlamasi = 1;
            FotoTakipContext dbContext = new FotoTakipContext();
            AyarlarSmsGonderim smsayar = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
            if (!smsayar.SmsTurkceKarakter)
            {
                // Türkçe Karakterler: ğ, Ğ, ç, ş, Ş, ı, İ 
                SmsMetin = SmsMetin.Replace("ğ", "g");
                SmsMetin = SmsMetin.Replace("Ğ", "G");
                SmsMetin = SmsMetin.Replace("ç", "c");
                SmsMetin = SmsMetin.Replace("ş", "s");
                SmsMetin = SmsMetin.Replace("Ş", "S");
                SmsMetin = SmsMetin.Replace("ı", "ı");
                SmsMetin = SmsMetin.Replace("İ", "I");
                karakterkodlamasi = 0;
            }

            #region Karakter hesabı ve Bakiye düşüm işlemleri

            if (smsayar == null)
            {
                return "SMS Gönderilemedi";
            }
            int karakter_sayisi = SmsHesap.SmsKarakterHesap(SmsMetin, smsayar.SmsTurkceKarakter);
            int sms_adet = SmsHesap.SmsSayisi(karakter_sayisi, smsayar.SmsTurkceKarakter);

            SmsBakiye bakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);

            if (bakiye.Bakiye<sms_adet)
            {
                return "Bakiye Yetersiz!";
            }
            #endregion
            
            SmsJsonData msj = new SmsJsonData()
            {
                type = 1,
                sendingType = 0,
                title = "FotografciTakip",
                content = SmsMetin,
                number = Convert.ToInt64(GSMNo),
                encoding = karakterkodlamasi, // Karakter Kodlaması =  0: Varsayılan(Ascii) 1: Türkçe 2: UTF - 8
                //sender = "KAMiL SEN",
                //sender = "FOTOTAKiP",
                sender = "TETRABiLiSM",
                periodicSettings = null,
                sendingDate = null,
                validity = 60,
                pushSettings = null
            };
            #region SMS_Gonderim ve Veritabanına Kayıt İşlemleri

            string Sonuc = JSONPOST_ATAKSMS(PostAddress, msj);
            if (Sonuc == "Uzak ad çözülemedi: 'panel4.ekomesaj.com'")
            {
                return "SMS sunucusuna bağlanılamadı";
            }
            //string[] GecerliSonuc;
            SmsSonuc smssonuc = JsonConvert.DeserializeObject<SmsSonuc>(Sonuc);

         
            if (smssonuc.err == null) // Sms başarıyla gönderildi ise kontör düşümü gerçekleşecek aksi taktirde kontör düşmeyedek.
            {
                pkgID = smssonuc.data.pkgID;
                err = "";
                returnmesaj = "Mesaj Başarıyla Göderildi";

                #region Karakter hesabı ve Bakiye düşüm işlemleri
                if (smsayar == null)
                {
                    return "SMS Gönderilemedi";
                }
                //int karakter_sayisi = SmsHesap.SmsKarakterHesap(SmsMetin, smsayar.SmsTurkceKarakter);
                //int sms_adet = SmsHesap.SmsSayisi(karakter_sayisi, smsayar.SmsTurkceKarakter);

                //SmsBakiye bakiye = dbContext.SmsBakiyes.FirstOrDefault(x => x.FirmaId == FirmaId && x.Aktif == true && x.Sil == false);
                bakiye.Bakiye = bakiye.Bakiye - sms_adet;
                bakiye.DegistirenKullaniciId = KullaniciId;
                bakiye.DegistirmeTarih = DateTime.Now;
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    HataLoglari hata = new HataLoglari();
                    hata.FirmaId = FirmaId;
                    hata.SubeId = SubeId;
                    hata.Islem = "SMS Bakiye Güncelleme";
                    hata.HataMesajı = e.Message;
                    hata.OlusturanKullaniciId = 1;
                    hata.OlusturmaTarih = DateTime.Now;
                    hata.DegistirenKullaniciId = 1;
                    hata.DegistirmeTarih = DateTime.Now;
                    hata.Aktif = true;
                    hata.Sil = false;
                    dbContext.HataLoglaris.Add(hata);
                    dbContext.SaveChanges();
                }
                #endregion
            }
            else if (smssonuc.data == null)
            {
                pkgID = "";
                err = smssonuc.err.message;
                returnmesaj = "HATA - Mesaj Gönderilemedi (" + err + ")";
            }

            GonderilenSmsler sms = new GonderilenSmsler();

            sms.FirmaId = FirmaId;
            sms.SubeId = SubeId;
            sms.GonderimTarihi = DateTime.Now;
            sms.SmsMetni = SmsMetin;
            sms.Telefon = GSMNo;
            sms.SmsGorevId = pkgID;
            sms.HataKodu = err;
            sms.Durum = "0"; // İletilmeyi Bekliyor.
            sms.OlusturanKullaniciId = KullaniciId;
            sms.OlusturmaTarih = DateTime.Now;
            sms.DegistirenKullaniciId = KullaniciId;
            sms.DegistirmeTarih = DateTime.Now;
            sms.Aktif = true;
            sms.Sil = false;
            try
            {
                dbContext.GonderilenSmslers.Add(sms);
                dbContext.SaveChanges();
                return returnmesaj;
            }
            catch (Exception e)
            {
                HataLoglari hata = new HataLoglari();
                hata.FirmaId = FirmaId;
                hata.SubeId = SubeId;
                hata.Islem = "Göderilen SMS kaydet";
                hata.HataMesajı = e.Message;
                hata.OlusturanKullaniciId = 1;
                hata.OlusturmaTarih = DateTime.Now;
                hata.DegistirenKullaniciId = 1;
                hata.DegistirmeTarih = DateTime.Now;
                hata.Aktif = true;
                hata.Sil = false;
                dbContext.HataLoglaris.Add(hata);
                dbContext.SaveChanges();
                return "SMS Gönderilemedi";
            }
            #endregion
        }
        private static string JSONPOST_ATAKSMS(string PostAddress, SmsJsonData SmsJsonData)
        {
            string username = "kamilsen";
            string password = "442Bir726";
            //string PostAddress;
            try
            {
                //PostAddress = "http://panel4.ekomesaj.com:9587/user/credit";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(PostAddress);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
                httpWebRequest.Headers.Add("Authorization", "Basic " + svcCredentials);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string jsonText = JsonConvert.SerializeObject(SmsJsonData);
                    streamWriter.Write(jsonText);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
    class SmsJsonData
    {
        public int type { get; set; }
        public int sendingType { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public long number { get; set; }
        public int encoding { get; set; }
        public string sender { get; set; }
        public string periodicSettings { get; set; }
        public string sendingDate { get; set; }
        public int validity { get; set; }
        public string pushSettings { get; set; }
    }
    class SmsSonuc
    {
        public SmsSonucError err { get; set; }
        public SmsSonucData data { get; set; }
    }

    class SmsSonucData
    {
        public string pkgID { get; set; }
    }
    class SmsSonucError
    {
        public string code { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }
}