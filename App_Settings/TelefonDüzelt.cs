namespace FotografciTakipWeb.App_Settings
{
    public class TelefonDüzelt
    {
        public string düzelt(string tel)
        {
            tel = tel.Replace('(', ' ');
            tel = tel.Replace(')', ' ');
            tel = tel.Replace('-', ' ');
            tel = tel.Replace(" ", string.Empty);
            return tel;
        }

    }
}