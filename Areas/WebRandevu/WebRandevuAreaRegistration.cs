using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.WebRandevu
{
    public class WebRandevuAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WebRandevu";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WebRandevu_default",
                "WebRandevu/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}