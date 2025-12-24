using System.Web.Mvc;

namespace FotografciTakipWeb.Areas.LinkRandevu
{
    public class LinkRandevuAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LinkRandevu";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LinkRandevu_default",
                "LinkRandevu/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}