using System;
using System.Web;

namespace FotografciTakipWeb.App_Settings
{
    public class ResimIsim
    {
        public string resimisimlendir(HttpPostedFileBase resim, string ad)
        {
            /*-----Seçilen resme yeni bir isim veriyorum. Tarihsaat kullanarak değer üreterek-------*/
            System.IO.FileInfo ff = new System.IO.FileInfo(resim.FileName);
            string uzanti = ff.Extension; // dosya uzantısı
            string resimadi = "";
            string tarihsaat = DateTime.Now.ToString();
            ad = ad.Replace('.', ' ');
            ad = ad.Replace(':', ' ');
            ad = ad.Replace(" ", string.Empty);

            ad = ad.Replace("ş", "s");
            ad = ad.Replace("Ş", "s");
            ad = ad.Replace("İ", "i");
            ad = ad.Replace("I", "i");
            ad = ad.Replace("ı", "i");
            ad = ad.Replace("ö", "o");
            ad = ad.Replace("Ö", "o");
            ad = ad.Replace("ü", "u");
            ad = ad.Replace("Ü", "u");
            ad = ad.Replace("Ç", "c");
            ad = ad.Replace("ç", "c");
            ad = ad.Replace("ğ", "g");
            ad = ad.Replace("Ğ", "g");
            ad = ad.Replace(" ", "-");
            ad = ad.Replace("---", "-");
            ad = ad.Replace("?", "");
            ad = ad.Replace("/", "");
            ad = ad.Replace(".", "");
            ad = ad.Replace("'", "");
            ad = ad.Replace("#", "");
            ad = ad.Replace("%", "");
            ad = ad.Replace("&", "");
            ad = ad.Replace("*", "");
            ad = ad.Replace("!", "");
            ad = ad.Replace("@", "");
            ad = ad.Replace("+", "");

            ad = ad.ToLower();
            ad = ad.Trim();

            tarihsaat = tarihsaat.Replace('.', ' ');
            tarihsaat = tarihsaat.Replace(':', ' ');
            tarihsaat = tarihsaat.Replace(" ", string.Empty);
            resimadi = ad + "_" + tarihsaat + uzanti;
            /*------------------------------------------------------------------------*/
            return resimadi;
        }
        public string resimisimlendir3(HttpPostedFileBase resim, string ad)
        {
            /*-----Seçilen resme yeni bir isim veriyorum. Tarihsaat kullanarak değer üreterek-------*/
            System.IO.FileInfo ff = new System.IO.FileInfo(resim.FileName);
            string uzanti = ff.Extension; // dosya uzantısı
            string resimadi = "";
            ad = ad.Replace('.', ' ');
            ad = ad.Replace(':', ' ');
            ad = ad.Replace(" ", string.Empty);

            ad = ad.Replace("ş", "s");
            ad = ad.Replace("Ş", "s");
            ad = ad.Replace("İ", "i");
            ad = ad.Replace("I", "i");
            ad = ad.Replace("ı", "i");
            ad = ad.Replace("ö", "o");
            ad = ad.Replace("Ö", "o");
            ad = ad.Replace("ü", "u");
            ad = ad.Replace("Ü", "u");
            ad = ad.Replace("Ç", "c");
            ad = ad.Replace("ç", "c");
            ad = ad.Replace("ğ", "g");
            ad = ad.Replace("Ğ", "g");
            ad = ad.Replace(" ", "-");
            ad = ad.Replace("---", "-");
            ad = ad.Replace("?", "");
            ad = ad.Replace("/", "");
            ad = ad.Replace(".", "");
            ad = ad.Replace("'", "");
            ad = ad.Replace("#", "");
            ad = ad.Replace("%", "");
            ad = ad.Replace("&", "");
            ad = ad.Replace("*", "");
            ad = ad.Replace("!", "");
            ad = ad.Replace("@", "");
            ad = ad.Replace("+", "");

            ad = ad.ToLower();
            ad = ad.Trim();

            resimadi = ad + uzanti;
            /*------------------------------------------------------------------------*/
            return resimadi;
        }
        public string resimisimlendir2(HttpPostedFileBase resim, string ad)
        {
            /*-----Seçilen resme yeni bir isim veriyorum. Tarihsaat kullanarak değer üreterek-------*/
            System.IO.FileInfo ff = new System.IO.FileInfo(resim.FileName);
            string uzanti = ff.Extension; // dosya uzantısı
            string resimadi = "";
            string tarihsaat = DateTime.Now.ToString();
            ad = ad.Replace('.', ' ');
            ad = ad.Replace(':', ' ');
            ad = ad.Replace(" ", string.Empty);

            ad = ad.Replace("ş", "s");
            ad = ad.Replace("Ş", "s");
            ad = ad.Replace("İ", "i");
            ad = ad.Replace("I", "i");
            ad = ad.Replace("ı", "i");
            ad = ad.Replace("ö", "o");
            ad = ad.Replace("Ö", "o");
            ad = ad.Replace("ü", "u");
            ad = ad.Replace("Ü", "u");
            ad = ad.Replace("Ç", "c");
            ad = ad.Replace("ç", "c");
            ad = ad.Replace("ğ", "g");
            ad = ad.Replace("Ğ", "g");
            ad = ad.Replace(" ", "-");
            ad = ad.Replace("---", "-");
            ad = ad.Replace("?", "");
            ad = ad.Replace("/", "");
            ad = ad.Replace(".", "");
            ad = ad.Replace("'", "");
            ad = ad.Replace("#", "");
            ad = ad.Replace("%", "");
            ad = ad.Replace("&", "");
            ad = ad.Replace("*", "");
            ad = ad.Replace("!", "");
            ad = ad.Replace("@", "");
            ad = ad.Replace("+", "");

            ad = ad.ToLower();
            ad = ad.Trim();

            tarihsaat = tarihsaat.Replace('.', ' ');
            tarihsaat = tarihsaat.Replace(':', ' ');
            tarihsaat = tarihsaat.Replace(" ", string.Empty);
            resimadi = ad + uzanti;
            /*------------------------------------------------------------------------*/
            return resimadi;
        }
    }
}