using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FotografciTakipWeb.Models;


namespace FotografciTakipWeb.Areas.Admin.OtomatikSMS
{
    public class SMSGorev
    {
        FotoTakipContext dbContext;
        public SMSGorev()
        {
            dbContext = new FotoTakipContext();
            List<Firma> FirmaListesi = dbContext.Firmas.Where(x => x.Aktif == true && x.Sil == false).ToList();
            foreach (var firma in FirmaListesi)
            {
                Lisanslar lisanslar = dbContext.Lisanslars.FirstOrDefault(x => x.FirmaId == firma.Id && x.Aktif == true && x.Sil == false);
                if (lisanslar.LisansBitisTarihi > DateTime.Now) // Firmanın lisansı bitti ise Müşterilerine SMS gönderilmeyecek.
                {
                    // Cari Alacak Hatırlatma Mesajı
                    AyarlarSmsGonderim ayarlarSmsGonderim = dbContext.AyarlarSmsGonderims.FirstOrDefault(x => x.FirmaId == firma.Id);
                    //0: Hiçbir Zaman Gönderme 1: Tarih ile aynı gün 2: 1 gün önce 3: 2 gün önce 4: 3 gün önce 5: 1 hafta önce 6: Kayıt Yapıldığında
                    #region Cari Alacak Hatırlatma Mesajı
                    if (ayarlarSmsGonderim.CariAlacakHatirlatmaMesaji != 0) // Firma tarafından Cari Alacak Hatırlatma Mesajı için bir metin seçilmişse seçilen Metin Otomatik olarak gönderilecek.
                    {
                        #region Mesaj Süresi 1 (Tarih ile Aynı Gün)
                        if (ayarlarSmsGonderim.CariAlacakHatirlatmaGonderimSuresi == 1) // Firma tarafından Cari Alacak Hatırlatma Mesajı Süresi 1 seçilmiş ise Ödeme tarihi bugün olan kayıtlar SMS listesine alınacak.
                        {
                            List<CariHareket> cariHarekets = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.Tip == "Alacak" && x.TahsilatOdemeBit == false && x.OdemeTarihi == DateTime.Now).ToList();
                            if (cariHarekets.Count > 0 || cariHarekets != null)
                            {
                                CariAlacakHatirlatmaSmsDetay(cariHarekets, firma, ayarlarSmsGonderim);
                            }
                        }
                        #endregion
                        #region Mesaj Süresi 2 (1 Gün Önce)
                        else if (ayarlarSmsGonderim.CariAlacakHatirlatmaGonderimSuresi == 2) // Firma tarafından Cari Alacak Hatırlatma Mesajı Süresi 1 seçilmiş ise Ödeme tarihi bugün olan kayıtlar SMS listesine alınacak.
                        {
                            DateTime smstarih = DateTime.Now.AddDays(-1);
                            List<CariHareket> cariHarekets = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.Tip == "Alacak" && x.TahsilatOdemeBit == false && x.OdemeTarihi == smstarih).ToList();
                            if (cariHarekets.Count > 0 || cariHarekets != null)
                            {
                                CariAlacakHatirlatmaSmsDetay(cariHarekets, firma, ayarlarSmsGonderim);
                            }
                        }
                        #endregion
                        #region Mesaj Süresi 3 (2 Gün Önce)
                        else if (ayarlarSmsGonderim.CariAlacakHatirlatmaGonderimSuresi == 3) // Firma tarafından Cari Alacak Hatırlatma Mesajı Süresi 1 seçilmiş ise Ödeme tarihi bugün olan kayıtlar SMS listesine alınacak.
                        {
                            DateTime smstarih = DateTime.Now.AddDays(-2);
                            List<CariHareket> cariHarekets = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.Tip == "Alacak" && x.TahsilatOdemeBit == false && x.OdemeTarihi == smstarih).ToList();
                            if (cariHarekets.Count > 0 || cariHarekets != null)
                            {
                                CariAlacakHatirlatmaSmsDetay(cariHarekets, firma, ayarlarSmsGonderim);
                            }
                        }
                        #endregion
                        #region Mesaj Süresi 4 (3 Gün Önce)
                        else if (ayarlarSmsGonderim.CariAlacakHatirlatmaGonderimSuresi == 4) // Firma tarafından Cari Alacak Hatırlatma Mesajı Süresi 1 seçilmiş ise Ödeme tarihi bugün olan kayıtlar SMS listesine alınacak.
                        {
                            DateTime smstarih = DateTime.Now.AddDays(-3);
                            List<CariHareket> cariHarekets = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.Tip == "Alacak" && x.TahsilatOdemeBit == false && x.OdemeTarihi == smstarih).ToList();
                            if (cariHarekets.Count > 0 || cariHarekets != null)
                            {
                                CariAlacakHatirlatmaSmsDetay(cariHarekets, firma, ayarlarSmsGonderim);
                            }
                        }
                        #endregion
                        #region Mesaj Süresi 5 (7 Gün Önce)
                        else if (ayarlarSmsGonderim.CariAlacakHatirlatmaGonderimSuresi == 5) // Firma tarafından Cari Alacak Hatırlatma Mesajı Süresi 1 seçilmiş ise Ödeme tarihi bugün olan kayıtlar SMS listesine alınacak.
                        {
                            DateTime smstarih = DateTime.Now.AddDays(-7);
                            List<CariHareket> cariHarekets = dbContext.CariHarekets.Where(x => x.FirmaId == firma.Id && x.Tip == "Alacak" && x.TahsilatOdemeBit == false && x.OdemeTarihi == smstarih).ToList();
                            if (cariHarekets.Count > 0 || cariHarekets != null)
                            {
                                CariAlacakHatirlatmaSmsDetay(cariHarekets, firma, ayarlarSmsGonderim);
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
            }
        }
        private void CariAlacakHatirlatmaSmsDetay(List<CariHareket> cariHarekets,Firma firma,AyarlarSmsGonderim ayarlarSmsGonderim)
        {
            string AliciAdSoyad = "";
            OtomatikSmsListesi SmsListesi = new OtomatikSmsListesi();
            foreach (var carihareket in cariHarekets)
            {
                if (carihareket.Cari.SMSKabul)  // Cari SMS almayı kabul ediyorsa
                {
                    string smsMetin = dbContext.SmsMetinleris.FirstOrDefault(x => x.FirmaId == firma.Id && x.Id == ayarlarSmsGonderim.CariAlacakHatirlatmaMesaji).SMSMetni;

                    if (smsMetin.IndexOf("{YetkiliAdSoyad}") != -1) // SmsMetni içine parametre olarak YetkiliAdSoyad yazıldı ise
                        if (carihareket.Cari.AdSoyad == "")  // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                            smsMetin = smsMetin.Replace("{YetkiliAdSoyad}", carihareket.Cari.Yetkili);
                        else                   // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                            smsMetin = smsMetin.Replace("{YetkiliAdSoyad}", carihareket.Cari.AdSoyad);
                    if (smsMetin.IndexOf("{AdSoyad}") != -1)  // SmsMetni içine parametre olarak AdSoyad yazıldı ise
                        if (carihareket.Cari.AdSoyad == "") // Cari Firma ise Yetkili bilgisini Parametre olarak ekle
                            smsMetin = smsMetin.Replace("{YetkiliAdSoyad}", carihareket.Cari.Yetkili);
                        else                    // Cari Şahıs ise AdSoyad bilgisini Parametre olarak ekle
                            smsMetin = smsMetin.Replace("{YetkiliAdSoyad}", carihareket.Cari.AdSoyad);
                    if (smsMetin.IndexOf("{FirmaAdi}") != -1)
                        smsMetin = smsMetin.Replace("{FirmaAdi}", carihareket.Cari.FirmaAdi);
                    if (smsMetin.IndexOf("{Tarih}") != -1)
                        smsMetin = smsMetin.Replace("{Tarih}", carihareket.IslemTarihi.ToShortDateString());
                    if (smsMetin.IndexOf("{OdemeTarihi}") != -1)
                        smsMetin = smsMetin.Replace("{OdemeTarihi}", carihareket.OdemeTarihi.ToShortDateString());
                    if (smsMetin.IndexOf("{Tutar}") != -1)
                        smsMetin = smsMetin.Replace("{Tutar}", carihareket.Tutar.ToString());


                    if (!string.IsNullOrEmpty(carihareket.Cari.FirmaAdi))
                        AliciAdSoyad = AliciAdSoyad + carihareket.Cari.FirmaAdi;
                    if (!string.IsNullOrEmpty(carihareket.Cari.Yetkili))
                        AliciAdSoyad = AliciAdSoyad + ((AliciAdSoyad == "") ? carihareket.Cari.Yetkili : " (" + carihareket.Cari.Yetkili + ")");
                    SmsListesi.FirmaId = firma.Id;
                    SmsListesi.AliciAdSoyad = AliciAdSoyad;
                    SmsListesi.GonderimTarihi = DateTime.Now;
                    SmsListesi.SmsMetni = smsMetin;
                    SmsListesi.AliciTel = carihareket.Cari.CepTel;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        HataLoglari hata = new HataLoglari();
                        hata.FirmaId = carihareket.FirmaId;
                        hata.SubeId = carihareket.SubeId;
                        hata.Islem = "OtomatikSmsGonder tablosuna Cari Alacak Hatırlatma Mesajı Ekleme";
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
                }
            }
        }


        // Gönderilecek SMS'ler tarihlerine göre listelecek, geçici tabloya alınacak ve SMS ler günü gelince gönderilecek.
        // 1 - Firmaya ait SMS gonderim ayarları kontrol edilecek.
        // 2 - Her ayara karşılık gelen SMS gönderim süresine göre ilgili ayara bağlı tablolardaki tarihler kontrol edilecek ve uygun tarihli kayıta karşılık
        //     gelecek SMS metni geçici bir tabloya (SMSGonderilecekler) alınacak.
        // 3 - Geçici tablodaki SMS ler sırası ile gönderilecek.
    }

  
}