namespace FotografciTakipWeb.App_Settings
{
    public class SmsHesap
    {
        public static int SmsKarakterHesap(string metin, bool TurkceKarakter)
        {
            int metinuzunluk = metin.Length;
            // Türkçe Karakterler: ğ, Ğ, ç, ş, Ş, ı, İ 
            char harf_g = 'ğ', harf_G = 'Ğ', harf_c = 'ç', harf_s = 'ş', harf_S = 'Ş', harf_i = 'ı', harf_I = 'İ';
            int adet_g = 0, adet_G = 0, adet_c = 0, adet_s = 0, adet_S = 0, adet_i = 0, adet_I = 0; //Aranacak harf, aranacak kelimede her bulundugunda asagida olusturdugumuz for döngüsünün içinde bir artacak.
            for (int i = 0; i < metin.Length; i++)//girilen kelimenin her bir karakterinin kontrol edilmesi için kelime.Length kullanimi ideal.
            {
                if (metin[i] == harf_g)//Burada girilen kelimenin içerisinde belirtilen harfin bulunmasi durumu.
                    adet_g++;//Eger bulduysa bir artirsin.
                if (metin[i] == harf_G)//Burada girilen kelimenin içerisinde belirtilen harfin bulunmasi durumu.
                    adet_G++;//Eger bulduysa bir artirsin.
                if (metin[i] == harf_c)//Burada girilen kelimenin içerisinde belirtilen harfin bulunmasi durumu.
                    adet_c++;//Eger bulduysa bir artirsin.
                if (metin[i] == harf_s)//Burada girilen kelimenin içerisinde belirtilen harfin bulunmasi durumu.
                    adet_s++;//Eger bulduysa bir artirsin.
                if (metin[i] == harf_S)//Burada girilen kelimenin içerisinde belirtilen harfin bulunmasi durumu.
                    adet_S++;//Eger bulduysa bir artirsin.
                if (metin[i] == harf_i)//Burada girilen kelimenin içerisinde belirtilen harfin bulunmasi durumu.
                    adet_i++;//Eger bulduysa bir artirsin.
                if (metin[i] == harf_I)//Burada girilen kelimenin içerisinde belirtilen harfin bulunmasi durumu.
                    adet_I++;//Eger bulduysa bir artirsin.
            }
            if (TurkceKarakter)
            {
                metinuzunluk = metinuzunluk + ((adet_g * 2) + (adet_G * 2) + (adet_c * 2) + (adet_s * 2) + (adet_S * 2) + (adet_i * 2) + (adet_I * 2));
            }
            return metinuzunluk;
        }

        public static int SmsSayisi(int karakter_sayisi, bool TurkceKarakter)
        {
            int smsuzunluk=TurkceKarakter?155:160; // türkçe karakter kullanılıyorsa 1 boy SMS 155 Karakter olacak.

            int sms_adet = (karakter_sayisi + 5) / smsuzunluk;

            return sms_adet+1;
        }

    }
}