using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.Otomasyon
{
    public class OtomasyonAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Otomasyon";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
               "Otomasyon_default",
               "Otomasyon/{controller}/{action}/{id}",
               new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
           );
        }
    }
}