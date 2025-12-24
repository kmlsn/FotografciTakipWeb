using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.Musteri
{
    public class MusteriAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Musteri";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
               "Musteri_profil",
               "Musteri/Profil",
               new { controller = "Dashboard", action = "Profil", id = UrlParameter.Optional }
           );
            context.MapRoute(
                "Musteri_rezervasyon",
                "Musteri/Rezervasyonlarim",
                new { controller = "Dashboard", action = "Rezervasyonlarim", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Musteri_rezervasyonteklif",
                "Musteri/RezervasyonTekliflerim",
                new { controller = "Dashboard", action = "RezervasyonTekliflerim", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Musteri_odemeler",
                "Musteri/Odemeler",
                new { controller = "Dashboard", action = "Odemeler", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Musteri_fotografSec",
                "Musteri/FotografSec",
                new { controller = "Dashboard", action = "FotografSec", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Musteri_mesajlar",
                "Musteri/Mesajlar",
                new { controller = "Dashboard", action = "Mesajlar", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Musteri_mesaj_detay",
                "Musteri/MesajDetay/{id}",
                new { controller = "Dashboard", action = "MesajDetay", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Musteri_dashboard",
                "Musteri/Dashboard",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Musteri_default",
                "Musteri/{controller}/{action}/{id}",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}