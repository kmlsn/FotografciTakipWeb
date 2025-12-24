using System;

namespace FotografciTakipWeb.App_Settings
{
    public class SifreOlustur
    {
        public string sifreolustur(int uzunluk)
        {
            char[] cr = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            string result = string.Empty;
            Random r = new Random();
            for (int i = 0; i < uzunluk; i++)
            {
                result += cr[r.Next(0, cr.Length - 1)].ToString();
            }
            return result;
        }
    }
}